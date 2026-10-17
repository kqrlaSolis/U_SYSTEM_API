# Módulo 13 — Reinscripción

| | |
|---|---|
| **Fase** | Fase 3 — Seguimiento académico |
| **Capa** | Operación |
| **Prefijo de permisos** | `ReEnrollments.*` |
| **Depende de** | `Enrollments`, `Grades`, `Payments` |
| **Tablas** | `Enrollments`, `EnrollmentClassGroups` |
| **Controller** | `ReEnrollmentsController` |

## 1. Objetivo

Selección de materias y grupos del siguiente cuatrimestre (mismo modelo `Enrollments` con `Type = ReEnrollment`).

## 2. Funcionalidades

- Proponer materias elegibles.
- Inscribir a grupos del siguiente cuatrimestre.
- Autorizar excepciones.

## 3. Reglas de negocio

1. Sin adeudos vencidos (o excepción con `ReEnrollments.AuthorizeException`).
2. Valida la seriación: prerrequisitos aprobados.
3. Valida cupo y empalme de horarios.
4. Materias reprobadas: recursamiento o regularización según configuración.
5. Máximo `ReEnrollment.MaxSubjects` materias por cuatrimestre.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/03_AcademicOperations.sql`

Entidades C# (en `Api/Entities/`): `Enrollment`, `EnrollmentClassGroup`

Este módulo **reutiliza el modelo de Enrollments**; no crea tablas nuevas.

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students/{Id}/reenrollment-options` | Materias y grupos elegibles | `ReEnrollments.View` |
| POST | `/api/v1/reenrollments` | Reinscribir | `ReEnrollments.Create` |
| POST | `/api/v1/reenrollments/{Id}/cancel` | Cancelar | `ReEnrollments.Cancel` |
| POST | `/api/v1/reenrollments/{Id}/authorize-exception` | Autorizar excepción | `ReEnrollments.AuthorizeException` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/ReEnrollments/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/ReEnrollment.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ReEnrollmentOption.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ReEnrollmentsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useReEnrollmentViewModel.ts` | Estado, validaciones y acciones de ReEnrollmentPage |
| View | `view/ReEnrollmentPage.tsx` | Selección de materias con validaciones en vivo |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `03_AcademicOperations` ejecutado(s) y revisado(s).
- [ ] Permisos `ReEnrollments.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] Reutiliza `Enrollment`, `IEnrollmentRepository` y `EnrollmentClassGroup` del módulo 12 (sin entidades nuevas)
- [ ] `IReEnrollmentService` / `ReEnrollmentService` (seriación, adeudos, cupo y empalmes); DTOs `ReEnrollmentDto`, `CreateReEnrollmentRequest`
- [ ] `ReEnrollmentsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddReEnrollmentsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `ReEnrollmentsService`
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

- [ ] Se bloquea la reinscripción con adeudo vencido.
- [ ] Se bloquea una materia sin prerrequisito aprobado.
- [ ] La excepción autorizada queda en bitácora.
