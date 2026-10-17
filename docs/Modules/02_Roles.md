# Módulo 02 — Roles y permisos (RBAC)

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Base |
| **Prefijo de permisos** | `Roles.*` |
| **Depende de** | `Users` |
| **Tablas** | `Roles`, `Permissions`, `RolePermissions`, `UserRoles` |
| **Controller** | `RolesController` |

## Estado (Fase 1 en curso)

- [x] CRUD de roles con permisos por código del catálogo (`GET/POST/PUT/DELETE /api/v1/roles`).
- [x] Catálogo de permisos agrupado por módulo (`GET /api/v1/permissions`, 23 módulos y 78 permisos).
- [x] Roles de sistema protegidos, rol con usuarios no se elimina y Administrator conserva el catálogo completo.
- [x] Asignación de roles a usuarios (`PUT /api/v1/users/{Id}/roles`) con protección del último administrador.
- [x] Auditoría de alta, edición, baja y asignación de roles.
- [x] Reactivación de un rol dado de baja al reutilizar su nombre (el índice único de `Name` lo exige).
- [x] Pantallas de roles en el desktop: listado (`/roles`) y editor de permisos por módulo (`/roles/new`, `/roles/:Id`).

## 1. Objetivo

Controlar qué puede ver y hacer cada usuario con roles: `User ──< UserRoles >── Role ──< RolePermissions >── Permission`.

## 2. Funcionalidades

- CRUD de roles.
- Editor de permisos por rol agrupado por módulo.
- Asignación de roles a usuarios.
- Menú dinámico en el desktop según permisos.
- Atributo `[RequirePermission("Module.Action")]` en la API.

## 3. Reglas de negocio

1. Los permisos son un catálogo semilla definido por el sistema; no se crean desde la UI.
2. Los permisos efectivos del usuario son la unión de los permisos de sus roles.
3. Los roles de sistema (`IsSystem = 1`) no se eliminan; Administrator conserva los permisos críticos.
4. No se puede eliminar un rol con usuarios asignados.
5. Los permisos viajan como claims en el JWT y siempre se validan en la API (la UI solo oculta).
6. Todo cambio de roles o permisos se registra en `AuditLogs`.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/01_Security.sql`

Entidades C# (en `Api/Entities/`): `Role`, `Permission`, `RolePermission`, `UserRole`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/roles` | Listar roles (paginado, búsqueda, orden) | `Roles.View` |
| GET | `/api/v1/roles/{Id}` | Detalle | `Roles.View` |
| POST | `/api/v1/roles` | Crear roles | `Roles.Create` |
| PUT | `/api/v1/roles/{Id}` | Editar roles | `Roles.Edit` |
| DELETE | `/api/v1/roles/{Id}` | Baja lógica | `Roles.Delete` |
| GET | `/api/v1/permissions` | Catálogo de permisos por módulo | `Roles.View` |
| PUT | `/api/v1/users/{Id}/roles` | Asignar roles a un usuario | `Roles.Assign` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Roles/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Role.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/Permission.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/RoleDetail.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/UserRoleAssignment.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/RolesService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useRoleListViewModel.ts` | Estado, validaciones y acciones de RoleListPage |
| View | `view/RoleListPage.tsx` | Listado de roles |
| ViewModel | `viewmodel/useRoleFormViewModel.ts` | Estado, validaciones y acciones de RoleFormPage |
| View | `view/RoleFormPage.tsx` | Editor de rol con árbol de permisos |
| ViewModel | `viewmodel/useUserRoleAssignmentViewModel.ts` | Estado, validaciones y acciones de UserRoleAssignmentPage |
| View | `view/UserRoleAssignmentPage.tsx` | Asignación de roles (también embebida en UserFormPage) |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `01_Security` ejecutado(s) y revisado(s).
- [ ] Permisos `Roles.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Role` (Entity) → `IRoleRepository` / `RoleRepository`
- [ ] `IRoleService` / `RoleService`; DTOs `RoleDto`, `CreateRoleRequest`, `UpdateRoleRequest`; validadores
- [ ] `Permission` (Entity) → `IPermissionRepository` / `PermissionRepository`
- [ ] `IPermissionService` / `PermissionService`; DTOs `PermissionDto`, `CreatePermissionRequest`, `UpdatePermissionRequest`; validadores
- [ ] `RolePermission` (Entity, tabla de unión) → `IRolePermissionRepository` / `RolePermissionRepository`; se gestiona desde el servicio del módulo
- [ ] `UserRole` (Entity, tabla de unión) → `IUserRoleRepository` / `UserRoleRepository`; se gestiona desde el servicio del módulo
- [ ] `RolesController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddRolesModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `RolesService`
- [ ] ViewModels (hooks) con validaciones y manejo de errores
- [ ] Views con AXZY UI; ruta y entrada de menú protegidas por permiso

### D. Pruebas
- [ ] Unitarias de servicios (reglas de negocio de la sección 3)
- [ ] Integración de repositorios contra MySQL de pruebas
- [ ] Pruebas de ViewModels
- [ ] Caso de prueba por cada criterio de aceptación

### E. Cierre
- [ ] Criterios de aceptación cumplidos
- [ ] Revisión de seguridad (permisos, datos sensibles)
- [ ] Documentación actualizada

## 8. Criterios de aceptación

- [ ] Un usuario solo accede a lo que sus roles permiten (403 en la API sin permiso).
- [ ] Se crea un rol nuevo y se le asignan permisos desde la UI.
- [ ] No se elimina un rol de sistema ni uno con usuarios asignados.
- [ ] Los cambios de permisos quedan en bitácora.
