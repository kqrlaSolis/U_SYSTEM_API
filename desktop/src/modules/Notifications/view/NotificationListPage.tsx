import { CrudListPage } from '../../../shared/crud';
import { NotificationTemplateConfig } from '../model/NotificationTemplateConfig';

// Listado de plantillas de notificación sobre el CRUD genérico.
export function NotificationListPage() {
  return <CrudListPage Config={NotificationTemplateConfig} />;
}
