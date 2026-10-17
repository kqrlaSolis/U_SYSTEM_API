# StudyPlan — `StudyPlans`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.StudyPlan` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `CareerId` | `INT` | `int` | No | Carrera |
| `Code` | `VARCHAR(20)` | `string` | No | Clave del plan |
| `Version` | `VARCHAR(20)` | `string` | No | Versión |
| `StartDate` | `DATE` | `DateOnly` | No | Inicio de vigencia |
| `EndDate` | `DATE` | `DateOnly?` | Sí | Fin de vigencia |
| `TotalCredits` | `SMALLINT` | `short` | No | Créditos totales |
| `IsCurrent` | `TINYINT(1)` | `bool` | No | Vigente para nuevos ingresos |

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
- **Único `UqStudyPlansCareerCodeVersion`:** `CareerId`, `Code`, `Version`
- **FK `CareerId`:** → `Careers` (`Career`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
