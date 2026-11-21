// Pruebas de los orígenes permitidos por CORS.
using Microsoft.Extensions.Configuration;
using USystem.Api.Configuration;

namespace USystem.Tests.UnitTests.Api.Configuration;

public class CorsConfigurationTests
{
    private static IConfiguration BuildConfiguration(Dictionary<string, string?> Values)
        => new ConfigurationBuilder().AddInMemoryCollection(Values).Build();

    [Fact]
    public void WithoutConfigurationAllowsTheDefaultOrigins()
    {
        var Origins = CorsConfiguration.ResolveAllowedOrigins(BuildConfiguration(new()));

        Assert.Equal(CorsConfiguration.DefaultOrigins, Origins);
    }

    [Fact]
    public void ConfiguredOriginWithAnyIndexIsAddedWithoutReplacingDefaults()
    {
        // Caso real de Railway: Cors__AllowedOrigins__1 reemplazaba a http://127.0.0.1:5173
        var Origins = CorsConfiguration.ResolveAllowedOrigins(BuildConfiguration(new()
        {
            ["Cors:AllowedOrigins:1"] = "https://usystem.example.com",
        }));

        Assert.Contains("http://localhost:5173", Origins);
        Assert.Contains("http://127.0.0.1:5173", Origins);
        Assert.Contains("null", Origins);
        Assert.Contains("https://usystem.example.com", Origins);
    }

    [Fact]
    public void OriginsAreTrimmedAndDeduplicated()
    {
        var Origins = CorsConfiguration.ResolveAllowedOrigins(BuildConfiguration(new()
        {
            ["Cors:AllowedOrigins:0"] = " https://usystem.example.com/ ",
            ["Cors:AllowedOrigins:1"] = "HTTP://LOCALHOST:5173",
            ["Cors:AllowedOrigins:2"] = "",
        }));

        Assert.Contains("https://usystem.example.com", Origins);
        Assert.Equal(4, Origins.Length);
    }
}
