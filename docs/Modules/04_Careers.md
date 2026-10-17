# Módulo 04 — Carreras y programas

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Catálogo |
| **Prefijo de permisos** | `Careers.*` |
| **Depende de** | — |
| **Tablas** | `Careers` |
| **Controller** | `CareersController` |

## Estado (Fase 1 en curso)

- [x] CRUD con clave única (`Careers.CodeTaken`) y paginado con búsqueda.
- [x] `TermCount` y `TotalCredits` mayores a cero.
- [x] Una carrera con planes o alumnos solo se desactiva (`Careers.InUse`).
- [x] Auditoría de alta, edición y baja.
- [x] Pantalla de carreras en el desktop (`/careers` con listado, búsqueda, alta y edición).

## 1. Objetivo

Catálogo de carreras (licenciatura, ingeniería, maestría) con duración en cuatrimestres y créditos totales.

## 2. Funcionalidades

- CRUD de carreras.
- Activar o desactivar.
- Consulta de planes asociados.

## 3. Reglas de negocio

1. `Code` único.
2. Una carrera con alumnos o planes asociados no se elimina, solo se desactiva.
3. `TermCount` y `TotalCredits` deben ser mayores a cero.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/02_AcademicCatalogs.sql`

Entidades C# (en `Api/Entities/`): `Career`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/careers` | Listar carreras (paginado, búsqueda, orden) | `Careers.View` |
| GET | `/api/v1/careers/{Id}` | Detalle | `Careers.View` |
| POST | `/api/v1/careers` | Crear carreras | `Careers.Create` |
| PUT | `/api/v1/careers/{Id}` | Editar carreras | `Careers.Edit` |
| DELETE | `/api/v1/careers/{Id}` | Baja lógica | `Careers.Delete` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Careers/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Career.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/CareersService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useCareerListViewModel.ts` | Estado, validaciones y acciones de CareerListPage |
| View | `view/CareerListPage.tsx` | Listado |
| ViewModel | `viewmodel/useCareerFormViewModel.ts` | Estado, validaciones y acciones de CareerFormPage |
| View | `view/CareerFormPage.tsx` | Crear o editar |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `02_AcademicCatalogs` ejecutado(s) y revisado(s).
- [ ] Permisos `Careers.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Career` (Entity) → `ICareerRepository` / `CareerRepository`
- [ ] `ICareerService` / `CareerService`; DTOs `CareerDto`, `CreateCareerRequest`, `UpdateCareerRequest`; validadores
- [ ] `CareersController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddCareersModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `CareersService`
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

- [ ] Se crea una carrera con clave única.
- [ ] No se puede eliminar una carrera con planes o alumnos.
