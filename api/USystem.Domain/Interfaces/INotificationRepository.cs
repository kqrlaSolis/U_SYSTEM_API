// Acceso a datos de plantillas de notificación.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface INotificationRepository
{
    Task<PagedResult<NotificationTemplate>> GetTemplatesPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default);

    Task<NotificationTemplate?> GetTemplateByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<NotificationTemplate?> GetTemplateByCodeAsync(string Code, CancellationToken Cancellation = default);

    Task<bool> ExistsTemplateByCodeAsync(
        string Code,
        int? ExcludeTemplateId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    Task AddTemplateAsync(NotificationTemplate Entity, CancellationToken Cancellation = default);

    void UpdateTemplate(NotificationTemplate Entity);

    void RemoveTemplate(NotificationTemplate Entity);

    Task AddNotificationAsync(Notification Entity, CancellationToken Cancellation = default);

    Task<Notification?> GetNotificationByIdAsync(long Id, CancellationToken Cancellation = default);
}
