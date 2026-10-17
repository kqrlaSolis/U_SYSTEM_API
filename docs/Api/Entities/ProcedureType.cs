// Entidad de la tabla ProcedureTypes.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class ProcedureType : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave del trámite
    public string Code { get; set; } = string.Empty;

    // Nombre
    public string Name { get; set; } = string.Empty;

    // Cobro asociado (opcional)
    public int? PaymentConceptId { get; set; }

    // Activo
    public bool IsActive { get; set; }
}
