# Backlog por fase

Registro único de pendientes de U SYSTEM. Cada fase cierra solo cuando los ítems marcados como
**bloqueantes** están resueltos; el resto puede arrastrarse a la fase siguiente.

> Estado: **Fase 0 y Fase 1 completas**, **Fase 2 completa** (API + desktop). Fase 3 en curso.

## Fase 0 — Cimientos

| Pendiente | Tipo | Nota |
|---|---|---|
| Bucket S3 responde 403 (política IAM) | Bloqueante para archivos | `tools/CheckS3.py`; el contrato S3 ya está implementado en `S3FileStorage` |
| Refresh token en `localStorage` | Seguridad | Mover a `safeStorage` de Electron (`docs/Operations/Secrets.md`) |
| Instaladores sin firma real | Distribución | `dist:win` genera el `.exe` con icono; falta certificado Authenticode |
| CI nunca ejecutado | Proceso | El directorio aún no es repo git; `.github/workflows/ci.yml` listo |
| Esquema `USystem` de respaldo en Railway | Limpieza | Se puede borrar con `DROP DATABASE USystem;` (la app usa `railway`) |

## Fase 1 — Seguridad y catálogos

| Pendiente | Tipo | Nota |
|---|---|---|
| Pruebas de ViewModel de Periodos y Planes | Calidad | Los servicios ya tienen pruebas .NET |
| Integración de repositorios contra MySQL de pruebas | Calidad | Gated por `USYSTEM_RUN_DB_TESTS=1` |
| Revisión de seguridad formal de la fase | Proceso | Permisos y datos sensibles revisados de forma manual |
| Tokens de recuperación sin envío de correo | Producto | El token se registra en logs de la API (no hay SMTP) |

## Fase 2 — Operación académica

| Pendiente | Tipo | Nota |
|---|---|---|
| Editor visual de horarios en ClassGroups | UX | Hoy se capturan como texto `día inicio-fin; …` |
| Selects dependientes en Students y Enrollments | UX | Carrera, plan, periodo y grupos se capturan por Id |
| Botón de cancelar inscripción en el desktop | Funcional | La API ya expone `POST /enrollments/{Id}/cancel` y libera cupo; falta el botón en la lista |
| ~~Recuperar `EnrolledCount` al cancelar una inscripción~~ | Resuelto | `EnrollmentService.CancelAsync` libera el cupo de cada grupo y se verificó por HTTP (2 → 1) |
| Documentación sin bloquear inscripción (gracia 3 meses) | Regla | Implementada: `Warnings` con `Enrollments.DocumentsPending`; solo los adeudos bloquean. Falta **persistir la fecha límite** por inscripción |
| ~~Validar que el periodo exista en inscripciones/reinscripciones~~ | Resuelto | `EnrollmentService` valida el periodo y devuelve `AcademicTerms.NotFound` |

## Fase 3 — Seguimiento académico (completada)

| Módulo | Pendiente | Estado |
|---|---|---|
| ReEnrollments (13) | Proponer materias elegibles y crear la reinscripción | ✅ Propuesta, alta y pantalla `/re-enrollments` |
| Grades (14) | Captura masiva por grupo con validación de rango | ✅ API, 5 pruebas, HTTP y pantalla `/grades` |
| Transcript (15) | Kardex por alumno ordenado por cuatrimestre | ✅ API, 2 pruebas, HTTP y pantalla `/tracking` |
| AcademicProgress (16) | Avance académico por alumno | ✅ API, 2 pruebas, HTTP y pantalla `/tracking` |

## Fase 4 — Finanzas y complementarios (completada)

| Módulo | Pendiente |
|---|---|
| Payments (17) | Conceptos, planes de pago y cobros | ✅ API (conceptos, planes, cargos con cancelación), 5 pruebas, HTTP y pantallas `/payment-concepts` y `/charges` |
| Procedures (18) | Trámites con folio | ✅ API (tipos, folio, emisión), 5 pruebas, HTTP y pantallas `/procedures` y `/procedure-types` |
| Notifications (19) | Plantillas con variables | ✅ API, 4 pruebas, HTTP y pantalla `/notifications` |
| Reports (20) | Dashboard con indicadores | ✅ API, prueba del dashboard, HTTP y pantalla `/dashboard` |
| ~~Procedures: `UserId` de la solicitud~~ | Resuelto | `ProcedureService` usa `ICurrentUser` (y falla con `Procedures.UserRequired` si no hay usuario); verificado por HTTP |
