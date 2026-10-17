// Entidad de la tabla StudyPlanSubjects.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class StudyPlanSubject : AuditableEntity
{
    // Plan de estudios
    public int StudyPlanId { get; set; }

    // Materia
    public int SubjectId { get; set; }

    // Cuatrimestre dentro del plan
    public byte TermNumber { get; set; }

    // Mandatory o Elective
    public string Type { get; set; } = string.Empty;
}
