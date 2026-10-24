import { cleanup, render } from '@testing-library/react';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { ThemeWrapper } from '../test/RenderWithTheme';
import { ApplyDefaultDarkMode, DarkModeStorageKey, USystemRadius, USystemTheme } from './Theme';

// El provider vive en main.tsx; estas pruebas cubren la configuración y su efecto en <html>.
describe('Tema de U SYSTEM', () => {
  beforeEach(() => {
    window.localStorage.clear();
  });

  afterEach(() => {
    cleanup();
  });

  it('fija el modo claro por defecto antes de montar', () => {
    ApplyDefaultDarkMode();

    expect(window.localStorage.getItem(DarkModeStorageKey)).toBe('light');
  });

  it('respeta la preferencia de modo que ya eligió el usuario', () => {
    window.localStorage.setItem(DarkModeStorageKey, 'dark');

    ApplyDefaultDarkMode();

    expect(window.localStorage.getItem(DarkModeStorageKey)).toBe('dark');
  });

  it('aplica paleta, apariencia plana y radio global en <html>', () => {
    render(
      <ThemeWrapper>
        <div>contenido</div>
      </ThemeWrapper>,
    );

    const Root = document.documentElement;

    expect(Root.getAttribute('data-it-appearance')).toBe('flat');
    expect(Root.style.getPropertyValue('--color-primary')).toBe(USystemTheme.primary);
    expect(Root.style.getPropertyValue('--radius-md')).toBe(`${USystemRadius}px`);
    expect(Root.style.getPropertyValue('--it-density')).toBe('1');
  });

  it('arranca en modo claro cuando no hay preferencia guardada', () => {
    render(
      <ThemeWrapper>
        <div>contenido</div>
      </ThemeWrapper>,
    );

    expect(document.documentElement.classList.contains('dark')).toBe(false);
    expect(document.documentElement.getAttribute('data-theme')).toBe('light');
  });
});
