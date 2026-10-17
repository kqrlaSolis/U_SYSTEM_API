// Contrato de POST /api/v1/auth/forgot-password. La respuesta es siempre genérica.
namespace USystem.Application.Dtos;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
