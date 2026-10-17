# Endpoints — Fase 1 — MVP: seguridad y catálogos

Prefijo `/api/v1`. Respuesta estándar `ApiResponse<T>` (ver `Api/Conventions.md`).

## 01. Usuarios y autenticación — `UsersController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Iniciar sesión | `Public` |
| POST | `/api/v1/auth/refresh` | Renovar token | `Public` |
| POST | `/api/v1/auth/logout` | Cerrar sesión | `Authenticated` |
| POST | `/api/v1/auth/forgot-password` | Solicitar recuperación | `Public` |
| POST | `/api/v1/auth/reset-password` | Restablecer con token | `Public` |
| POST | `/api/v1/auth/change-password` | Cambiar contraseña | `Authenticated` |
| GET | `/api/v1/auth/me` | Datos y permisos del usuario actual | `Authenticated` |
| GET | `/api/v1/users` | Listar usuarios (paginado) | `Users.View` |
| GET | `/api/v1/users/{Id}` | Detalle | `Users.View` |
| POST | `/api/v1/users` | Crear usuario | `Users.Create` |
| PUT | `/api/v1/users/{Id}` | Editar usuario | `Users.Edit` |
| PATCH | `/api/v1/users/{Id}/status` | Activar o desactivar | `Users.Edit` |
| POST | `/api/v1/users/{Id}/reset-password` | Forzar contraseña temporal | `Users.Edit` |

## 02. Roles y permisos (RBAC) — `RolesController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/roles` | Listar roles (paginado, búsqueda, orden) | `Roles.View` |
| GET | `/api/v1/roles/{Id}` | Detalle | `Roles.View` |
| POST | `/api/v1/roles` | Crear roles | `Roles.Create` |
| PUT | `/api/v1/roles/{Id}` | Editar roles | `Roles.Edit` |
| DELETE | `/api/v1/roles/{Id}` | Baja lógica | `Roles.Delete` |
| GET | `/api/v1/permissions` | Catálogo de permisos por módulo | `Roles.View` |
| PUT | `/api/v1/users/{Id}/roles` | Asignar roles a un usuario | `Roles.Assign` |

## 03. Auditoría y bitácora — `AuditLogsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/audit-logs` | Consultar bitácora (filtros y paginado) | `AuditLogs.View` |
| GET | `/api/v1/audit-logs/{Id}` | Detalle con valores anterior y nuevo | `AuditLogs.View` |

## 04. Carreras y programas — `CareersController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/careers` | Listar carreras (paginado, búsqueda, orden) | `Careers.View` |
| GET | `/api/v1/careers/{Id}` | Detalle | `Careers.View` |
| POST | `/api/v1/careers` | Crear carreras | `Careers.Create` |
| PUT | `/api/v1/careers/{Id}` | Editar carreras | `Careers.Edit` |
| DELETE | `/api/v1/careers/{Id}` | Baja lógica | `Careers.Delete` |

## 05. Planes de estudio — `StudyPlansController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/study-plans` | Listar planes (paginado, búsqueda, orden) | `StudyPlans.View` |
| GET | `/api/v1/study-plans/{Id}` | Detalle | `StudyPlans.View` |
| POST | `/api/v1/study-plans` | Crear planes | `StudyPlans.Create` |
| PUT | `/api/v1/study-plans/{Id}` | Editar planes | `StudyPlans.Edit` |
| DELETE | `/api/v1/study-plans/{Id}` | Baja lógica | `StudyPlans.Delete` |
| PUT | `/api/v1/study-plans/{Id}/subjects` | Reemplazar materias del plan | `StudyPlans.Edit` |
| PUT | `/api/v1/study-plans/{Id}/prerequisites` | Definir seriación | `StudyPlans.Edit` |
| POST | `/api/v1/study-plans/{Id}/set-current` | Marcar como vigente | `StudyPlans.Edit` |

## 06. Materias — `SubjectsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/subjects` | Listar materias (paginado, búsqueda, orden) | `Subjects.View` |
| GET | `/api/v1/subjects/{Id}` | Detalle | `Subjects.View` |
| POST | `/api/v1/subjects` | Crear materias | `Subjects.Create` |
| PUT | `/api/v1/subjects/{Id}` | Editar materias | `Subjects.Edit` |
| DELETE | `/api/v1/subjects/{Id}` | Baja lógica | `Subjects.Delete` |

## 07. Periodos cuatrimestrales — `AcademicTermsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/academic-terms` | Listar periodos | `AcademicTerms.View` |
| GET | `/api/v1/academic-terms/{Id}` | Detalle | `AcademicTerms.View` |
| POST | `/api/v1/academic-terms` | Crear periodo | `AcademicTerms.Create` |
| PUT | `/api/v1/academic-terms/{Id}` | Editar periodo | `AcademicTerms.Edit` |
| POST | `/api/v1/academic-terms/{Id}/open` | Abrir periodo | `AcademicTerms.Open` |
| POST | `/api/v1/academic-terms/{Id}/close` | Cerrar periodo | `AcademicTerms.Close` |
| GET | `/api/v1/academic-terms/current` | Periodo en curso | `AcademicTerms.View` |

## 21. Configuración del sistema — `SettingsController`

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/settings` | Listar parámetros | `Settings.View` |
| PUT | `/api/v1/settings/{Key}` | Editar valor | `Settings.Edit` |
