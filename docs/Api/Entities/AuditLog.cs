// Entidad de la tabla AuditLogs.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class AuditLog : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Usuario que ejecutó la acción
    public long? UserId { get; set; }

    // Acción (Create, Update, Delete, Login)
    public string Action { get; set; } = string.Empty;

    // Módulo afectado
    public string Module { get; set; } = string.Empty;

    // Entidad afectada
    public string Entity { get; set; } = string.Empty;

    // Identificador del registro
    public string? EntityId { get; set; }

    // Valores anteriores
    public string? OldValue { get; set; }

    // Valores nuevos
    public string? NewValue { get; set; }

    // IP de origen
    public string? IpAddress { get; set; }
}
