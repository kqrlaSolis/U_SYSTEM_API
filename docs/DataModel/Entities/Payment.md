# Payment — `Payments`

**Script:** `DataModel/Scripts/05_Finance.sql` · **Clase:** `USystem.Domain.Entities.Payment` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `ChargeId` | `BIGINT` | `long` | No | Cargo |
| `Amount` | `DECIMAL(12,2)` | `decimal` | No | Monto pagado |
| `Method` | `VARCHAR(20)` | `string` | No | Cash, Transfer, Card, Deposit |
| `Reference` | `VARCHAR(100)` | `string?` | Sí | Referencia |
| `PaymentDate` | `DATETIME` | `DateTime` | No | Fecha del pago |
| `ReceiptNumber` | `VARCHAR(30)` | `string` | No | Folio de recibo |
| `UserId` | `BIGINT` | `long` | No | Usuario de caja |
| `Status` | `VARCHAR(20)` | `string` | No | Applied o Cancelled |
| `CancelledAt` | `DATETIME` | `DateTime?` | Sí | Fecha de cancelación |
| `CancellationReason` | `VARCHAR(255)` | `string?` | Sí | Motivo de cancelación |

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
- **Único `UqPaymentsReceiptNumber`:** `ReceiptNumber`
- **FK `StudentId`:** → `Students` (`Student`)
- **FK `ChargeId`:** → `Charges` (`Charge`)
- **FK `UserId`:** → `Users` (`User`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
