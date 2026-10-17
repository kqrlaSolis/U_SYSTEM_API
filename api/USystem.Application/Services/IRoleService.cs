// Casos de uso de roles y permisos (RBAC).
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IRoleService
{
    Task<PagedResult<RoleDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<RoleDto> GetByIdAsync(int Id, CancellationToken Cancellation = default);

    Task<RoleDto> CreateAsync(CreateRoleRequest Request, CancellationToken Cancellation = default);

    Task<RoleDto> UpdateAsync(
        int Id,
        UpdateRoleRequest Request,
        CancellationToken Cancellation = default);

    Task DeleteAsync(int Id, CancellationToken Cancellation = default);

    Task<IReadOnlyList<PermissionModuleDto>> GetPermissionCatalogAsync(
        CancellationToken Cancellation = default);
}
