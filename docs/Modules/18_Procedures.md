# Módulo 18 — Trámites y constancias

| | |
|---|---|
| **Fase** | Fase 4 — Finanzas y complementarios |
| **Capa** | Complementario |
| **Prefijo de permisos** | `Procedures.*` |
| **Depende de** | `Students`, `Transcript`, `Payments` |
| **Tablas** | `ProcedureTypes`, `Procedures` |
| **Controller** | `ProceduresController` |

## 1. Objetivo

Solicitud y emisión de constancia de estudios, historial académico y carta de pasante.

## 2. Funcionalidades

- Solicitud con folio.
- Cobro asociado opcional.
- Emisión en PDF.

## 3. Reglas de negocio

1. `FolioNumber` único y consecutivo.
2. Si el tipo tiene `PaymentConceptId`, se requiere el pago antes de emitir.
3. Estatus: Requested, InProcess, Issued, Delivered, Cancelled.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/06_Extras.sql`

Entidades C# (en `Api/Entities/`): `ProcedureType`, `Procedure`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/procedures` | Listar | `Procedures.View` |
| POST | `/api/v1/procedures` | Solicitar trámite | `Procedures.Request` |
| POST | `/api/v1/procedures/{Id}/issue` | Emitir documento | `Procedures.Issue` |
| GET | `/api/v1/procedures/{Id}/pdf` | Descargar PDF | `Procedures.View` |
| GET | `/api/v1/procedure-types` | Catálogo de tipos | `Procedures.View` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Procedures/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Procedure.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ProcedureType.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ProceduresService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useProcedureListViewModel.ts` | Estado, validaciones y acciones de ProcedureListPage |
| View | `view/ProcedureListPage.tsx` | Bandeja de trámites |
| ViewModel | `viewmodel/useProcedureRequestViewModel.ts` | Estado, validaciones y acciones de ProcedureRequestPage |
| View | `view/ProcedureRequestPage.tsx` | Solicitud |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `06_Extras` ejecutado(s) y revisado(s).
- [ ] Permisos `Procedures.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `ProcedureType` (Entity) → `IProcedureTypeRepository` / `ProcedureTypeRepository`
- [ ] `IProcedureTypeService` / `ProcedureTypeService`; DTOs `ProcedureTypeDto`, `CreateProcedureTypeRequest`, `UpdateProcedureTypeRequest`; validadores
- [ ] `Procedure` (Entity) → `IProcedureRepository` / `ProcedureRepository`
- [ ] `IProcedureService` / `ProcedureService`; DTOs `ProcedureDto`, `CreateProcedureRequest`, `UpdateProcedureRequest`; validadores
- [ ] `IPdfService` para emitir la constancia con folio
- [ ] `ProceduresController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddProceduresModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `ProceduresService`
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

- [ ] Se emite una constancia con folio único.
- [ ] No se emite un trámite con cobro pendiente.
