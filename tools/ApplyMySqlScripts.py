# Aplica los scripts SQL de docs/DataModel/Scripts contra el servidor MySQL configurado.
# Se usa cuando no hay cliente `mysql` instalado: lee la cadena de .env o de las variables de entorno.
# Regla del proyecto: identificadores en inglés y PascalCase; solo los comentarios van en español.

import argparse
import os
import sys
from pathlib import Path
from urllib.parse import unquote, urlparse

import pymysql

Root = Path(__file__).resolve().parents[1]
DefaultScripts = Root / "docs" / "DataModel" / "Scripts"
DefaultOrder = [
    "00_CreateDatabase.sql",
    "01_Security.sql",
    "02_AcademicCatalogs.sql",
    "03_AcademicOperations.sql",
    "04_AcademicTracking.sql",
    "05_Finance.sql",
    "06_Extras.sql",
    "99_SeedData.sql",
]


def ReadEnvFile(PathFile):
    # Lee KEY=VALUE sin exportar nada al proceso
    Values = {}
    if not PathFile.exists():
        return Values
    for Line in PathFile.read_text(encoding="utf-8").splitlines():
        Text = Line.strip()
        if not Text or Text.startswith("#") or "=" not in Text:
            continue
        Key, Value = Text.split("=", 1)
        Values[Key.strip()] = Value.strip().strip('"').strip("'")
    return Values


def ParseUri(Uri):
    Parsed = urlparse(Uri)
    if Parsed.scheme not in ("mysql", "mysql+pymysql"):
        raise ValueError(f"Esquema de URI no soportado: {Uri}")
    return {
        "host": Parsed.hostname or "localhost",
        "port": Parsed.port or 3306,
        "user": unquote(Parsed.username or "root"),
        "password": unquote(Parsed.password or ""),
        "database": (Parsed.path or "").lstrip("/"),
    }


def ParseAdoString(ConnectionString):
    Settings = {}
    for Part in ConnectionString.split(";"):
        if "=" not in Part:
            continue
        Key, Value = Part.split("=", 1)
        Settings[Key.strip().lower()] = Value.strip()
    return {
        "host": Settings.get("server") or Settings.get("host") or "localhost",
        "port": int(Settings.get("port") or 3306),
        "user": Settings.get("user") or Settings.get("userid") or Settings.get("uid") or "root",
        "password": Settings.get("password") or Settings.get("pwd") or "",
        "database": Settings.get("database") or "",
    }


def ResolveConnection(ExplicitUrl):
    if ExplicitUrl:
        return ParseUri(ExplicitUrl)

    EnvironmentUrl = os.environ.get("MYSQL_URL")
    if EnvironmentUrl:
        return ParseUri(EnvironmentUrl)

    EnvironmentAdo = os.environ.get("ConnectionStrings__DefaultConnection")
    if EnvironmentAdo:
        return ParseAdoString(EnvironmentAdo)

    EnvValues = ReadEnvFile(Root / ".env")
    if EnvValues.get("MYSQL_URL"):
        return ParseUri(EnvValues["MYSQL_URL"])
    if EnvValues.get("ConnectionStrings__DefaultConnection"):
        return ParseAdoString(EnvValues["ConnectionStrings__DefaultConnection"])

    raise SystemExit("No hay cadena de conexión: usa --url, MYSQL_URL o el archivo .env")


def SplitStatements(Sql):
    # Separa por ; respetando cadenas, identificadores y comentarios
    Statements = []
    Buffer = []
    Line = 1
    StartLine = 1
    Index = 0
    Length = len(Sql)

    while Index < Length:
        Character = Sql[Index]
        NextCharacter = Sql[Index + 1] if Index + 1 < Length else ""

        if Character == "\n":
            Line += 1

        # Comentario de línea con -- o #
        if (Character == "-" and NextCharacter == "-") or Character == "#":
            EndOfLine = Sql.find("\n", Index)
            if EndOfLine == -1:
                break
            Index = EndOfLine
            continue

        # Comentario de bloque
        if Character == "/" and NextCharacter == "*":
            EndOfComment = Sql.find("*/", Index + 2)
            if EndOfComment == -1:
                break
            Line += Sql[Index:EndOfComment].count("\n")
            Index = EndOfComment + 2
            continue

        # Cadenas e identificadores entre comillas
        if Character in ("'", '"', "`"):
            Quote = Character
            Buffer.append(Character)
            Index += 1
            while Index < Length:
                Current = Sql[Index]
                Buffer.append(Current)
                if Current == "\\" and Quote in ("'", '"') and Index + 1 < Length:
                    Buffer.append(Sql[Index + 1])
                    Index += 2
                    continue
                if Current == "\n":
                    Line += 1
                if Current == Quote:
                    # Comilla doble escapada ('' o "")
                    if Index + 1 < Length and Sql[Index + 1] == Quote:
                        Buffer.append(Sql[Index + 1])
                        Index += 2
                        continue
                    Index += 1
                    break
                Index += 1
            continue

        if Character == ";":
            Statement = "".join(Buffer).strip()
            if Statement:
                Statements.append((Statement, StartLine))
            Buffer = []
            Index += 1
            StartLine = Line
            continue

        Buffer.append(Character)
        Index += 1

    Statement = "".join(Buffer).strip()
    if Statement:
        Statements.append((Statement, StartLine))

    return Statements


def Main():
    Parser = argparse.ArgumentParser(description="Aplica los scripts SQL de U SYSTEM.")
    Parser.add_argument("--url", help="Cadena mysql://usuario:clave@host:puerto/basededatos")
    Parser.add_argument("--scripts", default=str(DefaultScripts), help="Carpeta con los scripts SQL")
    Parser.add_argument("--database", default="", help="Base de datos a verificar al final (por defecto la de --target-database)")
    Parser.add_argument("--target-database", default="", help="Aplica los scripts dentro de esta base ya existente (omite CREATE DATABASE y USE)")
    Parser.add_argument("--skip-seed", action="store_true", help="No ejecuta 99_SeedData.sql")
    Arguments = Parser.parse_args()

    Settings = ResolveConnection(Arguments.url)
    ScriptsPath = Path(Arguments.scripts)

    print(f"Servidor: {Settings['host']}:{Settings['port']} | Usuario: {Settings['user']}")

    TargetDatabase = Arguments.target_database.strip()
    if TargetDatabase:
        print(f"Base destino: {TargetDatabase} (se omiten CREATE DATABASE y USE)")

    Connection = pymysql.connect(
        host=Settings["host"],
        port=Settings["port"],
        user=Settings["user"],
        password=Settings["password"],
        database=TargetDatabase or None,
        charset="utf8mb4",
        autocommit=True,
        connect_timeout=20,
    )

    try:
        with Connection.cursor() as Cursor:
            Cursor.execute("SELECT VERSION()")
            print(f"MySQL: {Cursor.fetchone()[0]}")

        for FileName in DefaultOrder:
            if Arguments.skip_seed and FileName == "99_SeedData.sql":
                continue

            ScriptFile = ScriptsPath / FileName
            if not ScriptFile.exists():
                raise SystemExit(f"Falta el script {ScriptFile}")

            Sql = ScriptFile.read_text(encoding="utf-8")
            Statements = SplitStatements(Sql)
            Executed = 0

            with Connection.cursor() as Cursor:
                for Statement, Line in Statements:
                    Head = Statement.strip().upper()
                    if TargetDatabase and (Head.startswith("CREATE DATABASE") or Head.startswith("USE ")):
                        continue

                    try:
                        Cursor.execute(Statement)
                        Executed += 1
                    except Exception as CaughtError:
                        Snippet = " ".join(Statement.split())[:180]
                        print(f"ERROR en {FileName} (línea {Line}): {CaughtError}")
                        print(f"  {Snippet}")
                        raise SystemExit(1)

            print(f"OK {FileName}: {Executed} sentencias")

        Database = Arguments.database or TargetDatabase or Settings["database"] or "USystem"
        with Connection.cursor() as Cursor:
            Cursor.execute(
                "SELECT COUNT(*) FROM information_schema.tables "
                "WHERE table_schema = %s AND table_type = 'BASE TABLE'",
                (Database,),
            )
            TableCount = Cursor.fetchone()[0]
            Cursor.execute(
                "SELECT COUNT(*) FROM information_schema.views WHERE table_schema = %s",
                (Database,),
            )
            ViewCount = Cursor.fetchone()[0]
            Cursor.execute(
                "SELECT COUNT(*) FROM information_schema.columns "
                "WHERE table_schema = %s AND BINARY column_name REGEXP BINARY '^[a-z]'",
                (Database,),
            )
            SnakeCaseColumns = Cursor.fetchone()[0]

        print(f"Base de datos {Database}: {TableCount} tablas, {ViewCount} vistas")
        print(f"Columnas que empiezan en minúscula (deben ser 0): {SnakeCaseColumns}")
    finally:
        Connection.close()


if __name__ == "__main__":
    Main()
