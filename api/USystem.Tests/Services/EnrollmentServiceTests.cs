// Pruebas del servicio de inscripciones.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class EnrollmentServiceTests
{
    private static (EnrollmentService Service, Mock<IEnrollmentRepository> Enrollments, Mock<IClassGroupRepository> Groups) CreateService()
    {
        var Enrollments = new Mock<IEnrollmentRepository>();
        var Groups = new Mock<IClassGroupRepository>();
        var Terms = new Mock<IAcademicTermRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Enrollments
            .Setup(Repo => Repo.GetClassGroupsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EnrollmentClassGroup>());

        Terms
            .Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AcademicTerm { Id = 1, Code = "2026-1", Name = "Cuatrimestre 1" });

        return (
            new EnrollmentService(Enrollments.Object, Groups.Object, Terms.Object, UnitOfWork.Object, AuditService.Object),
            Enrollments,
            Groups);
    }

    private static CreateEnrollmentRequest BuildRequest()
        => new() { StudentId = 1, AcademicTermId = 1, CareerId = 1, ClassGroupIds = new List<int> { 1 } };

    [Fact]
    public async Task CreateAsync_AlumnoYaInscrito_DevuelveConflicto()
    {
        var (Service, Enrollments, _) = CreateService();

        Enrollments
            .Setup(Repo => Repo.ExistsForStudentTermAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task CreateAsync_GrupoCerrado_DevuelveConflicto()
    {
        var (Service, Enrollments, Groups) = CreateService();

        Enrollments
            .Setup(Repo => Repo.ExistsForStudentTermAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Closed", Capacity = 30 });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task CreateAsync_GrupoSinCupo_DevuelveConflicto()
    {
        var (Service, Enrollments, Groups) = CreateService();

        Enrollments
            .Setup(Repo => Repo.ExistsForStudentTermAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open", Capacity = 1 });
        Enrollments
            .Setup(Repo => Repo.CountInGroupAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task CreateAsync_GrupoConCupo_GuardaInscripcion()
    {
        var (Service, Enrollments, Groups) = CreateService();

        Enrollments
            .Setup(Repo => Repo.ExistsForStudentTermAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open", Capacity = 30 });
        Enrollments
            .Setup(Repo => Repo.CountInGroupAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var Dto = await Service.CreateAsync(BuildRequest());

        Assert.Equal("Enrollment", Dto.Type);
        Assert.Equal("Draft", Dto.Status);
        Enrollments.Verify(
            Repo => Repo.AddClassGroupAsync(It.IsAny<EnrollmentClassGroup>(), It.IsAny<CancellationToken>()),
            Times.Once);
        Groups.Verify(Repo => Repo.Update(It.Is<ClassGroup>(Group => Group.EnrolledCount == 1)), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_YaCancelada_DevuelveConflicto()
    {
        var (Service, Enrollments, _) = CreateService();

        Enrollments
            .Setup(Repo => Repo.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Enrollment { Id = 3, Status = "Cancelled" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CancelAsync(3, "motivo"));
    }
}
