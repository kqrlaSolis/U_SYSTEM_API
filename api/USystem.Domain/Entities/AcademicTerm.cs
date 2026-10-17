// Entidad de la tabla AcademicTerms.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class AcademicTerm : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave YYYY-N
    public string Code { get; set; } = string.Empty;

    // Nombre del periodo
    public string Name { get; set; } = string.Empty;

    // Año académico
    public short AcademicYear { get; set; }

    // 1, 2 o 3
    public byte TermNumber { get; set; }

    // Inicio del periodo
    public DateOnly StartDate { get; set; }

    // Fin del periodo
    public DateOnly EndDate { get; set; }

    // Inicio de inscripciones
    public DateOnly EnrollmentStartDate { get; set; }

    // Fin de inscripciones
    public DateOnly EnrollmentEndDate { get; set; }

    // Inicio de reinscripciones
    public DateOnly ReEnrollmentStartDate { get; set; }

    // Fin de reinscripciones
    public DateOnly ReEnrollmentEndDate { get; set; }

    // Inicio de exámenes
    public DateOnly ExamStartDate { get; set; }

    // Fin de exámenes
    public DateOnly ExamEndDate { get; set; }

    // Fecha de cierre de actas
    public DateOnly GradeReportCloseDate { get; set; }

    // Planned, Open, InProgress, Closed
    public string Status { get; set; } = string.Empty;
}
