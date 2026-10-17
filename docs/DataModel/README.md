# Modelo de datos

- Motor: **MySQL 8**, InnoDB, `utf8mb4`.
- Base de datos: `USystem`.
- **PascalCase** en tablas y columnas, en inglés (ver `Conventions.md`).
- Scripts en `Scripts/` (ejecutar con `RunAll.sql` o uno por uno en orden numérico).
- Diccionario de campos en `Entities/` (un archivo por tabla).

## Conexión

Instancia de **desarrollo** en Railway (MySQL 8.4). La API nunca lee esta tabla: toma la cadena de
`ConnectionStrings__DefaultConnection` en `.env`.

| Dato | Valor |
|---|---|
| Host / puerto | `iriguchi.proxy.rlwy.net` / `45128` |
| Base de datos | **`railway`** (la que asigna Railway; el esquema se instala dentro de ella) |
| Usuario | `root` (solo desarrollo) |
| Versión del servidor | `8.4.11` |

> Los scripts crean por sí solos una base llamada `USystem` (líneas `CREATE DATABASE` y `USE`). En esta
> instancia de Railway el esquema se aplicó **dentro de `railway`** omitiendo esas dos sentencias, para que
> todo aparezca al abrir la conexión que entrega Railway. La copia antigua en `USystem` se conserva como
> respaldo y se puede eliminar con `DROP DATABASE USystem;`.

Cadena en formato URI, tal como la entrega Railway:

```
mysql://root:FpbMtyBxtxesmXPskXqJjPhYQAYSOOyN@iriguchi.proxy.rlwy.net:45128/railway
```

Cadena que consume EF Core con Pomelo (`ConnectionStrings__DefaultConnection`):

```
Server=iriguchi.proxy.rlwy.net;Port=45128;Database=railway;User=root;Password=FpbMtyBxtxesmXPskXqJjPhYQAYSOOyN;SslMode=Preferred;AllowPublicKeyRetrieval=True;
```

### Aplicar los scripts

Sin cliente `mysql` instalado (Windows, Python 3):

```bash
python -m pip install pymysql
# Instalación nueva en su propia base (crea USystem)
python tools/ApplyMySqlScripts.py
# Instalación dentro de la base que ya existe (por ejemplo railway)
python tools/ApplyMySqlScripts.py --target-database railway
```

El ejecutor toma la cadena de `.env` (`MYSQL_URL` o `ConnectionStrings__DefaultConnection`), aplica
`00` → `06` y `99_SeedData`, y al final informa tablas, vistas y columnas en minúscula (debe ser `0`).
Con `--target-database` se omite `CREATE DATABASE`/`USE` y todo se crea en la base indicada.

Con cliente `mysql`:

```bash
cd docs/DataModel/Scripts
mysql -h iriguchi.proxy.rlwy.net -P 45128 -u root -p railway < 01_Security.sql
```

> **Credencial expuesta.** Esta contraseña se compartió por chat y quedó en la documentación del
> repositorio, así que se considera comprometida. Antes de producción: rotarla, crear el usuario
> `USystemApp` con permisos mínimos (`SELECT`, `INSERT`, `UPDATE`, `DELETE`) y cargar el secreto desde el
> gestor de secretos. Ver [`Operations/Secrets.md`](../Operations/Secrets.md).

## Scripts

| Script | Contenido | Fase |
|---|---|---|
| `01_Security.sql` | Seguridad, roles, permisos y auditoría | 1 |
| `02_AcademicCatalogs.sql` | Catálogos académicos | 1 |
| `03_AcademicOperations.sql` | Operación académica | 2 |
| `04_AcademicTracking.sql` | Seguimiento académico | 3 |
| `05_Finance.sql` | Finanzas | 4 |
| `06_Extras.sql` | Trámites, notificaciones y configuración | 4 |

## Tablas por script

### `01_Security.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| [`Users`](Entities/User.md) | `User` | Users |
| [`RefreshTokens`](Entities/RefreshToken.md) | `RefreshToken` | RefreshTokens |
| [`PasswordResetTokens`](Entities/PasswordResetToken.md) | `PasswordResetToken` | PasswordResetTokens |
| [`Roles`](Entities/Role.md) | `Role` | Roles |
| [`Permissions`](Entities/Permission.md) | `Permission` | Permissions |
| [`RolePermissions`](Entities/RolePermission.md) | `RolePermission` | RolePermissions |
| [`UserRoles`](Entities/UserRole.md) | `UserRole` | UserRoles |
| [`AuditLogs`](Entities/AuditLog.md) | `AuditLog` | AuditLogs |

### `02_AcademicCatalogs.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| [`Careers`](Entities/Career.md) | `Career` | Careers |
| [`Subjects`](Entities/Subject.md) | `Subject` | Subjects |
| [`AcademicTerms`](Entities/AcademicTerm.md) | `AcademicTerm` | AcademicTerms |
| [`StudyPlans`](Entities/StudyPlan.md) | `StudyPlan` | StudyPlans |
| [`StudyPlanSubjects`](Entities/StudyPlanSubject.md) | `StudyPlanSubject` | StudyPlanSubjects |
| [`SubjectPrerequisites`](Entities/SubjectPrerequisite.md) | `SubjectPrerequisite` | SubjectPrerequisites |
| [`CourseMaterials`](Entities/CourseMaterial.md) | `CourseMaterial` | CourseMaterials |

### `03_AcademicOperations.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| [`Teachers`](Entities/Teacher.md) | `Teacher` | Teachers |
| [`TeacherSubjects`](Entities/TeacherSubject.md) | `TeacherSubject` | TeacherSubjects |
| [`ClassGroups`](Entities/ClassGroup.md) | `ClassGroup` | ClassGroups |
| [`ClassGroupSchedules`](Entities/ClassGroupSchedule.md) | `ClassGroupSchedule` | ClassGroupSchedules |
| [`Students`](Entities/Student.md) | `Student` | Students |
| [`StudentGuardians`](Entities/StudentGuardian.md) | `StudentGuardian` | StudentGuardians |
| [`DocumentTypes`](Entities/DocumentType.md) | `DocumentType` | DocumentTypes |
| [`StudentDocuments`](Entities/StudentDocument.md) | `StudentDocument` | StudentDocuments |
| [`Enrollments`](Entities/Enrollment.md) | `Enrollment` | Enrollments |
| [`EnrollmentClassGroups`](Entities/EnrollmentClassGroup.md) | `EnrollmentClassGroup` | EnrollmentClassGroups |

### `04_AcademicTracking.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| [`Grades`](Entities/Grade.md) | `Grade` | Grades |

### `05_Finance.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| [`PaymentConcepts`](Entities/PaymentConcept.md) | `PaymentConcept` | PaymentConcepts |
| [`PaymentPlans`](Entities/PaymentPlan.md) | `PaymentPlan` | PaymentPlans |
| [`Scholarships`](Entities/Scholarship.md) | `Scholarship` | Scholarships |
| [`StudentScholarships`](Entities/StudentScholarship.md) | `StudentScholarship` | StudentScholarships |
| [`Charges`](Entities/Charge.md) | `Charge` | Charges |
| [`Payments`](Entities/Payment.md) | `Payment` | Payments |

### `06_Extras.sql`

| Tabla | Clase C# | Descripción |
|---|---|---|
| [`ProcedureTypes`](Entities/ProcedureType.md) | `ProcedureType` | ProcedureTypes |
| [`Procedures`](Entities/Procedure.md) | `Procedure` | Procedures |
| [`NotificationTemplates`](Entities/NotificationTemplate.md) | `NotificationTemplate` | NotificationTemplates |
| [`Notifications`](Entities/Notification.md) | `Notification` | Notifications |
| [`SystemSettings`](Entities/SystemSetting.md) | `SystemSetting` | SystemSettings |

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
