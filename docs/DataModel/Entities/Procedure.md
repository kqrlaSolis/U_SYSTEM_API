# Procedure — `Procedures`

**Script:** `DataModel/Scripts/06_Extras.sql` · **Clase:** `USystem.Domain.Entities.Procedure` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `ProcedureTypeId` | `INT` | `int` | No | Tipo de trámite |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `FolioNumber` | `VARCHAR(30)` | `string` | No | Folio consecutivo |
| `Status` | `VARCHAR(20)` | `string` | No | Requested, InProcess, Issued, Delivered, Cancelled |
| `RequestedAt` | `DATETIME` | `DateTime` | No | Fecha de solicitud |
| `IssuedAt` | `DATETIME` | `DateTime?` | Sí | Fecha de emisión |
| `FilePath` | `VARCHAR(500)` | `string?` | Sí | Ruta del PDF emitido |
| `UserId` | `BIGINT` | `long` | No | Usuario que atiende |

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
- **Único `UqProceduresFolioNumber`:** `FolioNumber`
- **FK `ProcedureTypeId`:** → `ProcedureTypes` (`ProcedureType`)
- **FK `StudentId`:** → `Students` (`Student`)
- **FK `UserId`:** → `Users` (`User`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
