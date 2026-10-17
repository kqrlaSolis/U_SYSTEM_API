// Contrato de POST /api/v1/auth/reset-password con el token de un solo uso.
namespace USystem.Application.Dtos;

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}
