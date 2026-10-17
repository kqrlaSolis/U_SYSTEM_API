// Parámetros del sistema (SystemSettings): claves fijas de la semilla, solo se edita el valor.
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface ISystemSettingRepository
{
    Task<IReadOnlyList<SystemSetting>> GetAllAsync(CancellationToken Cancellation = default);

    Task<SystemSetting?> GetByKeyAsync(string SettingKey, CancellationToken Cancellation = default);

    void Update(SystemSetting Setting);
}
