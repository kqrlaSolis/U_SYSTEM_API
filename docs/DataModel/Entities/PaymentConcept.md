# PaymentConcept — `PaymentConcepts`

**Script:** `DataModel/Scripts/05_Finance.sql` · **Clase:** `USystem.Domain.Entities.PaymentConcept` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `Code` | `VARCHAR(20)` | `string` | No | Clave del concepto |
| `Name` | `VARCHAR(150)` | `string` | No | Nombre |
| `BaseAmount` | `DECIMAL(12,2)` | `decimal` | No | Monto base |
| `IsRecurring` | `TINYINT(1)` | `bool` | No | Recurrente |
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
- **Único `UqPaymentConceptsCode`:** `Code`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
