// Contrato de PUT /api/v1/careers/{Id}.
namespace USystem.Application.Dtos;

public class UpdateCareerRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public byte TermCount { get; set; }

    public short TotalCredits { get; set; }

    public string Modality { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
