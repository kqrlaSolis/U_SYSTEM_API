// Seriación: una materia exige otra dentro del mismo plan.
namespace USystem.Application.Dtos;

public class SubjectPrerequisiteDto
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public int PrerequisiteSubjectId { get; set; }

    public string PrerequisiteSubjectCode { get; set; } = string.Empty;
}
