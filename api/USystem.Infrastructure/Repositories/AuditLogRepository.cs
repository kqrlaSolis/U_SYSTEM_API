// Bitácora: solo alta y consulta paginada con filtros.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly USystemDbContext _Context;

    public AuditLogRepository(USystemDbContext Context) => _Context = Context;

    public Task<AuditLog?> GetByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.AuditLogs.FirstOrDefaultAsync(Entry => Entry.Id == Id, Cancellation);

    public async Task<PagedResult<AuditLog>> GetPagedAsync(
        AuditLogQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Entry => Entry.Action.Contains(Search)
                                        || Entry.Module.Contains(Search)
                                        || Entry.Entity.Contains(Search));
        }

        if (!string.IsNullOrWhiteSpace(Query.Action))
        {
            var Action = Query.Action.Trim();
            Source = Source.Where(Entry => Entry.Action == Action);
        }

        if (!string.IsNullOrWhiteSpace(Query.Module))
        {
            var Module = Query.Module.Trim();
            Source = Source.Where(Entry => Entry.Module == Module);
        }

        if (!string.IsNullOrWhiteSpace(Query.Entity))
        {
            var Entity = Query.Entity.Trim();
            Source = Source.Where(Entry => Entry.Entity == Entity);
        }

        if (Query.UserId is not null)
        {
            Source = Source.Where(Entry => Entry.UserId == Query.UserId);
        }

        if (Query.From is not null)
        {
            Source = Source.Where(Entry => Entry.CreatedAt >= Query.From);
        }

        if (Query.To is not null)
        {
            Source = Source.Where(Entry => Entry.CreatedAt <= Query.To);
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderByDescending(Entry => Entry.CreatedAt)
            .ThenByDescending(Entry => Entry.Id)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<AuditLog>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(AuditLog Entity, CancellationToken Cancellation = default)
        => await _Context.AuditLogs.AddAsync(Entity, Cancellation);
}
