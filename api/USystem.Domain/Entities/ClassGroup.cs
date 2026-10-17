// Entidad de la tabla ClassGroups.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class ClassGroup : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Periodo
    public int AcademicTermId { get; set; }

    // Materia
    public int SubjectId { get; set; }

    // Docente
    public int TeacherId { get; set; }

    // Clave del grupo
    public string Code { get; set; } = string.Empty;

    // Aula
    public string? Classroom { get; set; }

    // Cupo
    public short Capacity { get; set; }

    // Inscritos
    public short EnrolledCount { get; set; }

    // Open, Closed, Cancelled
    public string Status { get; set; } = string.Empty;
}
