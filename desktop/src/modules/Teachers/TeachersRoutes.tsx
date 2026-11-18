import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { TeacherFormPage } from './view/TeacherFormPage';
import { TeacherListPage } from './view/TeacherListPage';

// Docentes: rutas y entradas del menú.
export const TeachersRoutes: ModuleRoutes = {
  Name: 'Teachers',
  Menu: [
    { Id: 'teachers', Path: '/teachers', LabelKey: 'Nav.Teachers', Icon: 'Teachers', Permission: 'Teachers.View' },
  ],
  Private: (
    <>
      <Route path="teachers" element={<TeacherListPage />} />
      <Route path="teachers/new" element={<TeacherFormPage />} />
      <Route path="teachers/:Id" element={<TeacherFormPage />} />
    </>
  ),
};
