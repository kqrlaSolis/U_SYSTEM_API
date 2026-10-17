# StudentScholarship — `StudentScholarships`

**Script:** `DataModel/Scripts/05_Finance.sql` · **Clase:** `USystem.Domain.Entities.StudentScholarship` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `ScholarshipId` | `INT` | `int` | No | Beca |
| `AcademicTermId` | `INT` | `int` | No | Periodo |
| `AssignedAt` | `DATETIME` | `DateTime` | No | Fecha de asignación |

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
- **Único `UqStudentScholarshipsStudentTerm`:** `StudentId`, `AcademicTermId`
- **FK `StudentId`:** → `Students` (`Student`)
- **FK `ScholarshipId`:** → `Scholarships` (`Scholarship`)
- **FK `AcademicTermId`:** → `AcademicTerms` (`AcademicTerm`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
