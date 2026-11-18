import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { ClassGroupFormPage } from './view/ClassGroupFormPage';
import { ClassGroupListPage } from './view/ClassGroupListPage';

// Grupos y horarios: rutas y entradas del menú.
export const ClassGroupsRoutes: ModuleRoutes = {
  Name: 'ClassGroups',
  Menu: [
    { Id: 'class-groups', Path: '/class-groups', LabelKey: 'Nav.ClassGroups', Icon: 'ClassGroups', Permission: 'ClassGroups.View' },
  ],
  Private: (
    <>
      <Route path="class-groups" element={<ClassGroupListPage />} />
      <Route path="class-groups/new" element={<ClassGroupFormPage />} />
      <Route path="class-groups/:Id" element={<ClassGroupFormPage />} />
    </>
  ),
};
