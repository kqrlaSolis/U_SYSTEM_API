// Contrato de POST /api/v1/subjects.
namespace USystem.Application.Dtos;

public class CreateSubjectRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public byte Credits { get; set; }

    public byte TheoryHours { get; set; }

    public byte PracticeHours { get; set; }
}
