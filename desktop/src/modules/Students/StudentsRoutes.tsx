import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { StudentFormPage } from './view/StudentFormPage';
import { StudentListPage } from './view/StudentListPage';

// Alumnos: rutas y entradas del menú.
export const StudentsRoutes: ModuleRoutes = {
  Name: 'Students',
  Menu: [
    { Id: 'students', Path: '/students', LabelKey: 'Nav.Students', Icon: 'Students', Permission: 'Students.View' },
  ],
  Private: (
    <>
      <Route path="students" element={<StudentListPage />} />
      <Route path="students/new" element={<StudentFormPage />} />
      <Route path="students/:Id" element={<StudentFormPage />} />
    </>
  ),
};
