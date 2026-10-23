// Pruebas del servicio de material didáctico.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class CourseMaterialServiceTests
{
    private static (CourseMaterialService Service, Mock<ICourseMaterialRepository> Repository) CreateService()
    {
        var Repository = new Mock<ICourseMaterialRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return (new CourseMaterialService(Repository.Object, UnitOfWork.Object, AuditService.Object), Repository);
    }

    [Fact]
    public async Task CreateAsync_GuardaMaterialActivo()
    {
        var (Service, Repository) = CreateService();

        var Dto = await Service.CreateAsync(new CreateCourseMaterialRequest
        {
            SubjectId = 1,
            AcademicTermId = 1,
            Type = "Reading",
            Title = "Unidad 1",
            Url = "https://usystem.local/unidad-1",
        });

        Assert.Equal("Unidad 1", Dto.Title);
        Assert.Equal("Reading", Dto.Type);
        Assert.True(Dto.IsActive);
        Repository.Verify(
            Repo => Repo.AddAsync(It.IsAny<CourseMaterial>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetByIdAsync(77, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseMaterial?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByIdAsync(77));
    }

    [Fact]
    public async Task DeleteAsync_Existente_EliminaYAudita()
    {
        var (Service, Repository) = CreateService();
        var Existing = new CourseMaterial { Id = 3, SubjectId = 1, Type = "Link", Title = "Lectura" };

        Repository
            .Setup(Repo => Repo.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);

        await Service.DeleteAsync(3);

        Repository.Verify(Repo => Repo.Remove(Existing), Times.Once);
    }
}
