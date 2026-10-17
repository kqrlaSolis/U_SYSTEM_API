// Entidad de la tabla Payments.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Payment : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Cargo
    public long ChargeId { get; set; }

    // Monto pagado
    public decimal Amount { get; set; }

    // Cash, Transfer, Card, Deposit
    public string Method { get; set; } = string.Empty;

    // Referencia
    public string? Reference { get; set; }

    // Fecha del pago
    public DateTime PaymentDate { get; set; }

    // Folio de recibo
    public string ReceiptNumber { get; set; } = string.Empty;

    // Usuario de caja
    public long UserId { get; set; }

    // Applied o Cancelled
    public string Status { get; set; } = string.Empty;

    // Fecha de cancelación
    public DateTime? CancelledAt { get; set; }

    // Motivo de cancelación
    public string? CancellationReason { get; set; }
}
