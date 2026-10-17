// Unidad de trabajo: agrupa los cambios de varios repositorios en una sola transacción.
namespace USystem.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken Cancellation = default);

    // Para operaciones que tocan varias tablas (inscripción, pagos)
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> Operation,
        CancellationToken Cancellation = default);
}
