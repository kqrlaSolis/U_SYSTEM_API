// Acceso a datos de inscripciones.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly USystemDbContext _Context;

    public EnrollmentRepository(USystemDbContext Context) => _Context = Context;

    public Task<Enrollment?> GetByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.Enrollments.FirstOrDefaultAsync(Enrollment => Enrollment.Id == Id, Cancellation);

    public Task<bool> ExistsForStudentTermAsync(
        long StudentId,
        int AcademicTermId,
        CancellationToken Cancellation = default)
        => _Context.Enrollments.AnyAsync(
            Enrollment => Enrollment.StudentId == StudentId && Enrollment.AcademicTermId == AcademicTermId,
            Cancellation);

    public async Task<IReadOnlyList<EnrollmentClassGroup>> GetClassGroupsAsync(
        long EnrollmentId,
        CancellationToken Cancellation = default)
        => await _Context.EnrollmentClassGroups
            .AsNoTracking()
            .Where(Item => Item.EnrollmentId == EnrollmentId)
            .ToListAsync(Cancellation);

    public async Task AddClassGroupAsync(EnrollmentClassGroup Item, CancellationToken Cancellation = default)
        => await _Context.EnrollmentClassGroups.AddAsync(Item, Cancellation);

    public Task<int> CountInGroupAsync(int ClassGroupId, CancellationToken Cancellation = default)
        => _Context.EnrollmentClassGroups.CountAsync(
            Item => Item.ClassGroupId == ClassGroupId,
            Cancellation);

    public async Task<PagedResult<Enrollment>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Enrollments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Enrollment => Enrollment.Status.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderByDescending(Enrollment => Enrollment.Id)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Enrollment>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(Enrollment Entity, CancellationToken Cancellation = default)
        => await _Context.Enrollments.AddAsync(Entity, Cancellation);

    public void Update(Enrollment Entity) => _Context.Enrollments.Update(Entity);

    public void Remove(Enrollment Entity) => _Context.Enrollments.Remove(Entity);
}
