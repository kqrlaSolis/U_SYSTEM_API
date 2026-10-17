# Utilidades de rutas y escritura para el generador de documentación.
# Los identificadores van en inglés y PascalCase; los comentarios en español.

from pathlib import Path as FsPath

Root = FsPath(__file__).resolve().parents[2]
Docs = Root / "docs"

# Regla del proyecto: todo el código en inglés y PascalCase.
ProjectRule = (
    "Todo el código (variables, clases, constantes, archivos, tablas, columnas, "
    "rutas de permisos) va en inglés y PascalCase. Solo los comentarios van en español."
)


def WriteDocument(RelativePath, Text):
    # Crea el archivo dentro de docs/ respetando la ruta relativa
    Target = Docs / RelativePath
    Target.parent.mkdir(parents=True, exist_ok=True)
    Target.write_text(Text, encoding="utf-8")
    return Target


def WriteSource(RelativePath, Text):
    # Escribe en el árbol de código (api/, desktop/) además de docs/.
    # Se fuerza LF porque .editorconfig exige end_of_line = lf en los archivos de código.
    Target = Root / RelativePath
    Target.parent.mkdir(parents=True, exist_ok=True)
    Target.write_text(Text, encoding="utf-8", newline="\n")
    return Target
