// Entidad de la tabla StudentScholarships.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class StudentScholarship : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Beca
    public int ScholarshipId { get; set; }

    // Periodo
    public int AcademicTermId { get; set; }

    // Fecha de asignación
    public DateTime AssignedAt { get; set; }
}
