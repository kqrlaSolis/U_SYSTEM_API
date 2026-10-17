# Módulo 16 — Avance académico

| | |
|---|---|
| **Fase** | Fase 3 — Seguimiento académico |
| **Capa** | Seguimiento |
| **Prefijo de permisos** | `AcademicProgress.*` |
| **Depende de** | `Grades`, `StudyPlans` |
| **Tablas** | — · Vistas: `AcademicProgressView` |
| **Controller** | `AcademicProgressController` |

## 1. Objetivo

Porcentaje de créditos, materias aprobadas y pendientes, promedio general, situación regular o irregular y semáforo de riesgo (`AcademicProgressView`).

## 2. Funcionalidades

- Consulta por alumno.
- Listado por carrera o generación con semáforo.

## 3. Reglas de negocio

1. El porcentaje de avance es créditos aprobados entre créditos del plan.
2. Cada materia cuenta una sola vez (mejor calificación aprobatoria).
3. El semáforo se basa en reprobación, adeudos y avance esperado (reglas configurables).

## 4. Modelo de datos

Este módulo no define tablas propias; consume tablas y vistas de otros módulos.

Vistas: `AcademicProgressView` (script `04_AcademicTracking.sql`).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students/{Id}/academic-progress` | Avance del alumno | `AcademicProgress.View` |
| GET | `/api/v1/academic-progress` | Listado con semáforo (filtros) | `AcademicProgress.View` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/AcademicProgress/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/AcademicProgress.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AcademicProgressFilter.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AcademicProgressService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useAcademicProgressViewModel.ts` | Estado, validaciones y acciones de AcademicProgressPage |
| View | `view/AcademicProgressPage.tsx` | Detalle del alumno |
| ViewModel | `viewmodel/useAcademicProgressListViewModel.ts` | Estado, validaciones y acciones de AcademicProgressListPage |
| View | `view/AcademicProgressListPage.tsx` | Listado con semáforo |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Vistas `AcademicProgressView` creadas por `04_AcademicTracking.sql` y revisadas.
- [ ] Permisos `AcademicProgress.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `IAcademicProgressRepository` / `AcademicProgressRepository` (consultas de lectura y exportación)
- [ ] `IAcademicProgressService` / `AcademicProgressService` y DTOs de respuesta
- [ ] `AcademicProgressController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddAcademicProgressModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `AcademicProgressService`
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

- [ ] El porcentaje de avance coincide con los créditos aprobados.
- [ ] Las materias repetidas no duplican créditos.
