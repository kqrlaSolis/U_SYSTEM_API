// Entidad de la tabla Subjects.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Subject : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave de la materia
    public string Code { get; set; } = string.Empty;

    // Nombre
    public string Name { get; set; } = string.Empty;

    // Créditos
    public byte Credits { get; set; }

    // Horas teóricas
    public byte TheoryHours { get; set; }

    // Horas prácticas
    public byte PracticeHours { get; set; }

    // Activa
    public bool IsActive { get; set; }
}
