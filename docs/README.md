# Documentación de U SYSTEM

Documentación técnica del código. Para levantar el proyecto por primera vez, empieza por
[`Guides/FirstTimeSetup.md`](Guides/FirstTimeSetup.md).

```
docs/
├── Architecture/   Cómo está hecho el sistema
├── Conventions/    Reglas para escribir código, base de datos y Git
├── Database/       Tablas, vistas y relaciones
├── Security/       Autenticación, permisos y protección de datos
├── Operations/     Ambientes, despliegue, secretos, archivos y respaldos
└── Guides/         Paso a paso: instalar, correr y agregar un módulo
```

## Architecture

| Documento | Contenido |
|---|---|
| [`Overview.md`](Architecture/Overview.md) | Qué es el sistema, stack, conceptos y módulos |
| [`SolutionStructure.md`](Architecture/SolutionStructure.md) | Carpetas y proyectos de la solución |
| [`Api.md`](Architecture/Api.md) | Capas de la API y flujo de una petición |
| [`ApiConventions.md`](Architecture/ApiConventions.md) | Contrato JSON, paginación, códigos HTTP y errores |
| [`Desktop.md`](Architecture/Desktop.md) | Patrón MVVM del desktop y reglas de la UI |
| [`Internationalization.md`](Architecture/Internationalization.md) | Textos en español e inglés (API y desktop) |
| [`Decisions.md`](Architecture/Decisions.md) | Decisiones de diseño ya tomadas |

## Conventions

| Documento | Contenido |
|---|---|
| [`Code.md`](Conventions/Code.md) | Nombres, idioma y estilo en C# y TypeScript |
| [`Database.md`](Conventions/Database.md) | Nombres de tablas, columnas, llaves y valores |
| [`Git.md`](Conventions/Git.md) | Ramas, commits y pull requests |

## Database, Security y Operations

| Documento | Contenido |
|---|---|
| [`Database/README.md`](Database/README.md) | Tablas y vistas; los scripts SQL están en [`database/`](../database) |
| [`Security/SecurityPolicy.md`](Security/SecurityPolicy.md) | Reglas de seguridad y checklist antes de producción |
| [`Security/Rbac.md`](Security/Rbac.md) | Roles y permisos |
| [`Operations/Environments.md`](Operations/Environments.md) | Variables de entorno de local y producción |
| [`Operations/Deployment.md`](Operations/Deployment.md) | Despliegue en Railway |
| [`Operations/Secrets.md`](Operations/Secrets.md) | Manejo y rotación de secretos |
| [`Operations/Storage.md`](Operations/Storage.md) | Archivos en S3 |
| [`Operations/Backups.md`](Operations/Backups.md) | Respaldos de la base de datos |
| [`Operations/Settings.md`](Operations/Settings.md) | Parámetros de `SystemSettings` |

## Guides

| Documento | Contenido |
|---|---|
| [`FirstTimeSetup.md`](Guides/FirstTimeSetup.md) | Bajar y correr el proyecto por primera vez |
| [`QuickStart.md`](Guides/QuickStart.md) | Comandos para levantar el ambiente |
| [`HowToAddAModule.md`](Guides/HowToAddAModule.md) | Pasos para agregar un módulo nuevo |
