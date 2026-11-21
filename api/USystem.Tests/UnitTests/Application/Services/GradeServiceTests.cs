// Pruebas del servicio de calificaciones.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Tests.UnitTests.Application.Services;

public class GradeServiceTests
{
    // Docente 7 vinculado al usuario 70; el grupo 1 es suyo y el grupo 2 es del docente 8
    private const long TeacherUserId = 70;
    private const int OwnTeacherId = 7;
    private const int OtherTeacherId = 8;

    private static (GradeService Service, Mock<IGradeRepository> Grades, Mock<IClassGroupRepository> Groups) CreateService()
        => CreateService(new Mock<ICurrentUser>(), new Mock<ITeacherRepository>());

    private static (GradeService Service, Mock<IGradeRepository> Grades, Mock<IClassGroupRepository> Groups) CreateService(
        Mock<ICurrentUser> CurrentUser,
        Mock<ITeacherRepository> Teachers)
    {
        var Grades = new Mock<IGradeRepository>();
        var Groups = new Mock<IClassGroupRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Grades
            .Setup(Repo => Repo.GetByClassGroupAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Grade>());

        return (
            new GradeService(
                Grades.Object,
                Groups.Object,
                Teachers.Object,
                CurrentUser.Object,
                UnitOfWork.Object,
                AuditService.Object),
            Grades,
            Groups);
    }

    private static Mock<ICurrentUser> BuildUser(long UserId, params string[] Roles)
    {
        var CurrentUser = new Mock<ICurrentUser>();
        CurrentUser.SetupGet(User => User.UserId).Returns(UserId);
        CurrentUser.SetupGet(User => User.Roles).Returns(Roles);
        CurrentUser
            .Setup(User => User.IsInRole(It.IsAny<string>()))
            .Returns((string Role) => Roles.Contains(Role));
        return CurrentUser;
    }

    private static Mock<ITeacherRepository> BuildTeachers()
    {
        var Teachers = new Mock<ITeacherRepository>();
        Teachers
            .Setup(Repo => Repo.GetByUserIdAsync(TeacherUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Teacher { Id = OwnTeacherId, UserId = TeacherUserId });
        return Teachers;
    }

    private static void SetupGroups(Mock<IClassGroupRepository> Groups)
    {
        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open", TeacherId = OwnTeacherId });
        Groups
            .Setup(Repo => Repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 2, Code = "G-02", Status = "Open", TeacherId = OtherTeacherId });
    }

    private static SaveGradesRequest BuildRequest()
        => new()
        {
            Grades = new List<GradeItemRequest>
            {
                new() { EnrollmentId = 1, Partial1 = 8.5m, Status = "InProgress" },
            },
        };

    [Fact]
    public async Task SaveAsync_GrupoCerrado_DevuelveConflicto()
    {
        var (Service, _, Groups) = CreateService();

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Closed" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SaveAsync(1, BuildRequest()));
    }

    [Fact]
    public async Task SaveAsync_ActaCerrada_DevuelveConflicto()
    {
        var (Service, Grades, Groups) = CreateService();

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open" });
        Grades
            .Setup(Repo => Repo.GetByEnrollmentAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Grade { Id = 5, EnrollmentId = 1, ClassGroupId = 1, IsReportClosed = true });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SaveAsync(1, BuildRequest()));
    }

    [Fact]
    public async Task SaveAsync_NuevaCalificacion_CreaRegistro()
    {
        var (Service, Grades, Groups) = CreateService();

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open" });
        Grades
            .Setup(Repo => Repo.GetByEnrollmentAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Grade?)null);

        await Service.SaveAsync(1, BuildRequest());

        Grades.Verify(
            Repo => Repo.AddAsync(
                It.Is<Grade>(Grade => Grade.Partial1 == 8.5m && Grade.AttemptNumber == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_CalificacionExistente_ActualizaParcial()
    {
        var (Service, Grades, Groups) = CreateService();
        var Existing = new Grade { Id = 9, EnrollmentId = 1, ClassGroupId = 1, IsReportClosed = false };

        Groups
            .Setup(Repo => Repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassGroup { Id = 1, Code = "G-01", Status = "Open" });
        Grades
            .Setup(Repo => Repo.GetByEnrollmentAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Existing);

        await Service.SaveAsync(1, BuildRequest());

        Assert.Equal(8.5m, Existing.Partial1);
        Grades.Verify(Repo => Repo.Update(Existing), Times.Once);
    }

    [Fact]
    public async Task GetByClassGroupAsync_MapeaCalificaciones()
    {
        var (Service, Grades, _) = CreateService();

        Grades
            .Setup(Repo => Repo.GetByClassGroupAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Grade>
            {
                new() { Id = 1, EnrollmentId = 10, ClassGroupId = 3, Ordinary = 9m, Status = "Passed" },
            });

        var Items = await Service.GetByClassGroupAsync(3);

        Assert.Single(Items);
        Assert.Equal(9m, Items[0].Ordinary);
    }

    [Fact]
    public async Task SaveAsync_DocenteEnGrupoPropio_GuardaCalificacion()
    {
        var (Service, Grades, Groups) = CreateService(BuildUser(TeacherUserId, "Teacher"), BuildTeachers());
        SetupGroups(Groups);

        await Service.SaveAsync(1, BuildRequest());

        Grades.Verify(Repo => Repo.AddAsync(It.IsAny<Grade>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByClassGroupAsync_DocenteEnGrupoPropio_DevuelveCalificaciones()
    {
        var (Service, Grades, Groups) = CreateService(BuildUser(TeacherUserId, "Teacher"), BuildTeachers());
        SetupGroups(Groups);

        await Service.GetByClassGroupAsync(1);

        Grades.Verify(Repo => Repo.GetByClassGroupAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_DocenteEnGrupoAjeno_Rechaza()
    {
        var (Service, Grades, Groups) = CreateService(BuildUser(TeacherUserId, "Teacher"), BuildTeachers());
        SetupGroups(Groups);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SaveAsync(2, BuildRequest()));

        Assert.Equal("Grades.ClassGroupNotAssigned", Error.Code);
        Assert.Equal(403, Error.StatusCode);
        Grades.Verify(Repo => Repo.AddAsync(It.IsAny<Grade>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByClassGroupAsync_DocenteEnGrupoAjeno_Rechaza()
    {
        var (Service, Grades, Groups) = CreateService(BuildUser(TeacherUserId, "Teacher"), BuildTeachers());
        SetupGroups(Groups);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.GetByClassGroupAsync(2));

        Assert.Equal("Grades.ClassGroupNotAssigned", Error.Code);
        Grades.Verify(Repo => Repo.GetByClassGroupAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_DocenteSinFichaVinculada_Rechaza()
    {
        // Usuario con rol Teacher sin registro en Teachers.UserId
        var (Service, _, Groups) = CreateService(BuildUser(99, "Teacher"), BuildTeachers());
        SetupGroups(Groups);

        var Error = await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SaveAsync(1, BuildRequest()));

        Assert.Equal("Grades.ClassGroupNotAssigned", Error.Code);
    }

    [Theory]
    [InlineData("Administrator")]
    [InlineData("Registrar")]
    [InlineData("ProgramCoordinator")]
    public async Task SaveAsync_RolAmplioConRolTeacher_NoSeAcota(string BroaderRole)
    {
        var Teachers = BuildTeachers();
        var (Service, Grades, Groups) = CreateService(BuildUser(TeacherUserId, "Teacher", BroaderRole), Teachers);
        SetupGroups(Groups);

        await Service.SaveAsync(2, BuildRequest());

        Grades.Verify(Repo => Repo.AddAsync(It.IsAny<Grade>(), It.IsAny<CancellationToken>()), Times.Once);
        Teachers.Verify(
            Repo => Repo.GetByUserIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByClassGroupAsync_Administrador_NoSeAcota()
    {
        var Teachers = BuildTeachers();
        var (Service, Grades, Groups) = CreateService(BuildUser(1, "Administrator"), Teachers);
        SetupGroups(Groups);

        await Service.GetByClassGroupAsync(2);

        Grades.Verify(Repo => Repo.GetByClassGroupAsync(2, It.IsAny<CancellationToken>()), Times.Once);
        Teachers.Verify(
            Repo => Repo.GetByUserIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
