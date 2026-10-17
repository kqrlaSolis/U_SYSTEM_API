// Datos de la sesión autenticada que consume el desktop.
namespace USystem.Api.Contracts;

public class SessionResponse
{
    public long UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public bool IsAuthenticated { get; set; }

    public string[] Permissions { get; set; } = Array.Empty<string>();
}
