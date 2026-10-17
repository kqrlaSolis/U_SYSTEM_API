// Política de contraseñas: mínimo 8 con mayúscula, minúscula, número y símbolo.
namespace USystem.Application.Common;

public static class PasswordPolicy
{
    public const int DefaultMinimumLength = 8;

    // Devuelve el código i18n del error o null cuando la contraseña cumple
    public static string? Validate(string? Password, int MinimumLength = DefaultMinimumLength)
    {
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < MinimumLength)
        {
            return "Validation.PasswordPolicy";
        }

        var HasUpper = Password.Any(char.IsUpper);
        var HasLower = Password.Any(char.IsLower);
        var HasDigit = Password.Any(char.IsDigit);
        var HasSymbol = Password.Any(Character => !char.IsLetterOrDigit(Character));

        return HasUpper && HasLower && HasDigit && HasSymbol ? null : "Validation.PasswordPolicy";
    }
}
