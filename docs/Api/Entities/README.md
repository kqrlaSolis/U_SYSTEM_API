# Entidades C#

Namespace: `USystem.Domain.Entities`. Todas heredan de `AuditableEntity`.

> Este catálogo se genera desde `Schema.py` y se escribe también en
> `api/USystem.Domain/Entities/`, que es el código real de la solución.

| Entidad | Tabla | Script | Documento |
|---|---|---|---|
| `User` | `Users` | `01_Security.sql` | [Ver](../../DataModel/Entities/User.md) |
| `RefreshToken` | `RefreshTokens` | `01_Security.sql` | [Ver](../../DataModel/Entities/RefreshToken.md) |
| `PasswordResetToken` | `PasswordResetTokens` | `01_Security.sql` | [Ver](../../DataModel/Entities/PasswordResetToken.md) |
| `Role` | `Roles` | `01_Security.sql` | [Ver](../../DataModel/Entities/Role.md) |
| `Permission` | `Permissions` | `01_Security.sql` | [Ver](../../DataModel/Entities/Permission.md) |
| `RolePermission` | `RolePermissions` | `01_Security.sql` | [Ver](../../DataModel/Entities/RolePermission.md) |
| `UserRole` | `UserRoles` | `01_Security.sql` | [Ver](../../DataModel/Entities/UserRole.md) |
| `AuditLog` | `AuditLogs` | `01_Security.sql` | [Ver](../../DataModel/Entities/AuditLog.md) |
| `Career` | `Careers` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/Career.md) |
| `Subject` | `Subjects` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/Subject.md) |
| `AcademicTerm` | `AcademicTerms` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/AcademicTerm.md) |
| `StudyPlan` | `StudyPlans` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/StudyPlan.md) |
| `StudyPlanSubject` | `StudyPlanSubjects` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/StudyPlanSubject.md) |
| `SubjectPrerequisite` | `SubjectPrerequisites` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/SubjectPrerequisite.md) |
| `CourseMaterial` | `CourseMaterials` | `02_AcademicCatalogs.sql` | [Ver](../../DataModel/Entities/CourseMaterial.md) |
| `Teacher` | `Teachers` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/Teacher.md) |
| `TeacherSubject` | `TeacherSubjects` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/TeacherSubject.md) |
| `ClassGroup` | `ClassGroups` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/ClassGroup.md) |
| `ClassGroupSchedule` | `ClassGroupSchedules` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/ClassGroupSchedule.md) |
| `Student` | `Students` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/Student.md) |
| `StudentGuardian` | `StudentGuardians` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/StudentGuardian.md) |
| `DocumentType` | `DocumentTypes` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/DocumentType.md) |
| `StudentDocument` | `StudentDocuments` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/StudentDocument.md) |
| `Enrollment` | `Enrollments` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/Enrollment.md) |
| `EnrollmentClassGroup` | `EnrollmentClassGroups` | `03_AcademicOperations.sql` | [Ver](../../DataModel/Entities/EnrollmentClassGroup.md) |
| `Grade` | `Grades` | `04_AcademicTracking.sql` | [Ver](../../DataModel/Entities/Grade.md) |
| `PaymentConcept` | `PaymentConcepts` | `05_Finance.sql` | [Ver](../../DataModel/Entities/PaymentConcept.md) |
| `PaymentPlan` | `PaymentPlans` | `05_Finance.sql` | [Ver](../../DataModel/Entities/PaymentPlan.md) |
| `Scholarship` | `Scholarships` | `05_Finance.sql` | [Ver](../../DataModel/Entities/Scholarship.md) |
| `StudentScholarship` | `StudentScholarships` | `05_Finance.sql` | [Ver](../../DataModel/Entities/StudentScholarship.md) |
| `Charge` | `Charges` | `05_Finance.sql` | [Ver](../../DataModel/Entities/Charge.md) |
| `Payment` | `Payments` | `05_Finance.sql` | [Ver](../../DataModel/Entities/Payment.md) |
| `ProcedureType` | `ProcedureTypes` | `06_Extras.sql` | [Ver](../../DataModel/Entities/ProcedureType.md) |
| `Procedure` | `Procedures` | `06_Extras.sql` | [Ver](../../DataModel/Entities/Procedure.md) |
| `NotificationTemplate` | `NotificationTemplates` | `06_Extras.sql` | [Ver](../../DataModel/Entities/NotificationTemplate.md) |
| `Notification` | `Notifications` | `06_Extras.sql` | [Ver](../../DataModel/Entities/Notification.md) |
| `SystemSetting` | `SystemSettings` | `06_Extras.sql` | [Ver](../../DataModel/Entities/SystemSetting.md) |

Total: **37 entidades**.
