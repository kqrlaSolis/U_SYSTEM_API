// Entidad de la tabla CourseMaterials.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class CourseMaterial : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Materia
    public int SubjectId { get; set; }

    // Periodo (opcional)
    public int? AcademicTermId { get; set; }

    // Syllabus, Outline, Bibliography, File, Link
    public string Type { get; set; } = string.Empty;

    // Título
    public string Title { get; set; } = string.Empty;

    // Descripción
    public string? Description { get; set; }

    // Enlace externo
    public string? Url { get; set; }

    // Ruta del archivo
    public string? FilePath { get; set; }

    // Activo
    public bool IsActive { get; set; }
}
