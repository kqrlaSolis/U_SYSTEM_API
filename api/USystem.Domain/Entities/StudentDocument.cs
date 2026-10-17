// Entidad de la tabla StudentDocuments.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class StudentDocument : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Tipo de documento
    public int DocumentTypeId { get; set; }

    // Pending, Delivered, Validated, Rejected
    public string Status { get; set; } = string.Empty;

    // Ruta del archivo
    public string? FilePath { get; set; }

    // Observaciones
    public string? Notes { get; set; }

    // Usuario que validó
    public long? ValidatedBy { get; set; }

    // Fecha de validación
    public DateTime? ValidatedAt { get; set; }
}
