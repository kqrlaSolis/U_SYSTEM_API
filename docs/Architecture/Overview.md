# Visión general

U SYSTEM administra el ciclo académico y financiero de una universidad con **periodos cuatrimestrales**
(ene–abr, may–ago, sep–dic). Es una aplicación de escritorio para el personal de la escuela; el alumno
**no** es usuario del sistema.

```
desktop/ (Electron + React)  ──HTTP/JSON──▶  api/ (.NET 8)  ──EF Core──▶  MySQL 8
                                                  └──────────▶  S3 (documentos)
```

## Stack

| Capa | Tecnología |
|---|---|
| API | .NET 8, ASP.NET Core Web API, Swagger (Swashbuckle) |
| Datos | MySQL 8, EF Core 8 + Pomelo; esquema en [`database/`](../../database) |
| Autenticación | JWT (access + refresh token), contraseñas con BCrypt |
| Validación | FluentValidation |
| Archivos | AWS S3 (`AWSSDK.S3`) |
| Logs | Serilog |
| Pruebas API | xUnit + Moq, `Microsoft.AspNetCore.Mvc.Testing` |
| Desktop | Electron 38, React 19, TypeScript 5, Vite 7 |
| UI | `@axzydev/axzy_ui_system` + Tailwind CSS v4 |
| i18n | Recursos JSON en la API; `react-i18next` en el desktop |
| Pruebas desktop | Vitest + React Testing Library |

## Conceptos clave

| Concepto | Descripción |
|---|---|
| **User** | Personal de la escuela con rol y permisos; es quien inicia sesión. |
| **Student** | Alumno con expediente, matrícula, kardex y pagos. No inicia sesión. |
| **AcademicTerm** | Cuatrimestre con estado `Planned → Open → InProgress → Closed`. |
| **ClassGroup** | Materia + periodo + docente + horario + aula + cupo. |
| **Enrollment** | Inscripción o reinscripción (`Type = Enrollment` o `ReEnrollment`). |
| **Transcript** | Kardex del alumno, calculado a partir de `Grades`. |

## Módulos

Cada módulo tiene su controller en la API y su carpeta en `desktop/src/modules/`.

| Área | Módulos |
|---|---|
| Base | Users, Roles, AuditLogs, Settings |
| Catálogos | Careers, StudyPlans, Subjects, AcademicTerms, CourseMaterials |
| Operación | Teachers, ClassGroups, Students, Enrollments, ReEnrollments |
| Seguimiento | Grades, Transcript, AcademicProgress |
| Finanzas y otros | Payments, Procedures, Notifications, Reports |

## Principios

1. **La API es la autoridad.** La UI oculta opciones, pero cada endpoint valida el permiso.
2. **Las reglas de negocio van en el servicio.** Controllers y repositorios no deciden reglas.
3. **Todo el código en inglés y PascalCase**, con comentarios en español ([`Conventions/Code.md`](../Conventions/Code.md)).
4. **Borrado lógico** en catálogos y expedientes; los pagos solo se cancelan.
5. **Se audita lo crítico** desde la capa de servicio.
