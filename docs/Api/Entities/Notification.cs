// Entidad de la tabla Notifications.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Notification : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Plantilla
    public int TemplateId { get; set; }

    // Alumno (opcional)
    public long? StudentId { get; set; }

    // Destinatario
    public string RecipientEmail { get; set; } = string.Empty;

    // Pending, Sent, Error
    public string Status { get; set; } = string.Empty;

    // Fecha de envío
    public DateTime? SentAt { get; set; }

    // Error de SMTP
    public string? ErrorMessage { get; set; }

    // Reintentos
    public byte RetryCount { get; set; }
}
