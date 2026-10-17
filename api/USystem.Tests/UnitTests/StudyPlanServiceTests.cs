// Pruebas de las reglas de los planes de estudio: créditos, seriación, vigencia y bloqueos.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests;

public class StudyPlanServiceTests
{
    private readonly Mock<IStudyPlanRepository> _Plans = new();
    private readonly Mock<ICareerRepository> _Careers = new();
    private readonly Mock<ISubjectRepository> _Subjects = new();
    private readonly Mock<IUnitOfWork> _UnitOfWork = new();
    private readonly Mock<IAuditService> _Audit = new();

    private StudyPlanService BuildService()
    {
        _UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _Careers
            .Setup(Repository => Repository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Career { Id = 1, Code = "LAE", Name = "Licenciatura en Administración", TermCount = 9 });

        _Plans
            .Setup(Repository => Repository.GetSubjectsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<StudyPlanSubject>());
        _Plans
            .Setup(Repository => Repository.GetPrerequisitesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<SubjectPrerequisite>());
        _Plans
            .Setup(Repository => Repository.AddAsync(It.IsAny<StudyPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _Plans
            .Setup(Repository => Repository.ReplaceSubjectsAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<StudyPlanSubject>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _Plans
            .Setup(Repository => Repository.ReplacePrerequisitesAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<SubjectPrerequisite>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _Plans
            .Setup(Repository => Repository.ClearCurrentAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _Subjects
            .Setup(Repository => Repository.GetByIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Subject>());

        return new StudyPlanService(
            _Plans.Object,
            _Careers.Object,
            _Subjects.Object,
            _UnitOfWork.Object,
            _Audit.Object);
    }

    private static StudyPlan BuildPlan(short TotalCredits = 20) => new()
    {
        Id = 5,
        CareerId = 1,
        Code = "LAE-2026",
        Version = "1",
        StartDate = new DateOnly(2026, 1, 1),
        TotalCredits = TotalCredits,
        IsCurrent = false,
    };

    private static CreateStudyPlanRequest BuildCreateRequest() => new()
    {
        CareerId = 1,
        Code = "LAE-2026",
        Version = "1",
        StartDate = new DateOnly(2026, 1, 1),
        TotalCredits = 20,
    };

    private void SetupPlanSubjects(int PlanId, params int[] SubjectIds)
        => _Plans
            .Setup(Repository => Repository.GetSubjectsAsync(PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SubjectIds
                .Select(SubjectId => new StudyPlanSubject
                {
                    StudyPlanId = PlanId,
                    SubjectId = SubjectId,
                    TermNumber = 1,
                    Type = "Mandatory",
                })
                .ToList());

    [Fact]
    public async Task CreateRejectsADuplicatedCodeAndVersion()
    {
        var Service = BuildService();

        _Plans
            .Setup(Repository => Repository.ExistsByCodeVersionAsync(
                1,
                "LAE-2026",
                "1",
                null,
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Service.CreateAsync(BuildCreateRequest()));

        Assert.Equal("StudyPlans.CodeTaken", Error.Code);
    }

    [Fact]
    public async Task CreateRejectsAnUnknownCareer()
    {
        var Service = BuildService();

        _Careers
            .Setup(Repository => Repository.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Career?)null);

        var Error = await Assert.ThrowsAsync<NotFoundException>(
            () => Service.CreateAsync(new CreateStudyPlanRequest
            {
                CareerId = 99,
                Code = "X",
                Version = "1",
                StartDate = new DateOnly(2026, 1, 1),
                TotalCredits = 10,
            }));

        Assert.Equal("Careers.NotFound", Error.Code);
    }

    [Fact]
    public async Task ReplaceSubjectsRejectsACreditMismatch()
    {
        var Service = BuildService();

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildPlan(TotalCredits: 20));
        _Subjects
            .Setup(Repository => Repository.GetByIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Subject { Id = 1, Code = "MAT-101", Name = "Mate", Credits = 8 } });

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.ReplaceSubjectsAsync(
            5,
            new StudyPlanSubjectsRequest
            {
                Subjects = new List<StudyPlanSubjectItem>
                {
                    new() { SubjectId = 1, TermNumber = 1, Type = "Mandatory" },
                },
            }));

        Assert.Equal("StudyPlans.CreditsMismatch", Error.Code);
        Assert.Equal("8", Error.Values["SubjectCredits"]);
        Assert.Equal("20", Error.Values["PlanCredits"]);
    }

    [Fact]
    public async Task PlanWithStudentsCannotChangeItsStructure()
    {
        var Service = BuildService();

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildPlan());
        _Plans
            .Setup(Repository => Repository.HasStudentsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.ReplaceSubjectsAsync(
            5,
            new StudyPlanSubjectsRequest
            {
                Subjects = new List<StudyPlanSubjectItem>
                {
                    new() { SubjectId = 1, TermNumber = 1, Type = "Mandatory" },
                },
            }));

        Assert.Equal("StudyPlans.InUse", Error.Code);
    }

    [Fact]
    public async Task CircularPrerequisitesAreRejected()
    {
        var Service = BuildService();
        SetupPlanSubjects(5, 1, 2);

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildPlan());
        _Plans
            .Setup(Repository => Repository.HasStudentsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.ReplacePrerequisitesAsync(
            5,
            new StudyPlanPrerequisitesRequest
            {
                Prerequisites = new List<StudyPlanPrerequisiteItem>
                {
                    new() { SubjectId = 1, PrerequisiteSubjectId = 2 },
                    new() { SubjectId = 2, PrerequisiteSubjectId = 1 },
                },
            }));

        Assert.Equal("StudyPlans.CircularPrerequisite", Error.Code);
    }

    [Fact]
    public async Task PrerequisitesOutsideThePlanAreRejected()
    {
        var Service = BuildService();
        SetupPlanSubjects(5, 1);

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildPlan());
        _Plans
            .Setup(Repository => Repository.HasStudentsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Error = await Assert.ThrowsAsync<ValidationException>(() => Service.ReplacePrerequisitesAsync(
            5,
            new StudyPlanPrerequisitesRequest
            {
                Prerequisites = new List<StudyPlanPrerequisiteItem>
                {
                    new() { SubjectId = 1, PrerequisiteSubjectId = 99 },
                },
            }));

        Assert.Equal("Validation.InvalidFormat", Error.Code);
    }

    [Fact]
    public async Task SetCurrentClearsThePreviousCurrentPlan()
    {
        var Service = BuildService();
        var Plan = BuildPlan(TotalCredits: 8);
        SetupPlanSubjects(5, 1);

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Plan);
        _Subjects
            .Setup(Repository => Repository.GetByIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Subject { Id = 1, Code = "MAT-101", Name = "Mate", Credits = 8 } });

        var Response = await Service.SetCurrentAsync(5);

        Assert.True(Response.IsCurrent);
        Assert.True(Plan.IsCurrent);
        _Plans.Verify(
            Repository => Repository.ClearCurrentAsync(1, 5, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetCurrentRequiresTheFullCreditLoad()
    {
        var Service = BuildService();
        SetupPlanSubjects(5, 1);

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildPlan(TotalCredits: 20));
        _Subjects
            .Setup(Repository => Repository.GetByIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Subject { Id = 1, Code = "MAT-101", Name = "Mate", Credits = 8 } });

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SetCurrentAsync(5));

        Assert.Equal("StudyPlans.CreditsMismatch", Error.Code);
    }

    [Fact]
    public async Task CurrentPlanCannotBeDeleted()
    {
        var Service = BuildService();
        var Plan = BuildPlan();
        Plan.IsCurrent = true;

        _Plans
            .Setup(Repository => Repository.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Plan);
        _Plans
            .Setup(Repository => Repository.HasStudentsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.DeleteAsync(5));

        Assert.Equal("StudyPlans.CurrentCannotDelete", Error.Code);
    }
}
