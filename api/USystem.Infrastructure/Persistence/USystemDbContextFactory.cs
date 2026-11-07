// Fábrica para `dotnet ef`: lee ConnectionStrings__DefaultConnection de las variables del proceso (no carga .env).
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace USystem.Infrastructure.Persistence;

public class USystemDbContextFactory : IDesignTimeDbContextFactory<USystemDbContext>
{
    private static readonly ServerVersion _FallbackVersion = new MySqlServerVersion(new Version(8, 4, 11));

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
