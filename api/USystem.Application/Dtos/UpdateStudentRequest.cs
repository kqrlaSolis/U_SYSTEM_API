// Contrato de PUT /api/v1/students/{Id}.
namespace USystem.Application.Dtos;

public class UpdateStudentRequest
{
    public int CareerId { get; set; }

    public int StudyPlanId { get; set; }

    public int EnrollmentTermId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string Curp { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? AddressLine { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    // Active, Inactive, Graduated, Dropout
    public string Status { get; set; } = string.Empty;
}
