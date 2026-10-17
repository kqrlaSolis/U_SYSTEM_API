# Career — `Careers`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.Career` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(20)` | `string` | No | Clave de la carrera |
| `Name` | `VARCHAR(150)` | `string` | No | Nombre |
| `Level` | `VARCHAR(30)` | `string` | No | Bachelor, Engineering, Master |
| `TermCount` | `TINYINT` | `byte` | No | Duración en cuatrimestres |
| `TotalCredits` | `SMALLINT` | `short` | No | Créditos totales |
| `Modality` | `VARCHAR(20)` | `string` | No | OnSite, Online, Hybrid |
| `IsActive` | `TINYINT(1)` | `bool` | No | Activa |

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
- **Único `UqCareersCode`:** `Code`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
