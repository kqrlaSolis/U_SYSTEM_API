// Contrato de PUT /api/v1/subjects/{Id}.
namespace USystem.Application.Dtos;

public class UpdateSubjectRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public byte Credits { get; set; }

    public byte TheoryHours { get; set; }

    public byte PracticeHours { get; set; }

    public bool IsActive { get; set; } = true;
}
