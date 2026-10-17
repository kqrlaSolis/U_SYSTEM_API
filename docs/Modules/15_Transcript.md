# Módulo 15 — Kardex

| | |
|---|---|
| **Fase** | Fase 3 — Seguimiento académico |
| **Capa** | Seguimiento |
| **Prefijo de permisos** | `Transcript.*` |
| **Depende de** | `Grades` |
| **Tablas** | — · Vistas: `TranscriptView` |
| **Controller** | `TranscriptController` |

## 1. Objetivo

Historial académico por alumno: materias cursadas, calificación, periodo, créditos y estatus. Se alimenta de la vista `TranscriptView`.

## 2. Funcionalidades

- Consulta por alumno ordenada por cuatrimestre.
- Exportación a PDF.

## 3. Reglas de negocio

1. Solo lectura (derivado de `Grades`).
2. Incluye intentos reprobados y extraordinarios.
3. La exportación queda en bitácora.

## 4. Modelo de datos

Este módulo no define tablas propias; consume tablas y vistas de otros módulos.

Vistas: `TranscriptView` (script `04_AcademicTracking.sql`).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students/{Id}/transcript` | Kardex del alumno | `Transcript.View` |
| GET | `/api/v1/students/{Id}/transcript/pdf` | Kardex en PDF | `Transcript.Export` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Transcript/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/TranscriptEntry.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/TranscriptService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useTranscriptViewModel.ts` | Estado, validaciones y acciones de TranscriptPage |
| View | `view/TranscriptPage.tsx` | Kardex con exportación a PDF |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Vistas `TranscriptView` creadas por `04_AcademicTracking.sql` y revisadas.
- [ ] Permisos `Transcript.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `ITranscriptRepository` / `TranscriptRepository` (consultas de lectura y exportación)
- [ ] `ITranscriptService` / `TranscriptService` y DTOs de respuesta
- [ ] `IPdfService` para generar el kardex en PDF
- [ ] `TranscriptController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddTranscriptModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `TranscriptService`
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

- [ ] El kardex coincide con las calificaciones capturadas.
- [ ] El PDF se genera con el formato institucional.
