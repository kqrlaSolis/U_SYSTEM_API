// Pruebas de las reglas del calendario cuatrimestral.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Constants;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class AcademicTermServiceTests
{
    private readonly Mock<IAcademicTermRepository> _Terms = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();

    private AcademicTermService BuildService()
    {
        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _Terms
            .Setup(Repository => Repository.AddAsync(It.IsAny<AcademicTerm>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new AcademicTermService(_Terms.Object, _UnitOfWork.Object, _Audit.Object);
    }

    private static AcademicTerm BuildTerm(string Status = AcademicTermStatuses.Planned) => new()
    {
        Id = 1,
        Code = "2026-1",
        Name = "Enero - Abril 2026",
        AcademicYear = 2026,
        TermNumber = 1,
        StartDate = new DateOnly(2026, 1, 12),
        EndDate = new DateOnly(2026, 4, 30),
        EnrollmentStartDate = new DateOnly(2026, 1, 5),
        EnrollmentEndDate = new DateOnly(2026, 1, 9),
        ReEnrollmentStartDate = new DateOnly(2026, 1, 5),
        ReEnrollmentEndDate = new DateOnly(2026, 1, 9),
        ExamStartDate = new DateOnly(2026, 4, 20),
        ExamEndDate = new DateOnly(2026, 4, 24),
        GradeReportCloseDate = new DateOnly(2026, 4, 30),
        Status = Status,
    };

    private static CreateAcademicTermRequest BuildRequest() => new()
    {
        Code = "2026-1",
        Name = "Enero - Abril 2026",
        AcademicYear = 2026,
        TermNumber = 1,
        StartDate = new DateOnly(2026, 1, 12),
        EndDate = new DateOnly(2026, 4, 30),
        EnrollmentStartDate = new DateOnly(2026, 1, 5),
        EnrollmentEndDate = new DateOnly(2026, 1, 9),
        ReEnrollmentStartDate = new DateOnly(2026, 1, 5),
        ReEnrollmentEndDate = new DateOnly(2026, 1, 9),
        ExamStartDate = new DateOnly(2026, 4, 20),
        ExamEndDate = new DateOnly(2026, 4, 24),
        GradeReportCloseDate = new DateOnly(2026, 4, 30),
    };

    [Fact]
    public async Task CreateRejectsADuplicatedCode()
    {
        var Service = BuildService();

        _Terms
            .Setup(Repository => Repository.ExistsByCodeAsync(
                "2026-1",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(BuildRequest()));

        Assert.Equal("AcademicTerms.CodeTaken", Error.Code);
    }

    [Fact]
    public async Task CreateRejectsAnOverlappingTerm()
    {
        var Service = BuildService();

        _Terms
            .Setup(Repository => Repository.ExistsByCodeAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _Terms
            .Setup(Repository => Repository.GetOverlappingAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTerm(AcademicTermStatuses.Planned));

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(BuildRequest()));

        Assert.Equal("AcademicTerms.Overlap", Error.Code);
        Assert.Equal("2026-1", Error.Values["Code"]);
    }

    [Fact]
    public async Task ClosedTermIsReadOnly()
    {
        var Service = BuildService();

        _Terms
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTerm(AcademicTermStatuses.Closed));

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.UpdateAsync(
            1,
            new UpdateAcademicTermRequest
            {
                Code = "2026-1",
                Name = "Otro nombre",
                AcademicYear = 2026,
                TermNumber = 1,
                StartDate = new DateOnly(2026, 1, 12),
                EndDate = new DateOnly(2026, 4, 30),
                EnrollmentStartDate = new DateOnly(2026, 1, 5),
                EnrollmentEndDate = new DateOnly(2026, 1, 9),
                ReEnrollmentStartDate = new DateOnly(2026, 1, 5),
                ReEnrollmentEndDate = new DateOnly(2026, 1, 9),
                ExamStartDate = new DateOnly(2026, 4, 20),
                ExamEndDate = new DateOnly(2026, 4, 24),
                GradeReportCloseDate = new DateOnly(2026, 4, 30),
            }));

        Assert.Equal("AcademicTerms.Closed", Error.Code);
    }

    [Fact]
    public async Task OnlyOneTermCanBeActive()
    {
        var Service = BuildService();
        var Term = BuildTerm();

        _Terms
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Term);
        _Terms
            .Setup(Repository => Repository.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AcademicTerm { Id = 2, Code = "2025-3", Status = AcademicTermStatuses.Open });

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.OpenAsync(1));

        Assert.Equal("AcademicTerms.OnlyOneInProgress", Error.Code);
    }

    [Fact]
    public async Task OpenMovesPlannedToOpen()
    {
        var Service = BuildService();
        var Term = BuildTerm();

        _Terms
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Term);
        _Terms
            .Setup(Repository => Repository.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademicTerm?)null);

        var Response = await Service.OpenAsync(1);

        Assert.Equal(AcademicTermStatuses.Open, Response.Status);
        _Terms.Verify(Repository => Repository.Update(Term), Times.Once);
    }

    [Fact]
    public async Task CloseMovesTermToClosed()
    {
        var Service = BuildService();
        var Term = BuildTerm(AcademicTermStatuses.Open);

        _Terms
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Term);

        var Response = await Service.CloseAsync(1);

        Assert.Equal(AcademicTermStatuses.Closed, Response.Status);
    }

    [Fact]
    public async Task ClosingAClosedTermFails()
    {
        var Service = BuildService();

        _Terms
            .Setup(Repository => Repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTerm(AcademicTermStatuses.Closed));

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CloseAsync(1));

        Assert.Equal("AcademicTerms.InvalidTransition", Error.Code);
    }

    [Fact]
    public async Task CurrentTermReturnsNotFoundWhenThereIsNone()
    {
        var Service = BuildService();

        _Terms
            .Setup(Repository => Repository.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademicTerm?)null);

        var Error = await Assert.ThrowsAsync<NotFoundException>(() => Service.GetCurrentAsync());

        Assert.Equal("AcademicTerms.NotFound", Error.Code);
    }
}
