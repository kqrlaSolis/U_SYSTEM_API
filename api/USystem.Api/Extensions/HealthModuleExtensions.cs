// Módulo de salud: /health revisa la conexión a MySQL para el despliegue y las pruebas de humo.
// /api/v1/health sigue sin tocar la base: solo confirma que la API responde.
using Microsoft.Extensions.DependencyInjection;
using USystem.Api.HealthChecks;

namespace USystem.Api.Extensions;

public static class HealthModuleExtensions
{
    public static IServiceCollection AddHealthModule(this IServiceCollection Services)
    {
        Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

        return Services;
    }
}
