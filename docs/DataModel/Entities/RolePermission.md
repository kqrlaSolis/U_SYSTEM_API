# RolePermission — `RolePermissions`

**Script:** `DataModel/Scripts/01_Security.sql` · **Clase:** `USystem.Domain.Entities.RolePermission` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `RoleId` | `INT` | `int` | No | Rol |
| `PermissionId` | `INT` | `int` | No | Permiso |

## Auditoría (heredada)

| Campo | Tipo | Notas |
|---|---|---|
| `CreatedAt` | `DATETIME` | Fecha de creación |
| `CreatedBy` | `BIGINT` | Usuario que creó el registro |
| `UpdatedAt` | `DATETIME` | Fecha de última modificación |
| `UpdatedBy` | `BIGINT` | Usuario que modificó el registro |
| `DeletedAt` | `DATETIME` | Borrado lógico |

## Llaves y relaciones

- **Llave primaria:** `RoleId`, `PermissionId`
- **FK `RoleId`:** → `Roles` (`Role`)
- **FK `PermissionId`:** → `Permissions` (`Permission`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
