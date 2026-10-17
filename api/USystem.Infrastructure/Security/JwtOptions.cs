// Opciones de configuración del JWT (sección "Jwt" o variables Jwt__*).
namespace USystem.Infrastructure.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "USystem";

    public string Audience { get; set; } = "USystem.Desktop";

    public string SecretKey { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; } = 15;

    public int RefreshTokenDays { get; set; } = 7;

    // Largo mínimo recomendado para HS256
    public const int MinimumSecretLength = 32;
}
