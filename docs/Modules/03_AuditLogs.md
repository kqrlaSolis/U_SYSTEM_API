# Módulo 03 — Auditoría y bitácora

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Base |
| **Prefijo de permisos** | `AuditLogs.*` |
| **Depende de** | `Users` |
| **Tablas** | `AuditLogs` |
| **Controller** | `AuditLogsController` |

## Estado (Fase 1 en curso)

- [x] Registro automático desde `IAuditService` en cada caso de uso (login, usuarios, roles y settings).
- [x] Consulta paginada con filtros por acción, módulo, entidad, usuario y rango de fechas.
- [x] Detalle con valores anterior y nuevo, nombre de usuario e IP.
- [x] Sin endpoints de modificación ni borrado (solo inserción y consulta).
- [x] Pantalla de bitácora en el desktop (`/audit-logs`) con filtros y detalle de valores antes/después.

## 1. Objetivo

Trazabilidad de acciones críticas (usuarios, roles, calificaciones, pagos, documentos).

## 2. Funcionalidades

- Registro automático desde la capa de servicio (`IAuditService`).
- Consulta con filtros por usuario, módulo, entidad y fecha.

## 3. Reglas de negocio

1. La bitácora es solo de inserción: no se edita ni se borra.
2. Se guardan `OldValue` y `NewValue` en JSON para los cambios de datos.
3. Se registra login, logout, altas, cambios, bajas, cierre o reapertura de actas y cancelación de pagos.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/01_Security.sql`

Entidades C# (en `Api/Entities/`): `AuditLog`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/audit-logs` | Consultar bitácora (filtros y paginado) | `AuditLogs.View` |
| GET | `/api/v1/audit-logs/{Id}` | Detalle con valores anterior y nuevo | `AuditLogs.View` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/AuditLogs/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/AuditLog.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AuditLogFilter.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AuditLogsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useAuditLogListViewModel.ts` | Estado, validaciones y acciones de AuditLogListPage |
| View | `view/AuditLogListPage.tsx` | Consulta con filtros |
| ViewModel | `viewmodel/useAuditLogDetailViewModel.ts` | Estado, validaciones y acciones de AuditLogDetailDialog |
| View | `view/AuditLogDetailDialog.tsx` | Diferencias anterior y nuevo |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `01_Security` ejecutado(s) y revisado(s).
- [ ] Permisos `AuditLogs.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `AuditLog` (Entity) → `IAuditLogRepository` / `AuditLogRepository`; solo consulta (sin CRUD)
- [ ] `AuditLogsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddAuditLogsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `AuditLogsService`
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

- [ ] Cada acción crítica genera un registro con usuario, fecha e IP.
- [ ] No existe endpoint para modificar o borrar registros.
