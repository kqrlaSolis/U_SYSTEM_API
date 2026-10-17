// Contrato de PUT /api/v1/study-plans/{Id}/prerequisites: reemplaza la seriación completa.
namespace USystem.Application.Dtos;

public class StudyPlanPrerequisitesRequest
{
    public List<StudyPlanPrerequisiteItem> Prerequisites { get; set; } = new();
}

public class StudyPlanPrerequisiteItem
{
    public int SubjectId { get; set; }

    public int PrerequisiteSubjectId { get; set; }
}
