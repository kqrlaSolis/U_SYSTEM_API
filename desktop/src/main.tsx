import { ITThemeProvider } from '@axzydev/axzy_ui_system';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { Providers } from './app/Providers';
import { Router } from './app/Router';
import {
  ApplyDefaultDarkMode,
  ResolveTheme,
  USystemAppearance,
  USystemDensity,
  USystemRadius,
  USystemShadow,
} from './app/Theme';
import './styles/Global.css';

const RootElement = document.getElementById('root');

if (RootElement === null) {
  throw new Error('RootElementIsMissing');
}

// Debe correr antes de renderizar: el provider lee su modo al inicializar.
ApplyDefaultDarkMode();

createRoot(RootElement).render(
  <StrictMode>
    {/* Un único ITThemeProvider en la raíz: paleta del logo, apariencia, radio, sombras y densidad. */}
    <ITThemeProvider
      theme={ResolveTheme()}
      appearance={USystemAppearance}
      radius={USystemRadius}
      shadow={USystemShadow}
      density={USystemDensity}
      showFab={false}
    >
      <Providers>
        <Router />
      </Providers>
    </ITThemeProvider>
  </StrictMode>,
);
