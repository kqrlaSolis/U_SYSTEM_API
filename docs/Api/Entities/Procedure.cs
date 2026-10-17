// Entidad de la tabla Procedures.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Procedure : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Tipo de trámite
    public int ProcedureTypeId { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Folio consecutivo
    public string FolioNumber { get; set; } = string.Empty;

    // Requested, InProcess, Issued, Delivered, Cancelled
    public string Status { get; set; } = string.Empty;

    // Fecha de solicitud
    public DateTime RequestedAt { get; set; }

    // Fecha de emisión
    public DateTime? IssuedAt { get; set; }

    // Ruta del PDF emitido
    public string? FilePath { get; set; }

    // Usuario que atiende
    public long UserId { get; set; }
}
