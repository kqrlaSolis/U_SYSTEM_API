// Entidad de la tabla Users.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class User : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Nombre(s)
    public string FirstName { get; set; } = string.Empty;

    // Apellido paterno
    public string PaternalLastName { get; set; } = string.Empty;

    // Apellido materno
    public string? MaternalLastName { get; set; }

    // Correo institucional
    public string Email { get; set; } = string.Empty;

    // Nombre de usuario
    public string Username { get; set; } = string.Empty;

    // Hash BCrypt/Argon2
    public string PasswordHash { get; set; } = string.Empty;

    // Teléfono
    public string? Phone { get; set; }

    // Usuario activo
    public bool IsActive { get; set; }

    // Cambio obligatorio en el primer ingreso
    public bool MustChangePassword { get; set; }

    // Intentos fallidos consecutivos
    public int FailedAttempts { get; set; }

    // Bloqueo temporal
    public DateTime? LockedUntil { get; set; }

    // Último inicio de sesión
    public DateTime? LastLoginAt { get; set; }
}
