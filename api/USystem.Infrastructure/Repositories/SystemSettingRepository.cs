// Parámetros del sistema: lectura y actualización de valores (la clave no cambia).
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class SystemSettingRepository : ISystemSettingRepository
{
    private readonly USystemDbContext _Context;

    public SystemSettingRepository(USystemDbContext Context) => _Context = Context;

    public async Task<IReadOnlyList<SystemSetting>> GetAllAsync(CancellationToken Cancellation = default)
        => await _Context.SystemSettings
            .AsNoTracking()
            .OrderBy(Setting => Setting.Category)
            .ThenBy(Setting => Setting.SettingKey)
            .ToListAsync(Cancellation);

    public Task<SystemSetting?> GetByKeyAsync(string SettingKey, CancellationToken Cancellation = default)
        => _Context.SystemSettings.FirstOrDefaultAsync(
            Setting => Setting.SettingKey == SettingKey,
            Cancellation);

    public void Update(SystemSetting Setting) => _Context.SystemSettings.Update(Setting);
}
