// Catálogo de permisos agrupado por módulo, para el editor de roles.
namespace USystem.Application.Dtos;

public class PermissionModuleDto
{
    public string Module { get; set; } = string.Empty;

    public List<PermissionDto> Permissions { get; set; } = new();
}
