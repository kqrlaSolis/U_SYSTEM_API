// Mapeo de docente a DTO.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Domain.Entities;

namespace USystem.Application.Mappings;

public static class TeacherMapper
{
    public static TeacherDto ToDto(Teacher Teacher)
    {
        var FullName = string.Join(
            ' ',
            new[] { Teacher.FirstName, Teacher.PaternalLastName, Teacher.MaternalLastName }
                .Where(Part => !string.IsNullOrWhiteSpace(Part)));

        return new TeacherDto
        {
            Id = Teacher.Id,
            EmployeeNumber = Teacher.EmployeeNumber,
            FirstName = Teacher.FirstName,
            PaternalLastName = Teacher.PaternalLastName,
            MaternalLastName = Teacher.MaternalLastName,
            FullName = FullName,
            Email = Teacher.Email,
            Phone = Teacher.Phone,
            Specialty = Teacher.Specialty,
            UserId = Teacher.UserId,
            IsActive = Teacher.IsActive,
        };
    }

    public static string FormatEmployeeNumber(int Sequence)
        => Sequence.ToString("D4", CultureInfo.InvariantCulture);
}
