// Acceso a datos de calificaciones.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly USystemDbContext _Context;

    public GradeRepository(USystemDbContext Context) => _Context = Context;

    public async Task<IReadOnlyList<Grade>> GetByClassGroupAsync(
        int ClassGroupId,
        CancellationToken Cancellation = default)
        => await _Context.Grades
            .AsNoTracking()
            .Where(Grade => Grade.ClassGroupId == ClassGroupId)
            .OrderBy(Grade => Grade.EnrollmentId)
            .ToListAsync(Cancellation);

    public Task<Grade?> GetByEnrollmentAsync(
        long EnrollmentId,
        int ClassGroupId,
        CancellationToken Cancellation = default)
        => _Context.Grades.FirstOrDefaultAsync(
            Grade => Grade.EnrollmentId == EnrollmentId && Grade.ClassGroupId == ClassGroupId,
            Cancellation);

    public async Task<IReadOnlyList<Grade>> GetByEnrollmentIdAsync(
        long EnrollmentId,
        CancellationToken Cancellation = default)
        => await _Context.Grades
            .AsNoTracking()
            .Where(Grade => Grade.EnrollmentId == EnrollmentId)
            .ToListAsync(Cancellation);

    public async Task AddAsync(Grade Entity, CancellationToken Cancellation = default)
        => await _Context.Grades.AddAsync(Entity, Cancellation);

    public void Update(Grade Entity) => _Context.Grades.Update(Entity);
}
