// Acceso a datos de alumnos.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly USystemDbContext _Context;

    public StudentRepository(USystemDbContext Context) => _Context = Context;

    public Task<Student?> GetByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.Students.FirstOrDefaultAsync(Student => Student.Id == Id, Cancellation);

    public Task<bool> ExistsByCurpAsync(
        string Curp,
        long? ExcludeStudentId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted ? _Context.Students.IgnoreQueryFilters() : _Context.Students;
        return Source.AnyAsync(
            Student => Student.Curp == Curp
                && (ExcludeStudentId == null || Student.Id != ExcludeStudentId),
            Cancellation);
    }

    public async Task<string?> GetLastStudentNumberAsync(
        string Prefix,
        CancellationToken Cancellation = default)
    {
        // Se incluyen los dados de baja: el índice único no admite repetidos
        var Numbers = await _Context.Students
            .IgnoreQueryFilters()
            .Where(Student => Student.StudentNumber.StartsWith(Prefix))
            .Select(Student => Student.StudentNumber)
            .ToListAsync(Cancellation);

        return Numbers.Count == 0 ? null : Numbers.Max();
    }

    public Task<bool> HasEnrollmentsAsync(long StudentId, CancellationToken Cancellation = default)
        => _Context.Enrollments.AnyAsync(Enrollment => Enrollment.StudentId == StudentId, Cancellation);

    public async Task<PagedResult<Student>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Student =>
                Student.StudentNumber.Contains(Search)
                || Student.FirstName.Contains(Search)
                || Student.PaternalLastName.Contains(Search)
                || Student.Curp.Contains(Search));
        }

        if (Query.CareerId is not null)
        {
            Source = Source.Where(Student => Student.CareerId == Query.CareerId);
        }

        if (Query.StudyPlanId is not null)
        {
            Source = Source.Where(Student => Student.StudyPlanId == Query.StudyPlanId);
        }

        if (Query.AcademicTermId is not null)
        {
            Source = Source.Where(Student => Student.EnrollmentTermId == Query.AcademicTermId);
        }

        if (!string.IsNullOrWhiteSpace(Query.Status))
        {
            Source = Source.Where(Student => Student.Status == Query.Status);
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Student => Student.PaternalLastName)
            .ThenBy(Student => Student.FirstName)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Student>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(Student Entity, CancellationToken Cancellation = default)
        => await _Context.Students.AddAsync(Entity, Cancellation);

    public void Update(Student Entity) => _Context.Students.Update(Entity);

    public void Remove(Student Entity) => _Context.Students.Remove(Entity);
}
