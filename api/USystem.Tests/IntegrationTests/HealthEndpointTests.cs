// Pruebas de integración de la API con el servidor en memoria.
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using USystem.Api.HealthChecks;

namespace USystem.Tests.IntegrationTests;

// El secreto del JWT no se versiona: la fábrica lo inyecta antes de levantar la API.
public class ApiFactory : WebApplicationFactory<USystem.Api.Program>
{
    public ApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Issuer", "USystem");
        Environment.SetEnvironmentVariable("Jwt__Audience", "USystem.Desktop");
        Environment.SetEnvironmentVariable("Jwt__SecretKey", new string('t', 64));
        Environment.SetEnvironmentVariable("Database__AutoDetectServerVersion", "false");
    }

    // Las pruebas no dependen de MySQL: se quita el chequeo de base de datos de /health
    // (DatabaseHealthCheckTests lo prueba por separado)
    protected override void ConfigureWebHost(IWebHostBuilder Builder)
        => Builder.ConfigureTestServices(Services =>
            Services.PostConfigure<HealthCheckServiceOptions>(Options =>
            {
                var DatabaseChecks = Options.Registrations
                    .Where(Registration => Registration.Name == DatabaseHealthCheck.Name)
                    .ToList();

                foreach (var Registration in DatabaseChecks)
                {
                    Options.Registrations.Remove(Registration);
                }
            }));
}

public class HealthEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _Factory;

    public HealthEndpointTests(ApiFactory Factory) => _Factory = Factory;

    [Fact]
    public async Task HealthReturnsTheStandardEnvelopeInPascalCase()
    {
        using var Client = _Factory.CreateClient();

        var Response = await Client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);

        using var Json = JsonDocument.Parse(await Response.Content.ReadAsStringAsync());
        var Root = Json.RootElement;

        Assert.True(Root.GetProperty("Success").GetBoolean());
        Assert.Equal("Healthy", Root.GetProperty("Data").GetProperty("Status").GetString());
        Assert.False(Root.TryGetProperty("success", out _));
    }

    [Fact]
    public async Task HealthChecksEndpointIsAnonymous()
    {
        using var Client = _Factory.CreateClient();

        var Response = await Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
    }

    [Fact]
    public async Task SwaggerIsExposedInDevelopment()
    {
        using var Client = _Factory.CreateClient();

        var Response = await Client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);

        var Document = await Response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("U SYSTEM API", Document.GetProperty("info").GetProperty("title").GetString());
    }
}
