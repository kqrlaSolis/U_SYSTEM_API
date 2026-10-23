// Plantillas de notificación y envíos.
namespace USystem.Application.Dtos;

public class NotificationTemplateDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string SubjectLine { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public class CreateNotificationTemplateRequest
{
    public string Code { get; set; } = string.Empty;

    public string SubjectLine { get; set; } = string.Empty;

    // Cuerpo con variables {{StudentName}}, {{Amount}}…
    public string Body { get; set; } = string.Empty;
}

public class UpdateNotificationTemplateRequest
{
    public string Code { get; set; } = string.Empty;

    public string SubjectLine { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public class SendNotificationRequest
{
    public string TemplateCode { get; set; } = string.Empty;

    public string RecipientEmail { get; set; } = string.Empty;

    public long? StudentId { get; set; }

    // Valores para las variables {{Nombre}}
    public Dictionary<string, string> Variables { get; set; } = new();
}

public class NotificationDto
{
    public long Id { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public string RecipientEmail { get; set; } = string.Empty;

    public string SubjectLine { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    // Pending, Sent, Error
    public string Status { get; set; } = string.Empty;

    public DateTime? SentAt { get; set; }
}
