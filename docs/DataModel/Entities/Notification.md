# Notification — `Notifications`

**Script:** `DataModel/Scripts/06_Extras.sql` · **Clase:** `USystem.Domain.Entities.Notification` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `TemplateId` | `INT` | `int` | No | Plantilla |
| `StudentId` | `BIGINT` | `long?` | Sí | Alumno (opcional) |
| `RecipientEmail` | `VARCHAR(150)` | `string` | No | Destinatario |
| `Status` | `VARCHAR(20)` | `string` | No | Pending, Sent, Error |
| `SentAt` | `DATETIME` | `DateTime?` | Sí | Fecha de envío |
| `ErrorMessage` | `VARCHAR(500)` | `string?` | Sí | Error de SMTP |
| `RetryCount` | `TINYINT` | `byte` | No | Reintentos |

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
- **FK `TemplateId`:** → `NotificationTemplates` (`NotificationTemplate`)
- **FK `StudentId`:** → `Students` (`Student`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
