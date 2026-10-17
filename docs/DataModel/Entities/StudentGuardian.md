# StudentGuardian — `StudentGuardians`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.StudentGuardian` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `FullName` | `VARCHAR(150)` | `string` | No | Nombre completo del tutor |
| `Relationship` | `VARCHAR(50)` | `string` | No | Parentesco |
| `Phone` | `VARCHAR(20)` | `string?` | Sí | Teléfono |
| `Email` | `VARCHAR(150)` | `string?` | Sí | Correo |

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
- **FK `StudentId`:** → `Students` (`Student`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
