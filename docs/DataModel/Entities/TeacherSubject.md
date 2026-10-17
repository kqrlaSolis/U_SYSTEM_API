# TeacherSubject — `TeacherSubjects`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.TeacherSubject` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `TeacherId` | `INT` | `int` | No | Docente |
| `SubjectId` | `INT` | `int` | No | Materia habilitada |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `TeacherId`, `SubjectId`
- **FK `TeacherId`:** → `Teachers` (`Teacher`)
- **FK `SubjectId`:** → `Subjects` (`Subject`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
