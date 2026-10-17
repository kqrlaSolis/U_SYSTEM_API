# Desktop: Electron + React TS con MVVM

## Mapeo del patrón

| MVVM | En React + TypeScript | Responsabilidad |
|------|----------------------|-----------------|
| **Model** | Tipos y DTOs (`.ts`) + servicios HTTP (`...Service.ts`) | Datos y comunicación con la API. Sin estado de UI. |
| **ViewModel** | Custom hooks `use...ViewModel` | Estado de la pantalla, validaciones, acciones (guardar, filtrar, paginar), errores y permisos. **Sin JSX.** |
| **View** | Componentes `...Page.tsx` / `...Dialog.tsx` | Renderiza con AXZY UI y delega eventos al ViewModel. **Sin lógica de negocio.** |

```
View  ──usa──►  ViewModel (hook)  ──usa──►  Model (Service + tipos)  ──HTTP──►  API
 ▲                    │
 └──── estado y comandos ────┘
```

## Reglas

1. Una View **solo** consume un ViewModel y componentes de UI; nunca llama a un servicio directamente.
2. El ViewModel **no** importa componentes de UI ni usa JSX.
3. El Model **no** conoce React.
4. Los ViewModels son testeables sin renderizar (hook testing).
5. El permiso se evalúa en el ViewModel (`usePermissions()`); el backend siempre es la autoridad final.
6. Los tipos del Model reflejan el contrato JSON de la API **en PascalCase**.
7. Las funciones, variables y constantes propias van en **PascalCase**; los hooks llevan el prefijo `use`
   que exige React (`useUserListViewModel`).
8. Ningún texto visible hardcodeado: todo sale de `t('Users.List.Title')`. Ver
   `Architecture/Internationalization.md`.
9. El `HttpClient` envía `Accept-Language` y muestra el `Message` localizado que devuelve la API.
10. El tema de AXZY UI se configura **una sola vez** en `desktop/src/main.tsx`: un único
    `ITThemeProvider` con la paleta de `desktop/src/app/Theme.ts`. Ninguna View crea providers ni importa
    el CSS de la librería.
11. Tailwind v4 se procesa con `@tailwindcss/vite` y el orden de capas se declara en
    `desktop/src/styles/Global.css` (`theme, base, axzy-ui-system, components, utilities`). Ver
    `docs/AxzyUiSystem/AXZY-UI-SYSTEM.md` §2.
12. **Chasis único:** `MainLayout.tsx` configura `ITLayout` (`topBar` + `sidebar`) y renderiza `<Outlet />`.
    Ninguna pantalla repite barra superior, menú lateral ni contenedores propios.
13. **Toda pantalla autenticada es un `ITPage`:** título, descripción, acciones, breadcrumbs y los estados
    `loading` / `error` / `empty` salen de la plantilla. No se crea un `PageHeader` propio, ni un `<h1>`
    de página, ni se envuelve la pantalla en un `div` con padding propio. Única excepción: el **login**,
    que vive fuera del chasis por ser público.

## Chasis: `ITLayout` + `ITPage`

```
MainLayout.tsx
└── ITLayout topBar={…} sidebar={…}      ← barra superior, menú por permisos y área de contenido
    └── <Outlet />
        └── NombrePage.tsx
            └── ITPage title description actions loading error empty
                └── contenido (ITStack, ITCard, DataTable…)
```

```tsx
// app/layout/MainLayout.tsx — el chasis se configura una sola vez
<ITLayout
  topBar={{ logoText: t('Common.AppName'), userMenu: { userName, userEmail, menuItems } }}
  sidebar={{ navigationItems: useNavigationItems(), brand: { logo, text: t('Common.AppName') } }}
>
  <Outlet />
</ITLayout>
```

```tsx
// modules/Users/view/UserListPage.tsx — la pantalla solo declara su ITPage
<ITPage
  title={t('Users.List.Title')}
  description={t('Users.List.Subtitle')}
  actions={CanCreate ? <AppButton Label={t('Users.List.New')} OnClick={…} /> : undefined}
  loading={IsLoading && Items.length === 0}
  error={ErrorMessage ?? undefined}
  onRetry={Reload}
>
  <ITStack spacing={4}>…</ITStack>
</ITPage>
```

| Sí | No |
|---|---|
| `ITLayout` en `MainLayout.tsx` (un solo chasis) | Repetir barra superior o menú dentro de una página |
| `ITPage` para cada ruta autenticada | `PageHeader` propio, `<h1>` de página o `div` con padding propio |
| Estados `loading` / `error` / `empty` de `ITPage` | Condicionales manuales que ocultan el contenido |
| `ITStack`/`ITGrid` para separar bloques | Márgenes ad hoc entre tarjetas |

La única pantalla fuera del chasis es `LoginPage` (pública): usa su propia tarjeta centrada con
`ITInput`/`AppButton` y el tema global.

## Estructura

```
desktop/src/
├── main.tsx             ITThemeProvider (tema) + Providers + Router
├── app/
│   ├── Theme.ts         Paleta y defaults del provider
│   ├── Router.tsx
│   ├── Providers.tsx    I18nProvider + AuthProvider + HashRouter
│   └── layout/          MainLayout.tsx (ITLayout), NavigationModules.ts,
│                        NavigationIcons.tsx, useNavigationItems.ts
├── modules/
│   ├── Auth/            login, perfil, cambio de contraseña
│   ├── Careers/         Career.ts, CareersService.ts, CareerConfig.ts + vistas
│   ├── Home/            Panel de inicio
│   ├── Roles/           Role.ts, RolesService.ts
│   ├── Settings/        Parámetros del sistema
│   ├── Subjects/        Subject.ts, SubjectsService.ts, SubjectConfig.ts + vistas
│   └── Users/
│       ├── model/        User.ts, LoginRequest.ts, UsersService.ts
│       ├── viewmodel/    useUserListViewModel.ts, useUserFormViewModel.ts
│       └── view/         UserListPage.tsx, UserFormPage.tsx
├── shared/
│   ├── components/       Envoltorios de AXZY UI (DataTable.tsx, FormField.tsx, ConfirmDialog.tsx)
│   ├── crud/             CrudListPage/CrudFormPage + ViewModels genéricos de catálogos
│   ├── hooks/            usePermissions.ts, usePagedQuery.ts
│   ├── http/             HttpClient.ts (JWT, refresh, Accept-Language, errores), ApiResponse.ts
│   ├── i18n/             I18nProvider.tsx, Config.ts, Es.ts, En.ts
│   └── utils/
├── styles/               Global.css (capas de Tailwind + CSS de AXZY UI)
└── test/                 Setup.ts, RenderWithTheme.tsx
```

> **Catálogos CRUD.** Para un módulo de catálogo (carreras, materias, periodos, planes) se declara un
> `CrudConfig` en `modules/<Módulo>/model/<Módulo>Config.ts` con columnas, campos, permisos y las llamadas
> al servicio; las pantallas son `CrudListPage` y `CrudFormPage` de `shared/crud`. El patrón MVVM se
> mantiene: Model = config + servicio, ViewModel = `useCrudListViewModel`/`useCrudFormViewModel`, View =
> las dos páginas. Las pantallas con lógica propia (roles, bitácora, malla de un plan) usan su propio hook.

## Convenciones de nombres (desktop)

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Tipos, interfaces, componentes, servicios | PascalCase | `User`, `UsersService`, `UserListPage` |
| Hooks / ViewModels | `use` + PascalCase | `useUserListViewModel` |
| Variables y funciones | PascalCase | `PageSize`, `LoadUsers` |
| Constantes | PascalCase (no SCREAMING_SNAKE) | `DefaultPageSize` |
| Archivos | PascalCase igual al export principal | `UserListPage.tsx` |
| Props de nuestros componentes | PascalCase | `Label`, `OnClick`, `Rows` |
| Props de React y de AXZY UI | las que define la librería (no se renombran) | `onClick`, `value`, `key` |
| Rutas de la aplicación | minúsculas con guiones (no son identificadores) | `/users`, `/class-groups` |

## Ejemplo mínimo: listado de usuarios

**Model — `modules/Users/model/User.ts`**

```ts
// Contrato que devuelve la API (PascalCase)
export interface User {
  Id: number;
  FirstName: string;
  PaternalLastName: string;
  Email: string;
  Username: string;
  IsActive: boolean;
}
```

**Model — `shared/http/ApiResponse.ts`**

```ts
// Envoltura estándar de la API
export interface ApiResponse<T> {
  Success: boolean;
  Message: string;
  Data: T;
  Errors: ApiError[];
  TraceId: string | null;
}

export interface ApiError {
  Code: string;
  Field: string;
  Message: string;
}

export interface PagedResult<T> {
  Items: T[];
  TotalCount: number;
  Page: number;
  PageSize: number;
}
```

**Model — `modules/Users/model/UsersService.ts`**

```ts
import { HttpClient } from '../../../shared/http/HttpClient';
import { PagedResult } from '../../../shared/http/ApiResponse';
import { User } from './User';

// Servicio del módulo: sin estado de UI
export const UsersService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<User>> {
    return HttpClient.Get<PagedResult<User>>('/api/v1/users', { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<User> {
    return HttpClient.Get<User>(`/api/v1/users/${Id}`);
  },
};
```

**ViewModel — `modules/Users/viewmodel/useUserListViewModel.ts`**

```ts
import { useCallback, useEffect, useState } from 'react';
import { User } from '../model/User';
import { UsersService } from '../model/UsersService';
import { usePermissions } from '../../../shared/hooks/usePermissions';

const DefaultPageSize = 20;

export function useUserListViewModel() {
  const { Can } = usePermissions();
  const [Items, SetItems] = useState<User[]>([]);
  const [TotalCount, SetTotalCount] = useState(0);
  const [Page, SetPage] = useState(1);
  const [Search, SetSearch] = useState('');
  const [IsLoading, SetIsLoading] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);
    try {
      const Result = await UsersService.GetPaged(Page, DefaultPageSize, Search);
      SetItems(Result.Items);
      SetTotalCount(Result.TotalCount);
    } catch (CaughtError) {
      SetErrorMessage((CaughtError as Error).message);
    } finally {
      SetIsLoading(false);
    }
  }, [Page, Search]);

  useEffect(() => { void Load(); }, [Load]);

  return {
    Items,
    TotalCount,
    Page,
    Search,
    IsLoading,
    ErrorMessage,
    CanCreate: Can('Users.Create'),
    CanEdit: Can('Users.Edit'),
    SetPage,
    SetSearch,
    Reload: Load,
  };
}
```

**View — `modules/Users/view/UserListPage.tsx`**

```tsx
import { ITPage, ITStack } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { AppButton, DataTable, SearchBox } from '../../../shared/components';
import { useUserListViewModel } from '../viewmodel/useUserListViewModel';

export function UserListPage() {
  const ViewModel = useUserListViewModel();
  const { t } = useTranslation();

  return (
    <ITPage
      title={t('Users.List.Title')}
      description={t('Users.List.Subtitle')}
      actions={
        ViewModel.CanCreate ? (
          <AppButton Label={t('Users.List.New')} OnClick={() => { /* navegar al formulario */ }} />
        ) : undefined
      }
      loading={ViewModel.IsLoading && ViewModel.Items.length === 0}
      error={ViewModel.ErrorMessage ?? undefined}
      onRetry={ViewModel.Reload}
    >
      <ITStack spacing={4}>
        <SearchBox Value={ViewModel.Search} OnChange={ViewModel.SetSearch} />
        <DataTable
          IsLoading={ViewModel.IsLoading}
          Rows={ViewModel.Items}
          TotalCount={ViewModel.TotalCount}
          Page={ViewModel.Page}
          OnPageChange={ViewModel.SetPage}
          Columns={[
            { Field: 'Username', Header: t('Users.Columns.Username') },
            { Field: 'Email', Header: t('Users.Columns.Email') },
            { Field: 'IsActive', Header: t('Users.Columns.IsActive') },
          ]}
        />
      </ITStack>
    </ITPage>
  );
}
```

Los componentes de `shared/components` son envoltorios con props en PascalCase que traducen a la API real
de AXZY UI (`label`, `onClick`, `value`, etc.) en un solo lugar. El chasis y la cabecera de página **no**
se envuelven: se usan `ITLayout` e `ITPage` de la librería. La documentación oficial está en
[`docs/AxzyUiSystem/`](../AxzyUiSystem/README.md).

## Electron

- `contextIsolation: true` y `nodeIntegration: false`; el preload expone solo lo necesario.
- URL de la API por entorno (`VITE_API_BASE_URL`).
- Idioma inicial desde `Localization.DefaultLanguage`; la preferencia del usuario se guarda en
  `localStorage` (`USystem.Language`).
- Tokens en memoria; el refresh token en el almacenamiento seguro del sistema operativo.
- Instalador y actualización automática: ver `Operations/Deployment.md`.
