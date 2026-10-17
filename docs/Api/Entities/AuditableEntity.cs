// Campos de auditoría comunes a todas las tablas.
using System;

namespace USystem.Domain.Entities;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}
