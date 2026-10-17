# Módulo 12 — Inscripción (primer ingreso)

| | |
|---|---|
| **Fase** | Fase 2 — Operación académica |
| **Capa** | Operación |
| **Prefijo de permisos** | `Enrollments.*` |
| **Depende de** | `Students`, `ClassGroups`, `AcademicTerms` |
| **Tablas** | `Enrollments`, `EnrollmentClassGroups` |
| **Controller** | `EnrollmentsController` |

## 1. Objetivo

Primera inscripción del alumno a carrera y periodo, validando documentación y pago.

## 2. Funcionalidades

- Crear inscripción (`Type = Enrollment`) y asignar grupos.
- Cancelar inscripción.
- Consultar inscripciones por periodo o alumno.

## 3. Reglas de negocio

1. Requiere documentos obligatorios `Validated` (o excepción autorizada).
2. Requiere el pago de inscripción registrado.
3. Solo en periodos con inscripciones abiertas.
4. Una inscripción por alumno y periodo.
5. Una inscripción cancelada se reactiva; no se crea otra fila para el mismo periodo.
6. Al inscribir se actualiza `ClassGroups.EnrolledCount`.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/03_AcademicOperations.sql`

Entidades C# (en `Api/Entities/`): `Enrollment`, `EnrollmentClassGroup`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/enrollments` | Listar (filtros periodo, carrera, alumno) | `Enrollments.View` |
| GET | `/api/v1/enrollments/{Id}` | Detalle | `Enrollments.View` |
| POST | `/api/v1/enrollments` | Inscribir alumno | `Enrollments.Create` |
| POST | `/api/v1/enrollments/{Id}/cancel` | Cancelar inscripción | `Enrollments.Cancel` |
| GET | `/api/v1/students/{Id}/enrollment-eligibility` | Validar requisitos (documentos y pago) | `Enrollments.View` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Enrollments/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Enrollment.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/EnrollmentEligibility.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/EnrollmentsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useEnrollmentListViewModel.ts` | Estado, validaciones y acciones de EnrollmentListPage |
| View | `view/EnrollmentListPage.tsx` | Listado |
| ViewModel | `viewmodel/useEnrollmentWizardViewModel.ts` | Estado, validaciones y acciones de EnrollmentWizardPage |
| View | `view/EnrollmentWizardPage.tsx` | Asistente: requisitos, grupos, confirmación |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `03_AcademicOperations` ejecutado(s) y revisado(s).
- [ ] Permisos `Enrollments.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Enrollment` (Entity) → `IEnrollmentRepository` / `EnrollmentRepository`
- [ ] `IEnrollmentService` / `EnrollmentService`; DTOs `EnrollmentDto`, `CreateEnrollmentRequest`, `UpdateEnrollmentRequest`; validadores
- [ ] `EnrollmentClassGroup` (Entity, tabla de unión) → `IEnrollmentClassGroupRepository` / `EnrollmentClassGroupRepository`; se gestiona desde el servicio del módulo
- [ ] `EnrollmentsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddEnrollmentsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `EnrollmentsService`
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

- [ ] No se inscribe sin documentos validados ni pago.
- [ ] Cancelar libera el cupo del grupo.
