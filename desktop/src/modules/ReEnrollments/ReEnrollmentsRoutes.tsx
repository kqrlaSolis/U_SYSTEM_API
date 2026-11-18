import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { ReEnrollmentPage } from './view/ReEnrollmentPage';

// Reinscripción: rutas y entradas del menú.
export const ReEnrollmentsRoutes: ModuleRoutes = {
  Name: 'ReEnrollments',
  Menu: [
    { Id: 're-enrollments', Path: '/re-enrollments', LabelKey: 'Nav.ReEnrollments', Icon: 'ReEnrollments', Permission: 'Enrollments.Create' },
  ],
  Private: <Route path="re-enrollments" element={<ReEnrollmentPage />} />,
};
