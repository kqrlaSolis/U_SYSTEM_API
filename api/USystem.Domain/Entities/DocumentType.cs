// Entidad de la tabla DocumentTypes.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class DocumentType : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Nombre del documento
    public string Name { get; set; } = string.Empty;

    // Obligatorio
    public bool IsMandatory { get; set; }

    // Nivel educativo al que aplica
    public string? AppliesToLevel { get; set; }
}
