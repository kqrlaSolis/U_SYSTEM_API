// Acceso a datos de alumnos.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<bool> ExistsByCurpAsync(
        string Curp,
        long? ExcludeStudentId = null,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    // Último número generado con el prefijo del año, para continuar la secuencia
    Task<string?> GetLastStudentNumberAsync(string Prefix, CancellationToken Cancellation = default);

    // Un alumno con inscripciones solo se desactiva
    Task<bool> HasEnrollmentsAsync(long StudentId, CancellationToken Cancellation = default);
}
