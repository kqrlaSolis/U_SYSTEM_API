// Acceso a datos de calificaciones.
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IGradeRepository
{
    Task<IReadOnlyList<Grade>> GetByClassGroupAsync(int ClassGroupId, CancellationToken Cancellation = default);

    Task<Grade?> GetByEnrollmentAsync(long EnrollmentId, int ClassGroupId, CancellationToken Cancellation = default);

    Task<IReadOnlyList<Grade>> GetByEnrollmentIdAsync(long EnrollmentId, CancellationToken Cancellation = default);

    Task AddAsync(Grade Entity, CancellationToken Cancellation = default);

    void Update(Grade Entity);
}
