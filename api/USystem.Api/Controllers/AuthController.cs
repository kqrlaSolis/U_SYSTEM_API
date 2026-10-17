// Autenticación: login, refresh, perfil, cambio y recuperación de contraseña.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Services;
using USystem.Domain.Interfaces;

namespace USystem.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _AuthService;
    private readonly IUserService _UserService;
    private readonly ICurrentUser _CurrentUser;

    public AuthController(
        IAuthService AuthService,
        IUserService UserService,
        ICurrentUser CurrentUser)
    {
        _AuthService = AuthService;
        _UserService = UserService;
        _CurrentUser = CurrentUser;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<LoginResponse>.Ok(await _AuthService.LoginAsync(Request, Cancellation)));

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Refresh(
        [FromBody] RefreshTokenRequest Request,
        CancellationToken Cancellation)
        => Ok(ApiResponse<LoginResponse>.Ok(
            await _AuthService.RefreshAsync(
                Request,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Cancellation)));

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout(
        [FromBody] RefreshTokenRequest Request,
        CancellationToken Cancellation)
    {
        await _AuthService.LogoutAsync(Request, CurrentUserId(), Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Me(CancellationToken Cancellation)
        => Ok(ApiResponse<UserDto>.Ok(await _UserService.GetByIdAsync(CurrentUserId(), Cancellation)));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(
        [FromBody] ChangePasswordRequest Request,
        CancellationToken Cancellation)
    {
        await _AuthService.ChangePasswordAsync(Request, CurrentUserId(), Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }

    // Siempre responde lo mismo, exista o no el correo
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword(
        [FromBody] ForgotPasswordRequest Request,
        CancellationToken Cancellation)
    {
        await _AuthService.ForgotPasswordAsync(Request, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(
        [FromBody] ResetPasswordRequest Request,
        CancellationToken Cancellation)
    {
        await _AuthService.ResetPasswordAsync(Request, Cancellation);
        return Ok(ApiResponse<bool>.Ok(true));
    }

    private long CurrentUserId()
        => _CurrentUser.UserId ?? throw new UnauthorizedAccessException();
}
