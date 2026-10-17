# Permission — `Permissions`

**Script:** `DataModel/Scripts/01_Security.sql` · **Clase:** `USystem.Domain.Entities.Permission` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(100)` | `string` | No | Código Module.Action |
| `Module` | `VARCHAR(50)` | `string` | No | Módulo al que pertenece |
| `Description` | `VARCHAR(255)` | `string` | No | Descripción |

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
- **Único `UqPermissionsCode`:** `Code`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
