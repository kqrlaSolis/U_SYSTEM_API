# StudentDocument — `StudentDocuments`

**Script:** `DataModel/Scripts/03_AcademicOperations.sql` · **Clase:** `USystem.Domain.Entities.StudentDocument` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `BIGINT` | `long` | No | Llave primaria |
| `StudentId` | `BIGINT` | `long` | No | Alumno |
| `DocumentTypeId` | `INT` | `int` | No | Tipo de documento |
| `Status` | `VARCHAR(20)` | `string` | No | Pending, Delivered, Validated, Rejected |
| `FilePath` | `VARCHAR(500)` | `string?` | Sí | Ruta del archivo |
| `Notes` | `VARCHAR(500)` | `string?` | Sí | Observaciones |
| `ValidatedBy` | `BIGINT` | `long?` | Sí | Usuario que validó |
| `ValidatedAt` | `DATETIME` | `DateTime?` | Sí | Fecha de validación |

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
- **Único `UqStudentDocumentsStudentType`:** `StudentId`, `DocumentTypeId`
- **FK `StudentId`:** → `Students` (`Student`)
- **FK `DocumentTypeId`:** → `DocumentTypes` (`DocumentType`)
- **FK `ValidatedBy`:** → `Users` (`User`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
