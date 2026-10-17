// Pruebas de la política de contraseñas.
using USystem.Application.Common;

namespace USystem.Tests.UnitTests;

public class PasswordPolicyTests
{
    [Theory]
    [InlineData("Admin.2026")]
    [InlineData("Lectura.2026")]
    [InlineData("Aa1!aaaa")]
    public void AcceptsPasswordsWithEveryCharacterClass(string Password)
        => Assert.Null(PasswordPolicy.Validate(Password));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Aa1!aaa")]          // 7 caracteres
    [InlineData("admin.2026")]        // sin mayúscula
    [InlineData("ADMIN.2026")]        // sin minúscula
    [InlineData("Admin.aaaa")]        // sin número
    [InlineData("Admin2026")]         // sin símbolo
    public void RejectsPasswordsThatDoNotComply(string? Password)
        => Assert.Equal("Validation.PasswordPolicy", PasswordPolicy.Validate(Password));

    [Fact]
    public void UsesTheConfiguredMinimumLength()
    {
        Assert.Null(PasswordPolicy.Validate("Aa1!", 4));
        Assert.Equal("Validation.PasswordPolicy", PasswordPolicy.Validate("Aa1!", 8));
    }
}
