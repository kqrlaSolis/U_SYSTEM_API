// Escritura de la bitácora. Cada registro se guarda de inmediato: la bitácora no puede perderse
// aunque el caso de uso continúe después.
using System.Text.Json;
using System.Text.Json.Serialization;
using USystem.Application.Interfaces;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class AuditService : IAuditService
{
    private static readonly JsonSerializerOptions _SerializerOptions = new()
    {
        // La bitácora guarda los nombres de propiedad como los ve la API (PascalCase)
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IAuditLogRepository _AuditLogRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly ICurrentUser _CurrentUser;

    public AuditService(
        IAuditLogRepository AuditLogRepository,
        IUnitOfWork UnitOfWork,
        ICurrentUser CurrentUser)
    {
        _AuditLogRepository = AuditLogRepository;
        _UnitOfWork = UnitOfWork;
        _CurrentUser = CurrentUser;
    }

    public async Task LogAsync(
        string Action,
        string Module,
        string Entity,
        string? EntityId,
        object? OldValue,
        object? NewValue,
        CancellationToken Cancellation = default)
    {
        var NewAuditLog = new AuditLog
        {
            UserId = _CurrentUser.UserId,
            Action = Action,
            Module = Module,
            Entity = Entity,
            EntityId = EntityId,
            OldValue = Serialize(OldValue),
            NewValue = Serialize(NewValue),
            IpAddress = _CurrentUser.IpAddress,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _CurrentUser.UserId,
        };

        await _AuditLogRepository.AddAsync(NewAuditLog, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);
    }

    private static string? Serialize(object? Value)
        => Value is null ? null : JsonSerializer.Serialize(Value, _SerializerOptions);
}
