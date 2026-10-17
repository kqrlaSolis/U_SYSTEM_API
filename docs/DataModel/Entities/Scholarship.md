# Scholarship — `Scholarships`

**Script:** `DataModel/Scripts/05_Finance.sql` · **Clase:** `USystem.Domain.Entities.Scholarship` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Name` | `VARCHAR(150)` | `string` | No | Nombre de la beca |
| `Type` | `VARCHAR(20)` | `string` | No | Percentage o Amount |
| `Value` | `DECIMAL(12,2)` | `decimal` | No | Valor de la beca |
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
- **Único `UqScholarshipsName`:** `Name`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
