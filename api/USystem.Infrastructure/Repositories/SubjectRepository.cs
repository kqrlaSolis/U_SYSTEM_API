// Acceso a datos del catálogo de materias.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly USystemDbContext _Context;

    public SubjectRepository(USystemDbContext Context) => _Context = Context;

    public Task<Subject?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.Subjects.FirstOrDefaultAsync(Subject => Subject.Id == Id, Cancellation);

    public async Task<IReadOnlyList<Subject>> GetByIdsAsync(
        IReadOnlyCollection<int> SubjectIds,
        CancellationToken Cancellation = default)
    {
        if (SubjectIds.Count == 0)
        {
            return Array.Empty<Subject>();
        }

        return await _Context.Subjects
            .AsNoTracking()
            .Where(Subject => SubjectIds.Contains(Subject.Id))
            .ToListAsync(Cancellation);
    }

    public Task<bool> ExistsByCodeAsync(
        string Code,
        int? ExcludeSubjectId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        // Con IncludeDeleted=true también cuenta la materia dada de baja: el índice único no admite repetidos
        var Source = IncludeDeleted ? _Context.Subjects.IgnoreQueryFilters() : _Context.Subjects;
        return Source.AnyAsync(
            Subject => Subject.Code == Code && (ExcludeSubjectId == null || Subject.Id != ExcludeSubjectId),
            Cancellation);
    }

    public Task<bool> IsUsedInStudyPlansAsync(int SubjectId, CancellationToken Cancellation = default)
        => _Context.StudyPlanSubjects.AnyAsync(
            PlanSubject => PlanSubject.SubjectId == SubjectId,
            Cancellation);

    public Task<bool> IsUsedInClassGroupsAsync(int SubjectId, CancellationToken Cancellation = default)
        => _Context.ClassGroups.AnyAsync(Group => Group.SubjectId == SubjectId, Cancellation);

    public async Task<PagedResult<Subject>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Subjects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Subject => Subject.Code.Contains(Search) || Subject.Name.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Subject => Subject.Name)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Subject>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(Subject Entity, CancellationToken Cancellation = default)
        => await _Context.Subjects.AddAsync(Entity, Cancellation);

    public void Update(Subject Entity) => _Context.Subjects.Update(Entity);

    public void Remove(Subject Entity) => _Context.Subjects.Remove(Entity);
}
