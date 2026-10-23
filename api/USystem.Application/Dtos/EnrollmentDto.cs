// Inscripción de un alumno con los grupos asignados.
namespace USystem.Application.Dtos;

public class EnrollmentDto
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public int AcademicTermId { get; set; }

    public int CareerId { get; set; }

    // Enrollment / ReEnrollment
    public string Type { get; set; } = string.Empty;

    // Draft, Confirmed, Cancelled
    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public List<int> ClassGroupIds { get; set; } = new();
}

public class CreateEnrollmentRequest
{
    public long StudentId { get; set; }

    public int AcademicTermId { get; set; }

    public int CareerId { get; set; }

    public string? Notes { get; set; }

    // Grupos que se asignan al alumno en el periodo
    public List<int> ClassGroupIds { get; set; } = new();
}
