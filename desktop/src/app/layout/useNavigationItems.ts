import type { ITNavigationItem } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router-dom';
import { usePermissions } from '../../shared/auth/usePermissions';
import { GetNavigationIcon } from './NavigationIcons';
import { GetNavigationModules } from './NavigationModules';

// Ítems del menú lateral: módulos visibles según permiso y ruta activa.
// Se pasan como `sidebar.navigationItems` del ITLayout.
export function useNavigationItems(): ITNavigationItem[] {
  const Translation = useTranslation();
  const Permissions = usePermissions();
  const Location = useLocation();
  const Navigator = useNavigate();

  return GetNavigationModules().filter(
    (Module) => Module.Permission === null || Permissions.Can(Module.Permission),
  ).map((Module) => ({
    id: Module.Id,
    label: Translation.t(Module.LabelKey),
    icon: GetNavigationIcon(Module.Icon),
    isActive:
      Module.Path === '/'
        ? Location.pathname === '/'
        : Location.pathname.startsWith(Module.Path),
    action: () => Navigator(Module.Path),
  }));
}
