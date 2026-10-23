// Acceso a datos de docentes.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<bool> ExistsByEmployeeNumberAsync(
        string EmployeeNumber,
        int? ExcludeTeacherId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    Task<bool> ExistsByEmailAsync(
        string Email,
        int? ExcludeTeacherId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    // Un docente con grupos asignados solo se desactiva
    Task<bool> HasClassGroupsAsync(int TeacherId, CancellationToken Cancellation = default);
}
