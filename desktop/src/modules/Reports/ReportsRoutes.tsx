import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { DashboardPage } from './view/DashboardPage';

// Tablero de indicadores: rutas y entradas del menú.
export const ReportsRoutes: ModuleRoutes = {
  Name: 'Reports',
  Menu: [
    { Id: 'dashboard', Path: '/dashboard', LabelKey: 'Nav.Dashboard', Icon: 'Dashboard', Permission: 'Reports.View' },
  ],
  Private: <Route path="dashboard" element={<DashboardPage />} />,
};
