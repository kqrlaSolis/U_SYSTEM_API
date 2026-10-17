# Grade — `Grades`

**Script:** `DataModel/Scripts/04_AcademicTracking.sql` · **Clase:** `USystem.Domain.Entities.Grade` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `EnrollmentId` | `BIGINT` | `long` | No | Inscripción |
| `ClassGroupId` | `INT` | `int` | No | Grupo |
| `Partial1` | `DECIMAL(5,2)` | `decimal?` | Sí | Parcial 1 |
| `Partial2` | `DECIMAL(5,2)` | `decimal?` | Sí | Parcial 2 |
| `Partial3` | `DECIMAL(5,2)` | `decimal?` | Sí | Parcial 3 |
| `Ordinary` | `DECIMAL(5,2)` | `decimal?` | Sí | Calificación ordinaria |
| `FinalScore` | `DECIMAL(5,2)` | `decimal?` | Sí | Calificación final |
| `AttemptNumber` | `TINYINT` | `byte` | No | Número de intento (ordinario, extraordinario, regularización) |
| `EvaluationType` | `VARCHAR(20)` | `string` | No | Ordinary, Extraordinary, Remedial |
| `Status` | `VARCHAR(20)` | `string` | No | InProgress, Passed, Failed, NoRight, NotPresented |
| `IsReportClosed` | `TINYINT(1)` | `bool` | No | Acta cerrada |
| `ClosedAt` | `DATETIME` | `DateTime?` | Sí | Fecha de cierre |
| `ClosedBy` | `BIGINT` | `long?` | Sí | Usuario que cerró el acta |

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
- **Único `UqGradesEnrollmentGroupAttempt`:** `EnrollmentId`, `ClassGroupId`, `AttemptNumber`
- **FK `EnrollmentId`:** → `Enrollments` (`Enrollment`)
- **FK `ClassGroupId`:** → `ClassGroups` (`ClassGroup`)
- **FK `ClosedBy`:** → `Users` (`User`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
