// CRUD de grupos de clase con horarios.
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
[Route("api/v1/class-groups")]
public class ClassGroupsController : ControllerBase
{
    private readonly IClassGroupService _ClassGroupService;

    public ClassGroupsController(IClassGroupService ClassGroupService)
        => _ClassGroupService = ClassGroupService;

    [HttpGet]
    [RequirePermission(PermissionCodes.ClassGroups.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<ClassGroupDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<ClassGroupDto>>.Ok(await _ClassGroupService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:int}")]
    [RequirePermission(PermissionCodes.ClassGroups.View)]
    public async Task<ActionResult<ApiResponse<ClassGroupDto>>> GetById(int Id, CancellationToken Cancellation)
        => Ok(ApiResponse<ClassGroupDto>.Ok(await _ClassGroupService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.ClassGroups.Create)]
    public async Task<ActionResult<ApiResponse<ClassGroupDto>>> Create(
        [FromBody] CreateClassGroupRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _ClassGroupService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(nameof(GetById), new { Id = Created.Id }, ApiResponse<ClassGroupDto>.Ok(Created));
    }

    [HttpPut("{Id:int}")]
    [RequirePermission(PermissionCodes.ClassGroups.Edit)]
    public async Task<ActionResult<ApiResponse<ClassGroupDto>>> Update(
        int Id,
        [FromBody] UpdateClassGroupRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<ClassGroupDto>.Ok(await _ClassGroupService.UpdateAsync(Id, Request, Cancellation)));

    [HttpDelete("{Id:int}")]
    [RequirePermission(PermissionCodes.ClassGroups.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int Id, CancellationToken Cancellation)
    {
        await _ClassGroupService.DeleteAsync(Id, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
