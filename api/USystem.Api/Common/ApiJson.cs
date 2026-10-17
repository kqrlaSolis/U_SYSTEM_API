// Opciones de serialización JSON compartidas por los controladores y por el middleware de errores.
// El contrato de la API usa PascalCase en todas las propiedades.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace USystem.Api.Common;

public static class ApiJson
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
