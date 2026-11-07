// Contrato de plantilla de notificación (GET /api/v1/notification-templates)
export interface NotificationTemplate {
  Id: number;
  Code: string;
  SubjectLine: string;
  Body: string;
  IsActive: boolean;
}

export interface NotificationTemplateRequest {
  Code: string;
  SubjectLine: string;
  Body: string;
  IsActive: boolean;
}

export interface SendNotificationRequest {
  TemplateCode: string;
  RecipientEmail: string;
  StudentId: number | null;
  Variables: Record<string, string>;
}
