// Lectura cacheada de los parámetros de SystemSettings (módulo 21).
namespace USystem.Application.Interfaces;

public interface ISettingsProvider
{
    Task<int> GetIntAsync(string SettingKey, int DefaultValue, CancellationToken Cancellation = default);

    Task<decimal> GetDecimalAsync(
        string SettingKey,
        decimal DefaultValue,
        CancellationToken Cancellation = default);

    Task<string> GetStringAsync(
        string SettingKey,
        string DefaultValue,
        CancellationToken Cancellation = default);

    // Se invoca al guardar un parámetro para que el cambio aplique sin reiniciar
    void Invalidate();
}
