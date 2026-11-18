import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { SubjectFormPage } from './view/SubjectFormPage';
import { SubjectListPage } from './view/SubjectListPage';

// Materias: rutas y entradas del menú.
export const SubjectsRoutes: ModuleRoutes = {
  Name: 'Subjects',
  Menu: [
    { Id: 'subjects', Path: '/subjects', LabelKey: 'Nav.Subjects', Icon: 'Subjects', Permission: 'Subjects.View' },
  ],
  Private: (
    <>
      <Route path="subjects" element={<SubjectListPage />} />
      <Route path="subjects/new" element={<SubjectFormPage />} />
      <Route path="subjects/:Id" element={<SubjectFormPage />} />
    </>
  ),
};
