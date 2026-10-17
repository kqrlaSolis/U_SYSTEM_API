# Subject — `Subjects`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.Subject` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(20)` | `string` | No | Clave de la materia |
| `Name` | `VARCHAR(150)` | `string` | No | Nombre |
| `Credits` | `TINYINT` | `byte` | No | Créditos |
| `TheoryHours` | `TINYINT` | `byte` | No | Horas teóricas |
| `PracticeHours` | `TINYINT` | `byte` | No | Horas prácticas |
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
- **Único `UqSubjectsCode`:** `Code`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
