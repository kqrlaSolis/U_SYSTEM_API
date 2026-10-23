// Pruebas del servicio de calificaciones.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class GradeServiceTests
{
    private static (GradeService Service, Mock<IGradeRepository> Grades, Mock<IClassGroupRepository> Groups) CreateService()
    {
        var Grades = new Mock<IGradeRepository>();
        var Groups = new Mock<IClassGroupRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Grades
            .Setup(Repo => Repo.GetByClassGroupAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Grade>());

        return (
            new GradeService(Grades.Object, Groups.Object, UnitOfWork.Object, AuditService.Object),
            Grades,
            Groups);
    }

    private static SaveGradesRequest BuildRequest()
        => new()
        {
            Grades = new List<GradeItemRequest>
            {
                new() { EnrollmentId = 1, Partial1 = 8.5m, Status = "InProgress" },
            },
        };

    [Fact]
    public async Task SaveAsync_GrupoCerrado_DevuelveConflicto()
    {
        var (Service, _, Groups) = CreateService();

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Closed" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SaveAsync(1, BuildRequest()));
    }

    [Fact]
    public async Task SaveAsync_ActaCerrada_DevuelveConflicto()
    {
        var (Service, Grades, Groups) = CreateService();

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open" });
        Grades
            .Setup(Repo => Repo.GetByEnrollmentAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Grade { Id = 5, EnrollmentId = 1, ClassGroupId = 1, IsReportClosed = true });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SaveAsync(1, BuildRequest()));
    }

    [Fact]
    public async Task SaveAsync_NuevaCalificacion_CreaRegistro()
    {
        var (Service, Grades, Groups) = CreateService();

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open" });
        Grades
            .Setup(Repo => Repo.GetByEnrollmentAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Grade?)null);

        await Service.SaveAsync(1, BuildRequest());

        Grades.Verify(
            Repo => Repo.AddAsync(
                It.Is<Grade>(Grade => Grade.Partial1 == 8.5m && Grade.AttemptNumber == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_CalificacionExistente_ActualizaParcial()
    {
        var (Service, Grades, Groups) = CreateService();
        var Existing = new Grade { Id = 9, EnrollmentId = 1, ClassGroupId = 1, IsReportClosed = false };

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open" });
        Grades
            .Setup(Repo => Repo.GetByEnrollmentAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);

        await Service.SaveAsync(1, BuildRequest());

        Assert.Equal(8.5m, Existing.Partial1);
        Grades.Verify(Repo => Repo.Update(Existing), Times.Once);
    }

    [Fact]
    public async Task GetByClassGroupAsync_MapeaCalificaciones()
    {
        var (Service, Grades, _) = CreateService();

        Grades
            .Setup(Repo => Repo.GetByClassGroupAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Grade>
            {
                new() { Id = 1, EnrollmentId = 10, ClassGroupId = 3, Ordinary = 9m, Status = "Passed" },
            });

        var Items = await Service.GetByClassGroupAsync(3);

        Assert.Single(Items);
        Assert.Equal(9m, Items[0].Ordinary);
    }
}
