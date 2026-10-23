// Motivo de cancelación de una inscripción.
namespace USystem.Application.Dtos;

public class CancelEnrollmentRequest
{
    public string Reason { get; set; } = string.Empty;
}
