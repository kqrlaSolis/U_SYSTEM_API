import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { AuditLogListPage } from './view/AuditLogListPage';

// Bitácora: rutas y entradas del menú.
export const AuditLogsRoutes: ModuleRoutes = {
  Name: 'AuditLogs',
  Menu: [
    { Id: 'audit-logs', Path: '/audit-logs', LabelKey: 'Nav.AuditLogs', Icon: 'AuditLogs', Permission: 'AuditLogs.View' },
  ],
  Private: <Route path="audit-logs" element={<AuditLogListPage />} />,
};
