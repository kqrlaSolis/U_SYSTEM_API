# Módulos de U SYSTEM

Cada módulo tiene su documento con objetivo, reglas, datos, endpoints, estructura MVVM del desktop, plan de construcción y criterios de aceptación.

| # | Módulo | Capa | Fase | Permisos | Documento |
|---|---|---|---|---|---|
| 1 | Usuarios y autenticación | Base | 1 | `Users.*` | [01_Users.md](01_Users.md) |
| 2 | Roles y permisos (RBAC) | Base | 1 | `Roles.*` | [02_Roles.md](02_Roles.md) |
| 3 | Auditoría y bitácora | Base | 1 | `AuditLogs.*` | [03_AuditLogs.md](03_AuditLogs.md) |
| 4 | Carreras y programas | Catálogo | 1 | `Careers.*` | [04_Careers.md](04_Careers.md) |
| 5 | Planes de estudio | Catálogo | 1 | `StudyPlans.*` | [05_StudyPlans.md](05_StudyPlans.md) |
| 6 | Materias | Catálogo | 1 | `Subjects.*` | [06_Subjects.md](06_Subjects.md) |
| 7 | Periodos cuatrimestrales | Catálogo | 1 | `AcademicTerms.*` | [07_AcademicTerms.md](07_AcademicTerms.md) |
| 8 | Material didáctico | Catálogo | 2 | `CourseMaterials.*` | [08_CourseMaterials.md](08_CourseMaterials.md) |
| 9 | Docentes | Operación | 2 | `Teachers.*` | [09_Teachers.md](09_Teachers.md) |
| 10 | Grupos y horarios | Operación | 2 | `ClassGroups.*` | [10_ClassGroups.md](10_ClassGroups.md) |
| 11 | Alumnos y documentación | Operación | 2 | `Students.*` · `StudentDocuments.*` | [11_Students.md](11_Students.md) |
| 12 | Inscripción (primer ingreso) | Operación | 2 | `Enrollments.*` | [12_Enrollments.md](12_Enrollments.md) |
| 13 | Reinscripción | Operación | 3 | `ReEnrollments.*` | [13_ReEnrollments.md](13_ReEnrollments.md) |
| 14 | Calificaciones | Seguimiento | 3 | `Grades.*` | [14_Grades.md](14_Grades.md) |
| 15 | Kardex | Seguimiento | 3 | `Transcript.*` | [15_Transcript.md](15_Transcript.md) |
| 16 | Avance académico | Seguimiento | 3 | `AcademicProgress.*` | [16_AcademicProgress.md](16_AcademicProgress.md) |
| 17 | Pagos y colegiaturas | Finanzas | 4 | `Payments.*` · `Scholarships.*` | [17_Payments.md](17_Payments.md) |
| 18 | Trámites y constancias | Complementario | 4 | `Procedures.*` | [18_Procedures.md](18_Procedures.md) |
| 19 | Notificaciones | Complementario | 4 | `Notifications.*` | [19_Notifications.md](19_Notifications.md) |
| 20 | Reportes y dashboard | Complementario | 4 | `Reports.*` | [20_Reports.md](20_Reports.md) |
| 21 | Configuración del sistema | Base | 1 | `Settings.*` | [21_Settings.md](21_Settings.md) |

## Orden por fase

**Fase 1 — MVP: seguridad y catálogos:** Users, Roles, AuditLogs, Careers, StudyPlans, Subjects, AcademicTerms, Settings

**Fase 2 — Operación académica:** CourseMaterials, Teachers, ClassGroups, Students, Enrollments

**Fase 3 — Seguimiento académico:** ReEnrollments, Grades, Transcript, AcademicProgress

**Fase 4 — Finanzas y complementarios:** Payments, Procedures, Notifications, Reports

## Dependencias

```
Users → Roles → Settings
Careers → StudyPlans ← Subjects
AcademicTerms + Subjects + Teachers → ClassGroups
Students + StudentDocuments + Payments → Enrollments
ClassGroups + Enrollments → ReEnrollments → Grades → Transcript → AcademicProgress
Payments <-> ReEnrollments (un adeudo vencido bloquea la reinscripción)
```
