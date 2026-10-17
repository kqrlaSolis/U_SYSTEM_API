// Contrato de POST /api/v1/auth/refresh y de /api/v1/auth/logout.
namespace USystem.Application.Dtos;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
