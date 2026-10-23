// Pruebas de la propuesta de reinscripción.
using Moq;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class ReEnrollmentServiceTests
{
    private static (ReEnrollmentService Service, Mock<ITranscriptRepository> Transcripts, Mock<IStudyPlanRepository> Plans, Mock<ISubjectRepository> Subjects, Mock<IClassGroupRepository> Groups) CreateService()
    {
        var Transcripts = new Mock<ITranscriptRepository>();
        var Plans = new Mock<IStudyPlanRepository>();
        var Subjects = new Mock<ISubjectRepository>();
        var Groups = new Mock<IClassGroupRepository>();

        return (new ReEnrollmentService(Transcripts.Object, Plans.Object, Subjects.Object, Groups.Object), Transcripts, Plans, Subjects, Groups);
    }

    [Fact]
    public async Task ProposeAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Transcripts, _, _, _) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(88, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.ProposeAsync(88));
    }

    [Fact]
    public async Task ProposeAsync_MateriaElegible_CuandoRequisitoAprobado()
    {
        var (Service, Transcripts, Plans, Subjects, Groups) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student { Id = 1, StudentNumber = "A20260001", FirstName = "Ana", PaternalLastName = "García", StudyPlanId = 4 });
        Plans
            .Setup(Repo => Repo.GetSubjectsAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StudyPlanSubject>
            {
                new() { StudyPlanId = 4, SubjectId = 1, TermNumber = 1 },
                new() { StudyPlanId = 4, SubjectId = 2, TermNumber = 2 },
            });
        Plans
            .Setup(Repo => Repo.GetPrerequisitesAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubjectPrerequisite>
            {
                new() { StudyPlanId = 4, SubjectId = 2, PrerequisiteSubjectId = 1 },
            });
        Transcripts
            .Setup(Repo => Repo.GetGradesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<(Grade, Enrollment)>
            {
                (new Grade { Id = 1, ClassGroupId = 1, Status = "Passed" }, new Enrollment { Id = 7, StudentId = 1 }),
            });
        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, SubjectId = 1, Code = "G-01" });
        Subjects
            .Setup(Repo => Repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = 2, Code = "MAT-102", Name = "Matemáticas II", Credits = 8 });

        var Proposal = await Service.ProposeAsync(1);

        Assert.Equal(2, Proposal.NextTermNumber);
        Assert.Single(Proposal.Subjects);
        Assert.True(Proposal.Subjects[0].IsEligible);
        Assert.Null(Proposal.Subjects[0].BlockedReason);
    }

    [Fact]
    public async Task ProposeAsync_RequisitoPendiente_BloqueaMateria()
    {
        var (Service, Transcripts, Plans, Subjects, Groups) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student { Id = 2, StudentNumber = "A20260002", FirstName = "Luis", PaternalLastName = "Pérez", StudyPlanId = 4 });
        Plans
            .Setup(Repo => Repo.GetSubjectsAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StudyPlanSubject>
            {
                new() { StudyPlanId = 4, SubjectId = 2, TermNumber = 1 },
            });
        Plans
            .Setup(Repo => Repo.GetPrerequisitesAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubjectPrerequisite>
            {
                new() { StudyPlanId = 4, SubjectId = 2, PrerequisiteSubjectId = 1 },
            });
        Transcripts
            .Setup(Repo => Repo.GetGradesAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<(Grade, Enrollment)>());
        Subjects
            .Setup(Repo => Repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = 2, Code = "MAT-102", Name = "Matemáticas II", Credits = 8 });

        var Proposal = await Service.ProposeAsync(2);

        Assert.Single(Proposal.Subjects);
        Assert.False(Proposal.Subjects[0].IsEligible);
        Assert.Equal("ReEnrollments.PendingPrerequisite", Proposal.Subjects[0].BlockedReason);
    }
}
