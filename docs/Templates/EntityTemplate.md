# Plantilla — Entidad `TableName` → `ClassName`

**Script:** `DataModel/Scripts/NN_Name.sql` · **Módulo:** NN

| Campo | Tipo | Nulo | Default | Notas |
|---|---|:-:|---|---|
| `Id` | BIGINT | No | | PK |
| | | | | |

## Auditoría (heredada de `AuditableEntity`)

`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt`.

## Relaciones

| FK | Referencia | Regla |
|---|---|---|
| `StudentId` | `Students.Id` | `ON DELETE RESTRICT` |

## Llaves únicas

| Constraint | Columnas |
|---|---|
| `Uq<Table><Column>` | |

## Reglas de integridad

- Borrado lógico con `DeletedAt`.
- Los nombres de columnas van en PascalCase (inglés).
