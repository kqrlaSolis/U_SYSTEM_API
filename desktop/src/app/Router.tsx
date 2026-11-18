import { Fragment } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { RequireAuth } from '../shared/auth/RequireAuth';
import { MainLayout } from './layout/MainLayout';
import { AppRoutes } from './AppRoutes';

// Rutas en minúsculas con guiones. Cada módulo declara las suyas en modules/<Module>/<Module>Routes.tsx:
// las públicas van fuera del layout y el resto exige sesión.
export function Router() {
  return (
    <Routes>
      {AppRoutes.map((Module) => (
        <Fragment key={Module.Name}>{Module.Public}</Fragment>
      ))}

      <Route
        element={
          <RequireAuth>
            <MainLayout />
          </RequireAuth>
        }
      >
        {AppRoutes.map((Module) => (
          <Fragment key={Module.Name}>{Module.Private}</Fragment>
        ))}
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
