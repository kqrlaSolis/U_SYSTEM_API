// Plan de estudios con su carrera, malla y seriación.
namespace USystem.Application.Dtos;

public class StudyPlanDto
{
    public int Id { get; set; }

    public int CareerId { get; set; }

    public string CareerName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public short TotalCredits { get; set; }

    // Créditos realmente asignados en la malla
    public short AssignedCredits { get; set; }

    // Vigente para nuevos ingresos
    public bool IsCurrent { get; set; }

    public List<StudyPlanSubjectDto> Subjects { get; set; } = new();

    public List<SubjectPrerequisiteDto> Prerequisites { get; set; } = new();
}
