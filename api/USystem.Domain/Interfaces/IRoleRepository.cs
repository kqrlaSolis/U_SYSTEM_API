// Roles del sistema (RBAC). El catálogo de permisos es fijo y vive en PermissionCodes.
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken Cancellation = default);

    Task<IReadOnlyList<Role>> GetByIdsAsync(
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation = default);

    Task<bool> ExistsByNameAsync(
        string Name,
        int? ExcludeRoleId = null,
        CancellationToken Cancellation = default);

    // Con IncludeDeleted=true se incluye el rol dado de baja: el índice único de Name no admite repetidos
    Task<Role?> GetByNameAsync(
        string Name,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default);

    Task<IReadOnlyList<string>> GetPermissionCodesAsync(int RoleId, CancellationToken Cancellation = default);

    Task<IReadOnlyList<int>> GetPermissionIdsAsync(int RoleId, CancellationToken Cancellation = default);

    Task ReplacePermissionsAsync(
        int RoleId,
        IReadOnlyCollection<int> PermissionIds,
        CancellationToken Cancellation = default);

    Task<int> CountUsersAsync(int RoleId, CancellationToken Cancellation = default);

    // Conteos para los listados (usuarios asignados y permisos por rol)
    Task<IReadOnlyDictionary<int, int>> GetUserCountsAsync(CancellationToken Cancellation = default);

    Task<IReadOnlyDictionary<int, int>> GetPermissionCountsAsync(CancellationToken Cancellation = default);

    // Permisos del catálogo (tabla Permissions)
    Task<IReadOnlyList<Permission>> GetCatalogAsync(CancellationToken Cancellation = default);
}
