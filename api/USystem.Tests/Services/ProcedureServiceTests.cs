// Pruebas del servicio de trámites.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class ProcedureServiceTests
{
    private static (ProcedureService Service, Mock<IProcedureRepository> Repository) CreateService()
    {
        var Repository = new Mock<IProcedureRepository>();
        var CurrentUser = new Mock<ICurrentUser>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        CurrentUser.SetupGet(User => User.UserId).Returns(1);

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Repository
            .Setup(Repo => Repo.GetLastFolioAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return (new ProcedureService(Repository.Object, CurrentUser.Object, UnitOfWork.Object, AuditService.Object), Repository);
    }

    [Fact]
    public async Task CreateTypeAsync_ClaveDuplicada_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsTypeByCodeAsync("CONST", null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateTypeAsync(new CreateProcedureTypeRequest
        {
            Code = "const",
            Name = "Constancia de estudios",
        }));
    }

    [Fact]
    public async Task CreateProcedureAsync_GeneraFolioYEstadoSolicitado()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetTypeByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcedureType { Id = 1, Code = "CONST", Name = "Constancia", IsActive = true });

        var Dto = await Service.CreateProcedureAsync(new CreateProcedureRequest
        {
            ProcedureTypeId = 1,
            StudentId = 1,
        });

        Assert.Equal($"TRM{DateTime.UtcNow.Year}0001", Dto.FolioNumber);
        Assert.Equal("Requested", Dto.Status);
        Assert.Equal("CONST", Dto.ProcedureTypeCode);
        Repository.Verify(
            Repo => Repo.AddAsync(It.IsAny<Procedure>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateProcedureAsync_TipoInactivo_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetTypeByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcedureType { Id = 2, Code = "OLD", Name = "Antiguo", IsActive = false });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateProcedureAsync(new CreateProcedureRequest
        {
            ProcedureTypeId = 2,
            StudentId = 1,
        }));
    }

    [Fact]
    public async Task IssueAsync_YaEmitido_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Procedure
            {
                Id = 5,
                ProcedureTypeId = 1,
                StudentId = 1,
                FolioNumber = "TRM20260001",
                Status = "Issued",
            });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.IssueAsync(5, new IssueProcedureRequest()));
    }

    [Fact]
    public async Task IssueAsync_Solicitado_MarcaEmitido()
    {
        var (Service, Repository) = CreateService();
        var Existing = new Procedure
        {
            Id = 6,
            ProcedureTypeId = 1,
            StudentId = 1,
            FolioNumber = "TRM20260002",
            Status = "Requested",
        };

        Repository
            .Setup(Repo => Repo.GetByIdAsync(6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);
        Repository
            .Setup(Repo => Repo.GetTypeByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcedureType { Id = 1, Code = "CONST", Name = "Constancia", IsActive = true });

        var Dto = await Service.IssueAsync(6, new IssueProcedureRequest { FilePath = "/tmp/constancia.pdf" });

        Assert.Equal("Issued", Dto.Status);
        Assert.NotNull(Dto.IssuedAt);
        Assert.Equal("/tmp/constancia.pdf", Dto.FilePath);
    }
}
