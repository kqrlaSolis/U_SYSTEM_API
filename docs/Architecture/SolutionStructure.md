# Estructura de la solución

```
USystem/
├── USystem.sln
├── docs/
├── api/
│   ├── USystem.Api/                  Controllers, Attributes, Middlewares, Extensions
│   │   ├── Common/ApiJson.cs
│   │   ├── Configuration/EnvironmentFileLoader.cs
│   │   ├── Contracts/HealthResponse.cs
│   │   ├── Controllers/HealthController.cs
│   │   ├── Attributes/RequirePermissionAttribute.cs
│   │   ├── Middlewares/ExceptionHandlingMiddleware.cs
│   │   ├── Extensions/ServiceCollectionExtensions.cs
│   │   ├── Extensions/SecurityModuleExtensions.cs
│   │   ├── Extensions/HealthModuleExtensions.cs
│   │   ├── Localization/JsonErrorMessageProvider.cs
│   │   ├── Resources/ErrorMessagesEs.json · ErrorMessagesEn.json
│   │   ├── Security/CurrentUserService.cs
│   │   └── Program.cs
│   ├── USystem.Application/          Common, Interfaces, Services
│   │   ├── Common/ApiResponse.cs
│   │   ├── Common/ApiError.cs
│   │   ├── Interfaces/IAuditService.cs
│   │   ├── Interfaces/IErrorMessageProvider.cs
│   │   ├── Services/AuditService.cs
│   │   └── ApplicationExtensions.cs   (AddApplication)
│   ├── USystem.Domain/               Common, Constants, Entities, Exceptions, Interfaces
│   │   ├── Common/PagedQuery.cs
│   │   ├── Common/PagedResult.cs
│   │   ├── Constants/PermissionCodes.cs   (78 permisos generados desde Rbac.py)
│   │   ├── Entities/User.cs               (38 archivos: 37 tablas + AuditableEntity)
│   │   ├── Exceptions/BusinessRuleException.cs
│   │   └── Interfaces/IUserRepository.cs
│   ├── USystem.Infrastructure/       Persistence, Repositories, Security, Storage
│   │   ├── Persistence/USystemDbContext.cs
│   │   ├── Persistence/UnitOfWork.cs
│   │   ├── Repositories/UserRepository.cs
│   │   ├── Security/BCryptPasswordHasher.cs
│   │   ├── Security/JwtTokenGenerator.cs
│   │   ├── Storage/S3FileStorage.cs
│   │   └── InfrastructureExtensions.cs    (AddInfrastructure)
│   └── USystem.Tests/
│       ├── UnitTests/UserServiceTests.cs
│       └── IntegrationTests/UsersEndpointTests.cs
└── desktop/
    ├── package.json
    ├── electron/
    │   ├── Main.ts
    │   └── Preload.ts
    └── src/
        ├── app/
        │   ├── Router.tsx
        │   ├── Providers.tsx
        │   └── layout/MainLayout.tsx
        ├── modules/
        │   └── Users/
        │       ├── model/User.ts
        │       ├── model/UsersService.ts
        │       ├── viewmodel/useUserListViewModel.ts
        │       └── view/UserListPage.tsx
        ├── shared/
        │   ├── components/
        │   ├── hooks/usePermissions.ts
        │   ├── http/HttpClient.ts
        │   ├── i18n/Es.ts, En.ts
        │   └── utils/
        └── assets/
```

## Dependencias entre proyectos

```
Api ──► Application ──► Domain ◄── Infrastructure
 │                                      ▲
 └──────────────────────────────────────┘   (solo para registrar DI en Program.cs)
```

- `Domain` no depende de nadie: entidades, enums, constantes e **interfaces de repositorio**.
- `Application` depende solo de `Domain`: servicios, DTOs, validadores e **interfaces de servicio**.
- `Infrastructure` implementa las interfaces de `Domain` (repositorios) y la infraestructura técnica.
- `Api` compone todo con inyección de dependencias y no instancia servicios.

`desktop/` se agrega a la solución de Visual Studio como proyecto de tipo Node.js o carpeta.

## Convenciones de nombres

| Elemento | Convención | Ejemplo |
|---|---|---|
| Proyectos | `USystem.<Capa>` | `USystem.Application` |
| Namespaces | `USystem.<Capa>.<Carpeta>` | `USystem.Application.Services` |
| Clases, interfaces, enums | PascalCase; interfaces con `I` | `UserService`, `IUserRepository` |
| Métodos y propiedades | PascalCase | `GetByIdAsync`, `FirstName` |
| Variables locales y parámetros | **PascalCase** | `UserId`, `CancellationToken` |
| Campos privados | `_` + PascalCase | `_UserRepository` |
| Constantes | PascalCase | `MaxFailedAttempts` |
| Archivos | PascalCase igual al tipo | `UserService.cs`, `UserListPage.tsx` |
| Tablas y columnas | PascalCase | `ClassGroups.Classroom` |
| Carpetas | C#: PascalCase (`Controllers/`) · desktop: minúsculas (`modules/`, `viewmodel/`) | `Services/`, `model/` |
| Rutas HTTP | minúsculas con guiones (estándar REST) | `/api/v1/class-groups` |

> Las variables locales y los parámetros en PascalCase son una decisión del proyecto, no del estándar de
> C#. Ver `Guides/CodingConventions.md` y la configuración de `.editorconfig`.
