// Entidad de la tabla Students.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Student : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Matrícula
    public string StudentNumber { get; set; } = string.Empty;

    // Carrera
    public int CareerId { get; set; }

    // Plan con el que ingresó
    public int StudyPlanId { get; set; }

    // Periodo de ingreso
    public int EnrollmentTermId { get; set; }

    // Nombre(s)
    public string FirstName { get; set; } = string.Empty;

    // Apellido paterno
    public string PaternalLastName { get; set; } = string.Empty;

    // Apellido materno
    public string? MaternalLastName { get; set; }

    // CURP
    public string Curp { get; set; } = string.Empty;

    // Fecha de nacimiento
    public DateOnly BirthDate { get; set; }

    // Género
    public string Gender { get; set; } = string.Empty;

    // Correo
    public string? Email { get; set; }

    // Teléfono
    public string? Phone { get; set; }

    // Calle y número
    public string? AddressLine { get; set; }

    // Ciudad
    public string? City { get; set; }

    // Estado
    public string? State { get; set; }

    // Código postal
    public string? PostalCode { get; set; }

    // Applicant, Active, TemporaryLeave, Dropped, Graduated, Titled
    public string Status { get; set; } = string.Empty;
}
