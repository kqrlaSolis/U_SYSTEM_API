// Acceso a datos del material didáctico.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class CourseMaterialRepository : ICourseMaterialRepository
{
    private readonly USystemDbContext _Context;

    public CourseMaterialRepository(USystemDbContext Context) => _Context = Context;

    public Task<CourseMaterial?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.CourseMaterials.FirstOrDefaultAsync(Material => Material.Id == Id, Cancellation);

    public async Task<PagedResult<CourseMaterial>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.CourseMaterials.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Material =>
                Material.Title.Contains(Search)
                || (Material.Description != null && Material.Description.Contains(Search)));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Material => Material.Title)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<CourseMaterial>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(CourseMaterial Entity, CancellationToken Cancellation = default)
        => await _Context.CourseMaterials.AddAsync(Entity, Cancellation);

    public void Update(CourseMaterial Entity) => _Context.CourseMaterials.Update(Entity);

    public void Remove(CourseMaterial Entity) => _Context.CourseMaterials.Remove(Entity);
}
