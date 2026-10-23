// Plantillas de notificación con variables y envío (sin SMTP: queda Pending).
using System.Globalization;
using System.Text.RegularExpressions;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface INotificationService
{
    Task<PagedResult<NotificationTemplateDto>> GetTemplatesAsync(PagedQuery Query, CancellationToken Cancellation = default);

    Task<NotificationTemplateDto> GetTemplateAsync(int Id, CancellationToken Cancellation = default);

    Task<NotificationTemplateDto> CreateTemplateAsync(CreateNotificationTemplateRequest Request, CancellationToken Cancellation = default);

    Task<NotificationTemplateDto> UpdateTemplateAsync(int Id, UpdateNotificationTemplateRequest Request, CancellationToken Cancellation = default);

    Task DeleteTemplateAsync(int Id, CancellationToken Cancellation = default);

    Task<NotificationDto> SendAsync(SendNotificationRequest Request, CancellationToken Cancellation = default);
}

public class NotificationService : INotificationService
{
    private const string VariablePattern = @"\{\{\s*(?<Name>[A-Za-z0-9_]+)\s*\}\}";

    private readonly INotificationRepository _NotificationRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public NotificationService(
        INotificationRepository NotificationRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _NotificationRepository = NotificationRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<NotificationTemplateDto>> GetTemplatesAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _NotificationRepository.GetTemplatesPagedAsync(Query, Cancellation);
        return PagedResult<NotificationTemplateDto>.Create(
            Page.Items.Select(ToTemplateDto).ToList(),
            Page.TotalCount,
            Query);
    }

    public async Task<NotificationTemplateDto> GetTemplateAsync(int Id, CancellationToken Cancellation = default)
    {
        var Template = await _NotificationRepository.GetTemplateByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Notifications.NotFound");

        return ToTemplateDto(Template);
    }

    public async Task<NotificationTemplateDto> CreateTemplateAsync(
        CreateNotificationTemplateRequest Request,
        CancellationToken Cancellation = default)
    {
        var Code = Request.Code.Trim().ToUpperInvariant();

        if (await _NotificationRepository.ExistsTemplateByCodeAsync(Code, null, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Notifications.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var NewTemplate = new NotificationTemplate
        {
            Code = Code,
            SubjectLine = Request.SubjectLine.Trim(),
            Body = Request.Body.Trim(),
            IsActive = true,
        };

        await _NotificationRepository.AddTemplateAsync(NewTemplate, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Notifications",
            nameof(NotificationTemplate),
            NewTemplate.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewTemplate.Code, NewTemplate.SubjectLine },
            Cancellation);

        return ToTemplateDto(NewTemplate);
    }

    public async Task<NotificationTemplateDto> UpdateTemplateAsync(
        int Id,
        UpdateNotificationTemplateRequest Request,
        CancellationToken Cancellation = default)
    {
        var Template = await _NotificationRepository.GetTemplateByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Notifications.NotFound");

        var Code = Request.Code.Trim().ToUpperInvariant();
        if (await _NotificationRepository.ExistsTemplateByCodeAsync(Code, Template.Id, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Notifications.CodeTaken",
                new Dictionary<string, string> { ["Code"] = Code });
        }

        var Before = new { Template.Code, Template.SubjectLine, Template.IsActive };

        Template.Code = Code;
        Template.SubjectLine = Request.SubjectLine.Trim();
        Template.Body = Request.Body.Trim();
        Template.IsActive = Request.IsActive;

        _NotificationRepository.UpdateTemplate(Template);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Notifications",
            nameof(NotificationTemplate),
            Template.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Template.Code, Template.SubjectLine, Template.IsActive },
            Cancellation);

        return ToTemplateDto(Template);
    }

    public async Task DeleteTemplateAsync(int Id, CancellationToken Cancellation = default)
    {
        var Template = await _NotificationRepository.GetTemplateByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Notifications.NotFound");

        _NotificationRepository.RemoveTemplate(Template);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Notifications",
            nameof(NotificationTemplate),
            Template.Id.ToString(CultureInfo.InvariantCulture),
            new { Template.Code },
            null,
            Cancellation);
    }

    public async Task<NotificationDto> SendAsync(
        SendNotificationRequest Request,
        CancellationToken Cancellation = default)
    {
        var Template = await _NotificationRepository.GetTemplateByCodeAsync(
            Request.TemplateCode.Trim().ToUpperInvariant(),
            Cancellation)
            ?? throw new NotFoundException("Notifications.NotFound");

        if (!Template.IsActive)
        {
            throw BusinessRuleException.Conflict("Notifications.TemplateInactive");
        }

        // Sustituye {{Variable}} con lo enviado; las desconocidas quedan vacías
        var Subject = Substitute(Template.SubjectLine, Request.Variables);
        var Body = Substitute(Template.Body, Request.Variables);

        var Notification = new Notification
        {
            TemplateId = Template.Id,
            StudentId = Request.StudentId,
            RecipientEmail = Request.RecipientEmail.Trim(),
            Status = "Pending",
            RetryCount = 0,
        };

        await _NotificationRepository.AddNotificationAsync(Notification, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Send",
            "Notifications",
            nameof(Notification),
            Notification.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { Template = Template.Code, Notification.RecipientEmail },
            Cancellation);

        return new NotificationDto
        {
            Id = Notification.Id,
            TemplateCode = Template.Code,
            RecipientEmail = Notification.RecipientEmail,
            SubjectLine = Subject,
            Body = Body,
            Status = Notification.Status,
            SentAt = Notification.SentAt,
        };
    }

    private static string Substitute(string Text, Dictionary<string, string> Variables)
        => Regex.Replace(
            Text,
            VariablePattern,
            Match => Variables.TryGetValue(Match.Groups["Name"].Value, out var Value) ? Value : string.Empty,
            RegexOptions.CultureInvariant);

    private static NotificationTemplateDto ToTemplateDto(NotificationTemplate Template)
        => new()
        {
            Id = Template.Id,
            Code = Template.Code,
            SubjectLine = Template.SubjectLine,
            Body = Template.Body,
            IsActive = Template.IsActive,
        };
}
