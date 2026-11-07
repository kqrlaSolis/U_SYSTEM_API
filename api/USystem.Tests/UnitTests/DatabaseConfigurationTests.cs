// Pruebas de la revisión de la cadena de conexión al arrancar.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using USystem.Api.Configuration;

namespace USystem.Tests.UnitTests;

public class DatabaseConfigurationTests
{
    private static IConfiguration BuildConfiguration(string? ConnectionString)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString,
            })
            .Build();

    private static IHostEnvironment BuildEnvironment(string Name) => new TestHostEnvironment { EnvironmentName = Name };

    [Fact]
    public void ProductionWithoutConnectionStringFails()
    {
        var Error = Assert.Throws<InvalidOperationException>(() =>
            DatabaseConfiguration.EnsureConfigured(BuildConfiguration(""), BuildEnvironment(Environments.Production)));

        Assert.Contains("ConnectionStrings__DefaultConnection", Error.Message);
    }

    [Fact]
    public void ProductionWithConnectionStringStarts()
        => DatabaseConfiguration.EnsureConfigured(
            BuildConfiguration("Server=db;Port=3306;Database=USystem;User=app;Password=x;"),
            BuildEnvironment(Environments.Production));

    [Fact]
    public void DevelopmentWithoutConnectionStringKeepsTheLocalFallback()
        => DatabaseConfiguration.EnsureConfigured(BuildConfiguration(null), BuildEnvironment(Environments.Development));

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "USystem.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
