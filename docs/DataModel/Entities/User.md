# User — `Users`

**Script:** `DataModel/Scripts/01_Security.sql` · **Clase:** `USystem.Domain.Entities.User` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `FirstName` | `VARCHAR(100)` | `string` | No | Nombre(s) |
| `PaternalLastName` | `VARCHAR(100)` | `string` | No | Apellido paterno |
| `MaternalLastName` | `VARCHAR(100)` | `string?` | Sí | Apellido materno |
| `Email` | `VARCHAR(150)` | `string` | No | Correo institucional |
| `Username` | `VARCHAR(50)` | `string` | No | Nombre de usuario |
| `PasswordHash` | `VARCHAR(255)` | `string` | No | Hash BCrypt/Argon2 |
| `Phone` | `VARCHAR(20)` | `string?` | Sí | Teléfono |
| `IsActive` | `TINYINT(1)` | `bool` | No | Usuario activo |
| `MustChangePassword` | `TINYINT(1)` | `bool` | No | Cambio obligatorio en el primer ingreso |
| `FailedAttempts` | `INT` | `int` | No | Intentos fallidos consecutivos |
| `LockedUntil` | `DATETIME` | `DateTime?` | Sí | Bloqueo temporal |
| `LastLoginAt` | `DATETIME` | `DateTime?` | Sí | Último inicio de sesión |

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
- **Único `UqUsersEmail`:** `Email`
- **Único `UqUsersUsername`:** `Username`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
