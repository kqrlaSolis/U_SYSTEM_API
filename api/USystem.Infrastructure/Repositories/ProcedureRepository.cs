// Acceso a datos de trámites y sus tipos.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class ProcedureRepository : IProcedureRepository
{
    private readonly USystemDbContext _Context;

    public ProcedureRepository(USystemDbContext Context) => _Context = Context;

    public async Task<PagedResult<ProcedureType>> GetTypesPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.ProcedureTypes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Type => Type.Code.Contains(Search) || Type.Name.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Type => Type.Name)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<ProcedureType>.Create(Items, TotalCount, Query);
    }

    public Task<ProcedureType?> GetTypeByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.ProcedureTypes.FirstOrDefaultAsync(Type => Type.Id == Id, Cancellation);

    public Task<bool> ExistsTypeByCodeAsync(
        string Code,
        int? ExcludeTypeId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted ? _Context.ProcedureTypes.IgnoreQueryFilters() : _Context.ProcedureTypes;
        return Source.AnyAsync(
            Type => Type.Code == Code && (ExcludeTypeId == null || Type.Id != ExcludeTypeId),
            Cancellation);
    }

    public async Task AddTypeAsync(ProcedureType Entity, CancellationToken Cancellation = default)
        => await _Context.ProcedureTypes.AddAsync(Entity, Cancellation);

    public void UpdateType(ProcedureType Entity) => _Context.ProcedureTypes.Update(Entity);

    public void RemoveType(ProcedureType Entity) => _Context.ProcedureTypes.Remove(Entity);

    public async Task<PagedResult<Procedure>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Procedures.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Procedure =>
                Procedure.FolioNumber.Contains(Search) || Procedure.Status.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderByDescending(Procedure => Procedure.Id)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Procedure>.Create(Items, TotalCount, Query);
    }

    public Task<Procedure?> GetByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.Procedures.FirstOrDefaultAsync(Procedure => Procedure.Id == Id, Cancellation);

    public async Task<string?> GetLastFolioAsync(string Prefix, CancellationToken Cancellation = default)
    {
        var Folios = await _Context.Procedures
            .IgnoreQueryFilters()
            .Where(Procedure => Procedure.FolioNumber.StartsWith(Prefix))
            .Select(Procedure => Procedure.FolioNumber)
            .ToListAsync(Cancellation);

        return Folios.Count == 0 ? null : Folios.Max();
    }

    public async Task AddAsync(Procedure Entity, CancellationToken Cancellation = default)
        => await _Context.Procedures.AddAsync(Entity, Cancellation);

    public void Update(Procedure Entity) => _Context.Procedures.Update(Entity);
}
