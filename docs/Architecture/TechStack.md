# Stack tecnológico

| Capa | Tecnología |
|------|-----------|
| Base de datos | MySQL 8 (cadena de conexión `ConnectionStrings:DefaultConnection`) |
| API | C# / .NET 8 (ASP.NET Core Web API) |
| ORM | Entity Framework Core 8 + Pomelo.EntityFrameworkCore.MySql |
| Autenticación | JWT (access token + refresh token) |
| Validación | FluentValidation |
| Archivos | S3 compatible: AWS S3, MinIO (desarrollo) o Backblaze |
| i18n API | Recursos JSON (`Api/Resources/`) + `IErrorMessageProvider` |
| i18n desktop | `react-i18next` + catálogos `Es.ts` / `En.ts` |
| Logging | Serilog |
| Documentación API | Swagger / OpenAPI |
| Pruebas API | xUnit + Moq o NSubstitute + Testcontainers (MySQL) |
| Desktop | Electron + React + TypeScript |
| UI | `@axzydev/axzy_ui_system` (v1.7.0, 61 componentes) |
| CSS | Tailwind CSS v4 con `@tailwindcss/vite` (sin `tailwind.config.js`); capas en `desktop/src/styles/Global.css` |
| Tema desktop | `ITThemeProvider` único en `desktop/src/main.tsx` con la paleta de `desktop/src/app/Theme.ts` |
| Documentación UI | `docs/AxzyUiSystem/` (paquete oficial de contexto para IA) |
| Patrón desktop | MVVM |
| Herramienta | Visual Studio (una solución con API y desktop) |

## Reglas derivadas del stack

- **C#**: PascalCase en tipos, miembros, constantes, variables locales y parámetros. `Async` como sufijo
  de métodos asíncronos y `CancellationToken` en la firma.
- **TypeScript**: PascalCase en tipos, componentes, hooks (`use` + PascalCase), variables, funciones y
  constantes. Las props de componentes de AXZY UI y React conservan su nombre original.
- **MySQL**: tablas y columnas en PascalCase; restricciones con prefijo `Uq`, `Fk`, `Ix`.
- **JSON**: propiedades en PascalCase (`"Success"`, `"Data"`, `"FirstName"`).
- **i18n**: claves en PascalCase (`Users.DuplicateEmail`, `Users.List.Title`) y valores traducidos en
  `es` y `en`.

## Paquetes de apoyo

FluentValidation, Serilog, xUnit, Moq o NSubstitute, Testcontainers y `react-i18next` son sugerencias; se
pueden cambiar sin afectar la arquitectura.

## AXZY UI System

La documentación oficial de la librería está copiada en [`docs/AxzyUiSystem/`](../AxzyUiSystem/README.md):
guía de uso, ejemplos de producción y catálogo de los 63 componentes con sus props. Se toma como fuente
para construir las pantallas y para envolver los componentes en `desktop/src/shared/components`.
