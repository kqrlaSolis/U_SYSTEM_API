// Rol con sus conteos y, en el detalle, sus permisos.
namespace USystem.Application.Dtos;

public class RoleDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Los roles de sistema no se eliminan
    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public int UserCount { get; set; }

    public int PermissionCount { get; set; }

    public List<string> PermissionCodes { get; set; } = new();
}
