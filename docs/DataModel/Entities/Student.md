# Student — `Students`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.Student` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentNumber` | `VARCHAR(20)` | `string` | No | Matrícula |
| `CareerId` | `INT` | `int` | No | Carrera |
| `StudyPlanId` | `INT` | `int` | No | Plan con el que ingresó |
| `EnrollmentTermId` | `INT` | `int` | No | Periodo de ingreso |
| `FirstName` | `VARCHAR(100)` | `string` | No | Nombre(s) |
| `PaternalLastName` | `VARCHAR(100)` | `string` | No | Apellido paterno |
| `MaternalLastName` | `VARCHAR(100)` | `string?` | Sí | Apellido materno |
| `Curp` | `VARCHAR(18)` | `string` | No | CURP |
| `BirthDate` | `DATE` | `DateOnly` | No | Fecha de nacimiento |
| `Gender` | `VARCHAR(20)` | `string` | No | Género |
| `Email` | `VARCHAR(150)` | `string?` | Sí | Correo |
| `Phone` | `VARCHAR(20)` | `string?` | Sí | Teléfono |
| `AddressLine` | `VARCHAR(200)` | `string?` | Sí | Calle y número |
| `City` | `VARCHAR(100)` | `string?` | Sí | Ciudad |
| `State` | `VARCHAR(100)` | `string?` | Sí | Estado |
| `PostalCode` | `VARCHAR(10)` | `string?` | Sí | Código postal |
| `Status` | `VARCHAR(20)` | `string` | No | Applicant, Active, TemporaryLeave, Dropped, Graduated, Titled |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `Id`
- **Único `UqStudentsStudentNumber`:** `StudentNumber`
- **Único `UqStudentsCurp`:** `Curp`
- **FK `CareerId`:** → `Careers` (`Career`)
- **FK `StudyPlanId`:** → `StudyPlans` (`StudyPlan`)
- **FK `EnrollmentTermId`:** → `AcademicTerms` (`AcademicTerm`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
