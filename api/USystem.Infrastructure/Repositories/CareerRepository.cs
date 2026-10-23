// Acceso a datos del catálogo de carreras.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class CareerRepository : ICareerRepository
{
    private readonly USystemDbContext _Context;

    public CareerRepository(USystemDbContext Context) => _Context = Context;

    public Task<Career?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.Careers.FirstOrDefaultAsync(Career => Career.Id == Id, Cancellation);

    public async Task<IReadOnlyList<Career>> GetByIdsAsync(
        IReadOnlyCollection<int> CareerIds,
        CancellationToken Cancellation = default)
    {
        if (CareerIds.Count == 0)
        {
            return Array.Empty<Career>();
        }

        return await _Context.Careers
            .AsNoTracking()
            .Where(Career => CareerIds.Contains(Career.Id))
            .ToListAsync(Cancellation);
    }

    public Task<bool> ExistsByCodeAsync(
        string Code,
        int? ExcludeCareerId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        // Con IncludeDeleted=true también cuenta la carrera dada de baja: el índice único no admite repetidos
        var Source = IncludeDeleted ? _Context.Careers.IgnoreQueryFilters() : _Context.Careers;
        return Source.AnyAsync(
            Career => Career.Code == Code && (ExcludeCareerId == null || Career.Id != ExcludeCareerId),
            Cancellation);
    }

    public Task<bool> HasStudyPlansAsync(int CareerId, CancellationToken Cancellation = default)
        => _Context.StudyPlans.AnyAsync(Plan => Plan.CareerId == CareerId, Cancellation);

    public Task<bool> HasStudentsAsync(int CareerId, CancellationToken Cancellation = default)
        => _Context.Students.AnyAsync(Student => Student.CareerId == CareerId, Cancellation);

    public async Task<PagedResult<Career>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Careers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Career => Career.Code.Contains(Search) || Career.Name.Contains(Search));
        }

        if (!string.IsNullOrWhiteSpace(Query.Level))
        {
            Source = Source.Where(Career => Career.Level == Query.Level);
        }

        if (!string.IsNullOrWhiteSpace(Query.Modality))
        {
            Source = Source.Where(Career => Career.Modality == Query.Modality);
        }

        if (Query.IsActive is not null)
        {
            Source = Source.Where(Career => Career.IsActive == Query.IsActive);
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Career => Career.Name)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Career>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(Career Entity, CancellationToken Cancellation = default)
        => await _Context.Careers.AddAsync(Entity, Cancellation);

    public void Update(Career Entity) => _Context.Careers.Update(Entity);

    public void Remove(Career Entity) => _Context.Careers.Remove(Entity);
}
