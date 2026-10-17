// Acceso a datos del catálogo de materias.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<Subject?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    // Varias materias a la vez, para enriquecer la malla de un plan
    Task<IReadOnlyList<Subject>> GetByIdsAsync(
        IReadOnlyCollection<int> SubjectIds,
        CancellationToken Cancellation = default);

    Task<bool> ExistsByCodeAsync(
        string Code,
        int? ExcludeSubjectId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    // Una materia usada en un plan o en un grupo solo se desactiva
    Task<bool> IsUsedInStudyPlansAsync(int SubjectId, CancellationToken Cancellation = default);

    Task<bool> IsUsedInClassGroupsAsync(int SubjectId, CancellationToken Cancellation = default);
}
