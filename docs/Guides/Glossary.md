# Glosario (dominio en español → código en inglés)

| Español | Código (inglés) |
|---------|-----------------|
| Usuario (personal) | `User` |
| Rol / Permiso | `Role` / `Permission` |
| Bitácora | `AuditLog` |
| Carrera | `Career` |
| Plan de estudios | `StudyPlan` |
| Materia / Asignatura | `Subject` |
| Seriación / Prerrequisito | `SubjectPrerequisite` |
| Periodo (cuatrimestre calendario) | `AcademicTerm` |
| Cuatrimestre del plan | `TermNumber` |
| Material didáctico | `CourseMaterial` |
| Docente | `Teacher` |
| Grupo | `ClassGroup` |
| Horario | `ClassGroupSchedule` |
| Alumno | `Student` |
| Matrícula | `StudentNumber` |
| Tutor | `StudentGuardian` |
| Documento / Tipo de documento | `StudentDocument` / `DocumentType` |
| Inscripción | `Enrollment` |
| Reinscripción | `ReEnrollment` |
| Calificación | `Grade` |
| Acta de calificaciones | `GradeReport` (`IsReportClosed`) |
| Ordinario / Extraordinario / Regularización | `Ordinary` / `Extraordinary` / `Remedial` |
| Kardex | `Transcript` |
| Avance académico | `AcademicProgress` |
| Concepto de pago | `PaymentConcept` |
| Plan de pagos | `PaymentPlan` |
| Cargo | `Charge` |
| Pago | `Payment` |
| Recibo / Folio | `ReceiptNumber` / `FolioNumber` |
| Recargo | `Surcharge` |
| Beca | `Scholarship` |
| Trámite / Constancia | `Procedure` |
| Notificación | `Notification` |
| Configuración | `SystemSetting` |

## Estados y valores de catálogo

| Español | Código |
|---|---|
| Activo / Inactivo | `Active` / `Inactive` (`IsActive`) |
| Planeado / Abierto / En curso / Cerrado | `Planned` / `Open` / `InProgress` / `Closed` |
| Pendiente / Entregado / Validado / Rechazado | `Pending` / `Delivered` / `Validated` / `Rejected` |
| Aprobado / Reprobado / Sin derecho / No presentado | `Passed` / `Failed` / `NoRight` / `NotPresented` |
| Pendiente / Parcial / Pagado / Vencido / Cancelado | `Pending` / `Partial` / `Paid` / `Overdue` / `Cancelled` |
| Solicitado / En proceso / Emitido / Entregado / Cancelado | `Requested` / `InProcess` / `Issued` / `Delivered` / `Cancelled` |
| Aspirante / Activo / Baja temporal / Baja definitiva / Egresado / Titulado | `Applicant` / `Active` / `TemporaryLeave` / `Dropped` / `Graduated` / `Titled` |

## Aclaración importante

**Alumno (`Student`) no es usuario (`User`).** El alumno es una entidad de negocio con expediente,
matrícula, kardex y pagos; no inicia sesión en U SYSTEM. Los usuarios son el personal de la escuela.
