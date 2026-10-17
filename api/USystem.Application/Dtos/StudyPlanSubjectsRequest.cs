// Contrato de PUT /api/v1/study-plans/{Id}/subjects: reemplaza la malla completa.
namespace USystem.Application.Dtos;

public class StudyPlanSubjectsRequest
{
    public List<StudyPlanSubjectItem> Subjects { get; set; } = new();
}

public class StudyPlanSubjectItem
{
    public int SubjectId { get; set; }

    public byte TermNumber { get; set; }

    // Mandatory o Elective
    public string Type { get; set; } = string.Empty;
}
