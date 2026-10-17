# Módulo 06 — Materias

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Catálogo |
| **Prefijo de permisos** | `Subjects.*` |
| **Depende de** | — |
| **Tablas** | `Subjects` |
| **Controller** | `SubjectsController` |

## Estado (Fase 1 en curso)

- [x] CRUD con clave única (`Subjects.CodeTaken`) y paginado con búsqueda por clave o nombre.
- [x] Créditos mayores a cero.
- [x] Una materia usada en un plan o grupo solo se desactiva (`Subjects.InUse`).
- [x] Auditoría de alta, edición y baja.
- [x] Pantalla de materias en el desktop (`/subjects` con listado, búsqueda, alta y edición).

## 1. Objetivo

Catálogo de materias reutilizables entre planes y carreras.

## 2. Funcionalidades

- CRUD de materias.
- Búsqueda por clave o nombre.
- Consulta de planes donde se usa la materia.

## 3. Reglas de negocio

1. `Code` único.
2. Los créditos deben ser mayores a cero.
3. Una materia usada en un plan o grupo solo se desactiva.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/02_AcademicCatalogs.sql`

Entidades C# (en `Api/Entities/`): `Subject`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/subjects` | Listar materias (paginado, búsqueda, orden) | `Subjects.View` |
| GET | `/api/v1/subjects/{Id}` | Detalle | `Subjects.View` |
| POST | `/api/v1/subjects` | Crear materias | `Subjects.Create` |
| PUT | `/api/v1/subjects/{Id}` | Editar materias | `Subjects.Edit` |
| DELETE | `/api/v1/subjects/{Id}` | Baja lógica | `Subjects.Delete` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Subjects/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Subject.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/SubjectsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useSubjectListViewModel.ts` | Estado, validaciones y acciones de SubjectListPage |
| View | `view/SubjectListPage.tsx` | Listado |
| ViewModel | `viewmodel/useSubjectFormViewModel.ts` | Estado, validaciones y acciones de SubjectFormPage |
| View | `view/SubjectFormPage.tsx` | Crear o editar |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `02_AcademicCatalogs` ejecutado(s) y revisado(s).
- [ ] Permisos `Subjects.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Subject` (Entity) → `ISubjectRepository` / `SubjectRepository`
- [ ] `ISubjectService` / `SubjectService`; DTOs `SubjectDto`, `CreateSubjectRequest`, `UpdateSubjectRequest`; validadores
- [ ] `SubjectsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddSubjectsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `SubjectsService`
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

- [ ] Se crea una materia con clave única.
- [ ] No se elimina una materia en uso.
