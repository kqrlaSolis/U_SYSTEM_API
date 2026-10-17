// Entidad de la tabla Permissions.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Permission : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Código Module.Action
    public string Code { get; set; } = string.Empty;

    // Módulo al que pertenece
    public string Module { get; set; } = string.Empty;

    // Descripción
    public string Description { get; set; } = string.Empty;
}
