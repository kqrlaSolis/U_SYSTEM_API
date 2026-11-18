import { createRoutesFromChildren } from 'react-router-dom';
import { describe, expect, it } from 'vitest';
import { GetNavigationModules } from './layout/NavigationModules';
import { AppRoutes } from './AppRoutes';

// Rutas privadas de un módulo normalizadas como "/ruta" ("/" para la ruta index)
function PrivatePaths(Routes: React.ReactNode): string[] {
  return createRoutesFromChildren(Routes).map((Route) => (Route.index === true ? '/' : `/${Route.path ?? ''}`));
}

describe('AppRoutes', () => {
  it('no repite nombres de módulo ni ids del menú', () => {
    const Names = AppRoutes.map((Module) => Module.Name);
    const MenuIds = GetNavigationModules().map((Entry) => Entry.Id);

    expect(new Set(Names).size).toBe(Names.length);
    expect(new Set(MenuIds).size).toBe(MenuIds.length);
  });

  it('cada entrada del menú apunta a una ruta de su propio módulo', () => {
    for (const Module of AppRoutes) {
      const Paths = PrivatePaths(Module.Private);
      for (const Entry of Module.Menu ?? []) {
        expect(Paths, `${Module.Name}: ${Entry.Path}`).toContain(Entry.Path);
      }
    }
  });

  it('no declara la misma ruta en dos módulos', () => {
    const Paths = AppRoutes.flatMap((Module) => PrivatePaths(Module.Private));

    expect(new Set(Paths).size).toBe(Paths.length);
  });
});
