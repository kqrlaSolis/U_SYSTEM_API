// Tokens opacos (refresh y recuperación): se generan aleatorios y se guardan solo como hash.
using System.Security.Cryptography;
using System.Text;

namespace USystem.Application.Common;

public static class SecureToken
{
    // 48 bytes ⇒ 64 caracteres base64url, suficiente entropía para un refresh token
    public const int DefaultByteLength = 48;

    public static string Generate(int ByteLength = DefaultByteLength)
    {
        var Bytes = RandomNumberGenerator.GetBytes(ByteLength);
        return Convert.ToBase64String(Bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    // SHA-256 en hexadecimal: lo que se persiste en la columna TokenHash
    public static string Hash(string Token)
    {
        var Bytes = SHA256.HashData(Encoding.UTF8.GetBytes(Token));
        return Convert.ToHexString(Bytes).ToLowerInvariant();
    }
}
