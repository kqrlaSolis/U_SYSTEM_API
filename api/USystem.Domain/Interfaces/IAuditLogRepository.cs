// Bitácora: solo inserción y consulta paginada con filtros.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<PagedResult<AuditLog>> GetPagedAsync(
        AuditLogQuery Query,
        CancellationToken Cancellation = default);

    Task AddAsync(AuditLog Entity, CancellationToken Cancellation = default);
}
