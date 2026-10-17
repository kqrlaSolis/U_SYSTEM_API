// Fábrica para `dotnet ef`: usa la cadena de conexión del entorno o del archivo .env.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace USystem.Infrastructure.Persistence;

public class USystemDbContextFactory : IDesignTimeDbContextFactory<USystemDbContext>
{
    private static readonly ServerVersion _FallbackVersion = new MySqlServerVersion(new Version(8, 0, 36));

    public USystemDbContext CreateDbContext(string[] Args)
    {
        var ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=USystem;User=USystemApp;Password=CHANGE_ME;";

        var ConfiguredVersion = Environment.GetEnvironmentVariable("Database__ServerVersion");
        var ResolvedVersion = string.IsNullOrWhiteSpace(ConfiguredVersion)
            ? _FallbackVersion
            : ServerVersion.Parse(ConfiguredVersion);

        var Options = new DbContextOptionsBuilder<USystemDbContext>()
            .UseMySql(ConnectionString, ResolvedVersion)
            .Options;

        return new USystemDbContext(Options);
    }
}
