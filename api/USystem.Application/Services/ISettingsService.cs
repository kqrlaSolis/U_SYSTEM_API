// Casos de uso de los parámetros del sistema.
using USystem.Application.Dtos;

namespace USystem.Application.Services;

public interface ISettingsService
{
    Task<IReadOnlyList<SystemSettingDto>> GetAllAsync(CancellationToken Cancellation = default);

    Task<SystemSettingDto> UpdateAsync(
        string SettingKey,
        UpdateSettingRequest Request,
        CancellationToken Cancellation = default);
}
