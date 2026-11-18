# U SYSTEM · Desktop

Cliente de escritorio de U SYSTEM: **Electron + React + TypeScript + Vite + AXZY UI System**, con
`react-i18next` y patrón **MVVM**. Las reglas del patrón, el chasis de pantallas y la estructura de
carpetas están en [`docs/Architecture/Desktop.md`](../docs/Architecture/Desktop.md).

## Requisitos

- Node 22 · npm 10
- La API corriendo (por defecto en `http://localhost:5000`)

## Cómo correr

```powershell
npm install
Copy-Item .env.example .env      # VITE_API_BASE_URL=http://localhost:5000
npm run dev                      # Vite en :5173 + Electron apuntando a ese servidor
```

En desarrollo, Electron carga `http://localhost:5173`; en producción carga `dist/index.html` con
`file://` (por eso `base: './'` en Vite y `HashRouter`).

| Script | Qué hace |
|---|---|
| `npm run dev` | Vite + Electron con recarga en caliente |
| `npm run build` | `tsc --noEmit` + `vite build` (genera `dist/`) |
| `npm run build:electron` | Compila `electron/` a `dist-electron/` (CommonJS) |
| `npm run start` | Abre Electron contra `dist/` (modo producción local) |
| `npm test` | Pruebas con Vitest + jsdom |
| `npm run lint` | ESLint |
| `npm run dist:win` / `dist:linux` / `dist:mac` | Sube la versión y genera el instalador en `release/` |
| `npm run version:bump` | Sube la versión patch (acepta `--minor`, `--major`, `--dry-run`) |
| `npm run icons` | Regenera `build/icon.png` y `build/icon.ico` desde `src/assets/brand/UsystemIcon.svg` |

## Versionado

Cada `dist:*` ejecuta `version:bump` **antes** de compilar, así que cada instalador sale con una versión
nueva (`U SYSTEM-0.1.4-setup.exe`). La versión vive solo en `package.json`.

```bash
npm run version:bump -- --minor         # 0.1.3 -> 0.2.0
npm run version:bump -- --dry-run       # muestra el siguiente número sin escribir
```

> Si la compilación falla después del bump, el número queda consumido; se corrige a mano en
> `package.json`.

## AXZY UI System y Tailwind v4

La documentación de la librería viene con el paquete:
`node_modules/@axzydev/axzy_ui_system/README.md`. En este proyecto se configura así:

1. **Tailwind v4 por Vite**: `@tailwindcss/vite` en `vite.config.ts`, sin `tailwind.config.js`.
2. **Capas de CSS** en `src/styles/Global.css`; el CSS de la librería entra en su propia capa:

   ```css
   @layer theme, base, axzy-ui-system, components, utilities;
   @import 'tailwindcss';
   @import '@axzydev/axzy_ui_system/dist/index.css' layer(axzy-ui-system);
   ```

3. **Un solo `ITThemeProvider`** en `src/main.tsx`, con la paleta de `src/app/Theme.ts`,
   `appearance="flat"` y `showFab={false}`.
4. **Modo claro por defecto**: `ApplyDefaultDarkMode()` escribe `it-theme-dark-mode = "light"` antes de
   montar, sin pisar la preferencia que el usuario ya haya guardado.
5. **Pruebas**: `src/test/RenderWithTheme.tsx` monta componentes con el mismo provider que `main.tsx`.

Para estilos propios usa utilidades de Tailwind o `var(--it-*)`; evita colores fijos.

## Identidad visual

La paleta sale del logo y vive en `src/app/Theme.ts` (`USystemTheme` claro y `USystemDarkTheme` oscuro):

| Token | Valor | Uso |
|---|---|---|
| `primary` | `#243556` | Azul marino del logo: acciones, títulos, marca |
| `secondary` | `#7DB394` | Verde menta del logo |
| `ternary` | `#EAF3EE` | Tinte menta para fondos y resaltados |
| `layout.sidebarBg` | `#F3F6F5` | Barra lateral |
| `table.headerBg` | `#F1F5F4` | Encabezado de tablas |

Recursos de marca en `src/assets/brand/`: el login usa `UsystemBannerAlt.svg`; la barra lateral, el
favicon (`public/favicon.svg`) y los iconos del instalador usan `UsystemIcon.svg`.
