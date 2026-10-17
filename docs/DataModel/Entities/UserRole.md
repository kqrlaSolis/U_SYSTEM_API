# UserRole — `UserRoles`

**Script:** `DataModel/Scripts/01_Security.sql` · **Clase:** `USystem.Domain.Entities.UserRole` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `UserId` | `BIGINT` | `long` | No | Usuario |
| `RoleId` | `INT` | `int` | No | Rol |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `UserId`, `RoleId`
- **FK `UserId`:** → `Users` (`User`)
- **FK `RoleId`:** → `Roles` (`Role`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
