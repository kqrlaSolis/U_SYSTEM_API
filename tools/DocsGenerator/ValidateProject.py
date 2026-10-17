# Valida la consistencia entre el esquema, los permisos, los módulos y los archivos generados.
# Los identificadores van en inglés y PascalCase; los comentarios en español.

import json
from pathlib import Path

from Schema import Tables, TableByName
from Rbac import AllCodes, RolePermissions, PermissionModules
from Modules import Modules

Docs = Path(__file__).resolve().parents[2] / "docs"
Problems = []


def Check(Condition, Message):
    if not Condition:
        Problems.append(Message)


def CheckSchema():
    Names = [Table["Name"] for Table in Tables]
    Classes = [Table["Class"] for Table in Tables]
    Check(len(Names) == len(set(Names)), "Duplicate table names")
    Check(len(Classes) == len(set(Classes)), "Duplicate entity class names")

    ConstraintNames = []
    for Table in Tables:
        PrimaryKeys = [Column[0] for Column in Table["Columns"] if "PK" in Column[2].split()]
        Check(PrimaryKeys, f"{Table['Name']} has no primary key")

        for Column in Table["Columns"]:
            ColumnName, SqlType, Flags, _ = Column
            for Token in Flags.split():
                if not Token.startswith("FK:"):
                    continue
                Referenced = Token[3:]
                Check(Referenced in TableByName, f"{Table['Name']}.{ColumnName} references unknown table {Referenced}")
                if Referenced in TableByName:
                    ReferencedId = TableByName[Referenced]["Columns"][0]
                    Check(ReferencedId[0] == "Id", f"{Referenced} first column is not Id")
                    Check(SqlType == ReferencedId[1],
                          f"{Table['Name']}.{ColumnName} type {SqlType} does not match {Referenced}.Id {ReferencedId[1]}")
                ConstraintNames.append(f"Fk{Table['Name']}{ColumnName}")

        for ConstraintName, Columns in Table["Unique"]:
            for ColumnName in Columns:
                Check(any(Column[0] == ColumnName for Column in Table["Columns"]),
                      f"{Table['Name']} unique {ConstraintName} uses unknown column {ColumnName}")
            ConstraintNames.append(ConstraintName)

    Check(len(ConstraintNames) == len(set(ConstraintNames)), "Duplicate constraint names")


def CheckModules():
    KnownCodes = set(AllCodes())
    UsedCodes = set()
    AssignedTables = set()

    for Module in Modules:
        for TableName in Module["Tables"]:
            Check(TableName in TableByName, f"Module {Module['File']} uses unknown table {TableName}")
            AssignedTables.add(TableName)
        for Method, Path, Description, Permission in Module["Endpoints"]:
            if Permission in ("Public", "Authenticated"):
                continue
            Check(Permission in KnownCodes, f"Module {Module['File']} uses unknown permission {Permission}")
            UsedCodes.add(Permission)

    Unassigned = sorted(set(TableByName) - AssignedTables)
    Check(not Unassigned, f"Tables without module: {Unassigned}")
    UnusedCodes = sorted(KnownCodes - UsedCodes)
    Check(not UnusedCodes, f"Permissions without endpoint: {UnusedCodes}")

    for RoleName, Codes in RolePermissions.items():
        Unknown = [Code for Code in Codes if Code not in KnownCodes]
        Check(not Unknown, f"Role {RoleName} has unknown permissions {Unknown}")


def CheckGeneratedFiles():
    for Table in Tables:
        Check((Docs / f"Api/Entities/{Table['Class']}.cs").exists(), f"Missing entity {Table['Class']}.cs")
        Check((Docs / f"DataModel/Entities/{Table['Class']}.md").exists(), f"Missing data dictionary {Table['Class']}.md")
        Check((Docs / f"DataModel/Scripts/{Table['Script']}.sql").exists(), f"Missing script {Table['Script']}.sql")

    for Module in Modules:
        Check((Docs / f"Modules/{Module['Number']:02d}_{Module['File']}.md").exists(),
              f"Missing module document {Module['File']}")

    for PathName in ["README.md", "Api/Conventions.md", "Api/LayerExample.md", "DataModel/README.md",
                     "DataModel/Conventions.md", "Guides/CodingConventions.md", "Security/RbacMatrix.md",
                     "Testing/Strategy.md", "Testing/AcceptanceCriteria.md", "Architecture/Phases.md",
                     "Architecture/Internationalization.md", "Operations/Storage.md",
                     "AxzyUiSystem/README.md"]:
        Check((Docs / PathName).exists(), f"Missing document {PathName}")


def CheckLocalization():
    Spanish = json.loads((Docs / "Api/Resources/ErrorMessagesEs.json").read_text(encoding="utf-8"))
    English = json.loads((Docs / "Api/Resources/ErrorMessagesEn.json").read_text(encoding="utf-8"))
    Check(set(Spanish) == set(English), "i18n catalogs do not have the same keys")
    Check(len(Spanish) > 0, "i18n catalogs are empty")
    for Key in Spanish:
        Check(Key.split(".")[0][0].isupper(), f"i18n key is not PascalCase: {Key}")


def Main():
    CheckSchema()
    CheckModules()
    CheckGeneratedFiles()
    CheckLocalization()
    if Problems:
        print("Validation FAILED")
        for Problem in Problems:
            print(f" - {Problem}")
        raise SystemExit(1)
    print(f"Validation OK: {len(Tables)} tables, {len(AllCodes())} permissions, {len(Modules)} modules")


if __name__ == "__main__":
    Main()
