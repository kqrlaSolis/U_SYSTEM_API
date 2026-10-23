# Fases de desarrollo

Orden recomendado dentro de cada fase: **base de datos → API → desktop (MVVM) → pruebas → cierre**.

## Fase 0 — Cimientos

- [x] Solución de Visual Studio `USystem.sln` con proyectos `USystem.Api`, `USystem.Application`, `USystem.Domain`, `USystem.Infrastructure` y `USystem.Tests`.
- [x] Conexión a MySQL por cadena de conexión (`ConnectionStrings:DefaultConnection`), Pomelo + EF Core 8 (servidor MySQL 8.4).
- [x] Scripts `00_CreateDatabase` y `99_SeedData` ejecutados (37 tablas, 2 vistas); estructura de capas con DI por módulo (`AddApplication`, `AddInfrastructure`, `AddSecurityModule`, `AddHealthModule`).
- [x] Middleware global de errores, `ApiResponse<T>`, paginación, Swagger y Serilog.
- [x] JWT y atributo `[RequirePermission]`.
- [x] Almacenamiento S3 (MinIO en desarrollo) y catálogos i18n base (`es`/`en`) de la API y del desktop.
- [x] Proyecto `desktop/` con Electron + React TS + AXZY UI (Tailwind v4, capas de CSS, `ITThemeProvider` en `main.tsx` y chasis `ITLayout` + `ITPage`), router, cliente HTTP con interceptores y estructura MVVM (verificado con `tsc --noEmit`, `vite build`, ESLint y 21 pruebas de Vitest).
- [x] Pipeline de build y convenciones de código (ver `Guides/CodingConventions.md`).

> **Pendiente de Fase 0:** el código de almacenamiento S3 está listo, pero el bucket
> `cfsp-s3-bucket-prod` responde `403` a `head_bucket` y `list USystem/` con el usuario IAM
> `axzydev`. Falta aplicar la política de identidad y la *bucket policy* descritas en
> [`Operations/Secrets.md`](../Operations/Secrets.md). Mientras tanto el desktop usa los envoltorios de
> `shared/components` y la API responde `Storage.NotConfigured` solo si falta el bucket.

## Fase 1 — MVP: seguridad y catálogos

| Módulo | Documento | Entregable |
|---|---|---|
| Usuarios y autenticación | [01_Users](../Modules/01_Users.md) | Login con usuario o correo y contraseña (access token JWT + refresh token). |
| Roles y permisos (RBAC) | [02_Roles](../Modules/02_Roles.md) | CRUD de roles. |
| Auditoría y bitácora | [03_AuditLogs](../Modules/03_AuditLogs.md) | Registro automático desde la capa de servicio (`IAuditService`). |
| Carreras y programas | [04_Careers](../Modules/04_Careers.md) | CRUD de carreras. |
| Planes de estudio | [05_StudyPlans](../Modules/05_StudyPlans.md) | CRUD de planes y versiones. |
| Materias | [06_Subjects](../Modules/06_Subjects.md) | CRUD de materias. |
| Periodos cuatrimestrales | [07_AcademicTerms](../Modules/07_AcademicTerms.md) | CRUD de periodos. |
| Configuración del sistema | [21_Settings](../Modules/21_Settings.md) | Listado y edición de parámetros. |

> **Progreso:** los **ocho módulos de la Fase 1** están implementados en la API y probados contra MySQL
> (119 pruebas .NET). El desktop conecta **todos los módulos**: login, perfil, contraseña, recuperación
> (/forgot-password, /reset-password), usuarios, roles, configuración, carreras, materias, periodos,
> planes con malla y seriación, y bitácora (32 pruebas de Vitest). Pendiente para cerrar: pruebas de
> ViewModel de periodos/planes, integración final y revisión de seguridad de la fase.

## Fase 2 — Operación académica

| Módulo | Documento | Entregable |
|---|---|---|
| Material didáctico | [08_CourseMaterials](../Modules/08_CourseMaterials.md) | CRUD de material. |
| Docentes | [09_Teachers](../Modules/09_Teachers.md) | CRUD de docentes. |
| Grupos y horarios | [10_ClassGroups](../Modules/10_ClassGroups.md) | CRUD de grupos y horarios. |
| Alumnos y documentación | [11_Students](../Modules/11_Students.md) | CRUD de alumnos con generación de matrícula (`StudentNumber`). |
| Inscripción (primer ingreso) | [12_Enrollments](../Modules/12_Enrollments.md) | Crear inscripción (`Type = Enrollment`) y asignar grupos. |

## Fase 3 — Seguimiento académico (completada)

| Módulo | Documento | Entregable |
|---|---|---|
| Reinscripción | [13_ReEnrollments](../Modules/13_ReEnrollments.md) | Proponer materias elegibles. |
| Calificaciones | [14_Grades](../Modules/14_Grades.md) | Captura masiva por grupo. |
| Kardex | [15_Transcript](../Modules/15_Transcript.md) | Consulta por alumno ordenada por cuatrimestre. |
| Avance académico | [16_AcademicProgress](../Modules/16_AcademicProgress.md) | Consulta por alumno. |

## Fase 4 — Finanzas y complementarios (completada)

| Módulo | Documento | Entregable |
|---|---|---|
| Pagos y colegiaturas | [17_Payments](../Modules/17_Payments.md) | Catálogo de conceptos y planes de pago. |
| Trámites y constancias | [18_Procedures](../Modules/18_Procedures.md) | Solicitud con folio. |
| Notificaciones | [19_Notifications](../Modules/19_Notifications.md) | Plantillas con variables. |
| Reportes y dashboard | [20_Reports](../Modules/20_Reports.md) | Dashboard con indicadores clave. |

## Futuro (fuera de alcance)

App del alumno. Ver `Architecture/StudentAppReadiness.md`.
