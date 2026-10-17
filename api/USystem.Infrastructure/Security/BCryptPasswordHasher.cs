// Hash de contraseñas con BCrypt. El algoritmo se puede cambiar sin tocar la capa de aplicación.
using USystem.Domain.Interfaces;

namespace USystem.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    // Costo 12: equilibrio entre seguridad y latencia del inicio de sesión
    private const int WorkFactor = 12;

    public string Hash(string Password)
        => BCrypt.Net.BCrypt.HashPassword(Password, WorkFactor);

    public bool Verify(string Password, string PasswordHash)
    {
        if (string.IsNullOrWhiteSpace(PasswordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(Password, PasswordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Un hash corrupto no debe tumbar el inicio de sesión: se trata como credencial inválida
            return false;
        }
    }

    public bool NeedsRehash(string PasswordHash)
    {
        if (string.IsNullOrWhiteSpace(PasswordHash))
        {
            return true;
        }

        try
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(PasswordHash, WorkFactor);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return true;
        }
    }
}
