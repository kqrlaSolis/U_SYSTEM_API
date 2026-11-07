import { CrudFormPage } from '../../../shared/crud';
import { NotificationTemplateConfig } from '../model/NotificationTemplateConfig';

// Alta y edición de plantillas de notificación.
export function NotificationFormPage() {
  return <CrudFormPage Config={NotificationTemplateConfig} />;
}
