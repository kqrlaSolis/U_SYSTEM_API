// Entidad de la tabla EnrollmentClassGroups.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class EnrollmentClassGroup : AuditableEntity
{
    // Inscripción
    public long EnrollmentId { get; set; }

    // Grupo
    public int ClassGroupId { get; set; }
}
