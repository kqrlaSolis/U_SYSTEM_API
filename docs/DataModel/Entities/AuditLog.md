# AuditLog — `AuditLogs`

**Script:** `DataModel/Scripts/01_Security.sql` · **Clase:** `USystem.Domain.Entities.AuditLog` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `UserId` | `BIGINT` | `long?` | Sí | Usuario que ejecutó la acción |
| `Action` | `VARCHAR(50)` | `string` | No | Acción (Create, Update, Delete, Login) |
| `Module` | `VARCHAR(50)` | `string` | No | Módulo afectado |
| `Entity` | `VARCHAR(50)` | `string` | No | Entidad afectada |
| `EntityId` | `VARCHAR(50)` | `string?` | Sí | Identificador del registro |
| `OldValue` | `JSON` | `string?` | Sí | Valores anteriores |
| `NewValue` | `JSON` | `string?` | Sí | Valores nuevos |
| `IpAddress` | `VARCHAR(45)` | `string?` | Sí | IP de origen |

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
- **FK `UserId`:** → `Users` (`User`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
