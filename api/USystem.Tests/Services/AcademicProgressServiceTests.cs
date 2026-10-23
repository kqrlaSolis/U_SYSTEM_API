// Pruebas del avance académico.
using Moq;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class AcademicProgressServiceTests
{
    private static (AcademicProgressService Service, Mock<ITranscriptRepository> Transcripts, Mock<IStudyPlanRepository> Plans, Mock<IClassGroupRepository> Groups, Mock<ISubjectRepository> Subjects) CreateService()
    {
        var Transcripts = new Mock<ITranscriptRepository>();
        var Plans = new Mock<IStudyPlanRepository>();
        var Groups = new Mock<IClassGroupRepository>();
        var Subjects = new Mock<ISubjectRepository>();

        return (
            new AcademicProgressService(Transcripts.Object, Plans.Object, Groups.Object, Subjects.Object),
            Transcripts,
            Plans,
            Groups,
            Subjects);
    }

    [Fact]
    public async Task GetByStudentAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Transcripts, _, _, _) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByStudentAsync(50));
    }

    [Fact]
    public async Task GetByStudentAsync_CalculaCreditosAprobadosYPorcentaje()
    {
        var (Service, Transcripts, Plans, Groups, Subjects) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student
            {
                Id = 1,
                StudentNumber = "A20260001",
                FirstName = "Ana",
                PaternalLastName = "García",
                StudyPlanId = 4,
            });
        Plans
            .Setup(Repo => Repo.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StudyPlan { Id = 4, Code = "LAE-2026", TotalCredits = 16 });
        Transcripts
            .Setup(Repo => Repo.GetGradesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<(Grade, Enrollment)>
            {
                (
                    new Grade { Id = 1, EnrollmentId = 7, ClassGroupId = 3, Status = "Passed" },
                    new Enrollment { Id = 7, StudentId = 1, AcademicTermId = 1 }
                ),
            });
        Groups
            .Setup(Repo => Repo.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 3, SubjectId = 4, Code = "G-03" });
        Subjects
            .Setup(Repo => Repo.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = 4, Code = "MAT-101", Name = "Álgebra", Credits = 8 });

        var Result = await Service.GetByStudentAsync(1);

        Assert.Equal("A20260001", Result.StudentNumber);
        Assert.Equal(8, Result.ApprovedCredits);
        Assert.Equal(50m, Result.ProgressPercent);
        Assert.Equal(1, Result.PassedSubjects);
        Assert.Equal(0, Result.FailedSubjects);
    }
}
