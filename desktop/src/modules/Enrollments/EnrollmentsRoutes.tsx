import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { EnrollmentFormPage } from './view/EnrollmentFormPage';
import { EnrollmentListPage } from './view/EnrollmentListPage';
import { EnrollmentWizardPage } from './view/EnrollmentWizardPage';

// Inscripciones y asistente de inscripción: rutas y entradas del menú.
export const EnrollmentsRoutes: ModuleRoutes = {
  Name: 'Enrollments',
  Menu: [
    { Id: 'enrollments', Path: '/enrollments', LabelKey: 'Nav.Enrollments', Icon: 'Enrollments', Permission: 'Enrollments.View' },
    { Id: 'enrollment-wizard', Path: '/enrollment-wizard', LabelKey: 'Nav.EnrollmentWizard', Icon: 'Enrollments', Permission: 'Enrollments.Create' },
  ],
  Private: (
    <>
      <Route path="enrollments" element={<EnrollmentListPage />} />
      <Route path="enrollments/new" element={<EnrollmentFormPage />} />
      <Route path="enrollments/:Id" element={<EnrollmentFormPage />} />
      <Route path="enrollment-wizard" element={<EnrollmentWizardPage />} />
    </>
  ),
};
