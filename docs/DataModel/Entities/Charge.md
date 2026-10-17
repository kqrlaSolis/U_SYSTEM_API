# Charge — `Charges`

**Script:** `DataModel/Scripts/05_Finance.sql` · **Clase:** `USystem.Domain.Entities.Charge` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `AcademicTermId` | `INT` | `int` | No | Periodo |
| `PaymentConceptId` | `INT` | `int` | No | Concepto |
| `Amount` | `DECIMAL(12,2)` | `decimal` | No | Monto |
| `Discount` | `DECIMAL(12,2)` | `decimal` | No | Descuento |
| `Surcharge` | `DECIMAL(12,2)` | `decimal` | No | Recargo |
| `DueDate` | `DATE` | `DateOnly` | No | Fecha de vencimiento |
| `Status` | `VARCHAR(20)` | `string` | No | Pending, Partial, Paid, Overdue, Cancelled |

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
- **FK `AcademicTermId`:** → `AcademicTerms` (`AcademicTerm`)
- **FK `PaymentConceptId`:** → `PaymentConcepts` (`PaymentConcept`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
