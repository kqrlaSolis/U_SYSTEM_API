# NotificationTemplate — `NotificationTemplates`

**Script:** `DataModel/Scripts/06_Extras.sql` · **Clase:** `USystem.Domain.Entities.NotificationTemplate` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(50)` | `string` | No | Clave de la plantilla |
| `SubjectLine` | `VARCHAR(200)` | `string` | No | Asunto |
| `Body` | `TEXT` | `string` | No | Cuerpo con variables {{StudentName}} |
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
- **Único `UqNotificationTemplatesCode`:** `Code`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
