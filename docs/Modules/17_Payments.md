# Módulo 17 — Pagos y colegiaturas

| | |
|---|---|
| **Fase** | Fase 4 — Finanzas y complementarios |
| **Capa** | Finanzas |
| **Prefijo de permisos** | `Payments.*` · `Scholarships.*` |
| **Depende de** | `Students`, `AcademicTerms` |
| **Tablas** | `PaymentConcepts`, `PaymentPlans`, `Scholarships`, `StudentScholarships`, `Charges`, `Payments` |
| **Controller** | `PaymentsController`, `ScholarshipsController` |

## 1. Objetivo

Conceptos, plan de pagos por periodo, cargos, recargos, becas y descuentos, registro de pagos, recibos y estado de cuenta.

## 2. Funcionalidades

- Catálogo de conceptos y planes de pago.
- Colegiatura mensual: 4 pagos por cuatrimestre más el cargo de inscripción.
- Becas y descuentos por alumno y periodo.
- Registro de pagos y recibo con folio.
- Cancelación de pagos con motivo.
- Estado de cuenta en PDF.

## 3. Reglas de negocio

1. Recargo por pago tardío configurable (`Payments.SurchargeType` y `Payments.SurchargeValue`).
2. Los pagos no se borran: se cancelan con motivo y quedan en bitácora.
3. `ReceiptNumber` consecutivo y único.
4. Un cargo con adeudo vencido bloquea la reinscripción.
5. Estatus del cargo: Pending, Partial, Paid, Overdue, Cancelled.
6. Condonar requiere `Payments.Waive`.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/05_Finance.sql`

Entidades C# (en `Api/Entities/`): `PaymentConcept`, `PaymentPlan`, `Scholarship`, `StudentScholarship`, `Charge`, `Payment`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/payment-concepts` | Listar conceptos | `Payments.View` |
| POST | `/api/v1/payment-concepts` | Crear concepto | `Payments.Register` |
| PUT | `/api/v1/payment-concepts/{Id}` | Editar concepto | `Payments.Register` |
| DELETE | `/api/v1/payment-concepts/{Id}` | Desactivar concepto | `Payments.Register` |
| POST | `/api/v1/charges/generate` | Generar cargos del periodo | `Payments.Register` |
| GET | `/api/v1/students/{Id}/charges` | Cargos del alumno | `Payments.View` |
| POST | `/api/v1/payments` | Registrar pago | `Payments.Register` |
| POST | `/api/v1/payments/{Id}/cancel` | Cancelar pago | `Payments.Cancel` |
| POST | `/api/v1/charges/{Id}/waive` | Condonar cargo | `Payments.Waive` |
| GET | `/api/v1/students/{Id}/account-statement` | Estado de cuenta | `Payments.View` |
| GET | `/api/v1/payments/{Id}/receipt` | Recibo en PDF | `Payments.View` |
| GET | `/api/v1/scholarships` | Listar becas | `Scholarships.View` |
| POST | `/api/v1/scholarships` | Crear beca | `Scholarships.Create` |
| PUT | `/api/v1/scholarships/{Id}` | Editar beca | `Scholarships.Edit` |
| POST | `/api/v1/students/{Id}/scholarships` | Asignar beca | `Scholarships.Assign` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Payments/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/PaymentConcept.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/Charge.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/Payment.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/Scholarship.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AccountStatement.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/PaymentsService.ts` | Llamadas HTTP al controlador del módulo |
| Model | `model/ScholarshipsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/usePaymentConceptListViewModel.ts` | Estado, validaciones y acciones de PaymentConceptListPage |
| View | `view/PaymentConceptListPage.tsx` | Conceptos |
| ViewModel | `viewmodel/useChargeGenerationViewModel.ts` | Estado, validaciones y acciones de ChargeGenerationPage |
| View | `view/ChargeGenerationPage.tsx` | Generación de cargos |
| ViewModel | `viewmodel/usePaymentRegistrationViewModel.ts` | Estado, validaciones y acciones de PaymentRegistrationPage |
| View | `view/PaymentRegistrationPage.tsx` | Caja: registrar pago |
| ViewModel | `viewmodel/useAccountStatementViewModel.ts` | Estado, validaciones y acciones de AccountStatementPage |
| View | `view/AccountStatementPage.tsx` | Estado de cuenta |
| ViewModel | `viewmodel/useScholarshipListViewModel.ts` | Estado, validaciones y acciones de ScholarshipListPage |
| View | `view/ScholarshipListPage.tsx` | Becas y descuentos |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `05_Finance` ejecutado(s) y revisado(s).
- [ ] Permisos `Payments.*` · `Scholarships.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `PaymentConcept` (Entity) → `IPaymentConceptRepository` / `PaymentConceptRepository`
- [ ] `IPaymentConceptService` / `PaymentConceptService`; DTOs `PaymentConceptDto`, `CreatePaymentConceptRequest`, `UpdatePaymentConceptRequest`; validadores
- [ ] `PaymentPlan` (Entity) → `IPaymentPlanRepository` / `PaymentPlanRepository`
- [ ] `IPaymentPlanService` / `PaymentPlanService`; DTOs `PaymentPlanDto`, `CreatePaymentPlanRequest`, `UpdatePaymentPlanRequest`; validadores
- [ ] `Scholarship` (Entity) → `IScholarshipRepository` / `ScholarshipRepository`
- [ ] `IScholarshipService` / `ScholarshipService`; DTOs `ScholarshipDto`, `CreateScholarshipRequest`, `UpdateScholarshipRequest`; validadores
- [ ] `StudentScholarship` (Entity) → `IStudentScholarshipRepository` / `StudentScholarshipRepository`; se gestiona desde el servicio del módulo (sin CRUD propio)
- [ ] `Charge` (Entity) → `IChargeRepository` / `ChargeRepository`
- [ ] `IChargeService` / `ChargeService`; DTOs `ChargeDto`, `CreateChargeRequest`, `UpdateChargeRequest`; validadores
- [ ] `Payment` (Entity) → `IPaymentRepository` / `PaymentRepository`
- [ ] `IPaymentService` / `PaymentService`; DTOs `PaymentDto`, `CreatePaymentRequest`, `UpdatePaymentRequest`; validadores
- [ ] `IPdfService` para el recibo y el estado de cuenta
- [ ] Folio de recibo consecutivo generado de forma transaccional
- [ ] `PaymentsController` con `[RequirePermission]` en cada acción
- [ ] `ScholarshipsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddPaymentsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `PaymentsService`
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

- [ ] Se registra un pago y se emite recibo con folio único.
- [ ] Un pago cancelado conserva su historial.
- [ ] Un adeudo vencido bloquea la reinscripción.
