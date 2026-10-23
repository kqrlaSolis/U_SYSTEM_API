// Contrato de POST /api/v1/teachers.
namespace USystem.Application.Dtos;

public class CreateTeacherRequest
{
    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Specialty { get; set; }

    // Cuenta de usuario opcional para que el docente entre al sistema
    public long? UserId { get; set; }
}
