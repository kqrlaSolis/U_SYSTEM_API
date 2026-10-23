// Reglas de docentes: nº de empleado y correo únicos; baja bloqueada si tiene grupos.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _TeacherRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public TeacherService(
        ITeacherRepository TeacherRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _TeacherRepository = TeacherRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<TeacherDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _TeacherRepository.GetPagedAsync(Query, Cancellation);
        var Items = Page.Items.Select(TeacherMapper.ToDto).ToList();
        return PagedResult<TeacherDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<TeacherDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Teacher = await _TeacherRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Teachers.NotFound");

        return TeacherMapper.ToDto(Teacher);
    }

    public async Task<TeacherDto> CreateAsync(
        CreateTeacherRequest Request,
        CancellationToken Cancellation = default)
    {
        var EmployeeNumber = Request.EmployeeNumber.Trim();
        var Email = Request.Email.Trim();

        await EnsureUniqueAsync(EmployeeNumber, Email, null, Cancellation);

        var NewTeacher = new Teacher
        {
            EmployeeNumber = EmployeeNumber,
            FirstName = Request.FirstName.Trim(),
            PaternalLastName = Request.PaternalLastName.Trim(),
            MaternalLastName = Normalize(Request.MaternalLastName),
            Email = Email,
            Phone = Normalize(Request.Phone),
            Specialty = Normalize(Request.Specialty),
            UserId = Request.UserId,
            IsActive = true,
        };

        await _TeacherRepository.AddAsync(NewTeacher, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Teachers",
            nameof(Teacher),
            NewTeacher.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewTeacher.EmployeeNumber, NewTeacher.FirstName, NewTeacher.PaternalLastName, NewTeacher.Email },
            Cancellation);

        return TeacherMapper.ToDto(NewTeacher);
    }

    public async Task<TeacherDto> UpdateAsync(
        int Id,
        UpdateTeacherRequest Request,
        CancellationToken Cancellation = default)
    {
        var Teacher = await _TeacherRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Teachers.NotFound");

        var EmployeeNumber = Request.EmployeeNumber.Trim();
        var Email = Request.Email.Trim();

        await EnsureUniqueAsync(EmployeeNumber, Email, Teacher.Id, Cancellation);

        var Before = new { Teacher.EmployeeNumber, Teacher.Email, Teacher.IsActive };

        Teacher.EmployeeNumber = EmployeeNumber;
        Teacher.FirstName = Request.FirstName.Trim();
        Teacher.PaternalLastName = Request.PaternalLastName.Trim();
        Teacher.MaternalLastName = Normalize(Request.MaternalLastName);
        Teacher.Email = Email;
        Teacher.Phone = Normalize(Request.Phone);
        Teacher.Specialty = Normalize(Request.Specialty);
        Teacher.UserId = Request.UserId;
        Teacher.IsActive = Request.IsActive;

        _TeacherRepository.Update(Teacher);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Teachers",
            nameof(Teacher),
            Teacher.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Teacher.EmployeeNumber, Teacher.Email, Teacher.IsActive },
            Cancellation);

        return TeacherMapper.ToDto(Teacher);
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Teacher = await _TeacherRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Teachers.NotFound");

        if (await _TeacherRepository.HasClassGroupsAsync(Teacher.Id, Cancellation))
        {
            throw new BusinessRuleException(
                "Teachers.InUse",
                new Dictionary<string, string> { ["EmployeeNumber"] = Teacher.EmployeeNumber });
        }

        _TeacherRepository.Remove(Teacher);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Teachers",
            nameof(Teacher),
            Teacher.Id.ToString(CultureInfo.InvariantCulture),
            new { Teacher.EmployeeNumber, Teacher.Email },
            null,
            Cancellation);
    }

    private async Task EnsureUniqueAsync(
        string EmployeeNumber,
        string Email,
        int? ExcludeTeacherId,
        CancellationToken Cancellation)
    {
        if (await _TeacherRepository.ExistsByEmployeeNumberAsync(
            EmployeeNumber,
            ExcludeTeacherId,
            true,
            Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Teachers.EmployeeNumberTaken",
                new Dictionary<string, string> { ["EmployeeNumber"] = EmployeeNumber });
        }

        if (await _TeacherRepository.ExistsByEmailAsync(Email, ExcludeTeacherId, false, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Teachers.EmailTaken",
                new Dictionary<string, string> { ["Email"] = Email });
        }
    }

    // Texto vacío se guarda como NULL: la base no distingue "" de NULL
    private static string? Normalize(string? Value)
        => string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
