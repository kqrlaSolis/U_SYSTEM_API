// Acceso a datos del kardex.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class TranscriptRepository : ITranscriptRepository
{
    private readonly USystemDbContext _Context;

    public TranscriptRepository(USystemDbContext Context) => _Context = Context;

    public Task<Student?> GetStudentAsync(long StudentId, CancellationToken Cancellation = default)
        => _Context.Students.FirstOrDefaultAsync(Student => Student.Id == StudentId, Cancellation);

    public async Task<IReadOnlyList<(Grade Grade, Enrollment Enrollment)>> GetGradesAsync(
        long StudentId,
        CancellationToken Cancellation = default)
    {
        var Rows = await (
            from Grade in _Context.Grades.AsNoTracking()
            join Enrollment in _Context.Enrollments.AsNoTracking()
                on Grade.EnrollmentId equals Enrollment.Id
            where Enrollment.StudentId == StudentId
            orderby Enrollment.AcademicTermId, Grade.ClassGroupId
            select new { Grade, Enrollment })
            .ToListAsync(Cancellation);

        return Rows.Select(Row => (Row.Grade, Row.Enrollment)).ToList();
    }
}
