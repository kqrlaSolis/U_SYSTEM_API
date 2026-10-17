import type { ReactNode } from 'react';
import { HashRouter } from 'react-router-dom';
import { AuthProvider } from '../shared/auth/AuthContext';
import { I18nProvider } from '../shared/i18n/I18nProvider';

export interface ProvidersProps {
  // `children` es la prop nativa de React: no se renombra.
  children: ReactNode;
}

// Providers de aplicación (i18n + sesión + router). El tema de AXZY UI vive en main.tsx,
// igual que en la receta oficial: un solo ITThemeProvider cerca de la raíz.
// Se usa HashRouter y no BrowserRouter: en producción Electron carga el renderer
// con file:// y no hay servidor que resuelva rutas profundas; con hash la ruta
// vive en el fragmento de la URL y una recarga sigue funcionando.
export function Providers({ children }: ProvidersProps) {
  return (
    <I18nProvider>
      <AuthProvider>
        <HashRouter>{children}</HashRouter>
      </AuthProvider>
    </I18nProvider>
  );
}
