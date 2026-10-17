# Endpoints — Fase 3 — Seguimiento académico

Prefijo `/api/v1`. Respuesta estándar `ApiResponse<T>` (ver `Api/Conventions.md`).

## 13. Reinscripción — `ReEnrollmentsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students/{Id}/reenrollment-options` | Materias y grupos elegibles | `ReEnrollments.View` |
| POST | `/api/v1/reenrollments` | Reinscribir | `ReEnrollments.Create` |
| POST | `/api/v1/reenrollments/{Id}/cancel` | Cancelar | `ReEnrollments.Cancel` |
| POST | `/api/v1/reenrollments/{Id}/authorize-exception` | Autorizar excepción | `ReEnrollments.AuthorizeException` |

## 14. Calificaciones — `GradesController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/class-groups/{Id}/grades` | Lista de calificaciones del grupo | `Grades.View` |
| PUT | `/api/v1/class-groups/{Id}/grades` | Capturar calificaciones (lote) | `Grades.Capture` |
| PATCH | `/api/v1/grades/{Id}` | Modificar calificación | `Grades.Modify` |
| POST | `/api/v1/class-groups/{Id}/close-report` | Cerrar acta | `Grades.CloseReport` |
| POST | `/api/v1/class-groups/{Id}/reopen-report` | Reabrir acta | `Grades.ReopenReport` |

## 15. Kardex — `TranscriptController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students/{Id}/transcript` | Kardex del alumno | `Transcript.View` |
| GET | `/api/v1/students/{Id}/transcript/pdf` | Kardex en PDF | `Transcript.Export` |

## 16. Avance académico — `AcademicProgressController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students/{Id}/academic-progress` | Avance del alumno | `AcademicProgress.View` |
| GET | `/api/v1/academic-progress` | Listado con semáforo (filtros) | `AcademicProgress.View` |
