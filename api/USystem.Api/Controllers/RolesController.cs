// CRUD de roles (RBAC).
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
[Route("api/v1/roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _RoleService;

    public RolesController(IRoleService RoleService) => _RoleService = RoleService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Roles.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<RoleDto>>.Ok(await _RoleService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.Roles.View)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<RoleDto>.Ok(await _RoleService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Roles.Create)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Create(
        [FromBody] CreateRoleRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _RoleService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = Created.Id },
            ApiResponse<RoleDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.Roles.Edit)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Update(
        int Id,
        [FromBody] UpdateRoleRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<RoleDto>.Ok(await _RoleService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.Roles.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _RoleService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
