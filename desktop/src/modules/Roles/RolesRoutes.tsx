import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { RoleFormPage } from './view/RoleFormPage';
import { RoleListPage } from './view/RoleListPage';

// Roles y permisos: rutas y entradas del menú.
export const RolesRoutes: ModuleRoutes = {
  Name: 'Roles',
  Menu: [
    { Id: 'roles', Path: '/roles', LabelKey: 'Nav.Roles', Icon: 'Roles', Permission: 'Roles.View' },
  ],
  Private: (
    <>
      <Route path="roles" element={<RoleListPage />} />
      <Route path="roles/new" element={<RoleFormPage />} />
      <Route path="roles/:Id" element={<RoleFormPage />} />
    </>
  ),
};
