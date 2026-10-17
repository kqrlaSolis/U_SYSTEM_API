// Usuario autenticado tomado del contexto HTTP (lo usa la auditoría y el DbContext).
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Security;

namespace USystem.Api.Security;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _HttpContextAccessor;

    public CurrentUserService(IHttpContextAccessor HttpContextAccessor)
        => _HttpContextAccessor = HttpContextAccessor;

    private ClaimsPrincipal? Principal => _HttpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public long? UserId
    {
        get
        {
            var Value = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return long.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var UserId)
                ? UserId
                : null;
        }
    }

    public string? Username
        => Principal?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
        ?? Principal?.Identity?.Name;

    public string? IpAddress
        => _HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public IReadOnlyCollection<string> Permissions
        => Principal?.FindAll(JwtTokenGenerator.PermissionClaimType)
            .Select(Claim => Claim.Value)
            .ToArray()
        ?? Array.Empty<string>();

    public bool HasPermission(string Permission)
        => Principal?.HasClaim(JwtTokenGenerator.PermissionClaimType, Permission) == true;
}
