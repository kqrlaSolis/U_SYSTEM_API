// Pruebas del catálogo de mensajes i18n de la API.
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using USystem.Api.Localization;

namespace USystem.Tests.UnitTests;

public class JsonErrorMessageProviderTests : IDisposable
{
    private readonly string _RootPath;
    private readonly JsonErrorMessageProvider _Provider;

    public JsonErrorMessageProviderTests()
    {
        _RootPath = Path.Combine(Path.GetTempPath(), "usystem-tests", Guid.NewGuid().ToString("N"));
        var ResourcesPath = Path.Combine(_RootPath, "Resources");
        Directory.CreateDirectory(ResourcesPath);

        File.WriteAllText(
            Path.Combine(ResourcesPath, "ErrorMessagesEs.json"),
            """
            {
              "Api.Forbidden": "No tienes permisos para esta operación.",
              "Users.DuplicateEmail": "Ya existe un usuario con el correo {{Email}}."
            }
            """);

        File.WriteAllText(
            Path.Combine(ResourcesPath, "ErrorMessagesEn.json"),
            """
            {
              "Users.DuplicateEmail": "A user with the email {{Email}} already exists."
            }
            """);

        var Environment = new Mock<IHostEnvironment>();
        Environment.SetupGet(HostEnvironment => HostEnvironment.ContentRootPath).Returns(_RootPath);

        var Options = Microsoft.Extensions.Options.Options.Create(new LocalizationOptions
        {
            DefaultLanguage = "es",
            AvailableLanguages = new[] { "es", "en" },
        });

        _Provider = new JsonErrorMessageProvider(
            Environment.Object,
            Options,
            NullLogger<JsonErrorMessageProvider>.Instance);
    }

    [Fact]
    public void TranslatesSpanishMessageWithPlaceholders()
    {
        var Message = _Provider.GetMessage(
            "Users.DuplicateEmail",
            new Dictionary<string, string> { ["Email"] = "ana@escuela.mx" });

        Assert.Equal("Ya existe un usuario con el correo ana@escuela.mx.", Message);
    }

    [Fact]
    public void TranslatesByLanguage()
    {
        var Message = _Provider.GetMessage(
            "Users.DuplicateEmail",
            new Dictionary<string, string> { ["Email"] = "ana@escuela.mx" },
            "en-US");

        Assert.Equal("A user with the email ana@escuela.mx already exists.", Message);
    }

    [Fact]
    public void UnknownKeyReturnsTheCode()
    {
        Assert.Equal("Users.Unknown", _Provider.GetMessage("Users.Unknown"));
    }

    [Fact]
    public void MissingKeyInLanguageFallsBackToTheCode()
    {
        Assert.Equal("Api.Forbidden", _Provider.GetMessage("Api.Forbidden", null, "en"));
    }

    [Theory]
    [InlineData("en-US,en;q=0.9", "en")]
    [InlineData("fr-FR,es;q=0.8", "es")]
    [InlineData("de-DE", "es")]
    [InlineData("", "es")]
    public void ResolveLanguagePicksTheFirstSupportedTag(string AcceptLanguage, string Expected)
    {
        Assert.Equal(Expected, _Provider.ResolveLanguage(AcceptLanguage));
    }

    public void Dispose()
    {
        if (Directory.Exists(_RootPath))
        {
            Directory.Delete(_RootPath, true);
        }
    }
}
