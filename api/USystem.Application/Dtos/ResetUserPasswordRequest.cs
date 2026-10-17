// Contrato de POST /api/v1/users/{Id}/reset-password.
namespace USystem.Application.Dtos;

public class ResetUserPasswordRequest
{
    // Opcional: si no se envía, la API genera una contraseña temporal
    public string? TemporaryPassword { get; set; }
}
