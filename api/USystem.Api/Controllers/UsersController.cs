// CRUD de usuarios del personal, estado de la cuenta y asignación de roles.
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
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _UserService;

    public UsersController(IUserService UserService) => _UserService = UserService;

    [HttpGet]
    [RequirePermission(PermissionCodes.Users.View)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetPaged(
        [FromQuery] PagedQuery Query,
        CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<UserDto>>.Ok(await _UserService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission(PermissionCodes.Users.View)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<UserDto>.Ok(await _UserService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission(PermissionCodes.Users.Create)]
    public async Task<ActionResult<ApiResponse<CreatedUserResponse>>> Create(
        [FromBody] CreateUserRequest Request,
        CancellationToken Cancellation)
    {
        var Created = await _UserService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = Created.User.Id },
            ApiResponse<CreatedUserResponse>.Ok(Created));
    }

    [HttpPut("{Id:long}")]
    [RequirePermission(PermissionCodes.Users.Edit)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(
        long Id,
        [FromBody] UpdateUserRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<UserDto>.Ok(await _UserService.UpdateAsync(Id, Request, Cancellation)));

    [HttpPatch("{Id:long}/status")]
    [RequirePermission(PermissionCodes.Users.Edit)]
    public async Task<ActionResult<ApiResponse<UserDto>>> ChangeStatus(
        long Id,
        [FromBody] ChangeUserStatusRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<UserDto>.Ok(
            await _UserService.SetStatusAsync(Id, Request.IsActive, Cancellation)));

    [HttpPost("{Id:long}/reset-password")]
    [RequirePermission(PermissionCodes.Users.Edit)]
    public async Task<ActionResult<ApiResponse<TemporaryPasswordResponse>>> ResetPassword(
        long Id,
        [FromBody] ResetUserPasswordRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<TemporaryPasswordResponse>.Ok(
            await _UserService.ResetPasswordAsync(Id, Request, Cancellation)));

    [HttpPut("{Id:long}/roles")]
    [RequirePermission(PermissionCodes.Roles.Assign)]
    public async Task<ActionResult<ApiResponse<UserDto>>> AssignRoles(
        long Id,
        [FromBody] AssignRolesRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<UserDto>.Ok(
            await _UserService.AssignRolesAsync(Id, Request.RoleIds, Cancellation)));
}
