// Contrato de POST /api/v1/auth/change-password (usuario autenticado).
namespace USystem.Application.Dtos;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}
