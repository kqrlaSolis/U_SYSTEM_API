// Entidad de la tabla Charges.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Charge : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Periodo
    public int AcademicTermId { get; set; }

    // Concepto
    public int PaymentConceptId { get; set; }

    // Monto
    public decimal Amount { get; set; }

    // Descuento
    public decimal Discount { get; set; }

    // Recargo
    public decimal Surcharge { get; set; }

    // Fecha de vencimiento
    public DateOnly DueDate { get; set; }

    // Pending, Partial, Paid, Overdue, Cancelled
    public string Status { get; set; } = string.Empty;
}
