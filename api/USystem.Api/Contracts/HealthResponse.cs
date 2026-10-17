// Estado del servicio; lo consume el chequeo de despliegue y la pantalla inicial del desktop.
namespace USystem.Api.Contracts;

public class HealthResponse
{
    public string Status { get; set; } = "Healthy";

    public string Environment { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
}
