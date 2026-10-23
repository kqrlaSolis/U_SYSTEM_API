// Acceso a datos de notificaciones.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly USystemDbContext _Context;

    public NotificationRepository(USystemDbContext Context) => _Context = Context;

    public async Task<PagedResult<NotificationTemplate>> GetTemplatesPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.NotificationTemplates.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Template =>
                Template.Code.Contains(Search) || Template.SubjectLine.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Template => Template.Code)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<NotificationTemplate>.Create(Items, TotalCount, Query);
    }

    public Task<NotificationTemplate?> GetTemplateByIdAsync(
        int Id,
        CancellationToken Cancellation = default)
        => _Context.NotificationTemplates.FirstOrDefaultAsync(Template => Template.Id == Id, Cancellation);

    public Task<NotificationTemplate?> GetTemplateByCodeAsync(
        string Code,
        CancellationToken Cancellation = default)
        => _Context.NotificationTemplates.FirstOrDefaultAsync(Template => Template.Code == Code, Cancellation);

    public Task<bool> ExistsTemplateByCodeAsync(
        string Code,
        int? ExcludeTemplateId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted
            ? _Context.NotificationTemplates.IgnoreQueryFilters()
            : _Context.NotificationTemplates;

        return Source.AnyAsync(
            Template => Template.Code == Code
                && (ExcludeTemplateId == null || Template.Id != ExcludeTemplateId),
            Cancellation);
    }

    public async Task AddTemplateAsync(
        NotificationTemplate Entity,
        CancellationToken Cancellation = default)
        => await _Context.NotificationTemplates.AddAsync(Entity, Cancellation);

    public void UpdateTemplate(NotificationTemplate Entity)
        => _Context.NotificationTemplates.Update(Entity);

    public void RemoveTemplate(NotificationTemplate Entity)
        => _Context.NotificationTemplates.Remove(Entity);

    public async Task AddNotificationAsync(Notification Entity, CancellationToken Cancellation = default)
        => await _Context.Notifications.AddAsync(Entity, Cancellation);

    public Task<Notification?> GetNotificationByIdAsync(
        long Id,
        CancellationToken Cancellation = default)
        => _Context.Notifications.FirstOrDefaultAsync(Notification => Notification.Id == Id, Cancellation);
}
