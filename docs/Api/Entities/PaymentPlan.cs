// Entidad de la tabla PaymentPlans.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class PaymentPlan : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Carrera (opcional)
    public int? CareerId { get; set; }

    // Periodo
    public int AcademicTermId { get; set; }

    // Nombre del plan
    public string Name { get; set; } = string.Empty;

    // Número de colegiaturas
    public byte InstallmentCount { get; set; }

    // Activo
    public bool IsActive { get; set; }
}
