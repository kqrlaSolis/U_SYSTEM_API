// Periodo cuatrimestral con su calendario y estado.
namespace USystem.Application.Dtos;

public class AcademicTermDto
{
    public int Id { get; set; }

    // Formato YYYY-N (N = 1, 2 o 3)
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public short AcademicYear { get; set; }

    public byte TermNumber { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateOnly EnrollmentStartDate { get; set; }

    public DateOnly EnrollmentEndDate { get; set; }

    public DateOnly ReEnrollmentStartDate { get; set; }

    public DateOnly ReEnrollmentEndDate { get; set; }

    public DateOnly ExamStartDate { get; set; }

    public DateOnly ExamEndDate { get; set; }

    public DateOnly GradeReportCloseDate { get; set; }

    // Planned, Open, InProgress, Closed
    public string Status { get; set; } = string.Empty;
}
