// Pruebas del kardex.
using Moq;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class TranscriptServiceTests
{
    private static (TranscriptService Service, Mock<ITranscriptRepository> Transcripts, Mock<IClassGroupRepository> Groups, Mock<ISubjectRepository> Subjects) CreateService()
    {
        var Transcripts = new Mock<ITranscriptRepository>();
        var Groups = new Mock<IClassGroupRepository>();
        var Subjects = new Mock<ISubjectRepository>();

        return (new TranscriptService(Transcripts.Object, Groups.Object, Subjects.Object), Transcripts, Groups, Subjects);
    }

    [Fact]
    public async Task GetByStudentAsync_Inexistente_LanzaNotFound()
    {
        var (Service, Transcripts, _, _) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.GetByStudentAsync(99));
    }

    [Fact]
    public async Task GetByStudentAsync_ResuelveMateriaYGrupo()
    {
        var (Service, Transcripts, Groups, Subjects) = CreateService();

        Transcripts
            .Setup(Repo => Repo.GetStudentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student
            {
                Id = 1,
                StudentNumber = "A20260001",
                FirstName = "Ana",
                PaternalLastName = "García",
            });
        Transcripts
            .Setup(Repo => Repo.GetGradesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<(Grade, Enrollment)>
            {
                (
                    new Grade { Id = 1, EnrollmentId = 7, ClassGroupId = 3, FinalScore = 9.5m, Status = "Passed", AttemptNumber = 1 },
                    new Enrollment { Id = 7, StudentId = 1, AcademicTermId = 2 }
                ),
            });
        Groups
            .Setup(Repo => Repo.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 3, SubjectId = 4, Code = "G-03" });
        Subjects
            .Setup(Repo => Repo.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = 4, Code = "MAT-101", Name = "Álgebra" });

        var Result = await Service.GetByStudentAsync(1);

        Assert.Equal("A20260001", Result.StudentNumber);
        Assert.Equal("Ana García", Result.FullName);
        Assert.Single(Result.Entries);
        Assert.Equal("MAT-101", Result.Entries[0].SubjectCode);
        Assert.Equal(9.5m, Result.Entries[0].FinalScore);
        Assert.Equal(2, Result.Entries[0].AcademicTermId);
    }
}
