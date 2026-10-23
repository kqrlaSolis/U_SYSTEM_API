# U SYSTEM · Desktop

Cliente de escritorio de U SYSTEM: **Electron + React + TypeScript + Vite + AXZY UI System**, con
`react-i18next` y arquitectura **MVVM**. Es la Fase 0 (cimientos): cascarón, router, cliente HTTP,
i18n, envoltorios de UI y pantalla de login preparada para la API (todavía sin backend).

## Requisitos

- Node 22 · npm 10
- Windows, Linux o macOS

## Cómo correr

```powershell
npm install
Copy-Item .env.example .env      # VITE_API_BASE_URL=http://localhost:5000
npm run dev                      # Vite en :5173 + Electron apuntando a ese servidor
```

> `npm run dev` no se puede ejecutar en un entorno sin pantalla; para comprobar la compilación usa
> `npm run build`.

Otros scripts:

| Script | Qué hace |
|---|---|
| `npm run build` | `tsc --noEmit` + `vite build` (genera `dist/`) |
| `npm run build:electron` | compila `electron/` a `dist-electron/` (CommonJS) |
| `npm run start` | compila Electron y lo abre contra `dist/` (modo producción local) |
| `npm test` | Vitest + jsdom |
| `npm run lint` | ESLint (flat config) |
| `npm run dist:win` / `dist:linux` / `dist:mac` | suben la versión y generan el instalador con electron-builder en `release/` |
| `npm run version:bump` | sube la versión patch de `package.json` (acepta `--minor`, `--major`, `--dry-run`) |
| `npm run icons` | regenera `build/icon.png` y `build/icon.ico` desde el SVG de marca |

En desarrollo, Electron carga `http://localhost:5173` cuando existe `VITE_DEV_SERVER_URL`; en
producción carga `dist/index.html` con `file://` (por eso `base: './'` en Vite y `HashRouter`).

### Versionado

Cada `dist:*` ejecuta `npm run version:bump` **antes** de compilar, así que el instalador sale con una
versión nueva (`0.1.0` → `0.1.1` → …) y el nombre del artefacto la incluye
(`U SYSTEM-0.1.1-setup.exe`). La versión vive solo en `package.json` y la app la muestra en el inicio
(`app.getVersion()`).

```bash
npm run dist:win                        # 0.1.1 -> 0.1.2 y empaqueta 0.1.2
npm run version:bump -- --minor         # 0.1.2 -> 0.2.0 (manual)
npm run version:bump -- --major         # 1.0.0 (manual)
npm run version:bump -- --dry-run       # muestra el siguiente número sin escribir
```

> Si la compilación falla después del bump, el número queda consumido; se puede corregir a mano en
> `package.json`.


## AXZY UI System: Tailwind v4 e `ITThemeProvider`

La receta oficial está en `docs/AxzyUiSystem/AXZY-UI-SYSTEM.md` (§2 puesta en marcha y §3 theming).
En este proyecto se aplica así:

1. **Tailwind v4 por Vite** (`vite.config.ts`): `@tailwindcss/vite` en `plugins` (junto a
   `@vitejs/plugin-react`). Sin `tailwind.config.js`: la configuración es CSS.
2. **Capas de CSS** (`src/styles/Global.css`): el orden se declara antes de los imports y el CSS de la
   librería entra en su propia capa, entre `base` y `utilities`:

   ```css
   @layer theme, base, axzy-ui-system, components, utilities;
   @import 'tailwindcss';
   @import '@axzydev/axzy_ui_system/dist/index.css' layer(axzy-ui-system);
   @variant dark (&:is(.dark &));
   ```

3. **Un solo `ITThemeProvider`** en `src/main.tsx`, envolviendo toda la app con la paleta institucional de
   `src/app/Theme.ts` (`primary: #14507A`, bloques `layout` y `table`), `appearance="flat"`,
   `radius={10}`, `shadow={1}`, `density={1}` y `showFab={false}` (sin diseñador de temas en producción).
4. **Modo claro por defecto**: `ApplyDefaultDarkMode()` escribe `it-theme-dark-mode = "light"` antes de
   montar, sin pisar la preferencia que el usuario ya haya guardado.
5. **Pruebas**: `src/test/RenderWithTheme.tsx` expone `ThemeWrapper` y `RenderWithTheme` para montar
   componentes de AXZY UI con el mismo provider que `main.tsx`.

> Si agregas estilos propios, usa utilidades de Tailwind o `var(--it-*)`; evita colores hardcodeados
> (regla del catálogo, `docs/AxzyUiSystem/AXZY-UI-SYSTEM.md` §3).

## Estructura

```
desktop/
├── electron/            Main.ts (BrowserWindow, menú, IPC), Preload.ts, Types.d.ts
├── scripts/             Manifiesto CommonJS de dist-electron
└── src/
    ├── main.tsx         ITThemeProvider (paleta, apariencia, radio, densidad) + Providers + Router
    ├── app/             Theme.ts (paleta y defaults), Providers.tsx, Router.tsx,
    │                    layout/(MainLayout con ITLayout, NavigationModules, NavigationIcons,
    │                    useNavigationItems)
    ├── modules/
    │   ├── Auth/        login, perfil, cambio de contraseña (model/ viewmodel/ view/)
    │   ├── Home/        view/HomePage.tsx
    │   ├── Roles/       model/Role.ts, RolesService.ts
    │   ├── Settings/    model/ viewmodel/ view/  (configuración)
    │   └── Users/       model/ viewmodel/ view/  (listado y formulario)
    ├── shared/
    │   ├── auth/        AuthContext.tsx, RequireAuth.tsx, AuthPaths.ts
    │   ├── components/  envoltorios PascalCase sobre AXZY UI + index.ts
    │   ├── hooks/       usePermissions.ts, usePagedQuery.ts
    │   ├── http/        HttpClient.ts, ApiResponse.ts, ApiErrorException.ts, GetErrorMessage.ts
    │   ├── i18n/        Config.ts, Es.ts, En.ts, I18nProvider.tsx
    │   └── utils/       Format.ts
    ├── test/            Setup.ts (jsdom), RenderWithTheme.tsx
    ├── assets/
    │   ├── brand/      UsystemBanner.svg, UsystemBannerAlt.svg, UsystemIcon.svg, UsystemIconAlt.svg
    │   └── lottie/     UsystemBanner.json, UsystemBannerAlt.json, UsystemIcon.json, UsystemIconAlt.json
    ├── public/          favicon.svg (copia del icono de marca)
    └── styles/          Global.css (capas + Tailwind v4 + CSS de AXZY UI)
```

## Reglas del proyecto

- **MVVM**: la View solo usa su ViewModel; el ViewModel no tiene JSX ni conoce AXZY UI; el Model no
  conoce React. Ver `docs/Architecture/DesktopMvvm.md`.
- **Nombres**: todo el código en inglés y PascalCase (variables, funciones, constantes, tipos,
  archivos). Excepciones: hooks `useAlgo`, props de React/AXZY UI, funciones de librería y variables
  de entorno (`VITE_API_BASE_URL`). Los comentarios van en español.
- **Rutas**: minúsculas con guiones (`/users`, `/login`).
- **i18n**: ningún texto visible hardcodeado; las claves son PascalCase (`Users.List.Title`) y `En.ts`
  debe cumplir el tipo `TranslationCatalog` de `Es.ts`. El idioma se guarda en
  `localStorage['USystem.Language']` (fallback `es`).
- **HTTP**: `HttpClient` añade `Accept-Language`, `Authorization: Bearer`, reintenta **una vez** con el
  refresh token en un 401, aplica timeout con `AbortController` y convierte los errores en
  `ApiErrorException` con `Message` localizado.
- **AXZY UI**: solo se usan componentes del catálogo (`docs/AxzyUiSystem/`); los de `shared/components`
  son envoltorios con props PascalCase que traducen a la API real (`label`, `onClick`, `value`…).
  El tema y el CSS de la librería se configuran una sola vez (`main.tsx` + `styles/Global.css`).
- **Chasis `ITLayout` + `ITPage`**: `app/layout/MainLayout.tsx` configura **un único** `ITLayout`
  (`topBar` + `sidebar` con el menú por permisos) y cada pantalla autenticada es un `ITPage` con su
  título, acciones y estados `loading` / `error` / `empty`. No se crean cabeceras de página propias
  (nada de `PageHeader`, `<h1>` de página ni `div` con padding propio). El login es la única pantalla
  fuera del chasis por ser pública.

## Identidad visual

La paleta sale del logo y vive en `app/Theme.ts` (`USystemTheme` claro y `USystemDarkTheme` oscuro):

| Token | Valor | Uso |
|---|---|---|
| `primary` | `#243556` | azul marino del logo: acciones, títulos, marca |
| `secondary` | `#7DB394` | verde menta del logo (más profundo para contraste) |
| `ternary` | `#EAF3EE` | tinte menta para fondos y resaltados |
| `layout.sidebarBg` | `#F3F6F5` | barra lateral clara |
| `table.headerBg` | `#F1F5F4` | encabezado de tablas |

- `main.tsx` monta **un único** `ITThemeProvider` con `ResolveTheme()`: usa la paleta clara por
  defecto y la oscura si `localStorage['it-theme-dark-mode']` es `dark` (o `system` con el SO en oscuro).
- Recursos de marca: `assets/brand/*.svg` (estáticos) y `assets/lottie/*.json` (animaciones).
  El login usa `UsystemBannerAlt.svg`, la barra lateral y el favicon `UsystemIcon.svg`, y el inicio
  usa `UsystemBanner.svg` + la animación `UsystemBanner.json` mediante `shared/components/LottieAnimation`.
- `lottie-react` se separa en su propio chunk (`vite.config.ts` → `manualChunks.Lottie`) para no inflar
  el arranque; en Vitest se reemplaza por un nodo vacío porque jsdom no implementa canvas.
- **Iconos del instalador**: `npm run icons` regenera `build/icon.png` (512×512) y `build/icon.ico`
  (16, 24, 32, 48, 64, 128 y 256) a partir de `assets/brand/UsystemIcon.svg` con `sharp` + `png-to-ico`.
  `electron-builder.yml` los usa en `win.icon`, `nsis.installerIcon`/`uninstallerIcon`/`installerHeaderIcon`
  y `linux`/`mac`; la ventana de Electron toma `build/icon.png` en desarrollo y `resources/icon.png` en la
  app empaquetada (`extraResources`).

## Estado de la Fase 0

- [x] Chasis `ITLayout` + `ITPage` con menú construido desde una lista de módulos con permiso requerido.
- [x] Login con validación, errores traducidos, estado de carga y Enter para enviar.
- [x] `HttpClient` con interceptores de idioma y de token + refresh en 401.
- [x] Catálogos es/en con paridad garantizada por el compilador.
- [x] Listado de usuarios (DataTable + SearchBox + paginación + permisos) contra `/api/v1/users`.
- [x] Instalación completa de AXZY UI: Tailwind v4, capas de CSS y `ITThemeProvider` en `main.tsx`.
- [ ] `safeStorage` para el refresh token (hoy va en `localStorage`, ver `AuthStorage.ts`).

## Fase 1: pantallas conectadas a la API

- [x] Login real contra `/api/v1/auth/login` y refresh en 401.
- [x] **Perfil** (`/profile`) contra `/auth/me`: datos, roles y permisos vigentes.
- [x] **Cambio de contraseña** (`/change-password`): obligatorio en el primer ingreso, validado por la API.
- [x] **Usuarios**: listado y formulario (`/users/new`, `/users/:Id`) con roles, estado y contraseña temporal.
- [x] **Configuración** (`/settings`): parámetros por categoría con guardado por fila.
- [x] **Carreras** y **Materias** (`/careers`, `/subjects`): CRUD completo sobre el CRUD genérico de `shared/crud`.
- [x] **Roles** (`/roles`): listado y editor de permisos agrupado por módulo (con selección por módulo completo).
- [x] **Bitácora** (`/audit-logs`): filtros por módulo, acción y fechas; detalle con valores antes/después.
- [x] **Periodos** (/academic-terms): calendario, abrir/cerrar y periodo en curso.
- [x] **Planes de estudio** (/study-plans): datos, malla por cuatrimestre, seriación y marcar vigente.
- [x] **Recuperación de contraseña**: /forgot-password y /reset-password con el token de la API.

> Los catálogos CRUD se declaran en `modules/<Módulo>/model/<Módulo>Config.ts` (columnas, campos y
> servicios) y las pantallas son `CrudListPage` / `CrudFormPage` de `shared/crud`: el patrón MVVM se
> mantiene (Model en el config+servicio, ViewModel en `useCrudListViewModel`/`useCrudFormViewModel`,
> View en las páginas).
