// Contrato de POST /api/v1/users: el administrador crea la cuenta y asigna roles.
namespace USystem.Application.Dtos;

public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string? Phone { get; set; }

    // Contraseña temporal; si no se envía, la API genera una y la devuelve una sola vez
    public string? TemporaryPassword { get; set; }

    public int[] RoleIds { get; set; } = Array.Empty<int>();
}
