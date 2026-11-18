# U SYSTEM

Sistema administrativo de gestión escolar universitaria (modelo cuatrimestral) para el personal de la
escuela. **El alumno no tiene acceso a la aplicación.**

## Regla del proyecto

**Todo el código en inglés y PascalCase** (variables, parámetros, clases, constantes, tablas, columnas,
permisos y nombres de archivo). **Nada de snake_case ni camelCase.** **Solo los comentarios van en español.**

La regla, sus excepciones de framework y cómo se aplica con `.editorconfig` y ESLint están en
[`docs/Conventions/Code.md`](docs/Conventions/Code.md).

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
├── database/            Scripts SQL del esquema y datos iniciales, y script de respaldo
├── desktop/             Electron + React TS + AXZY UI (Model · ViewModel · View)
├── docs/                Documentación técnica (arquitectura, convenciones, operación)
└── tools/               Scripts de Python: crear la base de datos y revisar S3
```

## Cómo correrlo

> **¿Primera vez con el proyecto?** Sigue la guía paso a paso
> [`docs/Guides/FirstTimeSetup.md`](docs/Guides/FirstTimeSetup.md): instalación, `.env`, API, desktop,
> pruebas y errores comunes.

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

Todo está indexado en [`docs/README.md`](docs/README.md). Lo más consultado:

| Documento | Contenido |
|---|---|
| [`docs/Architecture/Overview.md`](docs/Architecture/Overview.md) | Qué es el sistema, stack y módulos |
| [`docs/Architecture/ApiConventions.md`](docs/Architecture/ApiConventions.md) | Contrato de la API (JSON en PascalCase) |
| [`docs/Database/README.md`](docs/Database/README.md) | Tablas, vistas y cómo crear la base |
| [`docs/Security/Rbac.md`](docs/Security/Rbac.md) | Roles y permisos |

## Scripts de apoyo

Los scripts de Python en [`tools/`](tools/README.md) crean la base de datos desde los scripts SQL y
verifican el acceso a S3.
