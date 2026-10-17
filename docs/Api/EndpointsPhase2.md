# Endpoints — Fase 2 — Operación académica

Prefijo `/api/v1`. Respuesta estándar `ApiResponse<T>` (ver `Api/Conventions.md`).

## 08. Material didáctico — `CourseMaterialsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/course-materials` | Listar material (paginado, búsqueda, orden) | `CourseMaterials.View` |
| GET | `/api/v1/course-materials/{Id}` | Detalle | `CourseMaterials.View` |
| POST | `/api/v1/course-materials` | Crear material | `CourseMaterials.Create` |
| PUT | `/api/v1/course-materials/{Id}` | Editar material | `CourseMaterials.Edit` |
| DELETE | `/api/v1/course-materials/{Id}` | Baja lógica | `CourseMaterials.Delete` |
| POST | `/api/v1/course-materials/{Id}/file` | Subir archivo | `CourseMaterials.Create` |

## 09. Docentes — `TeachersController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/teachers` | Listar docentes (paginado, búsqueda, orden) | `Teachers.View` |
| GET | `/api/v1/teachers/{Id}` | Detalle | `Teachers.View` |
| POST | `/api/v1/teachers` | Crear docentes | `Teachers.Create` |
| PUT | `/api/v1/teachers/{Id}` | Editar docentes | `Teachers.Edit` |
| DELETE | `/api/v1/teachers/{Id}` | Baja lógica | `Teachers.Delete` |
| PUT | `/api/v1/teachers/{Id}/subjects` | Asignar materias habilitadas | `Teachers.Edit` |

## 10. Grupos y horarios — `ClassGroupsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/class-groups` | Listar grupos (paginado, búsqueda, orden) | `ClassGroups.View` |
| GET | `/api/v1/class-groups/{Id}` | Detalle | `ClassGroups.View` |
| POST | `/api/v1/class-groups` | Crear grupos | `ClassGroups.Create` |
| PUT | `/api/v1/class-groups/{Id}` | Editar grupos | `ClassGroups.Edit` |
| DELETE | `/api/v1/class-groups/{Id}` | Baja lógica | `ClassGroups.Delete` |
| PUT | `/api/v1/class-groups/{Id}/schedules` | Definir horarios | `ClassGroups.Edit` |

## 11. Alumnos y documentación — `StudentsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students` | Listar alumnos (paginado, búsqueda, orden) | `Students.View` |
| GET | `/api/v1/students/{Id}` | Detalle | `Students.View` |
| POST | `/api/v1/students` | Crear alumnos | `Students.Create` |
| PUT | `/api/v1/students/{Id}` | Editar alumnos | `Students.Edit` |
| DELETE | `/api/v1/students/{Id}` | Baja lógica | `Students.Delete` |
| GET | `/api/v1/students/{Id}/documents` | Documentos del alumno | `StudentDocuments.View` |
| POST | `/api/v1/students/{Id}/documents/{DocumentTypeId}` | Cargar documento | `StudentDocuments.Upload` |
| POST | `/api/v1/students/{Id}/documents/{DocumentTypeId}/validate` | Validar documento | `StudentDocuments.Validate` |
| POST | `/api/v1/students/{Id}/documents/{DocumentTypeId}/reject` | Rechazar documento | `StudentDocuments.Reject` |
| PATCH | `/api/v1/students/{Id}/status` | Cambiar estatus | `Students.Edit` |
| GET | `/api/v1/document-types` | Catálogo de tipos de documento | `StudentDocuments.View` |

## 12. Inscripción (primer ingreso) — `EnrollmentsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/enrollments` | Listar (filtros periodo, carrera, alumno) | `Enrollments.View` |
| GET | `/api/v1/enrollments/{Id}` | Detalle | `Enrollments.View` |
| POST | `/api/v1/enrollments` | Inscribir alumno | `Enrollments.Create` |
| POST | `/api/v1/enrollments/{Id}/cancel` | Cancelar inscripción | `Enrollments.Cancel` |
| GET | `/api/v1/students/{Id}/enrollment-eligibility` | Validar requisitos (documentos y pago) | `Enrollments.View` |
