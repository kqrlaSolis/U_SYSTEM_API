// Consulta de la bitácora (sin endpoints de modificación: es solo de inserción).
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Attributes;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Services;
using USystem.Domain.Common;
using USystem.Domain.Constants;

namespace USystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/audit-logs")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _AuditLogService;

    public AuditLogsController(IAuditLogService AuditLogService) => _AuditLogService = AuditLogService;

    [HttpGet]
    [RequirePermission(PermissionCodes.AuditLogs.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditLogDto>>>> GetPaged(
        [FromQuery] AuditLogQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<AuditLogDto>>.Ok(
            await _AuditLogService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission(PermissionCodes.AuditLogs.View)]
    public async Task<ActionResult<ApiResponse<AuditLogDto>>> GetById(
        long Id,
        CancellationToken Cancellation)
        => Ok(ApiResponse<AuditLogDto>.Ok(await _AuditLogService.GetByIdAsync(Id, Cancellation)));
}
