// Entidad de la tabla PaymentConcepts.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class PaymentConcept : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave del concepto
    public string Code { get; set; } = string.Empty;

    // Nombre
    public string Name { get; set; } = string.Empty;

    // Monto base
    public decimal BaseAmount { get; set; }

    // Recurrente
    public bool IsRecurring { get; set; }

    // Activo
    public bool IsActive { get; set; }
}
