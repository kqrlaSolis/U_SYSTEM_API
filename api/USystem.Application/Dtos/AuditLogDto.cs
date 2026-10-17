// Registro de bitácora que consume el desktop.
namespace USystem.Application.Dtos;

public class AuditLogDto
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string? Username { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string Entity { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }
}
