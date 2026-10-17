# Módulo 07 — Periodos cuatrimestrales

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Catálogo |
| **Prefijo de permisos** | `AcademicTerms.*` |
| **Depende de** | — |
| **Tablas** | `AcademicTerms` |
| **Controller** | `AcademicTermsController` |

## Estado (Fase 1 en curso)

- [x] CRUD con clave `YYYY-N`, validación de fechas y detección de traslapes (`AcademicTerms.Overlap`).
- [x] `POST /{Id}/open` y `POST /{Id}/close` con permisos `AcademicTerms.Open` y `AcademicTerms.Close`.
- [x] Un solo periodo abierto o en curso (`AcademicTerms.OnlyOneInProgress`) y cerrado de solo lectura.
- [x] `GET /academic-terms/current` devuelve el periodo en curso o el último abierto.
- [x] Auditoría de alta, edición, apertura y cierre.
- [ ] Pantalla de periodos en el desktop.

## 1. Objetivo

Calendario cuatrimestral (ene–abr, may–ago, sep–dic) con fechas de inscripción, reinscripción, exámenes y cierre de actas.

## 2. Funcionalidades

- CRUD de periodos.
- Cambio de estado: `Planned → Open → InProgress → Closed`.
- Consulta del periodo actual.

## 3. Reglas de negocio

1. Solo un periodo `InProgress` a la vez.
2. Un periodo `Closed` es de solo lectura (grupos, inscripciones y calificaciones bloqueados).
3. Las fechas no se traslapan con otro periodo.
4. Abrir y cerrar requieren `AcademicTerms.Open` y `AcademicTerms.Close`.
5. `Code` con formato `YYYY-N` (N = 1, 2 o 3).

## 4. Modelo de datos

Script(s): `DataModel/Scripts/02_AcademicCatalogs.sql`

Entidades C# (en `Api/Entities/`): `AcademicTerm`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/academic-terms` | Listar periodos | `AcademicTerms.View` |
| GET | `/api/v1/academic-terms/{Id}` | Detalle | `AcademicTerms.View` |
| POST | `/api/v1/academic-terms` | Crear periodo | `AcademicTerms.Create` |
| PUT | `/api/v1/academic-terms/{Id}` | Editar periodo | `AcademicTerms.Edit` |
| POST | `/api/v1/academic-terms/{Id}/open` | Abrir periodo | `AcademicTerms.Open` |
| POST | `/api/v1/academic-terms/{Id}/close` | Cerrar periodo | `AcademicTerms.Close` |
| GET | `/api/v1/academic-terms/current` | Periodo en curso | `AcademicTerms.View` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/AcademicTerms/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/AcademicTerm.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AcademicTermsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useAcademicTermListViewModel.ts` | Estado, validaciones y acciones de AcademicTermListPage |
| View | `view/AcademicTermListPage.tsx` | Listado con estado |
| ViewModel | `viewmodel/useAcademicTermFormViewModel.ts` | Estado, validaciones y acciones de AcademicTermFormPage |
| View | `view/AcademicTermFormPage.tsx` | Fechas del calendario |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `02_AcademicCatalogs` ejecutado(s) y revisado(s).
- [ ] Permisos `AcademicTerms.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `AcademicTerm` (Entity) → `IAcademicTermRepository` / `AcademicTermRepository`
- [ ] `IAcademicTermService` / `AcademicTermService`; DTOs `AcademicTermDto`, `CreateAcademicTermRequest`, `UpdateAcademicTermRequest`; validadores
- [ ] `AcademicTermsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddAcademicTermsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `AcademicTermsService`
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

- [ ] Se crea el calendario de un cuatrimestre.
- [ ] Un periodo cerrado no admite cambios.
- [ ] No se permiten dos periodos en curso.
