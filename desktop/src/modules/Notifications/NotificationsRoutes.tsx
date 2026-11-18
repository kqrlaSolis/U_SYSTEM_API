import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { NotificationFormPage } from './view/NotificationFormPage';
import { NotificationListPage } from './view/NotificationListPage';

// Plantillas de notificación: rutas y entradas del menú.
export const NotificationsRoutes: ModuleRoutes = {
  Name: 'Notifications',
  Menu: [
    { Id: 'notifications', Path: '/notifications', LabelKey: 'Nav.Notifications', Icon: 'Notifications', Permission: 'Notifications.View' },
  ],
  Private: (
    <>
      <Route path="notifications" element={<NotificationListPage />} />
      <Route path="notifications/new" element={<NotificationFormPage />} />
      <Route path="notifications/:Id" element={<NotificationFormPage />} />
    </>
  ),
};
