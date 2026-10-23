// Contrato de PUT /api/v1/teachers/{Id}.
namespace USystem.Application.Dtos;

public class UpdateTeacherRequest
{
    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Specialty { get; set; }

    public long? UserId { get; set; }

    public bool IsActive { get; set; }
}
