import { ITLoader } from '@axzydev/axzy_ui_system';
import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { ChangePasswordPath, LoginPath } from './AuthPaths';

export interface RequireAuthProps {
  children: ReactNode;
}

// Guarda de rutas: exige sesión, espera la restauración y fuerza el cambio de
// contraseña del primer ingreso antes de dejar entrar al resto de la app.
export function RequireAuth({ children }: RequireAuthProps) {
  const Auth = useAuth();
  const Location = useLocation();

  if (Auth.IsLoading) {
    return (
      <div className="USystemCentered">
        <ITLoader size="lg" />
      </div>
    );
  }

  if (!Auth.IsAuthenticated) {
    return <Navigate to={LoginPath} replace state={{ From: Location.pathname }} />;
  }

  if (Auth.MustChangePassword && Location.pathname !== ChangePasswordPath) {
    return <Navigate to={ChangePasswordPath} replace />;
  }

  return <>{children}</>;
}
