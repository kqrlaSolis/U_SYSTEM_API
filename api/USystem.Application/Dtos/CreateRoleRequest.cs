// Contrato de POST /api/v1/roles.
namespace USystem.Application.Dtos;

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Códigos del catálogo, por ejemplo "Users.View"
    public string[] PermissionCodes { get; set; } = Array.Empty<string>();
}
