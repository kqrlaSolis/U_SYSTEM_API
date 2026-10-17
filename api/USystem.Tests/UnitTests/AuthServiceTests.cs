// Pruebas de las reglas de autenticación: credenciales, bloqueo, refresh y cambio de contraseña.
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _Users = new();
    private readonly Mock<IRefreshTokenRepository> _RefreshTokens = new();
    private readonly Mock<IPasswordResetTokenRepository> _ResetTokens = new();
    private readonly Mock<IPasswordHasher> _Hasher = new();
    private readonly Mock<ITokenGenerator> _Tokens = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();
    private readonly Mock<ISettingsProvider> _Settings = new();

    private AuthService BuildService()
    {
        _Settings
            .Setup(Settings => Settings.GetIntAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string Key, int DefaultValue, CancellationToken Cancellation) => DefaultValue);

        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _Tokens
            .Setup(Generator => Generator.GenerateAccessToken(
                It.IsAny<User>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>()))
            .Returns(new AccessToken("access-token", DateTime.UtcNow.AddMinutes(15)));

        _Users
            .Setup(Repository => Repository.GetRoleNamesAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Administrator" });

        _Users
            .Setup(Repository => Repository.GetPermissionCodesAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Users.View" });

        return new AuthService(
            _Users.Object,
            _RefreshTokens.Object,
            _ResetTokens.Object,
            _Hasher.Object,
            _Tokens.Object,
            _UnitOfWork.Object,
            _Audit.Object,
            _Settings.Object,
            NullLogger<AuthService>.Instance);
    }

    private static User BuildUser(
        bool IsActive = true,
        DateTime? LockedUntil = null,
        int FailedAttempts = 0) => new()
        {
            Id = 1,
            FirstName = "Ana",
            PaternalLastName = "Pérez",
            Email = "ana@escuela.mx",
            Username = "ana",
            PasswordHash = "hash",
            IsActive = IsActive,
            FailedAttempts = FailedAttempts,
            LockedUntil = LockedUntil,
            MustChangePassword = true,
        };

    private static LoginRequest BuildRequest() => new()
    {
        UsernameOrEmail = "ana",
        Password = "Admin.2026",
    };

    [Fact]
    public async Task LoginWithValidCredentialsReturnsTokensAndProfile()
    {
        var User = BuildUser();
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByLoginAsync("ana", It.IsAny<CancellationToken>()))
            .ReturnsAsync(User);
        _Hasher.Setup(Hasher => Hasher.Verify("Admin.2026", "hash")).Returns(true);

        var Response = await Service.LoginAsync(BuildRequest());

        Assert.Equal("access-token", Response.AccessToken);
        Assert.False(string.IsNullOrWhiteSpace(Response.RefreshToken));
        Assert.True(Response.MustChangePassword);
        Assert.Equal("ana", Response.User.Username);
        Assert.Equal(new[] { "Administrator" }, Response.User.Roles);

        _RefreshTokens.Verify(
            Repository => Repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _Audit.Verify(
            Service => Service.LogAsync(
                "Login",
                "Auth",
                nameof(User),
                "1",
                null,
                It.IsAny<object?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UnknownUserGetsTheSameErrorAsAWrongPassword()
    {
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByLoginAsync("ana", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.LoginAsync(BuildRequest()));

        Assert.Equal("Auth.InvalidCredentials", Error.Code);
        _RefreshTokens.Verify(
            Repository => Repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task WrongPasswordIncrementsFailedAttempts()
    {
        var User = BuildUser();
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByLoginAsync("ana", It.IsAny<CancellationToken>()))
            .ReturnsAsync(User);
        _Hasher.Setup(Hasher => Hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.LoginAsync(BuildRequest()));

        Assert.Equal("Auth.InvalidCredentials", Error.Code);
        Assert.Equal(1, User.FailedAttempts);
        Assert.Null(User.LockedUntil);
    }

    [Fact]
    public async Task AccountIsLockedAfterMaxFailedAttempts()
    {
        var User = BuildUser(FailedAttempts: 1);
        var Service = BuildService();

        _Settings
            .Setup(Settings => Settings.GetIntAsync("Login.MaxFailedAttempts", 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _Users.Setup(Repository => Repository.GetByLoginAsync("ana", It.IsAny<CancellationToken>()))
            .ReturnsAsync(User);
        _Hasher.Setup(Hasher => Hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.LoginAsync(BuildRequest()));

        Assert.NotNull(User.LockedUntil);
        Assert.True(User.LockedUntil > DateTime.UtcNow);
        Assert.Equal(0, User.FailedAttempts);
    }

    [Fact]
    public async Task InactiveUserCannotLogin()
    {
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByLoginAsync("ana", It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildUser(IsActive: false));

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.LoginAsync(BuildRequest()));

        Assert.Equal("Auth.AccountInactive", Error.Code);
    }

    [Fact]
    public async Task LockedUserGetsTheErrorMessageWithRemainingMinutes()
    {
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByLoginAsync("ana", It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildUser(LockedUntil: DateTime.UtcNow.AddMinutes(8)));

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.LoginAsync(BuildRequest()));

        Assert.Equal("Auth.AccountLocked", Error.Code);
        Assert.True(Error.Values.ContainsKey("Minutes"));
    }

    [Fact]
    public async Task RefreshRotationRevokesTheUsedTokenAndIssuesANewOne()
    {
        var Stored = new RefreshToken
        {
            Id = 10,
            UserId = 1,
            TokenHash = "hash-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
        };

        var Service = BuildService();

        _RefreshTokens
            .Setup(Repository => Repository.GetByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Stored);
        _Users.Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildUser());

        var Response = await Service.RefreshAsync(new RefreshTokenRequest { RefreshToken = "token" });

        Assert.NotNull(Stored.RevokedAt);
        Assert.False(string.IsNullOrWhiteSpace(Response.RefreshToken));
        _RefreshTokens.Verify(
            Repository => Repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RefreshWithAnUnknownTokenFails()
    {
        var Service = BuildService();

        _RefreshTokens
            .Setup(Repository => Repository.GetByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.RefreshAsync(new RefreshTokenRequest { RefreshToken = "token" }));

        Assert.Equal("Auth.RefreshTokenInvalid", Error.Code);
    }

    [Fact]
    public async Task ChangePasswordRejectsAWrongCurrentPassword()
    {
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildUser());
        _Hasher.Setup(Hasher => Hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.ChangePasswordAsync(
            new ChangePasswordRequest { CurrentPassword = "mala", NewPassword = "Nueva.2026" },
            1));

        Assert.Equal("Auth.CurrentPasswordInvalid", Error.Code);
    }

    [Fact]
    public async Task ChangePasswordUpdatesTheHashAndClosesSessions()
    {
        var User = BuildUser();
        var Service = BuildService();

        _Users.Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(User);
        _Hasher.Setup(Hasher => Hasher.Verify("Admin.2026", "hash")).Returns(true);
        _Hasher.Setup(Hasher => Hasher.Hash("Nueva.2026")).Returns("new-hash");

        await Service.ChangePasswordAsync(
            new ChangePasswordRequest { CurrentPassword = "Admin.2026", NewPassword = "Nueva.2026" },
            1);

        Assert.Equal("new-hash", User.PasswordHash);
        Assert.False(User.MustChangePassword);
        _RefreshTokens.Verify(
            Repository => Repository.RevokeAllForUserAsync(1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
