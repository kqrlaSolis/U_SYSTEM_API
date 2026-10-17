// Pruebas de las reglas del módulo Users: unicidad, roles y protección del último administrador.
using Moq;
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _Users = new();
    private readonly Mock<IRoleRepository> _Roles = new();
    private readonly Mock<IRefreshTokenRepository> _RefreshTokens = new();
    private readonly Mock<IPasswordHasher> _Hasher = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();
    private readonly Mock<ISettingsProvider> _Settings = new();

    private UserService BuildService()
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

        _Hasher
            .Setup(Hasher => Hasher.Hash(It.IsAny<string>()))
            .Returns("hashed-password");

        _Users
            .Setup(Repository => Repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _Users
            .Setup(Repository => Repository.ReplaceRolesAsync(
                It.IsAny<long>(),
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new UserService(
            _Users.Object,
            _Roles.Object,
            _RefreshTokens.Object,
            _Hasher.Object,
            _UnitOfWork.Object,
            _Audit.Object,
            _Settings.Object);
    }

    private static User BuildAdministrator() => new()
    {
        Id = 1,
        FirstName = "System",
        PaternalLastName = "Administrator",
        Email = "admin@usystem.local",
        Username = "Administrator",
        PasswordHash = "hash",
        IsActive = true,
    };

    private static CreateUserRequest BuildCreateRequest() => new()
    {
        FirstName = "Ana",
        PaternalLastName = "Pérez",
        Email = "ana@escuela.mx",
        Username = "aperez",
        RoleIds = new[] { 6 },
    };

    private void SetupRoleCatalog()
        => _Roles
            .Setup(Repository => Repository.GetByIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Role { Id = 6, Name = "ReadOnly", IsActive = true } });

    [Fact]
    public async Task CreateRejectsADuplicatedEmail()
    {
        var Service = BuildService();

        _Users
            .Setup(Repository => Repository.ExistsByEmailAsync(
                "ana@escuela.mx",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(BuildCreateRequest()));

        Assert.Equal("Users.DuplicateEmail", Error.Code);
    }

    [Fact]
    public async Task CreateRejectsUnknownRoles()
    {
        var Service = BuildService();

        _Users
            .Setup(Repository => Repository.ExistsByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<long?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _Users
            .Setup(Repository => Repository.ExistsByUsernameAsync(
                It.IsAny<string>(),
                It.IsAny<long?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _Roles
            .Setup(Repository => Repository.GetByIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Role>());

        var Error = await Assert.ThrowsAsync<ValidationException>(
            () => Service.CreateAsync(BuildCreateRequest()));

        Assert.Equal("Users.InvalidRole", Error.Code);
    }

    [Fact]
    public async Task CreateGeneratesACompliantTemporaryPassword()
    {
        var Service = BuildService();
        SetupRoleCatalog();

        _Users
            .Setup(Repository => Repository.ExistsByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<long?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _Users
            .Setup(Repository => Repository.ExistsByUsernameAsync(
                It.IsAny<string>(),
                It.IsAny<long?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _Users
            .Setup(Repository => Repository.GetRoleNamesAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "ReadOnly" });

        var Response = await Service.CreateAsync(BuildCreateRequest());

        Assert.Equal("aperez", Response.User.Username);
        Assert.True(Response.User.MustChangePassword);
        Assert.False(string.IsNullOrWhiteSpace(Response.TemporaryPassword));
        Assert.Null(PasswordPolicy.Validate(Response.TemporaryPassword));
    }

    [Fact]
    public async Task DeactivatingTheLastAdministratorIsBlocked()
    {
        var Service = BuildService();

        _Users
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAdministrator());
        _Users
            .Setup(Repository => Repository.GetRoleNamesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Administrator" });
        _Users
            .Setup(Repository => Repository.CountActiveAdministratorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.SetStatusAsync(1, false));

        Assert.Equal("Auth.LastAdministrator", Error.Code);
    }

    [Fact]
    public async Task CannotRemoveTheAdministratorRoleFromTheLastAdministrator()
    {
        var Service = BuildService();
        SetupRoleCatalog();

        _Users
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAdministrator());
        _Users
            .Setup(Repository => Repository.GetRoleNamesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Administrator" });
        _Users
            .Setup(Repository => Repository.CountActiveAdministratorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.AssignRolesAsync(1, new[] { 6 }));

        Assert.Equal("Auth.LastAdministrator", Error.Code);
    }

    [Fact]
    public async Task ResetPasswordRevokesSessionsAndMarksTheChangeAsMandatory()
    {
        var Service = BuildService();
        var User = BuildAdministrator();

        _Users
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(User);

        var Response = await Service.ResetPasswordAsync(1, new ResetUserPasswordRequest());

        Assert.False(string.IsNullOrWhiteSpace(Response.TemporaryPassword));
        Assert.True(User.MustChangePassword);
        Assert.Equal("hashed-password", User.PasswordHash);
        _RefreshTokens.Verify(
            Repository => Repository.RevokeAllForUserAsync(1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
