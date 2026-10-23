// Alumno con matrícula, carrera, plan y datos personales.
using USystem.Domain.Entities;

namespace USystem.Application.Dtos;

public class StudentDto
{
    public long Id { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public int CareerId { get; set; }

    public string CareerName { get; set; } = string.Empty;

    public int StudyPlanId { get; set; }

    public int EnrollmentTermId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string PaternalLastName { get; set; } = string.Empty;

    public string? MaternalLastName { get; set; }

    public string FullName { get; set; } = string.Empty;

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

    public static StudentDto FromEntity(Student Student, string CareerName)
        => new()
        {
            Id = Student.Id,
            StudentNumber = Student.StudentNumber,
            CareerId = Student.CareerId,
            CareerName = CareerName,
            StudyPlanId = Student.StudyPlanId,
            EnrollmentTermId = Student.EnrollmentTermId,
            FirstName = Student.FirstName,
            PaternalLastName = Student.PaternalLastName,
            MaternalLastName = Student.MaternalLastName,
            FullName = string.Join(
                ' ',
                new[] { Student.FirstName, Student.PaternalLastName, Student.MaternalLastName }
                    .Where(Part => !string.IsNullOrWhiteSpace(Part))),
            Curp = Student.Curp,
            BirthDate = Student.BirthDate,
            Gender = Student.Gender,
            Email = Student.Email,
            Phone = Student.Phone,
            AddressLine = Student.AddressLine,
            City = Student.City,
            State = Student.State,
            PostalCode = Student.PostalCode,
            Status = Student.Status,
        };
}
