// Pruebas del chequeo de base de datos que usa /health.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using USystem.Api.HealthChecks;
using USystem.Infrastructure.Persistence;

namespace USystem.Tests.UnitTests;

public class DatabaseHealthCheckTests
{
    [Fact]
    public async Task ReportsHealthyWhenTheDatabaseResponds()
    {
        var Options = new DbContextOptionsBuilder<USystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        await using var Context = new USystemDbContext(Options);

        var Result = await new DatabaseHealthCheck(Context).CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, Result.Status);
    }

    [Fact]
    public async Task ReportsUnhealthyWhenTheDatabaseIsUnreachable()
    {
        // Puerto 1 en localhost: nadie escucha, la conexión se rechaza de inmediato
        var Options = new DbContextOptionsBuilder<USystemDbContext>()
            .UseMySql(
                "Server=127.0.0.1;Port=1;Database=USystem;User=test;Password=test;Connection Timeout=3;",
                new MySqlServerVersion(new Version(8, 0, 36)))
            .Options;
        await using var Context = new USystemDbContext(Options);

        var Result = await new DatabaseHealthCheck(Context).CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, Result.Status);
    }
}
