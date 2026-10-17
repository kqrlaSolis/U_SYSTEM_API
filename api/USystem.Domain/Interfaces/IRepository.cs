// Operaciones comunes de acceso a datos; las implementaciones viven en Infrastructure.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : AuditableEntity
{
    Task<PagedResult<TEntity>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task AddAsync(TEntity Entity, CancellationToken Cancellation = default);

    void Update(TEntity Entity);

    // Borrado lógico: el DbContext convierte el estado Deleted en DeletedAt
    void Remove(TEntity Entity);
}
