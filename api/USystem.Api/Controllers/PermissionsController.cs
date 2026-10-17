// Catálogo de permisos por módulo (solo lectura: lo define la semilla).
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Attributes;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Services;
using USystem.Domain.Constants;

namespace USystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IRoleService _RoleService;

    public PermissionsController(IRoleService RoleService) => _RoleService = RoleService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Roles.View)]
    public async Task<ActionResult<ApiResponse<List<PermissionModuleDto>>>> GetCatalog(
        CancellationToken Cancellation)
    {
        var Catalog = await _RoleService.GetPermissionCatalogAsync(Cancellation);
        return Ok(ApiResponse<List<PermissionModuleDto>>.Ok(Catalog.ToList()));
    }
}
