# Módulo 14 — Calificaciones

| | |
|---|---|
| **Fase** | Fase 3 — Seguimiento académico |
| **Capa** | Seguimiento |
| **Prefijo de permisos** | `Grades.*` |
| **Depende de** | `Enrollments`, `ClassGroups` |
| **Tablas** | `Grades` |
| **Controller** | `GradesController` |

## 1. Objetivo

Captura por parciales y ordinario, extraordinarios, regularización y cierre de actas.

## 2. Funcionalidades

- Captura masiva por grupo.
- Cálculo de la calificación final con ponderación configurable.
- Extraordinario y regularización.
- Cierre y reapertura de acta.

## 3. Reglas de negocio

1. Escala y mínimo aprobatorio en `SystemSettings` (`Grades.MaxScore`, `Grades.MinPassingScore`).
2. Con `IsReportClosed = 1` solo se modifica con `Grades.ReopenReport` y queda en bitácora.
3. Un periodo `Closed` bloquea la captura.
4. Los intentos de extraordinario se limitan con `Grades.MaxExtraordinaryAttempts`; el recursamiento con `Grades.MaxRecurrenceAttempts` y la regularización con `Grades.MaxRemedialAttempts`.
5. Cada intento se guarda con `AttemptNumber` para conservar el historial.
6. Solo se captura a alumnos con inscripción activa en el grupo.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/04_AcademicTracking.sql`

Entidades C# (en `Api/Entities/`): `Grade`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/class-groups/{Id}/grades` | Lista de calificaciones del grupo | `Grades.View` |
| PUT | `/api/v1/class-groups/{Id}/grades` | Capturar calificaciones (lote) | `Grades.Capture` |
| PATCH | `/api/v1/grades/{Id}` | Modificar calificación | `Grades.Modify` |
| POST | `/api/v1/class-groups/{Id}/close-report` | Cerrar acta | `Grades.CloseReport` |
| POST | `/api/v1/class-groups/{Id}/reopen-report` | Reabrir acta | `Grades.ReopenReport` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Grades/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Grade.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/GradeCaptureRequest.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/GradesService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useGradeCaptureViewModel.ts` | Estado, validaciones y acciones de GradeCapturePage |
| View | `view/GradeCapturePage.tsx` | Captura por grupo |
| ViewModel | `viewmodel/useGradeReportViewModel.ts` | Estado, validaciones y acciones de GradeReportPage |
| View | `view/GradeReportPage.tsx` | Acta de calificaciones |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `04_AcademicTracking` ejecutado(s) y revisado(s).
- [ ] Permisos `Grades.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Grade` (Entity) → `IGradeRepository` / `GradeRepository`
- [ ] `IGradeService` / `GradeService`; DTOs `GradeDto`, `CreateGradeRequest`, `UpdateGradeRequest`; validadores
- [ ] `GradesController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddGradesModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `GradesService`
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

- [ ] Un acta cerrada no se modifica sin reapertura.
- [ ] El cálculo final respeta la ponderación y el mínimo aprobatorio.
