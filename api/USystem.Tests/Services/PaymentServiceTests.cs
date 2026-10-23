// Pruebas de pagos y dashboard.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class PaymentServiceTests
{
    private static (PaymentService Service, Mock<IPaymentRepository> Repository) CreateService()
    {
        var Repository = new Mock<IPaymentRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return (new PaymentService(Repository.Object, UnitOfWork.Object, AuditService.Object), Repository);
    }

    [Fact]
    public async Task CreateConceptAsync_ClaveDuplicada_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsConceptByCodeAsync("COL-MEN", null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateConceptAsync(new CreatePaymentConceptRequest
        {
            Code = "col-men",
            Name = "Colegiatura",
            BaseAmount = 1500,
        }));
    }

    [Fact]
    public async Task CreateChargeAsync_ConceptoInactivo_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetConceptByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentConcept { Id = 2, Code = "OLD", Name = "Antiguo", IsActive = false });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateChargeAsync(new ChargeRequest
        {
            StudentId = 1,
            AcademicTermId = 1,
            PaymentConceptId = 2,
            Amount = 100,
            DueDate = new DateOnly(2026, 3, 15),
        }));
    }

    [Fact]
    public async Task CreateChargeAsync_CreaCargoPendiente()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetConceptByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentConcept { Id = 5, Code = "COL-MEN", Name = "Colegiatura", IsActive = true });

        var Dto = await Service.CreateChargeAsync(new ChargeRequest
        {
            StudentId = 1,
            AcademicTermId = 1,
            PaymentConceptId = 5,
            Amount = 1500,
            Discount = 100,
            DueDate = new DateOnly(2026, 3, 15),
        });

        Assert.Equal("Pending", Dto.Status);
        Assert.Equal(1400m, Dto.Total);
        Repository.Verify(
            Repo => Repo.AddChargeAsync(It.IsAny<Charge>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelChargeAsync_CargoPagado_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetChargeByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Charge { Id = 9, Status = "Paid", Amount = 100 });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CancelChargeAsync(9));
    }

    [Fact]
    public async Task GetDashboardAsync_ComponeIndicadores()
    {
        var Repository = new Mock<IReportRepository>();
        Repository.Setup(Repo => Repo.CountStudentsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);
        Repository.Setup(Repo => Repo.CountTeachersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        Repository.Setup(Repo => Repo.CountOpenClassGroupsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(4);
        Repository.Setup(Repo => Repo.GetPendingChargesAsync(It.IsAny<CancellationToken>())).ReturnsAsync((2, 2500m));
        Repository.Setup(Repo => Repo.GetProceduresAsync(It.IsAny<CancellationToken>())).ReturnsAsync((1, 1));
        Repository.Setup(Repo => Repo.CountEnrollmentsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        Repository.Setup(Repo => Repo.CountEnrollmentsInTermAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(4);

        var TermRepository = new Mock<IAcademicTermRepository>();
        TermRepository
            .Setup(Repo => Repo.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AcademicTerm
            {
                Id = 1,
                Code = "2026-1",
                Name = "Cuatrimestre 1",
                Status = "InProgress",
                StartDate = new DateOnly(2026, 1, 12),
                EndDate = new DateOnly(2026, 4, 30),
            });

        var Service = new ReportService(Repository.Object, TermRepository.Object);
        var Dashboard = await Service.GetDashboardAsync();

        Assert.Equal(3, Dashboard.ActiveStudents);
        Assert.Equal(2500m, Dashboard.PendingAmount);
        Assert.Equal(1, Dashboard.RequestedProcedures);
        Assert.Equal(5, Dashboard.TotalEnrollments);
        Assert.Equal("2026-1", Dashboard.CurrentTermCode);
        Assert.Equal(4, Dashboard.TermEnrollments);
    }
}
