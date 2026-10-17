// Entidad de la tabla SubjectPrerequisites.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class SubjectPrerequisite : AuditableEntity
{
    // Plan de estudios
    public int StudyPlanId { get; set; }

    // Materia
    public int SubjectId { get; set; }

    // Materia prerrequisito
    public int PrerequisiteSubjectId { get; set; }
}
