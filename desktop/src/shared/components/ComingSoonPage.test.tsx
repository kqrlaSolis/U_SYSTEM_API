import { screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { I18nProvider } from '../../shared/i18n/I18nProvider';
import { RenderWithTheme } from '../../test/RenderWithTheme';
import { ComingSoonPage } from './ComingSoonPage';

// Pantalla de tránsito: valida que ITPage pinte cabecera y estado vacío.
describe('ComingSoonPage', () => {
  it('muestra el título de la ruta y el estado de módulo en construcción', () => {
    RenderWithTheme(
      <I18nProvider>
        <ComingSoonPage TitleKey="Users.List.New" />
      </I18nProvider>,
    );

    expect(screen.getByText('Nuevo usuario')).toBeTruthy();
    expect(screen.getAllByText('Módulo en construcción').length).toBeGreaterThan(0);
  });
});
