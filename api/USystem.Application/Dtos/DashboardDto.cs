// Indicadores del dashboard.
namespace USystem.Application.Dtos;

public class DashboardDto
{
    // Periodo en curso (null cuando no hay ninguno abierto o en progreso)
    public int? CurrentTermId { get; set; }

    public string? CurrentTermCode { get; set; }

    public string? CurrentTermName { get; set; }

    public string? CurrentTermStatus { get; set; }

    public DateOnly? CurrentTermStartDate { get; set; }

    public DateOnly? CurrentTermEndDate { get; set; }

    public int ActiveStudents { get; set; }

    public int ActiveTeachers { get; set; }

    public int OpenClassGroups { get; set; }

    public int TermEnrollments { get; set; }

    public int PendingCharges { get; set; }

    public decimal PendingAmount { get; set; }

    public int RequestedProcedures { get; set; }

    public int IssuedProcedures { get; set; }

    public int TotalEnrollments { get; set; }
}
