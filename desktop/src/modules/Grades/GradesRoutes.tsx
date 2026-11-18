import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { GradesPage } from './view/GradesPage';

// Calificaciones: rutas y entradas del menú.
export const GradesRoutes: ModuleRoutes = {
  Name: 'Grades',
  Menu: [
    { Id: 'grades', Path: '/grades', LabelKey: 'Nav.Grades', Icon: 'Grades', Permission: 'Grades.View' },
  ],
  Private: <Route path="grades" element={<GradesPage />} />,
};
