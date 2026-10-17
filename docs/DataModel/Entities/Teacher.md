# Teacher — `Teachers`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.Teacher` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `EmployeeNumber` | `VARCHAR(20)` | `string` | No | Número de empleado |
| `FirstName` | `VARCHAR(100)` | `string` | No | Nombre(s) |
| `PaternalLastName` | `VARCHAR(100)` | `string` | No | Apellido paterno |
| `MaternalLastName` | `VARCHAR(100)` | `string?` | Sí | Apellido materno |
| `Email` | `VARCHAR(150)` | `string` | No | Correo |
| `Phone` | `VARCHAR(20)` | `string?` | Sí | Teléfono |
| `Specialty` | `VARCHAR(150)` | `string?` | Sí | Especialidad |
| `UserId` | `BIGINT` | `long?` | Sí | Usuario del sistema (opcional) |
| `IsActive` | `TINYINT(1)` | `bool` | No | Activo |

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
- **Único `UqTeachersEmployeeNumber`:** `EmployeeNumber`
- **Único `UqTeachersEmail`:** `Email`
- **FK `UserId`:** → `Users` (`User`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
