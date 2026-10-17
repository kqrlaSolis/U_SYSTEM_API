// Pruebas de la consulta de la bitácora.
using Moq;
using USystem.Application.Services;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class AuditLogServiceTests
{
    private readonly Mock<IAuditLogRepository> _AuditLogs = new();
    private readonly Mock<IUserRepository> _Users = new();

    private AuditLogService BuildService() => new(_AuditLogs.Object, _Users.Object);

    private static AuditLog BuildEntry(long Id, long? UserId) => new()
    {
        Id = Id,
        UserId = UserId,
        Action = "Login",
        Module = "Auth",
        Entity = "User",
        EntityId = UserId?.ToString(),
        IpAddress = "127.0.0.1",
        CreatedAt = DateTime.UtcNow,
    };

    [Fact]
    public async Task PagedQueryResolvesTheUsername()
    {
        var Service = BuildService();
        var Query = new AuditLogQuery { Page = 1, PageSize = 20 };

        _AuditLogs
            .Setup(Repository => Repository.GetPagedAsync(Query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PagedResult<AuditLog>.Create(
                new List<AuditLog> { BuildEntry(1, 7) },
                1,
                Query));

        _Users
            .Setup(Repository => Repository.GetUsernamesAsync(
                It.IsAny<IReadOnlyCollection<long>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<long, string> { [7] = "aperez" });

        var Page = await Service.GetPagedAsync(Query);

        var Entry = Assert.Single(Page.Items);
        Assert.Equal("aperez", Entry.Username);
        Assert.Equal("Login", Entry.Action);
        Assert.Equal(1, Page.TotalCount);
    }

    [Fact]
    public async Task UnknownEntryReturnsNotFound()
    {
        var Service = BuildService();

        _AuditLogs
            .Setup(Repository => Repository.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuditLog?)null);

        var Error = await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByIdAsync(99));

        Assert.Equal("Api.NotFound", Error.Code);
    }
}
