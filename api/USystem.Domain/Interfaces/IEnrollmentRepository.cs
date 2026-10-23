// Acceso a datos de inscripciones.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<bool> ExistsForStudentTermAsync(
        long StudentId,
        int AcademicTermId,
        CancellationToken Cancellation = default);

    Task<IReadOnlyList<EnrollmentClassGroup>> GetClassGroupsAsync(
        long EnrollmentId,
        CancellationToken Cancellation = default);

    Task AddClassGroupAsync(EnrollmentClassGroup Item, CancellationToken Cancellation = default);

    Task<int> CountInGroupAsync(int ClassGroupId, CancellationToken Cancellation = default);
}
