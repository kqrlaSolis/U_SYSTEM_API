// Entidad de la tabla Scholarships.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Scholarship : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Nombre de la beca
    public string Name { get; set; } = string.Empty;

    // Percentage o Amount
    public string Type { get; set; } = string.Empty;

    // Valor de la beca
    public decimal Value { get; set; }

    // Activa
    public bool IsActive { get; set; }
}
