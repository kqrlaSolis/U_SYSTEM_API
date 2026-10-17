// Reglas de roles: catálogo de permisos fijo, roles de sistema protegidos y anti-bloqueo del Administrator.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class RoleService : IRoleService
{
    private const string AdministratorRoleName = "Administrator";

    private readonly IRoleRepository _RoleRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public RoleService(
        IRoleRepository RoleRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _RoleRepository = RoleRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<RoleDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _RoleRepository.GetPagedAsync(Query, Cancellation);
        var UserCounts = await _RoleRepository.GetUserCountsAsync(Cancellation);
        var PermissionCounts = await _RoleRepository.GetPermissionCountsAsync(Cancellation);

        var Items = Page.Items
            .Select(Role => RoleMapper.ToDto(
                Role,
                UserCounts.TryGetValue(Role.Id, out var UserCount) ? UserCount : 0,
                PermissionCounts.TryGetValue(Role.Id, out var PermissionCount) ? PermissionCount : 0))
            .ToList();

        return PagedResult<RoleDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<RoleDto> GetByIdAsync(int Id, CancellationToken Cancellation = default)
    {
        var Role = await _RoleRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Roles.NotFound");

        var PermissionCodes = await _RoleRepository.GetPermissionCodesAsync(Role.Id, Cancellation);
        var UserCount = await _RoleRepository.CountUsersAsync(Role.Id, Cancellation);

        return RoleMapper.ToDto(Role, UserCount, PermissionCodes.Count, PermissionCodes);
    }

    public async Task<RoleDto> CreateAsync(
        CreateRoleRequest Request,
        CancellationToken Cancellation = default)
    {
        var Name = Request.Name.Trim();
        var Description = string.IsNullOrWhiteSpace(Request.Description) ? null : Request.Description.Trim();
        var PermissionIds = await ResolvePermissionIdsAsync(Request.PermissionCodes, Cancellation);

        // El índice único de Roles.Name incluye los roles dados de baja: si existe uno eliminado, se reactiva
        var ExistingRole = await _RoleRepository.GetByNameAsync(Name, true, Cancellation);
        if (ExistingRole is not null)
        {
            if (ExistingRole.DeletedAt is null)
            {
                throw BusinessRuleException.Conflict(
                    "Roles.NameTaken",
                    new Dictionary<string, string> { ["Name"] = Name });
            }

            ExistingRole.DeletedAt = null;
            ExistingRole.IsActive = true;
            ExistingRole.IsSystem = false;
            ExistingRole.Description = Description;
            _RoleRepository.Update(ExistingRole);

            await _RoleRepository.ReplacePermissionsAsync(ExistingRole.Id, PermissionIds, Cancellation);
            await _UnitOfWork.SaveChangesAsync(Cancellation);

            await _AuditService.LogAsync(
                "Restore",
                "Roles",
                nameof(Role),
                ExistingRole.Id.ToString(CultureInfo.InvariantCulture),
                null,
                new { ExistingRole.Name, Permissions = Request.PermissionCodes },
                Cancellation);

            return await GetByIdAsync(ExistingRole.Id, Cancellation);
        }

        var NewRole = new Role
        {
            Name = Name,
            Description = Description,
            IsSystem = false,
            IsActive = true,
        };

        await _RoleRepository.AddAsync(NewRole, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _RoleRepository.ReplacePermissionsAsync(NewRole.Id, PermissionIds, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Roles",
            nameof(Role),
            NewRole.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewRole.Name, Permissions = Request.PermissionCodes },
            Cancellation);

        return await GetByIdAsync(NewRole.Id, Cancellation);
    }

    public async Task<RoleDto> UpdateAsync(
        int Id,
        UpdateRoleRequest Request,
        CancellationToken Cancellation = default)
    {
        var Role = await _RoleRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Roles.NotFound");

        var Name = Request.Name.Trim();
        var NameOwner = await _RoleRepository.GetByNameAsync(Name, true, Cancellation);
        if (NameOwner is not null && NameOwner.Id != Role.Id)
        {
            throw BusinessRuleException.Conflict(
                "Roles.NameTaken",
                new Dictionary<string, string> { ["Name"] = Name });
        }

        var Catalog = await _RoleRepository.GetCatalogAsync(Cancellation);
        var RequestedCodes = Request.PermissionCodes
            .Where(Code => !string.IsNullOrWhiteSpace(Code))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        // El rol Administrator conserva el catálogo completo: es la garantía de acceso del sistema
        if (Role.Name == AdministratorRoleName)
        {
            var MissingCode = Catalog
                .Select(Permission => Permission.Code)
                .FirstOrDefault(Code => !RequestedCodes.Contains(Code, StringComparer.Ordinal));

            if (MissingCode is not null)
            {
                throw new BusinessRuleException(
                    "Roles.AdminPermissionRequired",
                    new Dictionary<string, string> { ["Permission"] = MissingCode });
            }
        }

        var PermissionIds = await ResolvePermissionIdsAsync(RequestedCodes, Cancellation);

        var Before = new { Role.Name, Role.Description, Role.IsActive, Permissions = RequestedCodes };

        Role.Name = Name;
        Role.Description = string.IsNullOrWhiteSpace(Request.Description) ? null : Request.Description.Trim();
        Role.IsActive = Request.IsActive;
        _RoleRepository.Update(Role);

        await _RoleRepository.ReplacePermissionsAsync(Role.Id, PermissionIds, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Roles",
            nameof(Role),
            Role.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Role.Name, Role.Description, Role.IsActive, Permissions = RequestedCodes },
            Cancellation);

        return await GetByIdAsync(Role.Id, Cancellation);
    }

    public async Task DeleteAsync(int Id, CancellationToken Cancellation = default)
    {
        var Role = await _RoleRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Roles.NotFound");

        // Los roles de sistema (incluido Administrator) no se eliminan
        if (Role.IsSystem)
        {
            throw new BusinessRuleException(
                "Roles.SystemProtected",
                new Dictionary<string, string> { ["Name"] = Role.Name });
        }

        var UserCount = await _RoleRepository.CountUsersAsync(Role.Id, Cancellation);
        if (UserCount > 0)
        {
            throw new BusinessRuleException(
                "Roles.HasUsers",
                new Dictionary<string, string>
                {
                    ["UserCount"] = UserCount.ToString(CultureInfo.InvariantCulture),
                });
        }

        _RoleRepository.Remove(Role);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Roles",
            nameof(Role),
            Role.Id.ToString(CultureInfo.InvariantCulture),
            new { Role.Name },
            null,
            Cancellation);
    }

    public async Task<IReadOnlyList<PermissionModuleDto>> GetPermissionCatalogAsync(
        CancellationToken Cancellation = default)
    {
        var Catalog = await _RoleRepository.GetCatalogAsync(Cancellation);

        return Catalog
            .GroupBy(Permission => Permission.Module)
            .OrderBy(Group => Group.Key, StringComparer.Ordinal)
            .Select(Group => new PermissionModuleDto
            {
                Module = Group.Key,
                Permissions = Group
                    .OrderBy(Permission => Permission.Code, StringComparer.Ordinal)
                    .Select(RoleMapper.ToDto)
                    .ToList(),
            })
            .ToList();
    }

    // Valida los códigos contra el catálogo y devuelve los Id de la tabla Permissions
    private async Task<IReadOnlyCollection<int>> ResolvePermissionIdsAsync(
        IReadOnlyCollection<string> PermissionCodes,
        CancellationToken Cancellation)
    {
        if (PermissionCodes.Count == 0)
        {
            return Array.Empty<int>();
        }

        var Catalog = await _RoleRepository.GetCatalogAsync(Cancellation);
        var CatalogByCode = Catalog.ToDictionary(
            Permission => Permission.Code,
            Permission => Permission.Id,
            StringComparer.Ordinal);

        var UnknownCodes = PermissionCodes
            .Where(Code => !CatalogByCode.ContainsKey(Code))
            .ToArray();

        if (UnknownCodes.Length > 0)
        {
            throw new ValidationException("Validation.InvalidFormat", nameof(PermissionCodes));
        }

        return PermissionCodes
            .Select(Code => CatalogByCode[Code])
            .Distinct()
            .ToArray();
    }
}
