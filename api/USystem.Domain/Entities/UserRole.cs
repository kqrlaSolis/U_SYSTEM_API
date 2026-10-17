// Entidad de la tabla UserRoles.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class UserRole : AuditableEntity
{
    // Usuario
    public long UserId { get; set; }

    // Rol
    public int RoleId { get; set; }
}
