// Pruebas del servicio de notificaciones.
using Moq;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Application.Services;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;
using Xunit;

namespace USystem.Tests.Services;

public class NotificationServiceTests
{
    private static (NotificationService Service, Mock<INotificationRepository> Repository) CreateService()
    {
        var Repository = new Mock<INotificationRepository>();
        var UnitOfWork = new Mock<IUnitOfWork>();
        var AuditService = new Mock<IAuditService>();

        UnitOfWork
            .Setup(Unit => Unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return (new NotificationService(Repository.Object, UnitOfWork.Object, AuditService.Object), Repository);
    }

    [Fact]
    public async Task CreateTemplateAsync_ClaveDuplicada_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.ExistsTemplateByCodeAsync("WELCOME", null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.CreateTemplateAsync(new CreateNotificationTemplateRequest
        {
            Code = "welcome",
            SubjectLine = "Bienvenido",
            Body = "Hola {{StudentName}}",
        }));
    }

    [Fact]
    public async Task SendAsync_SustituyeVariablesYCreaNotificacion()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetTemplateByCodeAsync("WELCOME", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationTemplate
            {
                Id = 1,
                Code = "WELCOME",
                SubjectLine = "Bienvenido {{StudentName}}",
                Body = "Hola {{StudentName}}, tu matrícula es {{StudentNumber}}",
                IsActive = true,
            });

        var Dto = await Service.SendAsync(new SendNotificationRequest
        {
            TemplateCode = "welcome",
            RecipientEmail = "ana@usystem.local",
            Variables = new Dictionary<string, string>
            {
                ["StudentName"] = "Ana",
                ["StudentNumber"] = "A20260001",
            },
        });

        Assert.Equal("Bienvenido Ana", Dto.SubjectLine);
        Assert.Contains("A20260001", Dto.Body);
        Assert.Equal("Pending", Dto.Status);
        Repository.Verify(
            Repo => Repo.AddNotificationAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_PlantillaInactiva_DevuelveConflicto()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetTemplateByCodeAsync("OLD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationTemplate { Id = 2, Code = "OLD", IsActive = false });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Service.SendAsync(new SendNotificationRequest
        {
            TemplateCode = "OLD",
            RecipientEmail = "ana@usystem.local",
        }));
    }

    [Fact]
    public async Task SendAsync_PlantillaInexistente_LanzaNotFound()
    {
        var (Service, Repository) = CreateService();

        Repository
            .Setup(Repo => Repo.GetTemplateByCodeAsync("NOPE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.SendAsync(new SendNotificationRequest
        {
            TemplateCode = "NOPE",
            RecipientEmail = "ana@usystem.local",
        }));
    }
}
