# SystemSetting — `SystemSettings`

**Script:** `DataModel/Scripts/06_Extras.sql` · **Clase:** `USystem.Domain.Entities.SystemSetting` · **Hereda:** `AuditableEntity`

## Campos

| Campo | Tipo SQL | Tipo C# | Nulo | Notas |
|---|---|---|:-:|---|
| `Id` | `INT` | `int` | No | Llave primaria |
| `SettingKey` | `VARCHAR(100)` | `string` | No | Clave del parámetro |
| `SettingValue` | `VARCHAR(500)` | `string` | No | Valor |
| `ValueType` | `VARCHAR(20)` | `string` | No | String, Int, Decimal, Bool |
| `Category` | `VARCHAR(50)` | `string` | No | Categoría |
| `Description` | `VARCHAR(255)` | `string?` | Sí | Descripción |

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
- **Único `UqSystemSettingsSettingKey`:** `SettingKey`
- **FK:** —

## Reglas de integridad

- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).
- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.
