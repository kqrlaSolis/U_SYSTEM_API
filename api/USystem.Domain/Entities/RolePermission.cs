// Entidad de la tabla RolePermissions.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class RolePermission : AuditableEntity
{
    // Rol
    public int RoleId { get; set; }

    // Permiso
    public int PermissionId { get; set; }
}
