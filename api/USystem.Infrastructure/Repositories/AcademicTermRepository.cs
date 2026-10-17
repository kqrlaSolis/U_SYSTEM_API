// Acceso a datos de los periodos cuatrimestrales.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Constants;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class AcademicTermRepository : IAcademicTermRepository
{
    private readonly USystemDbContext _Context;

    public AcademicTermRepository(USystemDbContext Context) => _Context = Context;

    public Task<AcademicTerm?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.AcademicTerms.FirstOrDefaultAsync(Term => Term.Id == Id, Cancellation);

    public Task<bool> ExistsByCodeAsync(
        string Code,
        int? ExcludeTermId = null,
        CancellationToken Cancellation = default)
        => _Context.AcademicTerms.AnyAsync(
            Term => Term.Code == Code && (ExcludeTermId == null || Term.Id != ExcludeTermId),
            Cancellation);

    public Task<AcademicTerm?> GetOverlappingAsync(
        DateOnly StartDate,
        DateOnly EndDate,
        int? ExcludeTermId = null,
        CancellationToken Cancellation = default)
        => _Context.AcademicTerms
            .AsNoTracking()
            .Where(Term => ExcludeTermId == null || Term.Id != ExcludeTermId)
            .Where(Term => Term.StartDate <= EndDate && Term.EndDate >= StartDate)
            .OrderBy(Term => Term.StartDate)
            .FirstOrDefaultAsync(Cancellation);

    public Task<AcademicTerm?> GetActiveAsync(CancellationToken Cancellation = default)
        => _Context.AcademicTerms
            .AsNoTracking()
            .Where(Term => AcademicTermStatuses.Active.Contains(Term.Status))
            .OrderBy(Term => Term.StartDate)
            .FirstOrDefaultAsync(Cancellation);

    public async Task<AcademicTerm?> GetCurrentAsync(CancellationToken Cancellation = default)
        => await _Context.AcademicTerms
                .AsNoTracking()
                .Where(Term => Term.Status == AcademicTermStatuses.InProgress)
                .OrderByDescending(Term => Term.StartDate)
                .FirstOrDefaultAsync(Cancellation)
            ?? await _Context.AcademicTerms
                .AsNoTracking()
                .Where(Term => Term.Status == AcademicTermStatuses.Open)
                .OrderByDescending(Term => Term.StartDate)
                .FirstOrDefaultAsync(Cancellation);

    public async Task<PagedResult<AcademicTerm>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.AcademicTerms.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Term => Term.Code.Contains(Search) || Term.Name.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderByDescending(Term => Term.StartDate)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<AcademicTerm>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(AcademicTerm Entity, CancellationToken Cancellation = default)
        => await _Context.AcademicTerms.AddAsync(Entity, Cancellation);

    public void Update(AcademicTerm Entity) => _Context.AcademicTerms.Update(Entity);

    public void Remove(AcademicTerm Entity) => _Context.AcademicTerms.Remove(Entity);
}
