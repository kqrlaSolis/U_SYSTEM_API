// Indicadores agregados para el dashboard.
namespace USystem.Domain.Interfaces;

public interface IReportRepository
{
    Task<int> CountStudentsAsync(CancellationToken Cancellation = default);

    Task<int> CountTeachersAsync(CancellationToken Cancellation = default);

    Task<int> CountOpenClassGroupsAsync(CancellationToken Cancellation = default);

    Task<(int Count, decimal Amount)> GetPendingChargesAsync(CancellationToken Cancellation = default);

    Task<(int Requested, int Issued)> GetProceduresAsync(CancellationToken Cancellation = default);

    Task<int> CountEnrollmentsAsync(CancellationToken Cancellation = default);

    Task<int> CountEnrollmentsInTermAsync(int AcademicTermId, CancellationToken Cancellation = default);
}
