# Base de datos

- Motor: **MySQL 8**, InnoDB, `utf8mb4`.
- Tablas y columnas en inglés y **PascalCase** ([`Conventions/Database.md`](../Conventions/Database.md)).
- Los scripts están en [`database/`](../../database): se ejecutan en orden numérico, o todos con `RunAll.sql`.
- La entidad C# de cada tabla está en `api/USystem.Domain/Entities/`.

## Conexión

La API toma la cadena de `ConnectionStrings__DefaultConnection`: en local desde `.env` y en producción
desde las variables de Railway ([`Operations/Environments.md`](../Operations/Environments.md)).

> Hoy local y producción apuntan a la **misma** base de Railway (`railway`). Lo que hagas en local cae en
> datos reales.

## Crear la base

### Base local completa (recomendado para desarrollar)

[`database/LocalSetup/`](../../database/LocalSetup/README.md) crea desde cero una base `USystem` en tu
MySQL local (o en Docker) con esquema, catálogos, administrador, usuario `USystemApp` y datos demo de
todos los módulos, y puede escribir la cadena de conexión en tu `.env`:

```powershell
.\database\LocalSetup\Setup.ps1 -WriteEnv        # Windows
bash database/LocalSetup/Setup.sh --write-env    # macOS / Linux
```

### Solo los scripts

Con [`tools/ApplyMySqlScripts.py`](../../tools/README.md), sin necesitar el cliente `mysql`:

```bash
# Base local nueva (crea la base USystem)
python tools/ApplyMySqlScripts.py --url mysql://root:TU_CLAVE@localhost:3306
# Dentro de una base que ya existe (omite CREATE DATABASE y USE)
python tools/ApplyMySqlScripts.py --target-database railway
```

Con cliente `mysql`:

```bash
cd database
mysql -u root -p < RunAll.sql
```

> **Credencial expuesta.** La contraseña de `root` de MySQL en Railway se subió en el primer commit
> (`d6fe7c8`, en `docs/DataModel/README.md`) y se quitó en `7fda7b4`, pero sigue en el historial de git.
> Hasta rotarla se considera comprometida. Antes de producción: rotarla y crear el usuario `USystemApp`
> con permisos mínimos (`SELECT`, `INSERT`, `UPDATE`, `DELETE`). Ver
> [`Operations/Secrets.md`](../Operations/Secrets.md).

## Scripts

| Script | Contenido |
|---|---|
| `01_Security.sql` | Seguridad, roles, permisos y auditoría |
| `02_AcademicCatalogs.sql` | Catálogos académicos |
| `03_AcademicOperations.sql` | Operación académica |
| `04_AcademicTracking.sql` | Seguimiento académico |
| `05_Finance.sql` | Finanzas |
| `06_Extras.sql` | Trámites, notificaciones y configuración |

## Tablas por script

### `01_Security.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| `Users` | `User` | Users |
| `RefreshTokens` | `RefreshToken` | RefreshTokens |
| `PasswordResetTokens` | `PasswordResetToken` | PasswordResetTokens |
| `Roles` | `Role` | Roles |
| `Permissions` | `Permission` | Permissions |
| `RolePermissions` | `RolePermission` | RolePermissions |
| `UserRoles` | `UserRole` | UserRoles |
| `AuditLogs` | `AuditLog` | AuditLogs |

### `02_AcademicCatalogs.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| `Careers` | `Career` | Careers |
| `Subjects` | `Subject` | Subjects |
| `AcademicTerms` | `AcademicTerm` | AcademicTerms |
| `StudyPlans` | `StudyPlan` | StudyPlans |
| `StudyPlanSubjects` | `StudyPlanSubject` | StudyPlanSubjects |
| `SubjectPrerequisites` | `SubjectPrerequisite` | SubjectPrerequisites |
| `CourseMaterials` | `CourseMaterial` | CourseMaterials |

### `03_AcademicOperations.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| `Teachers` | `Teacher` | Teachers |
| `TeacherSubjects` | `TeacherSubject` | TeacherSubjects |
| `ClassGroups` | `ClassGroup` | ClassGroups |
| `ClassGroupSchedules` | `ClassGroupSchedule` | ClassGroupSchedules |
| `Students` | `Student` | Students |
| `StudentGuardians` | `StudentGuardian` | StudentGuardians |
| `DocumentTypes` | `DocumentType` | DocumentTypes |
| `StudentDocuments` | `StudentDocument` | StudentDocuments |
| `Enrollments` | `Enrollment` | Enrollments |
| `EnrollmentClassGroups` | `EnrollmentClassGroup` | EnrollmentClassGroups |

### `04_AcademicTracking.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| `Grades` | `Grade` | Grades |

### `05_Finance.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| `PaymentConcepts` | `PaymentConcept` | PaymentConcepts |
| `PaymentPlans` | `PaymentPlan` | PaymentPlans |
| `Scholarships` | `Scholarship` | Scholarships |
| `StudentScholarships` | `StudentScholarship` | StudentScholarships |
| `Charges` | `Charge` | Charges |
| `Payments` | `Payment` | Payments |

### `06_Extras.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| `ProcedureTypes` | `ProcedureType` | ProcedureTypes |
| `Procedures` | `Procedure` | Procedures |
| `NotificationTemplates` | `NotificationTemplate` | NotificationTemplates |
| `Notifications` | `Notification` | Notifications |
| `SystemSettings` | `SystemSetting` | SystemSettings |

## Vistas

- `TranscriptView`: kardex por alumno (materias cursadas, calificación, créditos y estatus).
- `AcademicProgressView`: créditos aprobados, porcentaje de avance y promedio por alumno.

## Diagrama del núcleo

```mermaid
erDiagram
    Users ||--o{ UserRoles : has
    Roles ||--o{ UserRoles : has
    Roles ||--o{ RolePermissions : grants
    Permissions ||--o{ RolePermissions : in
    Careers ||--o{ StudyPlans : has
    StudyPlans ||--o{ StudyPlanSubjects : contains
    Subjects ||--o{ StudyPlanSubjects : in
    Subjects ||--o{ ClassGroups : taught_as
    AcademicTerms ||--o{ ClassGroups : offers
    Teachers ||--o{ ClassGroups : teaches
    Careers ||--o{ Students : enrolls
    StudyPlans ||--o{ Students : follows
    Students ||--o{ StudentDocuments : submits
    DocumentTypes ||--o{ StudentDocuments : of
    Students ||--o{ Enrollments : makes
    AcademicTerms ||--o{ Enrollments : in
    Enrollments ||--o{ EnrollmentClassGroups : includes
    ClassGroups ||--o{ EnrollmentClassGroups : in
    Enrollments ||--o{ Grades : produces
    ClassGroups ||--o{ Grades : evaluates
    Students ||--o{ Charges : owes
    PaymentConcepts ||--o{ Charges : of
    Charges ||--o{ Payments : settled_by
    Students ||--o{ Procedures : requests
```

Total: **37 tablas** y **2 vistas**.
