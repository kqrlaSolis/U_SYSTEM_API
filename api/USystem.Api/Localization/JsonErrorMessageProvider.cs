// Traducción de los códigos de error usando los catálogos JSON de Resources/.
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using USystem.Application.Interfaces;

namespace USystem.Api.Localization;

public class JsonErrorMessageProvider : IErrorMessageProvider
{
    // Marcadores del estilo {{Field}}
    private static readonly Regex _PlaceholderPattern = new(
        @"\{\{\s*(?<Key>[A-Za-z0-9_]+)\s*\}\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly Dictionary<string, Dictionary<string, string>> _Catalogs =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly LocalizationOptions _Options;
    private readonly ILogger<JsonErrorMessageProvider> _Logger;

    public JsonErrorMessageProvider(
        IHostEnvironment Environment,
        IOptions<LocalizationOptions> Options,
        ILogger<JsonErrorMessageProvider> Logger)
    {
        _Options = Options.Value;
        _Logger = Logger;

        foreach (var Language in _Options.AvailableLanguages.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var Catalog = LoadCatalog(Environment.ContentRootPath, Language);
            if (Catalog is not null)
            {
                _Catalogs[Language] = Catalog;
            }
        }

        if (_Catalogs.Count == 0)
        {
            _Logger.LogWarning("No se cargó ningún catálogo de mensajes: revisa la carpeta Resources de la API.");
        }
    }

    public string GetMessage(
        string Code,
        IReadOnlyDictionary<string, string>? Values = null,
        string? Language = null)
    {
        var ResolvedLanguage = ResolveLanguage(Language);

        if (!_Catalogs.TryGetValue(ResolvedLanguage, out var Catalog)
            || !Catalog.TryGetValue(Code, out var Template))
        {
            // Si la clave no existe se devuelve el código: nunca se oculta el error
            return Code;
        }

        return ApplyValues(Template, Values);
    }

    public string ResolveLanguage(string? AcceptLanguage)
    {
        var DefaultLanguage = NormalizeLanguage(_Options.DefaultLanguage);
        if (string.IsNullOrWhiteSpace(AcceptLanguage))
        {
            return DefaultLanguage;
        }

        // Accept-Language puede traer varios idiomas con pesos: gana el primero soportado
        foreach (var Candidate in AcceptLanguage.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var LanguageTag = Candidate.Split(';')[0].Trim();
            var Language = NormalizeLanguage(LanguageTag);

            if (_Catalogs.ContainsKey(Language))
            {
                return Language;
            }
        }

        return DefaultLanguage;
    }

    // "es-MX" y "es_MX" se reducen a "es"
    private static string NormalizeLanguage(string? LanguageTag)
    {
        if (string.IsNullOrWhiteSpace(LanguageTag))
        {
            return "es";
        }

        var PrimaryTag = LanguageTag.Replace('_', '-').Split('-')[0].Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(PrimaryTag) ? "es" : PrimaryTag;
    }

    private Dictionary<string, string>? LoadCatalog(string ContentRootPath, string Language)
    {
        var FileLanguage = char.ToUpperInvariant(Language[0]) + Language[1..].ToLowerInvariant();
        var FilePath = Path.Combine(ContentRootPath, "Resources", $"ErrorMessages{FileLanguage}.json");

        if (!File.Exists(FilePath))
        {
            _Logger.LogWarning("No existe el catálogo de mensajes {FilePath}", FilePath);
            return null;
        }

        try
        {
            var Json = File.ReadAllText(FilePath);
            var Catalog = JsonSerializer.Deserialize<Dictionary<string, string>>(Json);
            return Catalog is null ? null : new Dictionary<string, string>(Catalog, StringComparer.Ordinal);
        }
        catch (JsonException CaughtException)
        {
            _Logger.LogError(CaughtException, "El catálogo de mensajes {FilePath} no es JSON válido", FilePath);
            return null;
        }
    }

    private static string ApplyValues(string Template, IReadOnlyDictionary<string, string>? Values)
    {
        if (Values is null || Values.Count == 0)
        {
            return Template;
        }

        return _PlaceholderPattern.Replace(Template, Match =>
        {
            var Key = Match.Groups["Key"].Value;
            return Values.TryGetValue(Key, out var Value)
                ? Value.ToString(CultureInfo.InvariantCulture)
                : Match.Value;
        });
    }
}
