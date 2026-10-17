# Enrollment — `Enrollments`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.Enrollment` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `AcademicTermId` | `INT` | `int` | No | Periodo |
| `CareerId` | `INT` | `int` | No | Carrera |
| `Type` | `VARCHAR(20)` | `string` | No | Enrollment o ReEnrollment |
| `Status` | `VARCHAR(20)` | `string` | No | Active o Cancelled |
| `Notes` | `VARCHAR(500)` | `string?` | Sí | Observaciones |

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
- **Único `UqEnrollmentsStudentTerm`:** `StudentId`, `AcademicTermId`
- **FK `StudentId`:** → `Students` (`Student`)
- **FK `AcademicTermId`:** → `AcademicTerms` (`AcademicTerm`)
- **FK `CareerId`:** → `Careers` (`Career`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
