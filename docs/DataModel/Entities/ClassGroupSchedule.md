# ClassGroupSchedule — `ClassGroupSchedules`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.ClassGroupSchedule` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `ClassGroupId` | `INT` | `int` | No | Grupo |
| `DayOfWeek` | `TINYINT` | `byte` | No | 1 = lunes ... 7 = domingo |
| `StartTime` | `TIME` | `TimeOnly` | No | Hora de inicio |
| `EndTime` | `TIME` | `TimeOnly` | No | Hora de fin |

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
- **FK `ClassGroupId`:** → `ClassGroups` (`ClassGroup`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
