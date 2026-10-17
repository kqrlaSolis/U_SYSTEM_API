// Contrato de POST /api/v1/study-plans.
namespace USystem.Application.Dtos;

public class CreateStudyPlanRequest
{
    public int CareerId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public short TotalCredits { get; set; }
}
