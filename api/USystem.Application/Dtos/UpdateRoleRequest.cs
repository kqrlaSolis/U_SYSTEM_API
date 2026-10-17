// Contrato de PUT /api/v1/roles/{Id}. Los permisos se reemplazan completos.
namespace USystem.Application.Dtos;

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public string[] PermissionCodes { get; set; } = Array.Empty<string>();
}
