// Revisa al arrancar que la cadena de conexión exista.
// En producción falta = error inmediato; en desarrollo y pruebas se permite el respaldo local.
namespace USystem.Api.Configuration;

public static class DatabaseConfiguration
{
    public static void EnsureConfigured(IConfiguration Configuration, IHostEnvironment Environment)
    {
        if (!Environment.IsProduction())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Configuration.GetConnectionString("DefaultConnection")))
        {
            throw new InvalidOperationException(
                "Configura ConnectionStrings:DefaultConnection (variable ConnectionStrings__DefaultConnection) "
                + "antes de arrancar la API en producción.");
        }
    }
}
