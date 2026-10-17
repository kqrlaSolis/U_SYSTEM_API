// Entidad de la tabla NotificationTemplates.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class NotificationTemplate : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave de la plantilla
    public string Code { get; set; } = string.Empty;

    // Asunto
    public string SubjectLine { get; set; } = string.Empty;

    // Cuerpo con variables {{StudentName}}
    public string Body { get; set; } = string.Empty;

    // Activa
    public bool IsActive { get; set; }
}
