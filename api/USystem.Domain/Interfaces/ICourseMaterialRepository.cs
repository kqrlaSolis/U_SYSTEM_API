// Acceso a datos del material didáctico.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ICourseMaterialRepository : IRepository<CourseMaterial>
{
    Task<CourseMaterial?> GetByIdAsync(int Id, CancellationToken Cancellation = default);
}
