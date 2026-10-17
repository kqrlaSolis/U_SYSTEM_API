# ProcedureType — `ProcedureTypes`

**Script:** `DataModel/Scripts/06_Extras.sql` · **Clase:** `USystem.Domain.Entities.ProcedureType` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(20)` | `string` | No | Clave del trámite |
| `Name` | `VARCHAR(150)` | `string` | No | Nombre |
| `PaymentConceptId` | `INT` | `int?` | Sí | Cobro asociado (opcional) |
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
- **Único `UqProcedureTypesCode`:** `Code`
- **FK `PaymentConceptId`:** → `PaymentConcepts` (`PaymentConcept`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
