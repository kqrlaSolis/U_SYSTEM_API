import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { UserFormPage } from './view/UserFormPage';
import { UserListPage } from './view/UserListPage';

// Usuarios: rutas y entradas del menú.
export const UsersRoutes: ModuleRoutes = {
  Name: 'Users',
  Menu: [
    { Id: 'users', Path: '/users', LabelKey: 'Nav.Users', Icon: 'Users', Permission: 'Users.View' },
  ],
  Private: (
    <>
      <Route path="users" element={<UserListPage />} />
      <Route path="users/new" element={<UserFormPage />} />
      <Route path="users/:Id" element={<UserFormPage />} />
    </>
  ),
};
