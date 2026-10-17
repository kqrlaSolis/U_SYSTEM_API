# PaymentPlan — `PaymentPlans`

**Script:** `DataModel/Scripts/05_Finance.sql` · **Clase:** `USystem.Domain.Entities.PaymentPlan` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `CareerId` | `INT` | `int?` | Sí | Carrera (opcional) |
| `AcademicTermId` | `INT` | `int` | No | Periodo |
| `Name` | `VARCHAR(150)` | `string` | No | Nombre del plan |
| `InstallmentCount` | `TINYINT` | `byte` | No | Número de colegiaturas |
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
- **FK `CareerId`:** → `Careers` (`Career`)
- **FK `AcademicTermId`:** → `AcademicTerms` (`AcademicTerm`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
