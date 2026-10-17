# StudyPlanSubject — `StudyPlanSubjects`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.StudyPlanSubject` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `StudyPlanId` | `INT` | `int` | No | Plan de estudios |
| `SubjectId` | `INT` | `int` | No | Materia |
| `TermNumber` | `TINYINT` | `byte` | No | Cuatrimestre dentro del plan |
| `Type` | `VARCHAR(20)` | `string` | No | Mandatory o Elective |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `StudyPlanId`, `SubjectId`
- **FK `StudyPlanId`:** → `StudyPlans` (`StudyPlan`)
- **FK `SubjectId`:** → `Subjects` (`Subject`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
