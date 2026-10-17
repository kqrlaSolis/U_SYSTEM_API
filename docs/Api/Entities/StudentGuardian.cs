// Entidad de la tabla StudentGuardians.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class StudentGuardian : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Alumno
    public long StudentId { get; set; }

    // Nombre completo del tutor
    public string FullName { get; set; } = string.Empty;

    // Parentesco
    public string Relationship { get; set; } = string.Empty;

    // Teléfono
    public string? Phone { get; set; }

    // Correo
    public string? Email { get; set; }
}
