# Endpoints — Fase 4 — Finanzas y complementarios

Prefijo `/api/v1`. Respuesta estándar `ApiResponse<T>` (ver `Api/Conventions.md`).

## 17. Pagos y colegiaturas — `PaymentsController`

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

## 18. Trámites y constancias — `ProceduresController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/procedures` | Listar | `Procedures.View` |
| POST | `/api/v1/procedures` | Solicitar trámite | `Procedures.Request` |
| POST | `/api/v1/procedures/{Id}/issue` | Emitir documento | `Procedures.Issue` |
| GET | `/api/v1/procedures/{Id}/pdf` | Descargar PDF | `Procedures.View` |
| GET | `/api/v1/procedure-types` | Catálogo de tipos | `Procedures.View` |

## 19. Notificaciones — `NotificationsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/notifications` | Historial | `Notifications.View` |
| POST | `/api/v1/notifications/send` | Enviar notificación | `Notifications.Send` |
| GET | `/api/v1/notification-templates` | Plantillas | `Notifications.View` |
| PUT | `/api/v1/notification-templates/{Id}` | Editar plantilla | `Notifications.Send` |

## 20. Reportes y dashboard — `ReportsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/reports/dashboard` | Indicadores del periodo | `Reports.View` |
| GET | `/api/v1/reports/enrollment-by-term` | Inscritos por periodo y carrera | `Reports.View` |
| GET | `/api/v1/reports/delinquency` | Morosidad | `Reports.View` |
| GET | `/api/v1/reports/failure-rate` | Reprobación por materia | `Reports.View` |
| GET | `/api/v1/reports/dropout` | Deserción | `Reports.View` |
| GET | `/api/v1/reports/{Name}/export` | Exportar (xlsx o pdf) | `Reports.Export` |
