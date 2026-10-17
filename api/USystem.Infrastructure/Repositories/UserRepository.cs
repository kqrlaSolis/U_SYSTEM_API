// Acceso a datos de usuarios, sus roles y sus permisos efectivos.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private const string AdministratorRoleName = "Administrator";

    private readonly USystemDbContext _Context;

    public UserRepository(USystemDbContext Context) => _Context = Context;

    public Task<User?> GetByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.Users.FirstOrDefaultAsync(User => User.Id == Id, Cancellation);

    public Task<User?> GetByLoginAsync(string UsernameOrEmail, CancellationToken Cancellation = default)
        => _Context.Users.FirstOrDefaultAsync(
            User => User.Username == UsernameOrEmail || User.Email == UsernameOrEmail,
            Cancellation);

    public Task<User?> GetByEmailAsync(string Email, CancellationToken Cancellation = default)
        => _Context.Users.FirstOrDefaultAsync(User => User.Email == Email, Cancellation);

    public Task<bool> ExistsByEmailAsync(
        string Email,
        long? ExcludeUserId = null,
        CancellationToken Cancellation = default)
        => _Context.Users.AnyAsync(
            User => User.Email == Email && (ExcludeUserId == null || User.Id != ExcludeUserId),
            Cancellation);

    public Task<bool> ExistsByUsernameAsync(
        string Username,
        long? ExcludeUserId = null,
        CancellationToken Cancellation = default)
        => _Context.Users.AnyAsync(
            User => User.Username == Username && (ExcludeUserId == null || User.Id != ExcludeUserId),
            Cancellation);

    public Task<int> CountActiveAdministratorsAsync(CancellationToken Cancellation = default)
        => (from User in _Context.Users
            join UserRole in _Context.UserRoles on User.Id equals UserRole.UserId
            join Role in _Context.Roles on UserRole.RoleId equals Role.Id
            where User.IsActive && Role.Name == AdministratorRoleName
            select User.Id).Distinct().CountAsync(Cancellation);

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(
        long UserId,
        CancellationToken Cancellation = default)
        => await (from UserRole in _Context.UserRoles
                  join Role in _Context.Roles on UserRole.RoleId equals Role.Id
                  where UserRole.UserId == UserId && Role.IsActive
                  select Role.Name).Distinct().OrderBy(Name => Name).ToListAsync(Cancellation);

    public async Task<IReadOnlyList<int>> GetRoleIdsAsync(
        long UserId,
        CancellationToken Cancellation = default)
        => await _Context.UserRoles
            .Where(UserRole => UserRole.UserId == UserId)
            .Select(UserRole => UserRole.RoleId)
            .Distinct()
            .ToListAsync(Cancellation);

    public async Task<IReadOnlyDictionary<long, IReadOnlyList<string>>> GetRoleNamesByUserIdsAsync(
        IReadOnlyCollection<long> UserIds,
        CancellationToken Cancellation = default)
    {
        if (UserIds.Count == 0)
        {
            return new Dictionary<long, IReadOnlyList<string>>();
        }

        var Links = await (from UserRole in _Context.UserRoles
                           join Role in _Context.Roles on UserRole.RoleId equals Role.Id
                           where UserIds.Contains(UserRole.UserId) && Role.IsActive
                           select new { UserRole.UserId, Role.Name }).ToListAsync(Cancellation);

        return Links
            .GroupBy(Link => Link.UserId)
            .ToDictionary(
                Group => Group.Key,
                Group => (IReadOnlyList<string>)Group.Select(Link => Link.Name).OrderBy(Name => Name).ToList());
    }

    // Permisos efectivos: se suman los de todos los roles activos del usuario
    public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(
        long UserId,
        CancellationToken Cancellation = default)
        => await (from UserRole in _Context.UserRoles
                  join Role in _Context.Roles on UserRole.RoleId equals Role.Id
                  join RolePermission in _Context.RolePermissions on Role.Id equals RolePermission.RoleId
                  join Permission in _Context.Permissions on RolePermission.PermissionId equals Permission.Id
                  where UserRole.UserId == UserId && Role.IsActive
                  select Permission.Code).Distinct().OrderBy(Code => Code).ToListAsync(Cancellation);

    public async Task<IReadOnlyDictionary<long, string>> GetUsernamesAsync(
        IReadOnlyCollection<long> UserIds,
        CancellationToken Cancellation = default)
    {
        if (UserIds.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        return await _Context.Users
            .AsNoTracking()
            .Where(User => UserIds.Contains(User.Id))
            .Select(User => new { User.Id, User.Username })
            .ToDictionaryAsync(Row => Row.Id, Row => Row.Username, Cancellation);
    }

    public async Task ReplaceRolesAsync(
        long UserId,
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation = default)
    {
        var DesiredIds = RoleIds.Distinct().ToHashSet();
        var Existing = await _Context.UserRoles
            .Where(UserRole => UserRole.UserId == UserId)
            .ToListAsync(Cancellation);

        // Solo se quita lo que sobra y se agrega lo que falta: evita chocar con la llave compuesta
        var ToRemove = Existing
            .Where(UserRole => !DesiredIds.Contains(UserRole.RoleId))
            .ToList();
        _Context.UserRoles.RemoveRange(ToRemove);

        var ExistingIds = Existing.Select(UserRole => UserRole.RoleId).ToHashSet();
        foreach (var RoleId in DesiredIds.Where(Id => !ExistingIds.Contains(Id)))
        {
            await _Context.UserRoles.AddAsync(new UserRole { UserId = UserId, RoleId = RoleId }, Cancellation);
        }
    }

    public async Task<PagedResult<User>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Source = _Context.Users.AsNoTracking();

        // Búsqueda por nombre, usuario o correo
        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            var Search = Query.Search.Trim();
            Source = Source.Where(User => User.Username.Contains(Search)
                                       || User.Email.Contains(Search)
                                       || User.FirstName.Contains(Search)
                                       || User.PaternalLastName.Contains(Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(User => User.Username)
            .Skip(Query.Skip)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return PagedResult<User>.Create(Items, TotalCount, Query);
    }

    public async Task AddAsync(User Entity, CancellationToken Cancellation = default)
        => await _Context.Users.AddAsync(Entity, Cancellation);

    public void Update(User Entity) => _Context.Users.Update(Entity);

    public void Remove(User Entity) => _Context.Users.Remove(Entity);
}
