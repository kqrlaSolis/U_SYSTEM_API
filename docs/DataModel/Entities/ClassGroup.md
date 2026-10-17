# ClassGroup — `ClassGroups`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.ClassGroup` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `AcademicTermId` | `INT` | `int` | No | Periodo |
| `SubjectId` | `INT` | `int` | No | Materia |
| `TeacherId` | `INT` | `int` | No | Docente |
| `Code` | `VARCHAR(20)` | `string` | No | Clave del grupo |
| `Classroom` | `VARCHAR(50)` | `string?` | Sí | Aula |
| `Capacity` | `SMALLINT` | `short` | No | Cupo |
| `EnrolledCount` | `SMALLINT` | `short` | No | Inscritos |
| `Status` | `VARCHAR(20)` | `string` | No | Open, Closed, Cancelled |

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
- **Único `UqClassGroupsTermCode`:** `AcademicTermId`, `Code`
- **FK `AcademicTermId`:** → `AcademicTerms` (`AcademicTerm`)
- **FK `SubjectId`:** → `Subjects` (`Subject`)
- **FK `TeacherId`:** → `Teachers` (`Teacher`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
