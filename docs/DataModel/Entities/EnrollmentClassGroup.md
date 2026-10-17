# EnrollmentClassGroup — `EnrollmentClassGroups`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.EnrollmentClassGroup` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `EnrollmentId` | `BIGINT` | `long` | No | Inscripción |
| `ClassGroupId` | `INT` | `int` | No | Grupo |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `EnrollmentId`, `ClassGroupId`
- **FK `EnrollmentId`:** → `Enrollments` (`Enrollment`)
- **FK `ClassGroupId`:** → `ClassGroups` (`ClassGroup`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
