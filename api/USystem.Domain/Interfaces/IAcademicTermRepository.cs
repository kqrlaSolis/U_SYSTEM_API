// Acceso a datos de los periodos cuatrimestrales.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IAcademicTermRepository : IRepository<AcademicTerm>
{
    Task<AcademicTerm?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<bool> ExistsByCodeAsync(
        string Code,
        int? ExcludeTermId = null,
        CancellationToken Cancellation = default);

    // Dos periodos no pueden traslapar fechas
    Task<AcademicTerm?> GetOverlappingAsync(
        DateOnly StartDate,
        DateOnly EndDate,
        int? ExcludeTermId = null,
        CancellationToken Cancellation = default);

    // Periodo abierto o en curso (solo puede haber uno)
    Task<AcademicTerm?> GetActiveAsync(CancellationToken Cancellation = default);

    // Periodo en curso: el que está InProgress (o el último abierto)
    Task<AcademicTerm?> GetCurrentAsync(CancellationToken Cancellation = default);
}
