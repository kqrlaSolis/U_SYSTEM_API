// Opciones de localización de la API (sección "Localization").
namespace USystem.Api.Localization;

public class LocalizationOptions
{
    public const string SectionName = "Localization";

    public string DefaultLanguage { get; set; } = "es";

    public string[] AvailableLanguages { get; set; } = { "es", "en" };
}
