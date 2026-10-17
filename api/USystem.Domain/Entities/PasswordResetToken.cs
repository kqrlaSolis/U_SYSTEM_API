// Entidad de la tabla PasswordResetTokens.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class PasswordResetToken : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Usuario
    public long UserId { get; set; }

    // Hash del token de recuperación
    public string TokenHash { get; set; } = string.Empty;

    // Vigencia de 30 minutos
    public DateTime ExpiresAt { get; set; }

    // Uso único
    public DateTime? UsedAt { get; set; }
}
