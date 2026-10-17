// Entidad de la tabla TeacherSubjects.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class TeacherSubject : AuditableEntity
{
    // Docente
    public int TeacherId { get; set; }

    // Materia habilitada
    public int SubjectId { get; set; }
}
