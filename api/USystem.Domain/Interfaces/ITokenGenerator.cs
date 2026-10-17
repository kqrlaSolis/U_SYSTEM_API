// Emisión de tokens JWT para la API.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ITokenGenerator
{
    // Un claim "Permission" por cada permiso, como exige RequirePermissionAttribute
    AccessToken GenerateAccessToken(
        User User,
        IEnumerable<string> Roles,
        IEnumerable<string> Permissions);
}
