// Acceso a datos de grupos y horarios.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class ClassGroupRepository : IClassGroupRepository
{
    private readonly USystemDbContext _Context;

    public ClassGroupRepository(USystemDbContext Context) => _Context = Context;

    public Task<ClassGroup?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.ClassGroups.FirstOrDefaultAsync(Group => Group.Id == Id, Cancellation);

    public async Task<IReadOnlyList<ClassGroupSchedule>> GetSchedulesAsync(
        int ClassGroupId,
        CancellationToken Cancellation = default)
        => await _Context.ClassGroupSchedules
            .AsNoTracking()
            .Where(Schedule => Schedule.ClassGroupId == ClassGroupId)
            .OrderBy(Schedule => Schedule.DayOfWeek)
            .ThenBy(Schedule => Schedule.StartTime)
            .ToListAsync(Cancellation);

    public Task<bool> ExistsByCodeAsync(
        int AcademicTermId,
        string Code,
        int? ExcludeClassGroupId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted ? _Context.ClassGroups.IgnoreQueryFilters() : _Context.ClassGroups;
        return Source.AnyAsync(
            Group => Group.AcademicTermId == AcademicTermId
                && Group.Code == Code
                && (ExcludeClassGroupId == null || Group.Id != ExcludeClassGroupId),
            Cancellation);
    }

    public Task<bool> HasEnrollmentsAsync(int ClassGroupId, CancellationToken Cancellation = default)
        => _Context.EnrollmentClassGroups.AnyAsync(
            EnrollmentClassGroup => EnrollmentClassGroup.ClassGroupId == ClassGroupId,
            Cancellation);

    public void RemoveSchedules(IEnumerable<ClassGroupSchedule> Schedules)
        => _Context.ClassGroupSchedules.RemoveRange(Schedules);

    public async Task AddScheduleAsync(ClassGroupSchedule Schedule, CancellationToken Cancellation = default)
        => await _Context.ClassGroupSchedules.AddAsync(Schedule, Cancellation);

    public async Task<PagedResult<ClassGroup>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.ClassGroups.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Group => Group.Code.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Group => Group.Code)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<ClassGroup>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(ClassGroup Entity, CancellationToken Cancellation = default)
        => await _Context.ClassGroups.AddAsync(Entity, Cancellation);

    public void Update(ClassGroup Entity) => _Context.ClassGroups.Update(Entity);

    public void Remove(ClassGroup Entity) => _Context.ClassGroups.Remove(Entity);
}
