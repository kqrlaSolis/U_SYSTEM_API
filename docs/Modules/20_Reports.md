# Módulo 20 — Reportes y dashboard

| | |
|---|---|
| **Fase** | Fase 4 — Finanzas y complementarios |
| **Capa** | Complementario |
| **Prefijo de permisos** | `Reports.*` |
| **Depende de** | `Enrollments`, `Payments`, `Grades` |
| **Tablas** | — |
| **Controller** | `ReportsController` |

## 1. Objetivo

Indicadores y reportes: inscritos por periodo, morosidad, reprobación, deserción, ingresos y avance por generación.

## 2. Funcionalidades

- Dashboard con indicadores clave.
- Reportes filtrables.
- Exportación a Excel o PDF.

## 3. Reglas de negocio

1. Solo lectura.
2. Consultas optimizadas con vistas e índices, siempre paginadas.
3. La exportación respeta el permiso `Reports.Export`.

## 4. Modelo de datos

Este módulo no define tablas propias; consume tablas y vistas de otros módulos.

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/reports/dashboard` | Indicadores del periodo | `Reports.View` |
| GET | `/api/v1/reports/enrollment-by-term` | Inscritos por periodo y carrera | `Reports.View` |
| GET | `/api/v1/reports/delinquency` | Morosidad | `Reports.View` |
| GET | `/api/v1/reports/failure-rate` | Reprobación por materia | `Reports.View` |
| GET | `/api/v1/reports/dropout` | Deserción | `Reports.View` |
| GET | `/api/v1/reports/{Name}/export` | Exportar (xlsx o pdf) | `Reports.Export` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Reports/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/DashboardSummary.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ReportFilter.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/ReportsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useDashboardViewModel.ts` | Estado, validaciones y acciones de DashboardPage |
| View | `view/DashboardPage.tsx` | Panel principal |
| ViewModel | `viewmodel/useReportViewModel.ts` | Estado, validaciones y acciones de ReportPage |
| View | `view/ReportPage.tsx` | Reporte con filtros y exportación |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Sin cambios de esquema; solo consume datos de otros módulos.
- [ ] Permisos `Reports.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `IReportsRepository` / `ReportsRepository` (consultas de lectura y exportación)
- [ ] `IReportsService` / `ReportsService` y DTOs de respuesta
- [ ] `IExcelExportService` e `IPdfService` para la exportación
- [ ] `ReportsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddReportsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `ReportsService`
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

- [ ] Los indicadores coinciden con los datos operativos.
- [ ] La exportación respeta permisos.
