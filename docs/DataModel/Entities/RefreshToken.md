# RefreshToken — `RefreshTokens`

**Script:** `DataModel/Scripts/01_Security.sql` · **Clase:** `USystem.Domain.Entities.RefreshToken` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `UserId` | `BIGINT` | `long` | No | Usuario dueño del token |
| `TokenHash` | `VARCHAR(255)` | `string` | No | Hash del refresh token |
| `ExpiresAt` | `DATETIME` | `DateTime` | No | Vigencia |
| `RevokedAt` | `DATETIME` | `DateTime?` | Sí | Revocación |
| `IpAddress` | `VARCHAR(45)` | `string?` | Sí | IP de origen |
| `UserAgent` | `VARCHAR(255)` | `string?` | Sí | Agente de usuario |

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
