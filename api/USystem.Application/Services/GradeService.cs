// Reglas de calificaciones: captura masiva por grupo, acta cerrada bloquea cambios
// y un docente solo trabaja con los grupos que tiene asignados.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IGradeService
{
    Task<IReadOnlyList<GradeDto>> GetByClassGroupAsync(int ClassGroupId, CancellationToken Cancellation = default);

    Task<IReadOnlyList<GradeDto>> SaveAsync(int ClassGroupId, SaveGradesRequest Request, CancellationToken Cancellation = default);
}

public class GradeService : IGradeService
{
    // Rol acotado a sus grupos y roles con alcance sobre todos los grupos
    private const string TeacherRoleName = "Teacher";

    // 403: la petición es válida pero el grupo no pertenece al docente
    private const int ForbiddenStatusCode = 403;

    private static readonly string[] UnrestrictedRoleNames =
    {
        "Administrator",
        "Registrar",
        "ProgramCoordinator",
    };

    private readonly IGradeRepository _GradeRepository;
    private readonly IClassGroupRepository _GroupRepository;
    private readonly ITeacherRepository _TeacherRepository;
    private readonly ICurrentUser _CurrentUser;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public GradeService(
        IGradeRepository GradeRepository,
        IClassGroupRepository GroupRepository,
        ITeacherRepository TeacherRepository,
        ICurrentUser CurrentUser,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _GradeRepository = GradeRepository;
        _GroupRepository = GroupRepository;
        _TeacherRepository = TeacherRepository;
        _CurrentUser = CurrentUser;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<IReadOnlyList<GradeDto>> GetByClassGroupAsync(
        int ClassGroupId,
        CancellationToken Cancellation = default)
    {
        if (IsTeacherScoped())
        {
            var Group = await _GroupRepository.GetByIdAsync(ClassGroupId, Cancellation)
                ?? throw new NotFoundException("ClassGroups.NotFound");

            await EnsureAssignedTeacherAsync(Group, Cancellation);
        }

        return await ListAsync(ClassGroupId, Cancellation);
    }

    public async Task<IReadOnlyList<GradeDto>> SaveAsync(
        int ClassGroupId,
        SaveGradesRequest Request,
        CancellationToken Cancellation = default)
    {
        var Group = await _GroupRepository.GetByIdAsync(ClassGroupId, Cancellation)
            ?? throw new NotFoundException("ClassGroups.NotFound");

        if (IsTeacherScoped())
        {
            await EnsureAssignedTeacherAsync(Group, Cancellation);
        }

        if (!string.Equals(Group.Status, "Open", StringComparison.OrdinalIgnoreCase))
        {
            throw BusinessRuleException.Conflict("Grades.GroupNotOpen");
        }

        foreach (var Item in Request.Grades)
        {
            var Existing = await _GradeRepository.GetByEnrollmentAsync(
                Item.EnrollmentId,
                ClassGroupId,
                Cancellation);

            if (Existing is not null && Existing.IsReportClosed)
            {
                throw BusinessRuleException.Conflict("Grades.ReportClosed");
            }

            if (Existing is null)
            {
                await _GradeRepository.AddAsync(
                    new Grade
                    {
                        EnrollmentId = Item.EnrollmentId,
                        ClassGroupId = ClassGroupId,
                        Partial1 = Item.Partial1,
                        Partial2 = Item.Partial2,
                        Partial3 = Item.Partial3,
                        Ordinary = Item.Ordinary,
                        FinalScore = Item.FinalScore,
                        AttemptNumber = 1,
                        EvaluationType = "Ordinary",
                        Status = Item.Status,
                        IsReportClosed = false,
                    },
                    Cancellation);
                continue;
            }

            Existing.Partial1 = Item.Partial1;
            Existing.Partial2 = Item.Partial2;
            Existing.Partial3 = Item.Partial3;
            Existing.Ordinary = Item.Ordinary;
            Existing.FinalScore = Item.FinalScore;
            Existing.Status = Item.Status;

            _GradeRepository.Update(Existing);
        }

        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Grades",
            nameof(Grade),
            ClassGroupId.ToString(CultureInfo.InvariantCulture),
            null,
            new { ClassGroupId, Count = Request.Grades.Count },
            Cancellation);

        return await ListAsync(ClassGroupId, Cancellation);
    }

    private async Task<IReadOnlyList<GradeDto>> ListAsync(int ClassGroupId, CancellationToken Cancellation)
    {
        var Grades = await _GradeRepository.GetByClassGroupAsync(ClassGroupId, Cancellation);
        return Grades.Select(ToDto).ToList();
    }

    // Solo se acota a quien es Teacher sin un rol con alcance sobre todos los grupos
    private bool IsTeacherScoped()
        => _CurrentUser.IsInRole(TeacherRoleName)
            && !UnrestrictedRoleNames.Any(_CurrentUser.IsInRole);

    // El grupo debe estar asignado al docente vinculado al usuario actual (Teachers.UserId)
    private async Task EnsureAssignedTeacherAsync(ClassGroup Group, CancellationToken Cancellation)
    {
        var Teacher = _CurrentUser.UserId is long UserId
            ? await _TeacherRepository.GetByUserIdAsync(UserId, Cancellation)
            : null;

        if (Teacher is null || Teacher.Id != Group.TeacherId)
        {
            throw new BusinessRuleException(
                "Grades.ClassGroupNotAssigned",
                StatusCode: ForbiddenStatusCode);
        }
    }

    private static GradeDto ToDto(Grade Grade)
        => new()
        {
            Id = Grade.Id,
            EnrollmentId = Grade.EnrollmentId,
            ClassGroupId = Grade.ClassGroupId,
            Partial1 = Grade.Partial1,
            Partial2 = Grade.Partial2,
            Partial3 = Grade.Partial3,
            Ordinary = Grade.Ordinary,
            FinalScore = Grade.FinalScore,
            AttemptNumber = Grade.AttemptNumber,
            EvaluationType = Grade.EvaluationType,
            Status = Grade.Status,
            IsReportClosed = Grade.IsReportClosed,
        };
}
