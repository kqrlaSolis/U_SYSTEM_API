// Contrato de PUT /api/v1/users/{Id}. Los roles se asignan aparte (Roles.Assign).
namespace USystem.Application.Dtos;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string? Phone { get; set; }
}
