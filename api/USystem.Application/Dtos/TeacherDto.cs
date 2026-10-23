// Docente con sus datos de contacto y vínculo opcional a una cuenta de usuario.
namespace USystem.Application.Dtos;

public class TeacherDto
{
    public int Id { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Specialty { get; set; }

    public long? UserId { get; set; }

    public bool IsActive { get; set; }
}
