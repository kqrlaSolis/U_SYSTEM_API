// Módulo de salud: /health para el despliegue y las pruebas de humo.
using Microsoft.Extensions.DependencyInjection;

namespace USystem.Api.Extensions;

public static class HealthModuleExtensions
{
    public static IServiceCollection AddHealthModule(this IServiceCollection Services)
    {
        Services.AddHealthChecks();
        return Services;
    }
}
