// Unidad de trabajo sobre EF Core: confirma los cambios o revierte todo si algo falla.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Interfaces;

namespace USystem.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly USystemDbContext _Context;

    public UnitOfWork(USystemDbContext Context) => _Context = Context;

    public Task<int> SaveChangesAsync(CancellationToken Cancellation = default)
        => _Context.SaveChangesAsync(Cancellation);

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> Operation,
        CancellationToken Cancellation = default)
    {
        // Una transacción anidada se une a la existente
        if (_Context.Database.CurrentTransaction is not null)
        {
            await Operation(Cancellation);
            return;
        }

        // Con EnableRetryOnFailure, EF solo permite transacciones manuales dentro de su estrategia:
        // si la conexión se cae, repite el bloque completo desde el inicio.
        var Strategy = _Context.Database.CreateExecutionStrategy();

        await Strategy.ExecuteAsync(async () =>
        {
            await using var Transaction = await _Context.Database.BeginTransactionAsync(Cancellation);
            try
            {
                await Operation(Cancellation);
                await _Context.SaveChangesAsync(Cancellation);
                await Transaction.CommitAsync(Cancellation);
            }
            catch (Exception)
            {
                // Sin token: el rollback debe correr aunque la petición se haya cancelado
                await Transaction.RollbackAsync(CancellationToken.None);

                // Lo que quedó en memoria ya no coincide con la base; un reintento empieza limpio
                _Context.ChangeTracker.Clear();
                throw;
            }
        });
    }
}
