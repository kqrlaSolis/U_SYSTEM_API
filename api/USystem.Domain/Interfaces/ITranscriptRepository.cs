// Acceso a datos del kardex: calificaciones del alumno con su inscripción.
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ITranscriptRepository
{
    Task<Student?> GetStudentAsync(long StudentId, CancellationToken Cancellation = default);

    // Calificaciones del alumno vía su inscripción
    Task<IReadOnlyList<(Grade Grade, Enrollment Enrollment)>> GetGradesAsync(
        long StudentId,
        CancellationToken Cancellation = default);
}
