import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { NotificationTemplate, NotificationTemplateRequest } from './NotificationTemplate';

const TemplatesPath = '/api/v1/notification-templates';

// Servicio de plantillas de notificación
export const NotificationsService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<NotificationTemplate>> {
    return HttpClient.Get<PagedResult<NotificationTemplate>>(TemplatesPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<NotificationTemplate> {
    return HttpClient.Get<NotificationTemplate>(`${TemplatesPath}/${Id}`);
  },

  async Create(Request: NotificationTemplateRequest): Promise<NotificationTemplate> {
    return HttpClient.Post<NotificationTemplate>(TemplatesPath, Request);
  },

  async Update(Id: number, Request: NotificationTemplateRequest): Promise<NotificationTemplate> {
    return HttpClient.Put<NotificationTemplate>(`${TemplatesPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${TemplatesPath}/${Id}`);
  },
};
