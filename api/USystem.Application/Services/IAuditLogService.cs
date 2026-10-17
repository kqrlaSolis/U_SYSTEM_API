// Consulta de la bitácora (solo lectura).
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDto>> GetPagedAsync(
        AuditLogQuery Query,
        CancellationToken Cancellation = default);

    Task<AuditLogDto> GetByIdAsync(long Id, CancellationToken Cancellation = default);
}
