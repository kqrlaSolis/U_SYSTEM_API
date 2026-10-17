// Pruebas del hash de contraseñas.
using USystem.Infrastructure.Security;

namespace USystem.Tests.UnitTests;

public class BCryptPasswordHasherTests
{
    [Fact]
    public void HashAndVerifyRoundTrip()
    {
        var Hasher = new BCryptPasswordHasher();

        var Hash = Hasher.Hash("Secreta.123");

        Assert.NotEqual("Secreta.123", Hash);
        Assert.True(Hasher.Verify("Secreta.123", Hash));
        Assert.False(Hasher.Verify("otra", Hash));
        Assert.False(Hasher.NeedsRehash(Hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("no-es-un-hash")]
    public void CorruptHashIsTreatedAsInvalid(string Hash)
    {
        var Hasher = new BCryptPasswordHasher();

        Assert.False(Hasher.Verify("Secreta.123", Hash));
        Assert.True(Hasher.NeedsRehash(Hash));
    }
}
