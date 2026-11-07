import type { CrudConfig } from '../../../shared/crud';
import type { NotificationTemplate, NotificationTemplateRequest } from './NotificationTemplate';
import { NotificationsService } from './NotificationsService';

// CRUD de plantillas de notificación (variables {{Nombre}} en el cuerpo).
export const NotificationTemplateConfig: CrudConfig<NotificationTemplate, NotificationTemplateRequest> = {
  Route: '/notifications',
  TitleKey: 'Notifications.Title',
  SubtitleKey: 'Notifications.Subtitle',
  NewKey: 'Notifications.New',
  EditKey: 'Notifications.Edit',
  EmptyKey: 'Notifications.Empty',
  SearchKey: 'Notifications.Search',

  PermissionView: 'Notifications.View',
  PermissionCreate: 'Notifications.Send',
  PermissionEdit: 'Notifications.Send',
  PermissionDelete: 'Notifications.Send',

  Columns: [
    { Field: 'Code', HeaderKey: 'Notifications.Columns.Code' },
    { Field: 'SubjectLine', HeaderKey: 'Notifications.Columns.Subject' },
    { Field: 'IsActive', HeaderKey: 'Notifications.Columns.IsActive', Type: 'boolean' },
  ],

  Fields: [
    { Name: 'Code', LabelKey: 'Notifications.Columns.Code', Required: true },
    { Name: 'SubjectLine', LabelKey: 'Notifications.Columns.Subject', Required: true },
    { Name: 'Body', LabelKey: 'Notifications.Fields.Body', Required: true },
    { Name: 'IsActive', LabelKey: 'Notifications.Columns.IsActive', Type: 'switch' },
  ],

  EmptyForm: {
    Code: '',
    SubjectLine: '',
    Body: 'Hola {{StudentName}},',
    IsActive: 'true',
  },

  List: (Page, PageSize, Search) => NotificationsService.GetPaged(Page, PageSize, Search),
  Load: (Id) => NotificationsService.GetById(Id),
  Create: (Request) => NotificationsService.Create(Request),
  Update: (Id, Request) => NotificationsService.Update(Id, Request),
  Remove: (Id) => NotificationsService.Delete(Id),

  ToForm: (Row: NotificationTemplate) => ({
    Code: Row.Code,
    SubjectLine: Row.SubjectLine,
    Body: Row.Body,
    IsActive: Row.IsActive ? 'true' : 'false',
  }),

  FromForm: (Values) => ({
    Code: Values.Code.trim(),
    SubjectLine: Values.SubjectLine.trim(),
    Body: Values.Body,
    IsActive: Values.IsActive === 'true',
  }),
};
