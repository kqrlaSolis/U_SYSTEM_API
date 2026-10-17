// Contrato de POST /api/v1/careers.
namespace USystem.Application.Dtos;

public class CreateCareerRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public byte TermCount { get; set; }

    public short TotalCredits { get; set; }

    public string Modality { get; set; } = string.Empty;
}
