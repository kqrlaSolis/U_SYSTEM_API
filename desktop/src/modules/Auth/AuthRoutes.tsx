import { Route } from 'react-router-dom';
import type { ModuleRoutes } from '../../app/ModuleRoutes';
import { ChangePasswordPage } from './view/ChangePasswordPage';
import { ForgotPasswordPage } from './view/ForgotPasswordPage';
import { LoginPage } from './view/LoginPage';
import { ProfilePage } from './view/ProfilePage';
import { ResetPasswordPage } from './view/ResetPasswordPage';

// Sesión, perfil y recuperación de contraseña: rutas (no tiene entrada en el menú).
export const AuthRoutes: ModuleRoutes = {
  Name: 'Auth',
  Private: (
    <>
      <Route path="profile" element={<ProfilePage />} />
      <Route path="change-password" element={<ChangePasswordPage />} />
    </>
  ),
  Public: (
    <>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />
    </>
  ),
};
