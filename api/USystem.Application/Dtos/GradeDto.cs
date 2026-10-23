// Calificaciones por grupo (captura masiva).
namespace USystem.Application.Dtos;

public class GradeDto
{
    public long Id { get; set; }

    public long EnrollmentId { get; set; }

    public int ClassGroupId { get; set; }

    public decimal? Partial1 { get; set; }

    public decimal? Partial2 { get; set; }

    public decimal? Partial3 { get; set; }

    public decimal? Ordinary { get; set; }

    public decimal? FinalScore { get; set; }

    public byte AttemptNumber { get; set; }

    // Ordinary, Extraordinary, Remedial
    public string EvaluationType { get; set; } = string.Empty;

    // InProgress, Passed, Failed, NoRight, NotPresented
    public string Status { get; set; } = string.Empty;

    public bool IsReportClosed { get; set; }
}

public class GradeItemRequest
{
    public long EnrollmentId { get; set; }

    public decimal? Partial1 { get; set; }

    public decimal? Partial2 { get; set; }

    public decimal? Partial3 { get; set; }

    public decimal? Ordinary { get; set; }

    public decimal? FinalScore { get; set; }

    public string Status { get; set; } = "InProgress";
}

public class SaveGradesRequest
{
    public List<GradeItemRequest> Grades { get; set; } = new();
}
