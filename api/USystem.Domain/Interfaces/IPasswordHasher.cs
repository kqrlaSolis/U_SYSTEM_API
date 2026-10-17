// Hash de contraseñas: la contraseña en texto plano nunca se guarda ni se registra.
namespace USystem.Domain.Interfaces;

public interface IPasswordHasher
{
    string Hash(string Password);

    bool Verify(string Password, string PasswordHash);

    // Permite migrar el algoritmo sin pedir la contraseña de nuevo
    bool NeedsRehash(string PasswordHash);
}
