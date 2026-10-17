# CourseMaterial — `CourseMaterials`

**Script:** `DataModel/Scripts/02_AcademicCatalogs.sql` · **Clase:** `USystem.Domain.Entities.CourseMaterial` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `SubjectId` | `INT` | `int` | No | Materia |
| `AcademicTermId` | `INT` | `int?` | Sí | Periodo (opcional) |
| `Type` | `VARCHAR(20)` | `string` | No | Syllabus, Outline, Bibliography, File, Link |
| `Title` | `VARCHAR(200)` | `string` | No | Título |
| `Description` | `TEXT` | `string?` | Sí | Descripción |
| `Url` | `VARCHAR(500)` | `string?` | Sí | Enlace externo |
| `FilePath` | `VARCHAR(500)` | `string?` | Sí | Ruta del archivo |
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
- **FK `SubjectId`:** → `Subjects` (`Subject`)
- **FK `AcademicTermId`:** → `AcademicTerms` (`AcademicTerm`)

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
