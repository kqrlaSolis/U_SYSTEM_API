// Pruebas de las reglas del catálogo de materias.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class SubjectServiceTests
{
    private readonly Mock<ISubjectRepository> _Subjects = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();

    private SubjectService BuildService()
    {
        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _Subjects
            .Setup(Repository => Repository.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new SubjectService(_Subjects.Object, _UnitOfWork.Object, _Audit.Object);
    }

    private static Subject BuildSubject() => new()
    {
        Id = 1,
        Code = "MAT-101",
        Name = "Matemáticas I",
        Credits = 8,
        TheoryHours = 3,
        PracticeHours = 2,
        IsActive = true,
    };

    private static CreateSubjectRequest BuildRequest() => new()
    {
        Code = "MAT-101",
        Name = "Matemáticas I",
        Credits = 8,
        TheoryHours = 3,
        PracticeHours = 2,
    };

    [Fact]
    public async Task CreateRejectsADuplicatedCode()
    {
        var Service = BuildService();

        _Subjects
            .Setup(Repository => Repository.ExistsByCodeAsync(
                "MAT-101",
                null,
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(BuildRequest()));

        Assert.Equal("Subjects.CodeTaken", Error.Code);
    }

    [Fact]
    public async Task CreateStoresTheSubject()
    {
        var Service = BuildService();

        _Subjects
            .Setup(Repository => Repository.ExistsByCodeAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Response = await Service.CreateAsync(BuildRequest());

        Assert.Equal("MAT-101", Response.Code);
        Assert.Equal(8, Response.Credits);
        _Subjects.Verify(
            Repository => Repository.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task SubjectInUseCannotBeDeleted(bool UsedInPlans, bool UsedInGroups)
    {
        var Service = BuildService();

        _Subjects
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildSubject());
        _Subjects
            .Setup(Repository => Repository.IsUsedInStudyPlansAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UsedInPlans);
        _Subjects
            .Setup(Repository => Repository.IsUsedInClassGroupsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UsedInGroups);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(1));

        Assert.Equal("Subjects.InUse", Error.Code);
    }

    [Fact]
    public async Task UnusedSubjectIsSoftDeleted()
    {
        var Service = BuildService();
        var Subject = BuildSubject();

        _Subjects
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Subject);

        await Service.DeleteAsync(1);

        _Subjects.Verify(Repository => Repository.Remove(Subject), Times.Once);
    }
}
