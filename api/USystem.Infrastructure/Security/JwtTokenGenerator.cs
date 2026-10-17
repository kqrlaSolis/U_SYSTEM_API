// Emisión del access token JWT. Los permisos viajan como un claim "Permission" por cada código.
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;

namespace USystem.Infrastructure.Security;

public class JwtTokenGenerator : ITokenGenerator
{
    // Claim que valida RequirePermissionAttribute
    public const string PermissionClaimType = "Permission";

    private readonly JwtOptions _Options;

    public JwtTokenGenerator(IOptions<JwtOptions> Options) => _Options = Options.Value;

    public AccessToken GenerateAccessToken(
        User User,
        IEnumerable<string> Roles,
        IEnumerable<string> Permissions)
    {
        if (string.IsNullOrWhiteSpace(_Options.SecretKey) || _Options.SecretKey.Length < JwtOptions.MinimumSecretLength)
        {
            throw new InvalidOperationException(
                $"Jwt:SecretKey no está configurado o tiene menos de {JwtOptions.MinimumSecretLength} caracteres.");
        }

        var ExpiresAt = DateTime.UtcNow.AddMinutes(_Options.AccessTokenMinutes <= 0 ? 15 : _Options.AccessTokenMinutes);
        var UserId = User.Id.ToString(CultureInfo.InvariantCulture);

        var Claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, UserId),
            new(ClaimTypes.NameIdentifier, UserId),
            new(JwtRegisteredClaimNames.UniqueName, User.Username),
            new(JwtRegisteredClaimNames.Email, User.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        Claims.AddRange(Roles.Select(Role => new Claim(ClaimTypes.Role, Role)));
        Claims.AddRange(Permissions
            .Where(Permission => !string.IsNullOrWhiteSpace(Permission))
            .Distinct(StringComparer.Ordinal)
            .Select(Permission => new Claim(PermissionClaimType, Permission)));

        var SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Options.SecretKey));
        var Credentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);

        var Token = new JwtSecurityToken(
            issuer: _Options.Issuer,
            audience: _Options.Audience,
            claims: Claims,
            notBefore: DateTime.UtcNow,
            expires: ExpiresAt,
            signingCredentials: Credentials);

        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(Token), ExpiresAt);
    }
}
