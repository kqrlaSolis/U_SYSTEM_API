// Pruebas del servicio de alumnos.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class StudentServiceTests
{
    private static (StudentService Service, Mock<IStudentRepository> Repository) CreateService()
    {
        var Repository = new Mock<IStudentRepository>();
        var CareerRepository = new Mock<ICareerRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        CareerRepository
            .Setup(Repo => Repo.GetByIdsAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Career> { new() { Id = 1, Code = "LAE", Name = "Administración" } });

        return (
            new StudentService(Repository.Object, CareerRepository.Object, UnitOfWork.Object, AuditService.Object),
            Repository);
    }

    private static CreateStudentRequest BuildRequest()
        => new()
        {
            CareerId = 1,
            StudyPlanId = 4,
            EnrollmentTermId = 1,
            FirstName = "Ana",
            PaternalLastName = "García",
            Curp = "GARA060101MDFRRN01",
            BirthDate = new DateOnly(2006, 1, 1),
            Gender = "F",
        };

    [Fact]
    public async Task CreateAsync_GeneraMatriculaYEstadoActivo()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByCurpAsync(It.IsAny<string>(), null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Repository
            .Setup(Repo => Repo.GetLastStudentNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var Dto = await Service.CreateAsync(BuildRequest());

        Assert.Equal($"A{DateTime.UtcNow.Year}0001", Dto.StudentNumber);
        Assert.Equal("Active", Dto.Status);
        Assert.Equal("Ana García", Dto.FullName);
        Assert.Equal("Administración", Dto.CareerName);
        Repository.Verify(
            Repo => Repo.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ContinuaLaSecuenciaDeMatricula()
    {
        var (Service, Repository) = CreateService();
        var Year = DateTime.UtcNow.Year;

        Repository
            .Setup(Repo => Repo.ExistsByCurpAsync(It.IsAny<string>(), null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Repository
            .Setup(Repo => Repo.GetLastStudentNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync($"A{Year}0007");

        var Dto = await Service.CreateAsync(BuildRequest());

        Assert.Equal($"A{Year}0008", Dto.StudentNumber);
    }

    [Fact]
    public async Task CreateAsync_CurpDuplicada_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByCurpAsync(It.IsAny<string>(), null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task DeleteAsync_ConInscripciones_LanzaReglaDeNegocio()
    {
        var (Service, Repository) = CreateService();
        var Existing = new Student { Id = 9, StudentNumber = "A20260009", Curp = "GARA060101MDFRRN01" };

        Repository
            .Setup(Repo => Repo.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);
        Repository
            .Setup(Repo => Repo.HasEnrollmentsAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(9));
    }

    [Fact]
    public async Task GetByIdAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByIdAsync(404));
    }
}
