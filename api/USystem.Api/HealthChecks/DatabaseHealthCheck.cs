// Chequeo de /health: confirma que la API puede abrir una conexión a MySQL.
// Railway lo usa al desplegar, así que una cadena de conexión mala detiene el deploy.
using Microsoft.Extensions.Diagnostics.HealthChecks;
using USystem.Infrastructure.Persistence;

namespace USystem.Api.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    public const string Name = "database";

    private readonly USystemDbContext _Context;

    public DatabaseHealthCheck(USystemDbContext Context) => _Context = Context;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext Context,
        CancellationToken Cancellation = default)
    {
        try
        {
            return await _Context.Database.CanConnectAsync(Cancellation)
                ? HealthCheckResult.Healthy("MySQL responde.")
                : HealthCheckResult.Unhealthy("No se pudo conectar a MySQL.");
        }
        catch (Exception Error)
        {
            return HealthCheckResult.Unhealthy("No se pudo conectar a MySQL.", Error);
        }
    }
}
