// Reinscripción: propuesta de materias elegibles para el siguiente periodo.
namespace USystem.Application.Dtos;

public class EligibleSubjectDto
{
    public int SubjectId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public byte Credits { get; set; }

    public byte TermNumber { get; set; }

    // Motivo cuando la materia no es elegible (seriación pendiente o ya aprobada)
    public string? BlockedReason { get; set; }

    public bool IsEligible { get; set; }
}

public class ReEnrollmentProposalDto
{
    public long StudentId { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int StudyPlanId { get; set; }

    public int NextTermNumber { get; set; }

    public List<EligibleSubjectDto> Subjects { get; set; } = new();
}

public class ProposeReEnrollmentRequest
{
    public long StudentId { get; set; }
}
