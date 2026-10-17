// Pruebas de la emisión del JWT.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using USystem.Domain.Entities;
using USystem.Infrastructure.Security;

namespace USystem.Tests.UnitTests;

public class JwtTokenGeneratorTests
{
    private const string SecretKey = "USystemTests_0123456789abcdefghijklmnopqrstuvwxyz_ABCDEFG";

    private static User BuildUser() => new()
    {
        Id = 7,
        FirstName = "Ana",
        PaternalLastName = "Pérez",
        Email = "ana@escuela.mx",
        Username = "aperez",
        PasswordHash = "hash",
    };

    [Fact]
    public void GeneratesTokenWithPermissionAndRoleClaims()
    {
        var Generator = new JwtTokenGenerator(Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            Issuer = "USystem",
            Audience = "USystem.Desktop",
            SecretKey = SecretKey,
            AccessTokenMinutes = 15,
        }));

        var Token = Generator.GenerateAccessToken(
            BuildUser(),
            new[] { "Administrator" },
            new[] { "Users.View", "Users.Create", "Users.View" });

        var Parsed = new JwtSecurityTokenHandler().ReadJwtToken(Token.Token);

        Assert.Equal("USystem", Parsed.Issuer);
        Assert.Contains(Parsed.Audiences, Audience => Audience == "USystem.Desktop");
        Assert.Equal("7", Parsed.Claims.First(Claim => Claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("aperez", Parsed.Claims.First(Claim => Claim.Type == JwtRegisteredClaimNames.UniqueName).Value);

        // Los permisos repetidos se envían una sola vez
        var Permissions = Parsed.Claims
            .Where(Claim => Claim.Type == JwtTokenGenerator.PermissionClaimType)
            .Select(Claim => Claim.Value)
            .ToArray();

        Assert.Equal(2, Permissions.Length);
        Assert.Contains("Users.View", Permissions);
        Assert.Contains("Users.Create", Permissions);

        Assert.Contains(
            Parsed.Claims,
            Claim => Claim.Type == ClaimTypes.Role && Claim.Value == "Administrator");

        Assert.True(Token.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void FailsWhenSecretIsMissing()
    {
        var Generator = new JwtTokenGenerator(Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            SecretKey = "short",
        }));

        Assert.Throws<InvalidOperationException>(
            () => Generator.GenerateAccessToken(BuildUser(), Array.Empty<string>(), Array.Empty<string>()));
    }
}
