// Reglas de inscripción: un alumno por periodo, grupos abiertos y con cupo.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<EnrollmentDto> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest Request, CancellationToken Cancellation = default);

    Task<EnrollmentDto> CreateReEnrollmentAsync(CreateEnrollmentRequest Request, CancellationToken Cancellation = default);
    Task<EnrollmentDto> CancelAsync(long Id, string Reason, CancellationToken Cancellation = default);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _EnrollmentRepository;
    private readonly IClassGroupRepository _GroupRepository;
    private readonly IAcademicTermRepository _TermRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public EnrollmentService(
        IEnrollmentRepository EnrollmentRepository,
        IClassGroupRepository GroupRepository,
        IAcademicTermRepository TermRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _EnrollmentRepository = EnrollmentRepository;
        _GroupRepository = GroupRepository;
        _TermRepository = TermRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<EnrollmentDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _EnrollmentRepository.GetPagedAsync(Query, Cancellation);
        var Items = new List<EnrollmentDto>();

        foreach (var Enrollment in Page.Items)
        {
            Items.Add(await ToDtoAsync(Enrollment, Cancellation));
        }

        return PagedResult<EnrollmentDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<EnrollmentDto> GetByIdAsync(long Id, CancellationToken Cancellation = default)
    {
        var Enrollment = await _EnrollmentRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Enrollments.NotFound");

        return await ToDtoAsync(Enrollment, Cancellation);
    }

    public Task<EnrollmentDto> CreateAsync(
        CreateEnrollmentRequest Request,
        CancellationToken Cancellation = default)
        => CreateInternalAsync(Request, "Enrollment", Cancellation);

    // Reinscripción: misma mecánica de cupo y grupos, con Type = ReEnrollment
    public Task<EnrollmentDto> CreateReEnrollmentAsync(
        CreateEnrollmentRequest Request,
        CancellationToken Cancellation = default)
        => CreateInternalAsync(Request, "ReEnrollment", Cancellation);

    private async Task<EnrollmentDto> CreateInternalAsync(
        CreateEnrollmentRequest Request,
        string EnrollmentType,
        CancellationToken Cancellation = default)
    {
        // El periodo debe existir: sin esto la FK responde con un error de base de datos
        _ = await _TermRepository.GetByIdAsync(Request.AcademicTermId, Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        if (await _EnrollmentRepository.ExistsForStudentTermAsync(
            Request.StudentId,
            Request.AcademicTermId,
            Cancellation))
        {
            throw BusinessRuleException.Conflict("Enrollments.Duplicate");
        }

        // Se valida cada grupo antes de escribir: abierto y con cupo disponible
        var Groups = new List<ClassGroup>();
        foreach (var GroupId in Request.ClassGroupIds.Distinct())
        {
            var Group = await _GroupRepository.GetByIdAsync(GroupId, Cancellation)
                ?? throw new NotFoundException("ClassGroups.NotFound");

            if (!string.Equals(Group.Status, "Open", StringComparison.OrdinalIgnoreCase))
            {
                throw BusinessRuleException.Conflict(
                    "Enrollments.GroupNotOpen",
                    new Dictionary<string, string> { ["Code"] = Group.Code });
            }

            var InGroup = await _EnrollmentRepository.CountInGroupAsync(Group.Id, Cancellation);
            if (InGroup + 1 > Group.Capacity)
            {
                throw BusinessRuleException.Conflict(
                    "Enrollments.GroupFull",
                    new Dictionary<string, string> { ["Code"] = Group.Code });
            }

            Groups.Add(Group);
        }

        var NewEnrollment = new Enrollment
        {
            StudentId = Request.StudentId,
            AcademicTermId = Request.AcademicTermId,
            CareerId = Request.CareerId,
            Type = EnrollmentType,
            Status = "Draft",
            Notes = Normalize(Request.Notes),
        };

        await _EnrollmentRepository.AddAsync(NewEnrollment, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        foreach (var Group in Groups)
        {
            await _EnrollmentRepository.AddClassGroupAsync(
                new EnrollmentClassGroup
                {
                    EnrollmentId = NewEnrollment.Id,
                    ClassGroupId = Group.Id,
                },
                Cancellation);

            Group.EnrolledCount = (short)(Group.EnrolledCount + 1);
            _GroupRepository.Update(Group);
        }

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Enrollments",
            nameof(Enrollment),
            NewEnrollment.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewEnrollment.StudentId, NewEnrollment.AcademicTermId, Groups = Groups.Select(Group => Group.Code) },
            Cancellation);

        return await ToDtoAsync(NewEnrollment, Cancellation);
    }

    public async Task<EnrollmentDto> CancelAsync(
        long Id,
        string Reason,
        CancellationToken Cancellation = default)
    {
        var Enrollment = await _EnrollmentRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Enrollments.NotFound");

        if (string.Equals(Enrollment.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            throw BusinessRuleException.Conflict("Enrollments.AlreadyCancelled");
        }

        var Before = new { Enrollment.Status, Enrollment.Notes };

        var Links = await _EnrollmentRepository.GetClassGroupsAsync(Enrollment.Id, Cancellation);

        Enrollment.Status = "Cancelled";
        Enrollment.Notes = Normalize(Reason) ?? Enrollment.Notes;

        _EnrollmentRepository.Update(Enrollment);

        // Al cancelar se libera el cupo de cada grupo asignado
        foreach (var Link in Links)
        {
            var Group = await _GroupRepository.GetByIdAsync(Link.ClassGroupId, Cancellation);
            if (Group is null)
            {
                continue;
            }

            Group.EnrolledCount = (short)Math.Max(0, Group.EnrolledCount - 1);
            _GroupRepository.Update(Group);
        }

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Cancel",
            "Enrollments",
            nameof(Enrollment),
            Enrollment.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Enrollment.Status },
            Cancellation);

        return await ToDtoAsync(Enrollment, Cancellation);
    }

    private async Task<EnrollmentDto> ToDtoAsync(Enrollment Enrollment, CancellationToken Cancellation)
    {
        var Links = await _EnrollmentRepository.GetClassGroupsAsync(Enrollment.Id, Cancellation);

        return new EnrollmentDto
        {
            Id = Enrollment.Id,
            StudentId = Enrollment.StudentId,
            AcademicTermId = Enrollment.AcademicTermId,
            CareerId = Enrollment.CareerId,
            Type = Enrollment.Type,
            Status = Enrollment.Status,
            Notes = Enrollment.Notes,
            ClassGroupIds = Links.Select(Link => Link.ClassGroupId).ToList(),
        };
    }

    private static string? Normalize(string? Value)
        => string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
