// Reglas de calificaciones: captura masiva por grupo y acta cerrada bloquea cambios.
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
    private readonly IGradeRepository _GradeRepository;
    private readonly IClassGroupRepository _GroupRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public GradeService(
        IGradeRepository GradeRepository,
        IClassGroupRepository GroupRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _GradeRepository = GradeRepository;
        _GroupRepository = GroupRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<IReadOnlyList<GradeDto>> GetByClassGroupAsync(
        int ClassGroupId,
        CancellationToken Cancellation = default)
    {
        var Grades = await _GradeRepository.GetByClassGroupAsync(ClassGroupId, Cancellation);
        return Grades.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<GradeDto>> SaveAsync(
        int ClassGroupId,
        SaveGradesRequest Request,
        CancellationToken Cancellation = default)
    {
        var Group = await _GroupRepository.GetByIdAsync(ClassGroupId, Cancellation)
            ?? throw new NotFoundException("ClassGroups.NotFound");

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

        return await GetByClassGroupAsync(ClassGroupId, Cancellation);
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
