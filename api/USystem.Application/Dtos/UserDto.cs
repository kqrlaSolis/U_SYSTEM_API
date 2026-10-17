// Datos del usuario que comparten el login, el perfil y los listados.
namespace USystem.Application.Dtos;

public class UserDto
{
    public long Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    // Nombre completo ya armado para la UI
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool IsActive { get; set; }

    public bool MustChangePassword { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> Roles { get; set; } = new();

    // Permisos efectivos; solo se llenan en el detalle, el login y el perfil
    public List<string> Permissions { get; set; } = new();
}
