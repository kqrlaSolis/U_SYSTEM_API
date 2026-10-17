# Módulo 05 — Planes de estudio

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Catálogo |
| **Prefijo de permisos** | `StudyPlans.*` |
| **Depende de** | `Careers`, `Subjects` |
| **Tablas** | `StudyPlans`, `StudyPlanSubjects`, `SubjectPrerequisites` |
| **Controller** | `StudyPlansController` |

## Estado (Fase 1 en curso)

- [x] CRUD por carrera con clave y versión únicas (`StudyPlans.CodeTaken`, incluye versiones dadas de baja).
- [x] `PUT /{Id}/subjects` reemplaza la malla por cuatrimestre y tipo (obligatoria/optativa) validando cuatrimestres.
- [x] `PUT /{Id}/prerequisites` define la seriación y **rechaza ciclos** (`StudyPlans.CircularPrerequisite`).
- [x] Créditos de la malla contra `TotalCredits` al publicar (`StudyPlans.CreditsMismatch`).
- [x] `POST /{Id}/set-current`: un solo plan vigente por carrera; el anterior deja de serlo.
- [x] Plan con alumnos inscritos: estructura inmutable y baja bloqueada (`StudyPlans.InUse`); el plan vigente no se elimina.
- [x] Auditoría de alta, edición, malla, seriación, vigencia y baja.
- [ ] Pantalla de planes y malla curricular en el desktop.

## 1. Objetivo

Versionar el plan de estudios de cada carrera y relacionarlo con sus materias por cuatrimestre.

## 2. Funcionalidades

- CRUD de planes y versiones.
- Asignar materias al plan por cuatrimestre (obligatoria u optativa).
- Definir la seriación (prerrequisitos).
- Marcar el plan vigente.

## 3. Reglas de negocio

1. Solo un plan `IsCurrent` por carrera para nuevos ingresos.
2. Los alumnos conservan el plan con el que ingresaron (`Students.StudyPlanId`).
3. Un plan en uso no cambia su estructura: se crea una nueva versión.
4. Los créditos de las materias deben sumar `TotalCredits` del plan al publicarlo.
5. La seriación no puede ser circular.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/02_AcademicCatalogs.sql`

Entidades C# (en `Api/Entities/`): `StudyPlan`, `StudyPlanSubject`, `SubjectPrerequisite`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/study-plans` | Listar planes (paginado, búsqueda, orden) | `StudyPlans.View` |
| GET | `/api/v1/study-plans/{Id}` | Detalle | `StudyPlans.View` |
| POST | `/api/v1/study-plans` | Crear planes | `StudyPlans.Create` |
| PUT | `/api/v1/study-plans/{Id}` | Editar planes | `StudyPlans.Edit` |
| DELETE | `/api/v1/study-plans/{Id}` | Baja lógica | `StudyPlans.Delete` |
| PUT | `/api/v1/study-plans/{Id}/subjects` | Reemplazar materias del plan | `StudyPlans.Edit` |
| PUT | `/api/v1/study-plans/{Id}/prerequisites` | Definir seriación | `StudyPlans.Edit` |
| POST | `/api/v1/study-plans/{Id}/set-current` | Marcar como vigente | `StudyPlans.Edit` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/StudyPlans/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/StudyPlan.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/StudyPlanSubject.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/SubjectPrerequisite.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/StudyPlansService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useStudyPlanListViewModel.ts` | Estado, validaciones y acciones de StudyPlanListPage |
| View | `view/StudyPlanListPage.tsx` | Listado por carrera |
| ViewModel | `viewmodel/useStudyPlanFormViewModel.ts` | Estado, validaciones y acciones de StudyPlanFormPage |
| View | `view/StudyPlanFormPage.tsx` | Datos del plan |
| ViewModel | `viewmodel/useStudyPlanCurriculumViewModel.ts` | Estado, validaciones y acciones de StudyPlanCurriculumPage |
| View | `view/StudyPlanCurriculumPage.tsx` | Malla curricular por cuatrimestre y seriación |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `02_AcademicCatalogs` ejecutado(s) y revisado(s).
- [ ] Permisos `StudyPlans.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `StudyPlan` (Entity) → `IStudyPlanRepository` / `StudyPlanRepository`
- [ ] `IStudyPlanService` / `StudyPlanService`; DTOs `StudyPlanDto`, `CreateStudyPlanRequest`, `UpdateStudyPlanRequest`; validadores
- [ ] `StudyPlanSubject` (Entity, tabla de unión) → `IStudyPlanSubjectRepository` / `StudyPlanSubjectRepository`; se gestiona desde el servicio del módulo
- [ ] `SubjectPrerequisite` (Entity, tabla de unión) → `ISubjectPrerequisiteRepository` / `SubjectPrerequisiteRepository`; se gestiona desde el servicio del módulo
- [ ] `StudyPlansController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddStudyPlansModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `StudyPlansService`
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

- [ ] Se arma una malla por cuatrimestre con seriación válida.
- [ ] Al marcar un plan vigente, el anterior deja de serlo.
- [ ] Se rechaza la seriación circular.
