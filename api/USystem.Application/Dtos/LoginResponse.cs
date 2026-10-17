// Respuesta del login y del refresh: access token, refresh token y datos del usuario.
namespace USystem.Application.Dtos;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiresAt { get; set; }

    public DateTime RefreshTokenExpiresAt { get; set; }

    public bool MustChangePassword { get; set; }

    public UserDto User { get; set; } = new();
}
