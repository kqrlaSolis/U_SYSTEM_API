// Contrato de POST /api/v1/academic-terms.
namespace USystem.Application.Dtos;

public class CreateAcademicTermRequest
{
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
}
