// Acceso a datos del módulo Users.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    // El inicio de sesión acepta el nombre de usuario o el correo
    Task<User?> GetByLoginAsync(string UsernameOrEmail, CancellationToken Cancellation = default);

    Task<User?> GetByEmailAsync(string Email, CancellationToken Cancellation = default);

    Task<bool> ExistsByEmailAsync(
        string Email,
        long? ExcludeUserId = null,
        CancellationToken Cancellation = default);

    Task<bool> ExistsByUsernameAsync(
        string Username,
        long? ExcludeUserId = null,
        CancellationToken Cancellation = default);

    Task<int> CountActiveAdministratorsAsync(CancellationToken Cancellation = default);

    Task<IReadOnlyList<string>> GetRoleNamesAsync(long UserId, CancellationToken Cancellation = default);

    Task<IReadOnlyList<int>> GetRoleIdsAsync(long UserId, CancellationToken Cancellation = default);

    // Nombres de rol de varios usuarios a la vez (para los listados)
    Task<IReadOnlyDictionary<long, IReadOnlyList<string>>> GetRoleNamesByUserIdsAsync(
        IReadOnlyCollection<long> UserIds,
        CancellationToken Cancellation = default);

    // Nombres de usuario a partir de sus Id (para la bitácora)
    Task<IReadOnlyDictionary<long, string>> GetUsernamesAsync(
        IReadOnlyCollection<long> UserIds,
        CancellationToken Cancellation = default);

    // Permisos efectivos: solo de roles activos
    Task<IReadOnlyList<string>> GetPermissionCodesAsync(long UserId, CancellationToken Cancellation = default);

    Task ReplaceRolesAsync(
        long UserId,
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation = default);
}
