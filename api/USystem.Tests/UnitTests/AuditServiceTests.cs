// Pruebas del servicio de auditoría.
using Moq;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class AuditServiceTests
{
    [Fact]
    public async Task LogAsyncStoresUserActionAndSerializedValues()
    {
        AuditLog? Captured = null;

        var Repository = new Mock<IAuditLogRepository>();
        Repository
            .Setup(AuditLogRepository => AuditLogRepository.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Callback<AuditLog, CancellationToken>((Entity, Cancellation) => Captured = Entity)
            .Returns(Task.CompletedTask);

        var UnitOfWork = new Mock<IUnitOfWork>();
        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var CurrentUser = new Mock<ICurrentUser>();
        CurrentUser.SetupGet(User => User.UserId).Returns(5);
        CurrentUser.SetupGet(User => User.IpAddress).Returns("127.0.0.1");

        var Service = new AuditService(Repository.Object, UnitOfWork.Object, CurrentUser.Object);

        await Service.LogAsync(
            "Create",
            "Users",
            nameof(User),
            "9",
            null,
            new { FirstName = "Ana" });

        Assert.NotNull(Captured);
        Assert.Equal("Create", Captured!.Action);
        Assert.Equal("Users", Captured.Module);
        Assert.Equal("9", Captured.EntityId);
        Assert.Equal(5, Captured.UserId);
        Assert.Null(Captured.OldValue);
        Assert.Contains("\"FirstName\":\"Ana\"", Captured.NewValue);
        Assert.NotEqual(default, Captured.CreatedAt);
        UnitOfWork.Verify(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
