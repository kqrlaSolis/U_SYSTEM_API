import { screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { MemoryRouter } from 'react-router-dom';
import { AuthProvider } from '../../shared/auth/AuthContext';
import { I18nProvider } from '../../shared/i18n/I18nProvider';
import { RenderWithTheme } from '../../test/RenderWithTheme';
import { MainLayout } from './MainLayout';

// Humo del chasis: ITLayout pinta la barra superior y el menú construido desde los módulos.
describe('MainLayout', () => {
  it('pinta el menú de módulos sin sesión', () => {
    RenderWithTheme(
      <I18nProvider>
        <AuthProvider>
          <MemoryRouter>
            <MainLayout />
          </MemoryRouter>
        </AuthProvider>
      </I18nProvider>,
    );

    // El nombre aparece en la barra superior y en la marca del menú lateral
    expect(screen.getAllByText('U SYSTEM').length).toBeGreaterThan(0);
    expect(screen.getByText('Inicio')).toBeTruthy();
  });
});
