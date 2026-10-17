# Convenciones de base de datos

## Regla principal

**Tablas, columnas, llaves, índices y restricciones en inglés y PascalCase.** Nada de snake_case ni
camelCase. Los comentarios (`--`) van en español.

## Nombres

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Tablas | PascalCase, inglés, plural | `ClassGroups` |
| Columnas | PascalCase, inglés | `PaternalLastName` |
| Llave primaria | `Id` (`BIGINT` en alumnos y transacciones; `INT` en catálogos) | `Users.Id` |
| Llave foránea | `<Entidad>Id` | `StudentId`, `AcademicTermId` |
| Booleanos | `TINYINT(1)` con prefijo `Is` o `Must` | `IsActive`, `MustChangePassword` |
| Fechas y horas | `DATETIME` para momentos; `DATE` para calendario | `CreatedAt`, `StartDate` |
| Dinero | `DECIMAL(12,2)` | `Charges.Amount` |
| Estatus y tipos | `VARCHAR` con valores en inglés y PascalCase | `Pending`, `InProgress` |
| Restricciones | Prefijo `Uq`, `Fk`, `Ix` + tabla + columna | `UqUsersEmail`, `FkGradesEnrollmentId` |
| Auditoría | `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt` | — |
| Borrado | Lógico con `DeletedAt`; los pagos solo se cancelan | — |

## Notas de MySQL

- En Linux los nombres de tabla distinguen mayúsculas de minúsculas. Mantener
  `lower_case_table_names=0` en todos los ambientes y usar siempre la misma capitalización.
- `Groups` es palabra reservada en MySQL 8: por eso la tabla se llama `ClassGroups`.
- Se evitan columnas con nombres de funciones o palabras reservadas:
  `Key` → `SettingKey`, `Year` → `AcademicYear`, `Value` → `SettingValue`, `Condition` → `Status`.
- Cada tabla usa `ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci`.
- Las relaciones usan `ON DELETE RESTRICT ON UPDATE CASCADE`: no se pierde historial.

## Valores de catálogo

| Tabla.Columna | Valores |
|---|---|
| `Careers.Level` | `Bachelor`, `Engineering`, `Master` |
| `Careers.Modality` | `OnSite`, `Online`, `Hybrid` |
| `StudyPlanSubjects.Type` | `Mandatory`, `Elective` |
| `AcademicTerms.Status` | `Planned`, `Open`, `InProgress`, `Closed` |
| `CourseMaterials.Type` | `Syllabus`, `Outline`, `Bibliography`, `File`, `Link` |
| `ClassGroups.Status` | `Open`, `Closed`, `Cancelled` |
| `Students.Status` | `Applicant`, `Active`, `TemporaryLeave`, `Dropped`, `Graduated`, `Titled` |
| `StudentDocuments.Status` | `Pending`, `Delivered`, `Validated`, `Rejected` |
| `Enrollments.Type` | `Enrollment`, `ReEnrollment` |
| `Enrollments.Status` | `Active`, `Cancelled` |
| `Grades.EvaluationType` | `Ordinary`, `Extraordinary`, `Remedial` |
| `Grades.Status` | `InProgress`, `Passed`, `Failed`, `NoRight`, `NotPresented` |
| `Charges.Status` | `Pending`, `Partial`, `Paid`, `Overdue`, `Cancelled` |
| `Payments.Method` | `Cash`, `Transfer`, `Card`, `Deposit` |
| `Payments.Status` | `Applied`, `Cancelled` |
| `Scholarships.Type` | `Percentage`, `Amount` |
| `Procedures.Status` | `Requested`, `InProcess`, `Issued`, `Delivered`, `Cancelled` |
| `Notifications.Status` | `Pending`, `Sent`, `Error` |
| `SystemSettings.ValueType` | `String`, `Int`, `Decimal`, `Bool` |

En C# estos valores viven en `USystem.Domain/Enums` o en `USystem.Domain/Constants/PermissionCodes.cs`.

## Cómo agregar una tabla

1. Definirla en el script correspondiente de `Scripts/` respetando el orden de dependencias.
2. Agregar su clase al diccionario de datos (`Entities/`) y a la lista de entidades C#.
3. Regenerar la documentación con `python3 tools/DocsGenerator/GenerateAll.py`.
4. Actualizar `Rbac.py` si el módulo necesita permisos nuevos.

## Vistas

- `TranscriptView`: kardex por alumno; se apoya en `Grades`, `Enrollments`, `Students`, `ClassGroups`,
  `Subjects`, `Careers` y `AcademicTerms`.
- `AcademicProgressView`: créditos aprobados, porcentaje de avance y promedio; cada materia cuenta una
  sola vez con su mejor calificación aprobatoria.
