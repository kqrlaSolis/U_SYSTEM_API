import { ITLayout } from '@axzydev/axzy_ui_system';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Outlet, useNavigate } from 'react-router-dom';
import LogoUrl from '../../assets/brand/UsystemIcon.svg';
import { useAuth } from '../../shared/auth/AuthContext';
import { ProfilePath } from '../../shared/auth/AuthPaths';
import { AppButton } from '../../shared/components';
import { ChangeLanguage, NormalizeLanguage } from '../../shared/i18n/Config';
import { useNavigationItems } from './useNavigationItems';

// Chasis único de la aplicación autenticada: ITLayout aporta barra superior,
// menú lateral y área de contenido; las pantallas solo aportan su ITPage.
export function MainLayout() {
  const Auth = useAuth();
  const Translation = useTranslation();
  const Navigator = useNavigate();
  const NavigationItems = useNavigationItems();
  const Language = NormalizeLanguage(Translation.i18n.language);
  const CurrentUser = Auth.CurrentUser;
  const [IsCollapsed, SetIsCollapsed] = useState(false);

  const ToggleLanguage = () => {
    void ChangeLanguage(Language === 'en' ? 'es' : 'en');
  };

  const HandleSignOut = async () => {
    await Auth.SignOut();
    Navigator('/login', { replace: true });
  };

  return (
    <ITLayout
      topBar={{
        // Icono grande de la marca al extremo izquierdo de la barra superior
        logo: <img className="USystemNavbarLogo" src={LogoUrl} alt="" width={40} height={40} />,
        // logoText: Translation.t('Common.AppName'),
        userMenu: {
          userName: CurrentUser?.FullName ?? Translation.t('Common.AppName'),
          userEmail: CurrentUser?.Email ?? '',
          menuItems: [
            {
              label: Translation.t('Common.Profile'),
              onClick: () => Navigator(ProfilePath),
            },
            {
              label: Translation.t('Common.Logout'),
              onClick: () => {
                void HandleSignOut();
              },
            },
          ],
        },
        // Acciones de la barra superior: cambio de idioma
        children: (
          <AppButton
            Variant="filled"
            Size="sm"
            Label={Language === 'es' ? 'EN' : 'ES'}
            AriaLabel={Translation.t('Common.Language')}
            OnClick={ToggleLanguage}
          />
        ),
      }}
      sidebar={{
        navigationItems: NavigationItems,
        // El colapso lo controla la app para ajustar el resaltado del icono
        isCollapsed: IsCollapsed,
        onToggleCollapse: () => SetIsCollapsed((Current) => !Current),
        expandOnHover: true,
        className: IsCollapsed ? 'USystemSidebar USystemSidebarCollapsed' : 'USystemSidebar',
      }}
    >
      <Outlet />
    </ITLayout>
  );
}
