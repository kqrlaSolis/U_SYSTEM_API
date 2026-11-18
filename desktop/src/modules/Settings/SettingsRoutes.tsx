import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { SettingsPage } from './view/SettingsPage';

// Configuración: rutas y entradas del menú.
export const SettingsRoutes: ModuleRoutes = {
  Name: 'Settings',
  Menu: [
    { Id: 'settings', Path: '/settings', LabelKey: 'Nav.Settings', Icon: 'Settings', Permission: 'Settings.View' },
  ],
  Private: <Route path="settings" element={<SettingsPage />} />,
};
