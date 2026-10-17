// Módulo Settings: repositorio y servicio de parámetros del sistema.
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;

namespace USystem.Api.Extensions;

public static class SettingsModuleExtensions
{
    public static IServiceCollection AddSettingsModule(this IServiceCollection Services)
    {
        Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
        Services.AddScoped<ISettingsService, SettingsService>();
        return Services;
    }
}
