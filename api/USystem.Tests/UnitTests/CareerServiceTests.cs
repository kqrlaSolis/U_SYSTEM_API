// Pruebas de las reglas del catálogo de carreras.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class CareerServiceTests
{
    private readonly Mock<ICareerRepository> _Careers = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();

    private CareerService BuildService()
    {
        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _Careers
            .Setup(Repository => Repository.AddAsync(It.IsAny<Career>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new CareerService(_Careers.Object, _UnitOfWork.Object, _Audit.Object);
    }

    private static Career BuildCareer() => new()
    {
        Id = 1,
        Code = "LAE",
        Name = "Licenciatura en Administración",
        Level = "Licenciatura",
        TermCount = 9,
        TotalCredits = 240,
        Modality = "Escolarizada",
        IsActive = true,
    };

    private static CreateCareerRequest BuildRequest() => new()
    {
        Code = "LAE",
        Name = "Licenciatura en Administración",
        Level = "Licenciatura",
        TermCount = 9,
        TotalCredits = 240,
        Modality = "Escolarizada",
    };

    [Fact]
    public async Task CreateRejectsADuplicatedCode()
    {
        var Service = BuildService();

        _Careers
            .Setup(Repository => Repository.ExistsByCodeAsync("LAE", null, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(BuildRequest()));

        Assert.Equal("Careers.CodeTaken", Error.Code);
    }

    [Fact]
    public async Task CreateStoresTheCareerAndAuditsIt()
    {
        var Service = BuildService();

        _Careers
            .Setup(Repository => Repository.ExistsByCodeAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Response = await Service.CreateAsync(BuildRequest());

        Assert.Equal("LAE", Response.Code);
        Assert.True(Response.IsActive);
        _Careers.Verify(
            Repository => Repository.AddAsync(It.IsAny<Career>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _Audit.Verify(
            Audit => Audit.LogAsync(
                "Create",
                "Careers",
                nameof(Career),
                "0",
                null,
                It.IsAny<object?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task CareerInUseCannotBeDeleted(bool HasPlans, bool HasStudents)
    {
        var Service = BuildService();

        _Careers
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildCareer());
        _Careers
            .Setup(Repository => Repository.HasStudyPlansAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HasPlans);
        _Careers
            .Setup(Repository => Repository.HasStudentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HasStudents);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(1));

        Assert.Equal("Careers.InUse", Error.Code);
    }

    [Fact]
    public async Task CareerWithoutHistoryIsSoftDeleted()
    {
        var Service = BuildService();
        var Career = BuildCareer();

        _Careers
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Career);

        await Service.DeleteAsync(1);

        _Careers.Verify(Repository => Repository.Remove(Career), Times.Once);
        _UnitOfWork.Verify(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UnknownCareerReturnsNotFound()
    {
        var Service = BuildService();

        _Careers
            .Setup(Repository => Repository.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Career?)null);

        var Error = await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByIdAsync(99));

        Assert.Equal("Careers.NotFound", Error.Code);
    }
}
