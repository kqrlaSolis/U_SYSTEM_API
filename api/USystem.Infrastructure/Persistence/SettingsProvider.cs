// Lectura cacheada de SystemSettings: los valores se invalidan al guardarlos desde el módulo 21.
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using USystem.Application.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Persistence;

public class SettingsProvider : ISettingsProvider
{
    private const string CacheKey = "USystem.Settings";
    private static readonly TimeSpan _CacheDuration = TimeSpan.FromMinutes(5);

    private readonly USystemDbContext _Context;
    private readonly IMemoryCache _Cache;

    public SettingsProvider(USystemDbContext Context, IMemoryCache Cache)
    {
        _Context = Context;
        _Cache = Cache;
    }

    public async Task<int> GetIntAsync(
        string SettingKey,
        int DefaultValue,
        CancellationToken Cancellation = default)
    {
        var RawValue = await GetRawValueAsync(SettingKey, Cancellation);
        return int.TryParse(RawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var Value)
            ? Value
            : DefaultValue;
    }

    public async Task<decimal> GetDecimalAsync(
        string SettingKey,
        decimal DefaultValue,
        CancellationToken Cancellation = default)
    {
        var RawValue = await GetRawValueAsync(SettingKey, Cancellation);
        return decimal.TryParse(RawValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var Value)
            ? Value
            : DefaultValue;
    }

    public async Task<string> GetStringAsync(
        string SettingKey,
        string DefaultValue,
        CancellationToken Cancellation = default)
    {
        var RawValue = await GetRawValueAsync(SettingKey, Cancellation);
        return string.IsNullOrWhiteSpace(RawValue) ? DefaultValue : RawValue;
    }

    public void Invalidate() => _Cache.Remove(CacheKey);

    private async Task<string?> GetRawValueAsync(string SettingKey, CancellationToken Cancellation)
    {
        var Settings = await GetAllAsync(Cancellation);
        return Settings.TryGetValue(SettingKey, out var Value) ? Value : null;
    }

    private async Task<Dictionary<string, string>> GetAllAsync(CancellationToken Cancellation)
    {
        if (_Cache.TryGetValue(CacheKey, out Dictionary<string, string>? Cached) && Cached is not null)
        {
            return Cached;
        }

        var Settings = await _Context.SystemSettings
            .AsNoTracking()
            .ToDictionaryAsync(
                Setting => Setting.SettingKey,
                Setting => Setting.SettingValue,
                Cancellation);

        _Cache.Set(CacheKey, Settings, _CacheDuration);
        return Settings;
    }
}
