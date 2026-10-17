import { screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { RenderWithTheme } from '../test/RenderWithTheme';
import { Providers } from './Providers';
import { Router } from './Router';

// Humo: monta el árbol real (tema + i18n + auth + router) sin sesión.
describe('App', () => {
  it('redirige al login cuando no hay sesión', async () => {
    RenderWithTheme(
      <Providers>
        <Router />
      </Providers>,
    );

    expect(await screen.findByRole('heading', { name: 'Iniciar sesión' })).toBeTruthy();
    expect(screen.getByText('Entrar')).toBeTruthy();
  });
});
