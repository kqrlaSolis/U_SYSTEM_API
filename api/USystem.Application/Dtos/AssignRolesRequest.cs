// Contrato de PUT /api/v1/users/{Id}/roles (permiso Roles.Assign).
namespace USystem.Application.Dtos;

public class AssignRolesRequest
{
    public int[] RoleIds { get; set; } = Array.Empty<int>();
}
