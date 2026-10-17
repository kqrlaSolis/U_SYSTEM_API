// Entidad de la tabla RefreshTokens.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class RefreshToken : AuditableEntity
{
    // Llave primaria
    public long Id { get; set; }

    // Usuario dueño del token
    public long UserId { get; set; }

    // Hash del refresh token
    public string TokenHash { get; set; } = string.Empty;

    // Vigencia
    public DateTime ExpiresAt { get; set; }

    // Revocación
    public DateTime? RevokedAt { get; set; }

    // IP de origen
    public string? IpAddress { get; set; }

    // Agente de usuario
    public string? UserAgent { get; set; }
}
