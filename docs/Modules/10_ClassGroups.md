# Módulo 10 — Grupos y horarios

| | |
|---|---|
| **Fase** | Fase 2 — Operación académica |
| **Capa** | Operación |
| **Prefijo de permisos** | `ClassGroups.*` |
| **Depende de** | `Subjects`, `AcademicTerms`, `Teachers` |
| **Tablas** | `ClassGroups`, `ClassGroupSchedules` |
| **Controller** | `ClassGroupsController` |

## 1. Objetivo

Un grupo es materia + periodo + docente + horario + aula + cupo.

## 2. Funcionalidades

- CRUD de grupos y horarios.
- Asignar docente y aula.
- Consultar ocupación (cupo contra inscritos).

## 3. Reglas de negocio

1. El docente no puede tener dos grupos empalmados en horario.
2. El docente debe estar habilitado para la materia.
3. `EnrolledCount` nunca excede `Capacity`.
4. No se crean ni modifican grupos en periodos `Closed`.
5. `Code` único por periodo.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/03_AcademicOperations.sql`

Entidades C# (en `Api/Entities/`): `ClassGroup`, `ClassGroupSchedule`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/class-groups` | Listar grupos (paginado, búsqueda, orden) | `ClassGroups.View` |
| GET | `/api/v1/class-groups/{Id}` | Detalle | `ClassGroups.View` |
| POST | `/api/v1/class-groups` | Crear grupos | `ClassGroups.Create` |
| PUT | `/api/v1/class-groups/{Id}` | Editar grupos | `ClassGroups.Edit` |
| DELETE | `/api/v1/class-groups/{Id}` | Baja lógica | `ClassGroups.Delete` |
| PUT | `/api/v1/class-groups/{Id}/schedules` | Definir horarios | `ClassGroups.Edit` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/ClassGroups/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/ClassGroup.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ClassGroupSchedule.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ClassGroupsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useClassGroupListViewModel.ts` | Estado, validaciones y acciones de ClassGroupListPage |
| View | `view/ClassGroupListPage.tsx` | Listado por periodo |
| ViewModel | `viewmodel/useClassGroupFormViewModel.ts` | Estado, validaciones y acciones de ClassGroupFormPage |
| View | `view/ClassGroupFormPage.tsx` | Datos, docente y horario |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `03_AcademicOperations` ejecutado(s) y revisado(s).
- [ ] Permisos `ClassGroups.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `ClassGroup` (Entity) → `IClassGroupRepository` / `ClassGroupRepository`
- [ ] `IClassGroupService` / `ClassGroupService`; DTOs `ClassGroupDto`, `CreateClassGroupRequest`, `UpdateClassGroupRequest`; validadores
- [ ] `ClassGroupSchedule` (Entity) → `IClassGroupScheduleRepository` / `ClassGroupScheduleRepository`; se gestiona desde el servicio del módulo (sin CRUD propio)
- [ ] `ClassGroupsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddClassGroupsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `ClassGroupsService`
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

- [ ] Se detecta el empalme de horario de un docente.
- [ ] No se excede el cupo.

## Estado (Fase 2 en curso)

- [x] `ClassGroup` + `ClassGroupSchedule` → `IClassGroupRepository` / `ClassGroupRepository` (código único por periodo, reemplazo de horarios, grupos con inscritos).
- [x] `IClassGroupService` / `ClassGroupService` con auditoría y reglas `ClassGroups.NotFound`, `CodeTaken`, `InUse`, `InvalidCapacity`, `InvalidSchedule`.
- [x] DTOs (`ClassGroupDto` con horarios, `Create/UpdateClassGroupRequest`) y validadores (día 1-7, horas HH:mm, inicio < fin).
- [x] `ClassGroupsController` con `[RequirePermission(ClassGroups.*)]` y `AddClassGroupsModule()`.
- [x] Pruebas unitarias (4), verificación HTTP real con horarios y pantalla de desktop (`/class-groups`).
