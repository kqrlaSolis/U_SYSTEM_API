// Contrato de POST /api/v1/students. La matrícula la genera la API.
namespace USystem.Application.Dtos;

public class CreateStudentRequest
{
    public int CareerId { get; set; }

    public int StudyPlanId { get; set; }

    public int EnrollmentTermId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string Curp { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    // M, F, Other
    public string Gender { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? AddressLine { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }
}
