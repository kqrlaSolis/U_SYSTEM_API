// Avance académico del alumno frente a su plan de estudios.
namespace USystem.Application.Dtos;

public class AcademicProgressDto
{
    public long StudentId { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int StudyPlanId { get; set; }

    public short PlanTotalCredits { get; set; }

    public short ApprovedCredits { get; set; }

    public decimal ProgressPercent { get; set; }

    public int PassedSubjects { get; set; }

    public int FailedSubjects { get; set; }
}
