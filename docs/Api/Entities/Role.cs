// Entidad de la tabla Roles.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Role : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Nombre del rol
    public string Name { get; set; } = string.Empty;

    // Descripción
    public string? Description { get; set; }

    // Rol de sistema no eliminable
    public bool IsSystem { get; set; }

    // Rol activo
    public bool IsActive { get; set; }
}
