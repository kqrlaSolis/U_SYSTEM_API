import type { ReactNode } from 'react';
import type { NavigationModule } from './layout/NavigationModules';

// Rutas de un módulo y sus entradas del menú, juntas en modules/<Module>/<Module>Routes.tsx.
// El router y el menú solo recorren la lista de app/AppRoutes.ts.
export interface ModuleRoutes {
  Name: string;
  // Entradas del menú lateral (en el orden en que aparecen)
  Menu?: NavigationModule[];
  // Rutas dentro del layout autenticado (relativas, sin "/" inicial)
  Private?: ReactNode;
  // Rutas públicas, fuera del layout y sin sesión (solo Auth)
  Public?: ReactNode;
}
