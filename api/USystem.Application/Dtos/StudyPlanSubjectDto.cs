// Materia dentro de la malla de un plan de estudios.
namespace USystem.Application.Dtos;

public class StudyPlanSubjectDto
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public byte Credits { get; set; }

    // Cuatrimestre en el que se cursa
    public byte TermNumber { get; set; }

    // Mandatory o Elective
    public string Type { get; set; } = string.Empty;
}
