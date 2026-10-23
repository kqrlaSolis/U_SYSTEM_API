// Pruebas del servicio de docentes.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class TeacherServiceTests
{
    private static (TeacherService Service, Mock<ITeacherRepository> Repository) CreateService()
    {
        var Repository = new Mock<ITeacherRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return (new TeacherService(Repository.Object, UnitOfWork.Object, AuditService.Object), Repository);
    }

    private static CreateTeacherRequest BuildRequest()
        => new()
        {
            EmployeeNumber = "D-0001",
            FirstName = "Laura",
            PaternalLastName = "Mendoza",
            Email = "laura.mendoza@usystem.local",
            Specialty = "Matemáticas",
        };

    [Fact]
    public async Task CreateAsync_NuevoDocente_GuardaYDevuelveDto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByEmployeeNumberAsync(It.IsAny<string>(), null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Repository
            .Setup(Repo => Repo.ExistsByEmailAsync(It.IsAny<string>(), null, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Dto = await Service.CreateAsync(BuildRequest());

        Assert.Equal("D-0001", Dto.EmployeeNumber);
        Assert.Equal("Laura Mendoza", Dto.FullName);
        Assert.True(Dto.IsActive);
        Repository.Verify(
            Repo => Repo.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NumeroDeEmpleadoDuplicado_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByEmployeeNumberAsync(It.IsAny<string>(), null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task CreateAsync_CorreoDuplicado_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsByEmployeeNumberAsync(It.IsAny<string>(), null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Repository
            .Setup(Repo => Repo.ExistsByEmailAsync(It.IsAny<string>(), null, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateAsync(BuildRequest()));
    }

    [Fact]
    public async Task GetByIdAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByIdAsync(99));
    }

    [Fact]
    public async Task DeleteAsync_ConGrupos_LanzaReglaDeNegocio()
    {
        var (Service, Repository) = CreateService();
        var Existing = new Teacher { Id = 5, EmployeeNumber = "D-0005", Email = "docente@usystem.local" };

        Repository
            .Setup(Repo => Repo.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);
        Repository
            .Setup(Repo => Repo.HasClassGroupsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(5));
    }
}
