# SubjectPrerequisite — `SubjectPrerequisites`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.SubjectPrerequisite` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `StudyPlanId` | `INT` | `int` | No | Plan de estudios |
| `SubjectId` | `INT` | `int` | No | Materia |
| `PrerequisiteSubjectId` | `INT` | `int` | No | Materia prerrequisito |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `StudyPlanId`, `SubjectId`, `PrerequisiteSubjectId`
- **FK `StudyPlanId`:** → `StudyPlans` (`StudyPlan`)
- **FK `SubjectId`:** → `Subjects` (`Subject`)
- **FK `PrerequisiteSubjectId`:** → `Subjects` (`Subject`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
