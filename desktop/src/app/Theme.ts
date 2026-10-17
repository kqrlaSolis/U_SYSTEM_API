import type { ITThemePalette } from '@axzydev/axzy_ui_system';

// Paleta de U SYSTEM, tomada del logo: azul marino + verde menta.
//   Azul marino (icono oscuro / texto):  #26344F · #1F2C45 · #0D2850
//   Verde menta (birrete y U):           #92BBA0 · #96D3B4
export const USystemTheme: Partial<ITThemePalette> = {
  primary: '#243556',
  secondary: '#7DB394',
  ternary: '#EAF3EE',
  danger: '#DC2626',
  success: '#16A34A',
  info: '#0EA5E9',
  alert: '#F97316',
  warning: '#F59E0B',
  layout: {
    sidebarBg: '#F3F6F5',
    sidebarText: '#26344F',
    navbarBg: '#FFFFFF',
    navbarText: '#1F2C45',
  },
  table: {
    headerBg: '#F1F5F4',
    headerText: '#3B4A66',
    rowBg: '#FFFFFF',
    rowText: '#1F2C45',
    rowHover: '#EEF4F1',
  },
};

// Misma identidad sobre el fondo azul marino del icono oscuro
export const USystemDarkTheme: Partial<ITThemePalette> = {
  primary: '#96D3B4',
  secondary: '#92BBA0',
  ternary: '#26344F',
  danger: '#F87171',
  success: '#4ADE80',
  info: '#38BDF8',
  alert: '#FB923C',
  warning: '#FBBF24',
  layout: {
    sidebarBg: '#18233A',
    sidebarText: '#CBD5E1',
    navbarBg: '#1F2C45',
    navbarText: '#F1F5F9',
  },
  table: {
    headerBg: '#26344F',
    headerText: '#CBD5E1',
    rowBg: '#1F2C45',
    rowText: '#F1F5F9',
    rowHover: '#2A3A58',
  },
};

// Tema por defecto de la aplicación: el claro
export const USystemDefaultTheme = USystemTheme;

// Configuración de producción del provider (ver docs/AxzyUiSystem/AXZY-UI-SYSTEM.md).
// appearance="flat" ⇒ bordes finos sin sombras; showFab=false ⇒ sin diseñador de temas flotante.
export const USystemAppearance = 'flat' as const;
export const USystemRadius = 10;
export const USystemShadow = 1;
export const USystemDensity = 1;

// El provider arranca en "system"; U SYSTEM trabaja en modo claro salvo que el usuario lo cambie.
export const DarkModeStorageKey = 'it-theme-dark-mode';
export const DefaultDarkMode = 'light';

// Se ejecuta antes de montar: ITThemeProvider lee su modo de localStorage al inicializar.
export function ApplyDefaultDarkMode(): void {
  try {
    if (window.localStorage.getItem(DarkModeStorageKey) === null) {
      window.localStorage.setItem(DarkModeStorageKey, DefaultDarkMode);
    }
  } catch {
    // Sin almacenamiento disponible se mantiene el default del provider
  }
}

// Elige la paleta según el modo guardado (claro por defecto y "system" respeta el SO).
export function ResolveTheme(): Partial<ITThemePalette> {
  try {
    const Stored = window.localStorage.getItem(DarkModeStorageKey);
    const PrefersDark =
      typeof window.matchMedia === 'function' &&
      window.matchMedia('(prefers-color-scheme: dark)').matches;
    const IsDark = Stored === 'dark' || ((Stored === null || Stored === 'system') && PrefersDark);
    return IsDark ? USystemDarkTheme : USystemTheme;
  } catch {
    return USystemTheme;
  }
}
