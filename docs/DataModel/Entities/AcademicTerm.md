# AcademicTerm — `AcademicTerms`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.AcademicTerm` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(10)` | `string` | No | Clave YYYY-N |
| `Name` | `VARCHAR(100)` | `string` | No | Nombre del periodo |
| `AcademicYear` | `SMALLINT` | `short` | No | Año académico |
| `TermNumber` | `TINYINT` | `byte` | No | 1, 2 o 3 |
| `StartDate` | `DATE` | `DateOnly` | No | Inicio del periodo |
| `EndDate` | `DATE` | `DateOnly` | No | Fin del periodo |
| `EnrollmentStartDate` | `DATE` | `DateOnly` | No | Inicio de inscripciones |
| `EnrollmentEndDate` | `DATE` | `DateOnly` | No | Fin de inscripciones |
| `ReEnrollmentStartDate` | `DATE` | `DateOnly` | No | Inicio de reinscripciones |
| `ReEnrollmentEndDate` | `DATE` | `DateOnly` | No | Fin de reinscripciones |
| `ExamStartDate` | `DATE` | `DateOnly` | No | Inicio de exámenes |
| `ExamEndDate` | `DATE` | `DateOnly` | No | Fin de exámenes |
| `GradeReportCloseDate` | `DATE` | `DateOnly` | No | Fecha de cierre de actas |
| `Status` | `VARCHAR(20)` | `string` | No | Planned, Open, InProgress, Closed |

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
- **Único `UqAcademicTermsCode`:** `Code`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
