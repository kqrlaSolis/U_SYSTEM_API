// Acceso a datos del catálogo de carreras.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ICareerRepository : IRepository<Career>
{
    Task<Career?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    // Varias carreras a la vez, para enriquecer listados de otros módulos
    Task<IReadOnlyList<Career>> GetByIdsAsync(
        IReadOnlyCollection<int> CareerIds,
        CancellationToken Cancellation = default);

    Task<bool> ExistsByCodeAsync(
        string Code,
        int? ExcludeCareerId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    // Una carrera con planes o alumnos solo se desactiva
    Task<bool> HasStudyPlansAsync(int CareerId, CancellationToken Cancellation = default);

    Task<bool> HasStudentsAsync(int CareerId, CancellationToken Cancellation = default);
}
