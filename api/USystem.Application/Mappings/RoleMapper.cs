// Mapeo de roles y permisos a DTOs.
using USystem.Application.Dtos;
using USystem.Domain.Entities;

namespace USystem.Application.Mappings;

public static class RoleMapper
{
    public static RoleDto ToDto(
        Role Role,
        int UserCount = 0,
        int PermissionCount = 0,
        IReadOnlyList<string>? PermissionCodes = null) => new()
        {
            Id = Role.Id,
            Name = Role.Name,
            Description = Role.Description,
            IsSystem = Role.IsSystem,
            IsActive = Role.IsActive,
            UserCount = UserCount,
            PermissionCount = PermissionCount,
            PermissionCodes = PermissionCodes?.ToList() ?? new List<string>(),
        };

    public static PermissionDto ToDto(Permission Permission) => new()
    {
        Id = Permission.Id,
        Code = Permission.Code,
        Module = Permission.Module,
        Description = Permission.Description,
    };
}
