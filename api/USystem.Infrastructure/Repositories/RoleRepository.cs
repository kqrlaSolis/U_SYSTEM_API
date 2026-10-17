// Acceso a datos de roles y su catálogo de permisos.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly USystemDbContext _Context;

    public RoleRepository(USystemDbContext Context) => _Context = Context;

    public Task<Role?> GetByIdAsync(int Id, CancellationToken Cancellation = default)
        => _Context.Roles.FirstOrDefaultAsync(Role => Role.Id == Id, Cancellation);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken Cancellation = default)
        => await _Context.Roles
            .AsNoTracking()
            .Where(Role => Role.IsActive)
            .OrderBy(Role => Role.Name)
            .ToListAsync(Cancellation);

    public async Task<IReadOnlyList<Role>> GetByIdsAsync(
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation = default)
    {
        if (RoleIds.Count == 0)
        {
            return Array.Empty<Role>();
        }

        return await _Context.Roles
            .AsNoTracking()
            .Where(Role => RoleIds.Contains(Role.Id))
            .OrderBy(Role => Role.Name)
            .ToListAsync(Cancellation);
    }

    public Task<bool> ExistsByNameAsync(
        string Name,
        int? ExcludeRoleId = null,
        CancellationToken Cancellation = default)
        => _Context.Roles.AnyAsync(
            Role => Role.Name == Name && (ExcludeRoleId == null || Role.Id != ExcludeRoleId),
            Cancellation);

    public Task<Role?> GetByNameAsync(
        string Name,
        bool IncludeDeleted = false,
        CancellationToken Cancellation = default)
    {
        var Source = IncludeDeleted ? _Context.Roles.IgnoreQueryFilters() : _Context.Roles;
        return Source.FirstOrDefaultAsync(Role => Role.Name == Name, Cancellation);
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(
        int RoleId,
        CancellationToken Cancellation = default)
        => await (from RolePermission in _Context.RolePermissions
                  join Permission in _Context.Permissions on RolePermission.PermissionId equals Permission.Id
                  where RolePermission.RoleId == RoleId
                  select Permission.Code).Distinct().OrderBy(Code => Code).ToListAsync(Cancellation);

    public async Task<IReadOnlyList<int>> GetPermissionIdsAsync(
        int RoleId,
        CancellationToken Cancellation = default)
        => await _Context.RolePermissions
            .Where(RolePermission => RolePermission.RoleId == RoleId)
            .Select(RolePermission => RolePermission.PermissionId)
            .Distinct()
            .ToListAsync(Cancellation);

    public async Task ReplacePermissionsAsync(
        int RoleId,
        IReadOnlyCollection<int> PermissionIds,
        CancellationToken Cancellation = default)
    {
        var DesiredIds = PermissionIds.Distinct().ToHashSet();
        var Existing = await _Context.RolePermissions
            .Where(RolePermission => RolePermission.RoleId == RoleId)
            .ToListAsync(Cancellation);

        // Solo se quita lo que sobra y se agrega lo que falta: evita chocar con la llave compuesta
        var ToRemove = Existing
            .Where(RolePermission => !DesiredIds.Contains(RolePermission.PermissionId))
            .ToList();
        _Context.RolePermissions.RemoveRange(ToRemove);

        var ExistingIds = Existing.Select(RolePermission => RolePermission.PermissionId).ToHashSet();
        foreach (var PermissionId in DesiredIds.Where(Id => !ExistingIds.Contains(Id)))
        {
            await _Context.RolePermissions.AddAsync(
                new RolePermission { RoleId = RoleId, PermissionId = PermissionId },
                Cancellation);
        }
    }

    public Task<int> CountUsersAsync(int RoleId, CancellationToken Cancellation = default)
        => _Context.UserRoles.CountAsync(UserRole => UserRole.RoleId == RoleId, Cancellation);

    public async Task<IReadOnlyDictionary<int, int>> GetUserCountsAsync(
        CancellationToken Cancellation = default)
        => await _Context.UserRoles
            .GroupBy(UserRole => UserRole.RoleId)
            .Select(Group => new { RoleId = Group.Key, Count = Group.Count() })
            .ToDictionaryAsync(Row => Row.RoleId, Row => Row.Count, Cancellation);

    public async Task<IReadOnlyDictionary<int, int>> GetPermissionCountsAsync(
        CancellationToken Cancellation = default)
        => await _Context.RolePermissions
            .GroupBy(RolePermission => RolePermission.RoleId)
            .Select(Group => new { RoleId = Group.Key, Count = Group.Count() })
            .ToDictionaryAsync(Row => Row.RoleId, Row => Row.Count, Cancellation);

    public async Task<IReadOnlyList<Permission>> GetCatalogAsync(CancellationToken Cancellation = default)
        => await _Context.Permissions
            .AsNoTracking()
            .OrderBy(Permission => Permission.Module)
            .ThenBy(Permission => Permission.Code)
            .ToListAsync(Cancellation);

    public async Task<PagedResult<Role>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(Role => Role.Name.Contains(Search)
                                       || (Role.Description != null && Role.Description.Contains(Search)));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(Role => Role.Name)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<Role>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(Role Entity, CancellationToken Cancellation = default)
        => await _Context.Roles.AddAsync(Entity, Cancellation);

    public void Update(Role Entity) => _Context.Roles.Update(Entity);

    public void Remove(Role Entity) => _Context.Roles.Remove(Entity);
}
