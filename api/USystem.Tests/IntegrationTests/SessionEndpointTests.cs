// Pruebas de integración del pipeline de JWT con el atributo de autorización.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using USystem.Domain.Entities;
using USystem.Infrastructure.Security;

namespace USystem.Tests.IntegrationTests;

public class SessionEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _Factory;

    public SessionEndpointTests(ApiFactory Factory) => _Factory = Factory;

    [Fact]
    public async Task SessionWithoutTokenReturnsUnauthorized()
    {
        using var Client = _Factory.CreateClient();

        var Response = await Client.GetAsync("/api/v1/session");

        Assert.Equal(HttpStatusCode.Unauthorized, Response.StatusCode);

        using var Json = JsonDocument.Parse(await Response.Content.ReadAsStringAsync());
        Assert.False(Json.RootElement.GetProperty("Success").GetBoolean());
    }

    [Fact]
    public async Task SessionWithValidTokenReturnsClaims()
    {
        // Se firma con el mismo secreto que la fábrica inyectó para la API
        var Generator = new JwtTokenGenerator(Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            Issuer = "USystem",
            Audience = "USystem.Desktop",
            SecretKey = new string('t', 64),
        }));

        var Token = Generator.GenerateAccessToken(
            new User
            {
                Id = 42,
                FirstName = "Ana",
                PaternalLastName = "Pérez",
                Email = "ana@escuela.mx",
                Username = "aperez",
                PasswordHash = "hash",
            },
            new[] { "Administrator" },
            new[] { "Users.View", "Users.Create" });

        using var Client = _Factory.CreateClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.Token);

        var Response = await Client.GetAsync("/api/v1/session");
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);

        var Body = await Response.Content.ReadFromJsonAsync<JsonElement>();
        var Data = Body.GetProperty("Data");

        Assert.Equal(42, Data.GetProperty("UserId").GetInt64());
        Assert.Equal("aperez", Data.GetProperty("Username").GetString());
        Assert.True(Data.GetProperty("IsAuthenticated").GetBoolean());

        var Permissions = Data.GetProperty("Permissions")
            .EnumerateArray()
            .Select(Item => Item.GetString())
            .ToArray();

        Assert.Contains("Users.View", Permissions);
    }
}
