// Prueba opcional contra la base de datos real: valida que el modelo de EF coincide con los scripts SQL.
// Se ejecuta solo cuando USYSTEM_RUN_DB_TESTS=1, para no depender de la red en CI.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using USystem.Api.Configuration;
using USystem.Infrastructure.Persistence;

namespace USystem.Tests.IntegrationTests;

public class DatabaseConnectivityTests
{
    private static string? ResolveConnectionString()
    {
        // Carga el .env del repositorio igual que la API
        EnvironmentFileLoader.Load();

        var Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        return Configuration.GetConnectionString("DefaultConnection");
    }

    [Fact]
    public async Task EfModelMatchesTheSeededDatabase()
    {
        if (Environment.GetEnvironmentVariable("USYSTEM_RUN_DB_TESTS") != "1")
        {
            // Prueba opcional: sin la bandera no se toca la base de datos real
            return;
        }

        var ConnectionString = ResolveConnectionString();
        Assert.False(string.IsNullOrWhiteSpace(ConnectionString));

        var Options = new DbContextOptionsBuilder<USystemDbContext>()
            .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 4, 11)))
            .Options;

        await using var Context = new USystemDbContext(Options);

        // Las 78 semillas de permisos y los roles del script 99_SeedData
        Assert.Equal(78, await Context.Permissions.CountAsync());
        Assert.True(await Context.Roles.CountAsync() >= 6);

        // El borrado lógico y la auditoría no rompen la consulta real
        var FirstUser = await Context.Users.FirstOrDefaultAsync();
        if (FirstUser is not null)
        {
            Assert.True(FirstUser.CreatedAt <= DateTime.UtcNow.AddMinutes(1));
        }
    }
}
