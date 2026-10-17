// Entidad de la tabla Grades.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Grade : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Inscripción
    public long EnrollmentId { get; set; }

    // Grupo
    public int ClassGroupId { get; set; }

    // Parcial 1
    public decimal? Partial1 { get; set; }

    // Parcial 2
    public decimal? Partial2 { get; set; }

    // Parcial 3
    public decimal? Partial3 { get; set; }

    // Calificación ordinaria
    public decimal? Ordinary { get; set; }

    // Calificación final
    public decimal? FinalScore { get; set; }

    // Número de intento (ordinario, extraordinario, regularización)
    public byte AttemptNumber { get; set; }

    // Ordinary, Extraordinary, Remedial
    public string EvaluationType { get; set; } = string.Empty;

    // InProgress, Passed, Failed, NoRight, NotPresented
    public string Status { get; set; } = string.Empty;

    // Acta cerrada
    public bool IsReportClosed { get; set; }

    // Fecha de cierre
    public DateTime? ClosedAt { get; set; }

    // Usuario que cerró el acta
    public long? ClosedBy { get; set; }
}
