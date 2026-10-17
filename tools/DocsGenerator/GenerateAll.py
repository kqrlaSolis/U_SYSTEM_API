# Ejecuta todos los generadores de la documentación de U SYSTEM.
# Uso: python3 tools/DocsGenerator/GenerateAll.py

from GenerateDataModel import Main as GenerateDataModel
from GenerateEntities import Main as GenerateEntities
from GenerateModules import Main as GenerateModules
from GenerateReference import Main as GenerateReference


def Main():
    GenerateDataModel()
    GenerateEntities()
    GenerateModules()
    GenerateReference()
    print("All documents generated under docs/")


if __name__ == "__main__":
    Main()
