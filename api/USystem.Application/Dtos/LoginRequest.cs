// Contrato de POST /api/v1/auth/login. Acepta usuario o correo.
namespace USystem.Application.Dtos;

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
