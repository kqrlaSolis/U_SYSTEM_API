// Usuario autenticado de la petición actual; lo consumen la auditoría y los repositorios.
namespace USystem.Domain.Interfaces;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    long? UserId { get; }

    string? Username { get; }

    string? IpAddress { get; }

    IReadOnlyCollection<string> Permissions { get; }

    bool HasPermission(string Permission);
}
