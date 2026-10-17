# Visión general

## Propósito

U SYSTEM administra el ciclo académico y financiero de una universidad con **periodos cuatrimestrales**
(ene–abr, may–ago, sep–dic): catálogos académicos, alumnos y documentación, inscripción y reinscripción,
calificaciones, kardex, avance académico y control de pagos y colegiaturas.

## Alcance actual

- Aplicación de escritorio administrativa para el personal de la escuela.
- Usuarios: Administrator, Registrar, Finance, ProgramCoordinator, Teacher y ReadOnly
  (ver `Security/RbacMatrix.md`).

## Fuera de alcance

- App o portal del alumno (ver `Architecture/StudentAppReadiness.md`).
- Pasarela de pagos en línea.
- Facturación electrónica (CFDI).
- Integraciones externas (SEP, bibliotecas, etc.).

## Conceptos clave

| Concepto | Descripción |
|---|---|
| **Student** | Entidad de negocio con expediente, matrícula, kardex y pagos. **No** es usuario del sistema. |
| **User** | Personal de la escuela con rol y permisos; es quien inicia sesión. |
| **AcademicTerm** | Cuatrimestre con calendario y estado `Planned → Open → InProgress → Closed`. |
| **ClassGroup** | Materia + periodo + docente + horario + aula + cupo. |
| **Enrollment** | Inscripción o reinscripción (`Type = Enrollment` o `ReEnrollment`). |
| **Transcript** | Historial académico del alumno, derivado de `Grades`. |

Glosario completo español → inglés en `Guides/Glossary.md`.

## Módulos

| # | Módulo | Capa | Fase |
|---|---|---|---|
| 1 | Users — usuarios y autenticación | Base | 1 |
| 2 | Roles — RBAC | Base | 1 |
| 3 | AuditLogs — auditoría | Base | 1 |
| 4 | Careers — carreras | Catálogo | 1 |
| 5 | StudyPlans — planes de estudio | Catálogo | 1 |
| 6 | Subjects — materias | Catálogo | 1 |
| 7 | AcademicTerms — periodos | Catálogo | 1 |
| 8 | CourseMaterials — material didáctico | Catálogo | 2 |
| 9 | Teachers — docentes | Operación | 2 |
| 10 | ClassGroups — grupos y horarios | Operación | 2 |
| 11 | Students — alumnos y documentación | Operación | 2 |
| 12 | Enrollments — inscripción | Operación | 2 |
| 13 | ReEnrollments — reinscripción | Operación | 3 |
| 14 | Grades — calificaciones | Seguimiento | 3 |
| 15 | Transcript — kardex | Seguimiento | 3 |
| 16 | AcademicProgress — avance | Seguimiento | 3 |
| 17 | Payments — pagos y colegiaturas | Finanzas | 4 |
| 18 | Procedures — trámites | Complementario | 4 |
| 19 | Notifications — notificaciones | Complementario | 4 |
| 20 | Reports — reportes y dashboard | Complementario | 4 |
| 21 | Settings — configuración | Base | 1 |

Detalle por módulo en `Modules/README.md`.

## Principios de diseño

1. **El alumno no es usuario.** `Users` es personal; el acceso del alumno será un mecanismo aparte.
2. **La API es la autoridad.** La UI oculta opciones, pero cada endpoint valida el permiso.
3. **Reglas de negocio en el servicio.** Controladores y repositorios no deciden reglas.
4. **Todo el código en inglés y PascalCase**, con comentarios en español.
5. **Borrado lógico** en catálogos y expedientes; los pagos solo se cancelan.
6. **Auditoría de lo crítico** desde la capa de servicio.
