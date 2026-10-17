// Entidad de la tabla Enrollments.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Enrollment : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Periodo
    public int AcademicTermId { get; set; }

    // Carrera
    public int CareerId { get; set; }

    // Enrollment o ReEnrollment
    public string Type { get; set; } = string.Empty;

    // Active o Cancelled
    public string Status { get; set; } = string.Empty;

    // Observaciones
    public string? Notes { get; set; }
}
