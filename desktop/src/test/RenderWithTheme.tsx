import { ITThemeProvider } from '@axzydev/axzy_ui_system';
import { render } from '@testing-library/react';
import type { ReactElement, ReactNode } from 'react';
import {
  USystemAppearance,
  USystemDensity,
  USystemRadius,
  USystemShadow,
  USystemTheme,
} from '../app/Theme';

// Mismo provider que main.tsx: sin él, los componentes de AXZY UI no tienen tema.
export function ThemeWrapper({ children }: { children: ReactNode }) {
  return (
    <ITThemeProvider
      theme={USystemTheme}
      appearance={USystemAppearance}
      radius={USystemRadius}
      shadow={USystemShadow}
      density={USystemDensity}
      showFab={false}
    >
      {children}
    </ITThemeProvider>
  );
}

export function RenderWithTheme(Ui: ReactElement) {
  return render(<ThemeWrapper>{Ui}</ThemeWrapper>);
}
