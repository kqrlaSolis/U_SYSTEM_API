// Acceso a datos de docentes.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly USystemDbContext _Context;

    public TeacherRepository(USystemDbContext Context) => _Context = Context;

    public Task<Teacher?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.Teachers.FirstOrDefaultAsync(Teacher => Teacher.Id == Id, Cancellation);

    public Task<bool> ExistsByEmployeeNumberAsync(
        string EmployeeNumber,
        int? ExcludeTeacherId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted ? _Context.Teachers.IgnoreQueryFilters() : _Context.Teachers;
        return Source.AnyAsync(
            Teacher => Teacher.EmployeeNumber == EmployeeNumber
                && (ExcludeTeacherId == null || Teacher.Id != ExcludeTeacherId),
            Cancellation);
    }

    public Task<bool> ExistsByEmailAsync(
        string Email,
        int? ExcludeTeacherId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted ? _Context.Teachers.IgnoreQueryFilters() : _Context.Teachers;
        return Source.AnyAsync(
            Teacher => Teacher.Email == Email
                && (ExcludeTeacherId == null || Teacher.Id != ExcludeTeacherId),
            Cancellation);
    }

    public Task<bool> HasClassGroupsAsync(int TeacherId, CancellationToken Cancellation = default)
        => _Context.ClassGroups.AnyAsync(Group => Group.TeacherId == TeacherId, Cancellation);

    public async Task<PagedResult<Teacher>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Teachers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Teacher =>
                Teacher.EmployeeNumber.Contains(Search)
                || Teacher.FirstName.Contains(Search)
                || Teacher.PaternalLastName.Contains(Search)
                || Teacher.Email.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Teacher => Teacher.PaternalLastName)
            .ThenBy(Teacher => Teacher.FirstName)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Teacher>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(Teacher Entity, CancellationToken Cancellation = default)
        => await _Context.Teachers.AddAsync(Entity, Cancellation);

    public void Update(Teacher Entity) => _Context.Teachers.Update(Entity);

    public void Remove(Teacher Entity) => _Context.Teachers.Remove(Entity);
}
