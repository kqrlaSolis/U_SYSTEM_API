// Acceso a datos de grupos y horarios.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IClassGroupRepository : IRepository<ClassGroup>
{
    Task<ClassGroup?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<IReadOnlyList<ClassGroupSchedule>> GetSchedulesAsync(
        int ClassGroupId,
        CancellationToken Cancellation = default);

    Task<bool> ExistsByCodeAsync(
        int AcademicTermId,
        string Code,
        int? ExcludeClassGroupId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    // Un grupo con alumnos inscritos solo se cierra
    Task<bool> HasEnrollmentsAsync(int ClassGroupId, CancellationToken Cancellation = default);

    void RemoveSchedules(IEnumerable<ClassGroupSchedule> Schedules);

    Task AddScheduleAsync(ClassGroupSchedule Schedule, CancellationToken Cancellation = default);
}
