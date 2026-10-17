// Resuelve la versión del servidor MySQL que necesita Pomelo.
// Por defecto se usa la versión declarada en la configuración para no abrir una conexión al arrancar
// (importante en pruebas y en CI); con Database:AutoDetectServerVersion=true se consulta el servidor.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace USystem.Infrastructure.Persistence;

public static class MySqlVersionResolver
{
    private static readonly ServerVersion _FallbackVersion = new MySqlServerVersion(new Version(8, 0, 36));

    public static ServerVersion Resolve(IConfiguration Configuration)
    {
        var ConnectionString = Configuration.GetConnectionString("DefaultConnection");
        var AutoDetect = bool.TryParse(Configuration["Database:AutoDetectServerVersion"], out var AutoDetectValue)
            && AutoDetectValue;

        if (AutoDetect && !string.IsNullOrWhiteSpace(ConnectionString))
        {
            try
            {
                return ServerVersion.AutoDetect(ConnectionString);
            }
            catch (Exception)
            {
                // Si el servidor no responde, la API arranca igual: la conexión se reintenta en la primera consulta
            }
        }

        var ConfiguredVersion = Configuration["Database:ServerVersion"];
        return string.IsNullOrWhiteSpace(ConfiguredVersion)
            ? _FallbackVersion
            : ServerVersion.Parse(ConfiguredVersion);
    }
}
