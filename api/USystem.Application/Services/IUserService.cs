// Casos de uso del módulo Users (alta, edición, estado, roles y contraseña temporal).
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<UserDto> GetByIdAsync(long Id, CancellationToken Cancellation = default);

    Task<CreatedUserResponse> CreateAsync(
        CreateUserRequest Request,
        CancellationToken Cancellation = default);

    Task<UserDto> UpdateAsync(
        long Id,
        UpdateUserRequest Request,
        CancellationToken Cancellation = default);

    Task<UserDto> SetStatusAsync(long Id, bool IsActive, CancellationToken Cancellation = default);

    Task<TemporaryPasswordResponse> ResetPasswordAsync(
        long Id,
        ResetUserPasswordRequest Request,
        CancellationToken Cancellation = default);

    Task<UserDto> AssignRolesAsync(
        long Id,
        IReadOnlyCollection<int> RoleIds,
        CancellationToken Cancellation = default);
}
