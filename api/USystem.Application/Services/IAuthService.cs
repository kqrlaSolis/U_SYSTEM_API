// Autenticación: login, refresh, cierre de sesión, recuperación y cambio de contraseña.
using USystem.Application.Dtos;

namespace USystem.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest Request, CancellationToken Cancellation = default);

    Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest Request,
        string? IpAddress = null,
        CancellationToken Cancellation = default);

    Task LogoutAsync(
        RefreshTokenRequest Request,
        long UserId,
        CancellationToken Cancellation = default);

    Task ChangePasswordAsync(
        ChangePasswordRequest Request,
        long UserId,
        CancellationToken Cancellation = default);

    // La respuesta es genérica exista o no la cuenta, para no filtrar correos
    Task ForgotPasswordAsync(ForgotPasswordRequest Request, CancellationToken Cancellation = default);

    Task ResetPasswordAsync(ResetPasswordRequest Request, CancellationToken Cancellation = default);
}
