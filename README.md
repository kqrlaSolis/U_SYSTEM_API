# U SYSTEM

Sistema administrativo de gestión escolar universitaria (modelo cuatrimestral) para el personal de la
escuela. **El alumno no tiene acceso a la aplicación.**

> Estado actual: **Fase 1 en curso (API completa)**. La Fase 0 está completa (solución .NET 8 en capas,
> MySQL 8.4 en Railway con 37 tablas y 2 vistas, JWT, Swagger, Serilog, i18n, S3 y desktop Electron +
> React + AXZY UI). Los **ocho módulos de la Fase 1** están en la API con 119 pruebas verdes y el login del
> desktop conectado: Usuarios/autenticación, Roles/RBAC, AuditLogs, Careers, StudyPlans, Subjects,
> AcademicTerms y Settings. Falta cerrar las pantallas del desktop. El plan está en
> [`docs/Architecture/Phases.md`](docs/Architecture/Phases.md).

## Regla del proyecto

**Todo el código en inglés y PascalCase** (variables, parámetros, clases, constantes, tablas, columnas,
permisos y nombres de archivo). **Nada de snake_case ni camelCase.** **Solo los comentarios van en español.**

La regla, sus excepciones de framework y cómo se aplica con `.editorconfig` y ESLint están en
[`docs/Guides/CodingConventions.md`](docs/Guides/CodingConventions.md).

## Estructura

```
USystem/
├── USystem.sln
├── .editorconfig        Reglas de nomenclatura para C#
├── .env                 Variables de entorno locales (no se versiona)
├── api/
│   ├── USystem.Api/            Controllers, Attributes, Middlewares, Extensions, Localization, Program.cs
│   ├── USystem.Application/    Common, Interfaces, Services, DependencyInjection
│   ├── USystem.Domain/         Common, Constants, Entities, Exceptions, Interfaces
│   ├── USystem.Infrastructure/ Persistence, Repositories, Security, Storage, DependencyInjection
│   └── USystem.Tests/          UnitTests, IntegrationTests
├── desktop/             Electron + React TS + AXZY UI (Model · ViewModel · View)
├── docs/                Documentación completa (171 archivos)
└── tools/               Generador de documentación, aplicador de scripts SQL y chequeo de S3
```

## Cómo correrlo

Hay dos ambientes: **local** (tu computadora, lee `.env`) y **producción** (Railway, lee sus variables).
Los dos usan el mismo MySQL de Railway. Guía completa: [`docs/Operations/Environments.md`](docs/Operations/Environments.md).

### Local

```bash
# 1. Una sola vez: crea tu .env y llénalo (cadena de MySQL, Jwt__SecretKey, AWS_*)
cp .env.example .env

# 2. API: Swagger en http://localhost:5000/swagger, salud en /health
dotnet run --project api/USystem.Api

# 3. Pruebas (no necesitan base de datos)
dotnet test
```

Solo si la base está vacía:

```bash
python -m pip install -r tools/Requirements.txt
python tools/ApplyMySqlScripts.py --target-database railway        # crea tablas y datos semilla
dotnet run --project api/USystem.Api -- --seed-admin                 # primer administrador (Seed__AdminPassword en .env)
```

### Producción (Railway)

1. En el servicio de la API → **Variables**, pega el bloque de
   [`Environments.md`](docs/Operations/Environments.md#correr-en-producción-railway) (una sola vez).
2. Haz push a `main` o corre `railway up`.
3. Revisa que `https://<tu-dominio>/health` diga `Healthy`.

## Documentación

| Documento | Contenido |
|---|---|
| [`docs/README.md`](docs/README.md) | Índice general, reglas y fases |
| [`docs/Architecture/Overview.md`](docs/Architecture/Overview.md) | Visión, alcance y conceptos |
| [`docs/Architecture/SolutionStructure.md`](docs/Architecture/SolutionStructure.md) | Capas, proyectos y dependencias |
| [`docs/Modules/README.md`](docs/Modules/README.md) | Los 21 módulos con plan y criterios |
| [`docs/DataModel/README.md`](docs/DataModel/README.md) | Conexión, scripts SQL, tablas y vistas |
| [`docs/Api/Conventions.md`](docs/Api/Conventions.md) | Contrato de la API (JSON en PascalCase) |
| [`docs/Api/LayerExample.md`](docs/Api/LayerExample.md) | Ejemplo completo por capas |
| [`docs/Security/RbacMatrix.md`](docs/Security/RbacMatrix.md) | Matriz de roles y permisos |
| [`docs/Architecture/Internationalization.md`](docs/Architecture/Internationalization.md) | i18n de la API y del desktop |
| [`docs/Operations/Storage.md`](docs/Operations/Storage.md) | Almacenamiento de archivos en S3 |
| [`docs/Operations/Secrets.md`](docs/Operations/Secrets.md) | Manejo y rotación de secretos |
| [`docs/AxzyUiSystem/README.md`](docs/AxzyUiSystem/README.md) | Documentación oficial de AXZY UI System |
| [`docs/Guides/QuickStart.md`](docs/Guides/QuickStart.md) | Cómo levantar el ambiente |

## Regenerar la documentación

```bash
python tools/DocsGenerator/GenerateAll.py
python tools/DocsGenerator/ValidateProject.py
```

El generador es la fuente única del esquema, los permisos y los módulos: al cambiar
`tools/DocsGenerator/Schema.py` o `Modules.py` se actualizan los scripts SQL, las entidades C#, el
diccionario de datos y los documentos de módulo.
