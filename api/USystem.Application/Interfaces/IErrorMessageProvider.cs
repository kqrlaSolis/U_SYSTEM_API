// Traducción de los códigos de error a los catálogos de mensajes (es/en).
namespace USystem.Application.Interfaces;

public interface IErrorMessageProvider
{
    // Devuelve el mensaje del idioma pedido; si la clave no existe, devuelve el código
    string GetMessage(
        string Code,
        IReadOnlyDictionary<string, string>? Values = null,
        string? Language = null);

    // Normaliza el encabezado Accept-Language al idioma disponible
    string ResolveLanguage(string? AcceptLanguage);
}
