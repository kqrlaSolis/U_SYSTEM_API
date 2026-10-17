// Acceso a datos de los planes de estudio, su malla y su seriación.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IStudyPlanRepository : IRepository<StudyPlan>
{
    Task<StudyPlan?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<bool> ExistsByCodeVersionAsync(
        int CareerId,
        string Code,
        string Version,
        int? ExcludePlanId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    Task<bool> HasStudentsAsync(int StudyPlanId, CancellationToken Cancellation = default);

    Task<StudyPlan?> GetCurrentByCareerAsync(int CareerId, CancellationToken Cancellation = default);

    // Marca el plan como vigente y quita la vigencia de los demás de la carrera
    Task ClearCurrentAsync(int CareerId, int? KeepPlanId = null, CancellationToken Cancellation = default);

    Task<IReadOnlyList<StudyPlanSubject>> GetSubjectsAsync(
        int StudyPlanId,
        CancellationToken Cancellation = default);

    Task<IReadOnlyList<SubjectPrerequisite>> GetPrerequisitesAsync(
        int StudyPlanId,
        CancellationToken Cancellation = default);

    Task ReplaceSubjectsAsync(
        int StudyPlanId,
        IReadOnlyCollection<StudyPlanSubject> Subjects,
        CancellationToken Cancellation = default);

    Task ReplacePrerequisitesAsync(
        int StudyPlanId,
        IReadOnlyCollection<SubjectPrerequisite> Prerequisites,
        CancellationToken Cancellation = default);
}
