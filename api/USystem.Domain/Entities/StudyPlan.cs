// Entidad de la tabla StudyPlans.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class StudyPlan : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Carrera
    public int CareerId { get; set; }

    // Clave del plan
    public string Code { get; set; } = string.Empty;

    // Versión
    public string Version { get; set; } = string.Empty;

    // Inicio de vigencia
    public DateOnly StartDate { get; set; }

    // Fin de vigencia
    public DateOnly? EndDate { get; set; }

    // Créditos totales
    public short TotalCredits { get; set; }

    // Vigente para nuevos ingresos
    public bool IsCurrent { get; set; }
}
