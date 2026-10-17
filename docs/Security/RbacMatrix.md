# Matriz RBAC

Generada desde el catálogo semilla (`99_SeedData.sql`). Leyenda: **Full** = todas las acciones · **View** = solo lectura · lista = acciones concedidas · — = sin acceso.

| Módulo | Administrador | Control escolar | Finanzas / Caja | Coordinador de carrera | Docente | Consulta |
|---|---|---|---|---|---|---|
| `Users` | Full | — | — | — | — | View |
| `Roles` | Full | — | — | — | — | View |
| `AuditLogs` | View | — | — | — | — | View |
| `Careers` | Full | View | View | Full | View | View |
| `StudyPlans` | Full | View | — | Full | — | View |
| `Subjects` | Full | View | — | Full | View | View |
| `AcademicTerms` | Full | View | View | View | View | View |
| `CourseMaterials` | Full | View | — | Full | View | View |
| `Teachers` | Full | View, Create, Edit | — | Full | View | View |
| `ClassGroups` | Full | View | — | Full | View | View |
| `Students` | Full | Full | View | View | View | View |
| `StudentDocuments` | Full | Full | View | View | — | View |
| `Enrollments` | Full | Full | View | View | — | View |
| `ReEnrollments` | Full | View, Create, Cancel | View | View | — | View |
| `Grades` | Full | View | View | View | View, Capture | View |
| `Transcript` | Full | Full | View | View | — | View |
| `AcademicProgress` | View | View | View | View | — | View |
| `Payments` | Full | View | Full | — | — | View |
| `Scholarships` | Full | View | Full | — | — | View |
| `Procedures` | Full | Full | View | — | — | View |
| `Notifications` | Full | — | View | View | — | View |
| `Reports` | Full | View | Full | Full | — | View |
| `Settings` | Full | — | — | — | — | View |

## Roles

| Rol | Equivalente | Descripción |
|---|---|---|
| `Administrator` | Administrador | Acceso total, gestión de usuarios y roles. |
| `Registrar` | Control escolar | Alumnos, documentación, inscripciones, reinscripciones, kardex y trámites. |
| `Finance` | Finanzas / Caja | Cargos, pagos, becas, recibos y estados de cuenta. |
| `ProgramCoordinator` | Coordinador de carrera | Carreras, planes, materias, docentes, grupos y avance académico. |
| `Teacher` | Docente | Captura de calificaciones de sus grupos. |
| `ReadOnly` | Consulta | Solo lectura. |

## Catálogo de permisos

| Código | Descripción |
|---|---|
| `Users.View` | View users |
| `Users.Create` | Create users |
| `Users.Edit` | Edit users and activate or deactivate accounts |
| `Roles.View` | View roles and permissions |
| `Roles.Create` | Create roles |
| `Roles.Edit` | Edit roles and their permissions |
| `Roles.Delete` | Delete or deactivate roles |
| `Roles.Assign` | Assign roles to users |
| `AuditLogs.View` | View the audit log |
| `Careers.View` | View careers |
| `Careers.Create` | Create careers |
| `Careers.Edit` | Edit careers |
| `Careers.Delete` | Delete or deactivate careers |
| `StudyPlans.View` | View study plans |
| `StudyPlans.Create` | Create study plans |
| `StudyPlans.Edit` | Edit study plans, curriculum and prerequisites |
| `StudyPlans.Delete` | Delete or deactivate study plans |
| `Subjects.View` | View subjects |
| `Subjects.Create` | Create subjects |
| `Subjects.Edit` | Edit subjects |
| `Subjects.Delete` | Delete or deactivate subjects |
| `AcademicTerms.View` | View academic terms |
| `AcademicTerms.Create` | Create academic terms |
| `AcademicTerms.Edit` | Edit academic terms |
| `AcademicTerms.Open` | Open an academic term for enrollment |
| `AcademicTerms.Close` | Close an academic term |
| `CourseMaterials.View` | View course materials |
| `CourseMaterials.Create` | Create course materials |
| `CourseMaterials.Edit` | Edit course materials |
| `CourseMaterials.Delete` | Delete or deactivate course materials |
| `Teachers.View` | View teachers |
| `Teachers.Create` | Create teachers |
| `Teachers.Edit` | Edit teachers and their enabled subjects |
| `Teachers.Delete` | Delete or deactivate teachers |
| `ClassGroups.View` | View class groups and schedules |
| `ClassGroups.Create` | Create class groups |
| `ClassGroups.Edit` | Edit class groups and schedules |
| `ClassGroups.Delete` | Delete or cancel class groups |
| `Students.View` | View students |
| `Students.Create` | Create students |
| `Students.Edit` | Edit students and change their status |
| `Students.Delete` | Delete or deactivate students |
| `StudentDocuments.View` | View student documents |
| `StudentDocuments.Upload` | Upload student documents |
| `StudentDocuments.Validate` | Validate student documents |
| `StudentDocuments.Reject` | Reject student documents |
| `Enrollments.View` | View enrollments |
| `Enrollments.Create` | Create enrollments |
| `Enrollments.Cancel` | Cancel enrollments |
| `ReEnrollments.View` | View re-enrollments |
| `ReEnrollments.Create` | Create re-enrollments |
| `ReEnrollments.Cancel` | Cancel re-enrollments |
| `ReEnrollments.AuthorizeException` | Authorize re-enrollment exceptions |
| `Grades.View` | View grades |
| `Grades.Capture` | Capture grades |
| `Grades.Modify` | Modify grades after capture |
| `Grades.CloseReport` | Close a grade report |
| `Grades.ReopenReport` | Reopen a grade report |
| `Transcript.View` | View transcripts |
| `Transcript.Export` | Export transcripts to PDF |
| `AcademicProgress.View` | View academic progress |
| `Payments.View` | View charges and payments |
| `Payments.Register` | Register payments and generate charges |
| `Payments.Cancel` | Cancel payments |
| `Payments.Waive` | Waive charges |
| `Scholarships.View` | View scholarships |
| `Scholarships.Create` | Create scholarships |
| `Scholarships.Edit` | Edit scholarships |
| `Scholarships.Assign` | Assign scholarships to students |
| `Procedures.View` | View procedures |
| `Procedures.Request` | Request procedures |
| `Procedures.Issue` | Issue procedure documents |
| `Notifications.View` | View notifications and templates |
| `Notifications.Send` | Send notifications and edit templates |
| `Reports.View` | View reports and the dashboard |
| `Reports.Export` | Export reports |
| `Settings.View` | View system settings |
| `Settings.Edit` | Edit system settings |
