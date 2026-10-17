// Pruebas de la edición de parámetros: clave existente, tipo/rango y caché invalidada.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class SettingsServiceTests
{
    private readonly Mock<ISystemSettingRepository> _Settings = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<ISettingsProvider> _Provider = new();
    private readonly Mock<IAuditService> _Audit = new();

    private SettingsService BuildService()
    {
        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return new SettingsService(_Settings.Object, _UnitOfWork.Object, _Provider.Object, _Audit.Object);
    }

    private static SystemSetting BuildSetting(string SettingKey, string SettingValue, string ValueType) => new()
    {
        Id = 1,
        SettingKey = SettingKey,
        SettingValue = SettingValue,
        ValueType = ValueType,
        Category = "Security",
        Description = "Prueba",
    };

    [Fact]
    public async Task UnknownKeyIsRejected()
    {
        var Service = BuildService();

        _Settings
            .Setup(Repository => Repository.GetByKeyAsync("Nope.Key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SystemSetting?)null);

        var Error = await Assert.ThrowsAsync<NotFoundException>(
            () => Service.UpdateAsync("Nope.Key", new UpdateSettingRequest { SettingValue = "1" }));

        Assert.Equal("Settings.UnknownKey", Error.Code);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("99")]
    public async Task InvalidIntValueIsRejected(string Value)
    {
        var Service = BuildService();

        _Settings
            .Setup(Repository => Repository.GetByKeyAsync(
                "Login.MaxFailedAttempts",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildSetting("Login.MaxFailedAttempts", "5", "Int"));

        var Error = await Assert.ThrowsAsync<ValidationException>(() => Service.UpdateAsync(
            "Login.MaxFailedAttempts",
            new UpdateSettingRequest { SettingValue = Value }));

        Assert.Equal("Settings.InvalidValue", Error.Code);
    }

    [Fact]
    public async Task ValidValueIsSavedInvalidatesCacheAndIsAudited()
    {
        var Service = BuildService();
        var Setting = BuildSetting("Login.MaxFailedAttempts", "5", "Int");

        _Settings
            .Setup(Repository => Repository.GetByKeyAsync(
                "Login.MaxFailedAttempts",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Setting);

        var Response = await Service.UpdateAsync(
            "Login.MaxFailedAttempts",
            new UpdateSettingRequest { SettingValue = "3" });

        Assert.Equal("3", Response.SettingValue);
        Assert.Equal("3", Setting.SettingValue);
        _Provider.Verify(Provider => Provider.Invalidate(), Times.Once);
        _Audit.Verify(
            Audit => Audit.LogAsync(
                "Update",
                "Settings",
                nameof(SystemSetting),
                "Login.MaxFailedAttempts",
                It.IsAny<object?>(),
                It.IsAny<object?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StringValuesAreAccepted()
    {
        var Service = BuildService();
        var Setting = BuildSetting("Students.NumberFormat", "YYYY-CCC-NNNN", "String");

        _Settings
            .Setup(Repository => Repository.GetByKeyAsync(
                "Students.NumberFormat",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Setting);

        var Response = await Service.UpdateAsync(
            "Students.NumberFormat",
            new UpdateSettingRequest { SettingValue = "CCC-YYYY-NNNN" });

        Assert.Equal("CCC-YYYY-NNNN", Response.SettingValue);
    }
}
