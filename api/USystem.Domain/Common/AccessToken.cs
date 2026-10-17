// Token de acceso que emite la API y consume el desktop.
namespace USystem.Domain.Common;

public record AccessToken(string Token, DateTime ExpiresAt);
