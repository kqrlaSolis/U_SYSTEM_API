// Entidad de la tabla Teachers.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Teacher : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Número de empleado
    public string EmployeeNumber { get; set; } = string.Empty;

    // Nombre(s)
    public string FirstName { get; set; } = string.Empty;

    // Apellido paterno
    public string PaternalLastName { get; set; } = string.Empty;

    // Apellido materno
    public string? MaternalLastName { get; set; }

    // Correo
    public string Email { get; set; } = string.Empty;

    // Teléfono
    public string? Phone { get; set; }

    // Especialidad
    public string? Specialty { get; set; }

    // Usuario del sistema (opcional)
    public long? UserId { get; set; }

    // Activo
    public bool IsActive { get; set; }
}
