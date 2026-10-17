// Bitácora de cambios: la invocan los servicios en altas, cambios, bajas y acciones críticas.
namespace USystem.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string Action,
        string Module,
        string Entity,
        string? EntityId,
        object? OldValue,
        object? NewValue,
        CancellationToken Cancellation = default);
}
