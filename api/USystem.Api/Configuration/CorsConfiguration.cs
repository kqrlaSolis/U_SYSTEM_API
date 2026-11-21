// Orígenes permitidos por CORS para el desktop.
// Los orígenes de base viven en código y los configurados (Cors__AllowedOrigins__N) se SUMAN a ellos.
// Antes vivían en el arreglo de appsettings.json y una variable de entorno con el mismo índice
// reemplazaba en silencio a uno de base (así se perdió http://127.0.0.1:5173 en Railway).
namespace USystem.Api.Configuration;

public static class CorsConfiguration
{
    public const string SectionName = "Cors:AllowedOrigins";

    // Vite en desarrollo (localhost y 127.0.0.1) y el renderer file:// del desktop empaquetado
    public static readonly IReadOnlyList<string> DefaultOrigins = new[]
    {
        "http://localhost:5173",
        "http://127.0.0.1:5173",
        "null",
    };

    public static string[] ResolveAllowedOrigins(IConfiguration Configuration)
    {
        var Configured = Configuration.GetSection(SectionName).Get<string?[]>() ?? Array.Empty<string?>();

        return DefaultOrigins
            .Concat(Configured)
            // Un origen nunca lleva "/" final; con él no coincidiría con el encabezado Origin
            .Select(Origin => Origin?.Trim().TrimEnd('/'))
            .Where(Origin => !string.IsNullOrEmpty(Origin))
            .Select(Origin => Origin!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
