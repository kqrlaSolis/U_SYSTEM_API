# Módulo 19 — Notificaciones

| | |
|---|---|
| **Fase** | Fase 4 — Finanzas y complementarios |
| **Capa** | Complementario |
| **Prefijo de permisos** | `Notifications.*` |
| **Depende de** | `Students`, `Payments` |
| **Tablas** | `NotificationTemplates`, `Notifications` |
| **Controller** | `NotificationsController` |

## 1. Objetivo

Correos a alumnos y tutores por pagos próximos, periodos y calificaciones. El alumno recibe correos, pero no accede al sistema.

## 2. Funcionalidades

- Plantillas con variables.
- Cola de envío con reintentos (SMTP).
- Historial de envíos.

## 3. Reglas de negocio

1. Estatus: Pending, Sent, Error; reintentos limitados.
2. El envío corre en un proceso en segundo plano (`BackgroundService`).
3. Las plantillas admiten variables como `{{StudentName}}`.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/06_Extras.sql`

Entidades C# (en `Api/Entities/`): `NotificationTemplate`, `Notification`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/notifications` | Historial | `Notifications.View` |
| POST | `/api/v1/notifications/send` | Enviar notificación | `Notifications.Send` |
| GET | `/api/v1/notification-templates` | Plantillas | `Notifications.View` |
| PUT | `/api/v1/notification-templates/{Id}` | Editar plantilla | `Notifications.Send` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Notifications/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Notification.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/NotificationTemplate.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/NotificationsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useNotificationListViewModel.ts` | Estado, validaciones y acciones de NotificationListPage |
| View | `view/NotificationListPage.tsx` | Historial |
| ViewModel | `viewmodel/useNotificationTemplateViewModel.ts` | Estado, validaciones y acciones de NotificationTemplatePage |
| View | `view/NotificationTemplatePage.tsx` | Editor de plantillas |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `06_Extras` ejecutado(s) y revisado(s).
- [ ] Permisos `Notifications.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `NotificationTemplate` (Entity) → `INotificationTemplateRepository` / `NotificationTemplateRepository`
- [ ] `INotificationTemplateService` / `NotificationTemplateService`; DTOs `NotificationTemplateDto`, `CreateNotificationTemplateRequest`, `UpdateNotificationTemplateRequest`; validadores
- [ ] `Notification` (Entity) → `INotificationRepository` / `NotificationRepository`
- [ ] `INotificationService` / `NotificationService`; DTOs `NotificationDto`, `CreateNotificationRequest`, `UpdateNotificationRequest`; validadores
- [ ] `NotificationSenderService` como `BackgroundService` para la cola SMTP con reintentos
- [ ] `NotificationsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddNotificationsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `NotificationsService`
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

- [ ] Se envía un correo con la plantilla y las variables resueltas.
- [ ] Un error de SMTP deja el registro en estatus Error con reintento.
