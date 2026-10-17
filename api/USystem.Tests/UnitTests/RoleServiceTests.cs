// Pruebas de las reglas de roles: catálogo fijo, roles protegidos y Administrator completo.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _Roles = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();

    private RoleService BuildService()
    {
        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _Roles
            .Setup(Repository => Repository.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _Roles
            .Setup(Repository => Repository.ReplacePermissionsAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new RoleService(_Roles.Object, _UnitOfWork.Object, _Audit.Object);
    }

    private void SetupCatalog()
        => _Roles
            .Setup(Repository => Repository.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Permission { Id = 1, Code = "Users.View", Module = "Users", Description = "View users" },
                new Permission { Id = 2, Code = "Users.Create", Module = "Users", Description = "Create users" },
            });

    [Fact]
    public async Task CreateRejectsADuplicatedName()
    {
        var Service = BuildService();

        _Roles
            .Setup(Repository => Repository.GetByNameAsync(
                "Supervisor",
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 3, Name = "Supervisor", IsActive = true });

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(new CreateRoleRequest { Name = "Supervisor" }));

        Assert.Equal("Roles.NameTaken", Error.Code);
    }

    [Fact]
    public async Task CreateReactivatesASoftDeletedRole()
    {
        var Service = BuildService();
        SetupCatalog();

        var DeletedRole = new Role
        {
            Id = 9,
            Name = "Supervisor",
            IsActive = false,
            DeletedAt = DateTime.UtcNow.AddDays(-1),
        };

        _Roles
            .Setup(Repository => Repository.GetByNameAsync(
                "Supervisor",
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeletedRole);
        _Roles
            .Setup(Repository => Repository.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeletedRole);
        _Roles
            .Setup(Repository => Repository.GetPermissionCodesAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        _Roles
            .Setup(Repository => Repository.CountUsersAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var Response = await Service.CreateAsync(new CreateRoleRequest
        {
            Name = "Supervisor",
            PermissionCodes = new[] { "Users.View" },
        });

        Assert.Equal(9, Response.Id);
        Assert.Null(DeletedRole.DeletedAt);
        Assert.True(DeletedRole.IsActive);
    }

    [Fact]
    public async Task CreateRejectsUnknownPermissionCodes()
    {
        var Service = BuildService();
        SetupCatalog();

        _Roles
            .Setup(Repository => Repository.ExistsByNameAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Error = await Assert.ThrowsAsync<ValidationException>(() => Service.CreateAsync(
            new CreateRoleRequest { Name = "Supervisor", PermissionCodes = new[] { "Users.Nope" } }));

        Assert.Equal("Validation.InvalidFormat", Error.Code);
    }

    [Fact]
    public async Task CreateStoresTheCatalogIdsOfTheRequestedPermissions()
    {
        var Service = BuildService();
        SetupCatalog();

        _Roles
            .Setup(Repository => Repository.GetByNameAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);
        _Roles
            .Setup(Repository => Repository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 5, Name = "Supervisor", IsActive = true });
        _Roles
            .Setup(Repository => Repository.GetPermissionCodesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Users.View" });
        _Roles
            .Setup(Repository => Repository.CountUsersAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var Response = await Service.CreateAsync(new CreateRoleRequest
        {
            Name = "Supervisor",
            PermissionCodes = new[] { "Users.View" },
        });

        Assert.Equal("Supervisor", Response.Name);
        _Roles.Verify(
            Repository => Repository.ReplacePermissionsAsync(
                0,
                It.Is<IReadOnlyCollection<int>>(Ids => Ids.Single() == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SystemRolesCannotBeDeleted()
    {
        var Service = BuildService();

        _Roles
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 1, Name = "Administrator", IsSystem = true, IsActive = true });

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(1));

        Assert.Equal("Roles.SystemProtected", Error.Code);
    }

    [Fact]
    public async Task RolesWithUsersCannotBeDeleted()
    {
        var Service = BuildService();

        _Roles
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 5, Name = "Supervisor", IsSystem = false, IsActive = true });
        _Roles
            .Setup(Repository => Repository.CountUsersAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(5));

        Assert.Equal("Roles.HasUsers", Error.Code);
    }

    [Fact]
    public async Task AdministratorMustKeepTheWholeCatalog()
    {
        var Service = BuildService();
        SetupCatalog();

        _Roles
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 1, Name = "Administrator", IsSystem = true, IsActive = true });
        _Roles
            .Setup(Repository => Repository.GetByNameAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 1, Name = "Administrator", IsSystem = true, IsActive = true });

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.UpdateAsync(
            1,
            new UpdateRoleRequest
            {
                Name = "Administrator",
                IsActive = true,
                PermissionCodes = new[] { "Users.View" },
            }));

        Assert.Equal("Roles.AdminPermissionRequired", Error.Code);
    }

    [Fact]
    public async Task PermissionCatalogIsGroupedByModule()
    {
        var Service = BuildService();
        SetupCatalog();

        var Catalog = await Service.GetPermissionCatalogAsync();

        var Module = Assert.Single(Catalog);
        Assert.Equal("Users", Module.Module);
        Assert.Equal(2, Module.Permissions.Count);
    }
}
