// Pruebas del servicio de grupos.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class ClassGroupServiceTests
{
    private static (ClassGroupService Service, Mock<IClassGroupRepository> Repository) CreateService()
    {
        var Repository = new Mock<IClassGroupRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Repository
            .Setup(Repo => Repo.GetSchedulesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ClassGroupSchedule>());

        return (new ClassGroupService(Repository.Object, UnitOfWork.Object, AuditService.Object), Repository);
    }

    private static CreateClassGroupRequest BuildRequest()
        => new()
        {
            AcademicTermId = 1,
            SubjectId = 1,
            TeacherId = 1,
            Code = "G-01",
            Capacity = 30,
            Status = "Open",
            Schedules = new List<ClassGroupScheduleRequest>
            {
                new() { DayOfWeek = 1, StartTime = "08:00", EndTime = "10:00" },
            },
        };

    [Fact]
    public async Task GetByIdAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassGroup?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByIdAsync(42));
    }

    [Fact]
    public async Task CreateAsync_ClaveDuplicada_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByCodeAsync(1, "G-01", null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task CreateAsync_GuardaGrupoConHorarios()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByCodeAsync(1, "G-01", null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Dto = await Service.CreateAsync(BuildRequest());

        Assert.Equal("G-01", Dto.Code);
        Assert.Equal(30, Dto.Capacity);
        Assert.Equal(0, Dto.EnrolledCount);
        Repository.Verify(
            Repo => Repo.AddScheduleAsync(It.IsAny<ClassGroupSchedule>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ConAlumnosInscritos_LanzaReglaDeNegocio()
    {
        var (Service, Repository) = CreateService();
        var Existing = new ClassGroup { Id = 7, AcademicTermId = 1, SubjectId = 1, TeacherId = 1, Code = "G-07" };

        Repository
            .Setup(Repo => Repo.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);
        Repository
            .Setup(Repo => Repo.HasEnrollmentsAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(7));
    }
}
