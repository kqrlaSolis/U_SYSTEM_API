// Materia del catálogo académico.
namespace USystem.Application.Dtos;

public class SubjectDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public byte Credits { get; set; }

    public byte TheoryHours { get; set; }

    public byte PracticeHours { get; set; }

    public bool IsActive { get; set; }
}
