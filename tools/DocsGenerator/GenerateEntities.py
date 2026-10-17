# Genera las entidades C# y el diccionario de datos por tabla.
# Los identificadores van en inglés y PascalCase; los comentarios en español.

from Paths import WriteDocument, WriteSource
from Schema import Tables, TableByName, AuditColumns

TypeMap = {
    "BIGINT": ("long", "long?"),
    "INT": ("int", "int?"),
    "SMALLINT": ("short", "short?"),
    "TINYINT(1)": ("bool", "bool?"),
    "TINYINT": ("byte", "byte?"),
    "DATE": ("DateOnly", "DateOnly?"),
    "DATETIME": ("DateTime", "DateTime?"),
    "TIME": ("TimeOnly", "TimeOnly?"),
    "DECIMAL(5,2)": ("decimal", "decimal?"),
    "DECIMAL(12,2)": ("decimal", "decimal?"),
    "JSON": ("string", "string?"),
    "TEXT": ("string", "string?"),
}

# Tipos VARCHAR(n) se resuelven por prefijo
StringTypes = ("VARCHAR",)


def CSharpType(SqlType, IsNullable):
    if SqlType.startswith(StringTypes):
        return "string?" if IsNullable else "string"
    Mapping = TypeMap.get(SqlType)
    if Mapping is None:
        raise ValueError(f"Unmapped SQL type: {SqlType}")
    return Mapping[1] if IsNullable else Mapping[0]


def IsNullableColumn(Column):
    Flags = Column[2].split()
    return "NULL" in Flags and "PK" not in Flags


def PropertyName(Name):
    return Name


def GenerateEntity(Table):
    Lines = [
        f"// Entidad de la tabla {Table['Name']}.",
        "// Los nombres siguen la regla del proyecto: inglés y PascalCase.",
        "",
        "using System;",
        "",
        "namespace USystem.Domain.Entities;",
        "",
        f"public class {Table['Class']} : AuditableEntity",
        "{",
    ]
    for Column in Table["Columns"]:
        Name, SqlType, Flags, Comment = Column
        Nullable = IsNullableColumn(Column)
        TypeName = CSharpType(SqlType, Nullable)
        if Comment:
            Lines.append(f"    // {Comment}")
        if TypeName == "string":
            Lines.append(f"    public string {PropertyName(Name)} {{ get; set; }} = string.Empty;")
        elif TypeName == "string?":
            Lines.append(f"    public string? {PropertyName(Name)} {{ get; set; }}")
        else:
            Lines.append(f"    public {TypeName} {PropertyName(Name)} {{ get; set; }}")
        Lines.append("")
    if Lines[-1] == "":
        Lines.pop()
    Lines.append("}")
    return "\n".join(Lines) + "\n"


def GenerateEntityDocument(Table):
    Lines = [f"# {Table['Class']} — `{Table['Name']}`", "",
             f"**Script:** `DataModel/Scripts/{Table['Script']}.sql` · "
             f"**Clase:** `USystem.Domain.Entities.{Table['Class']}` · "
             f"**Hereda:** `AuditableEntity`", "",
             "## Campos", "",
             "| Campo | Tipo SQL | Tipo C# | Nulo | Notas |",
             "|---|---|---|:-:|---|"]
    for Column in Table["Columns"]:
        Name, SqlType, Flags, Comment = Column
        Nullable = "Sí" if IsNullableColumn(Column) else "No"
        Lines.append(f"| `{Name}` | `{SqlType}` | `{CSharpType(SqlType, IsNullableColumn(Column))}` | {Nullable} | {Comment} |")
    Lines += ["",
              "## Auditoría (heredada)", "",
              "| Campo | Tipo | Notas |", "|---|---|---|"]
    for Column in AuditColumns:
        Lines.append(f"| `{Column[0]}` | `{Column[1]}` | {Column[3]} |")

    PrimaryKeys = [Column[0] for Column in Table["Columns"] if "PK" in Column[2].split()]
    ForeignKeys = [(Column[0], Token[3:], Column[1])
                   for Column in Table["Columns"]
                   for Token in Column[2].split() if Token.startswith("FK:")]
    UniqueKeys = [(f"Uq{Table['Name']}{Column[0]}", [Column[0]])
                  for Column in Table["Columns"] if "UQ" in Column[2].split()]
    UniqueKeys += Table["Unique"]

    Lines += ["", "## Llaves y relaciones", "",
              f"- **Llave primaria:** {', '.join(f'`{Key}`' for Key in PrimaryKeys)}"]
    if UniqueKeys:
        for ConstraintName, Columns in UniqueKeys:
            Lines.append(f"- **Único `{ConstraintName}`:** {', '.join(f'`{Column}`' for Column in Columns)}")
    if ForeignKeys:
        for ColumnName, ReferencedTable, SqlType in ForeignKeys:
            ReferencedClass = TableByName[ReferencedTable]["Class"]
            Lines.append(f"- **FK `{ColumnName}`:** → `{ReferencedTable}` (`{ReferencedClass}`)")
    else:
        Lines.append("- **FK:** —")

    Lines += ["", "## Reglas de integridad", "",
              "- Borrado lógico mediante `DeletedAt` (excepto los registros que solo se cancelan).",
              "- Las relaciones usan `ON DELETE RESTRICT` para no perder historial.", ""]
    return "\n".join(Lines)


def Main():
    AuditableEntity = """// Campos de auditoría comunes a todas las tablas.
using System;

namespace USystem.Domain.Entities;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}
"""
    WriteDocument("Api/Entities/AuditableEntity.cs", AuditableEntity)
    WriteSource("api/USystem.Domain/Entities/AuditableEntity.cs", AuditableEntity)

    IndexLines = ["# Entidades C#", "",
                  "Namespace: `USystem.Domain.Entities`. Todas heredan de `AuditableEntity`.", "",
                  "> Este catálogo se genera desde `Schema.py` y se escribe también en",
                  "> `api/USystem.Domain/Entities/`, que es el código real de la solución.", "",
                  "| Entidad | Tabla | Script | Documento |", "|---|---|---|---|"]
    for Table in Tables:
        EntitySource = GenerateEntity(Table)
        WriteDocument(f"Api/Entities/{Table['Class']}.cs", EntitySource)
        WriteSource(f"api/USystem.Domain/Entities/{Table['Class']}.cs", EntitySource)
        WriteDocument(f"DataModel/Entities/{Table['Class']}.md", GenerateEntityDocument(Table))
        IndexLines.append(f"| `{Table['Class']}` | `{Table['Name']}` | `{Table['Script']}.sql` | [Ver](../../DataModel/Entities/{Table['Class']}.md) |")
    IndexLines += ["", f"Total: **{len(Tables)} entidades**.", ""]
    WriteDocument("Api/Entities/README.md", "\n".join(IndexLines))
    print(f"Entities generated: {len(Tables)}")


if __name__ == "__main__":
    Main()
