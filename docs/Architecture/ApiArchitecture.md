# Arquitectura de la API

## Flujo de una petición

```
HTTP → Controller → IService → Service → IRepository → Repository → MySQL
                      (DTOs)                (Entities)
```

| Capa | Responsabilidad | Puede depender de |
|------|-----------------|-------------------|
| **Controller** | Recibir HTTP, validar formato, autorizar por permiso, responder con `ApiResponse<T>`. Sin lógica de negocio. | Interfaces de servicio |
| **Service** | Reglas de negocio, transacciones, mapeo Entity ↔ DTO, auditoría. | Interfaces de repositorio |
| **Repository** | Acceso a datos (consultas y persistencia). Sin reglas de negocio. | `DbContext`, entidades |
| **Entity** | Modelo de dominio mapeado a tablas. | — |

## Inversión e inyección de dependencias

- Las **interfaces** se definen en las capas internas: repositorios en `Domain`, servicios en `Application`.
- Las **implementaciones** viven en `Application` (servicios) e `Infrastructure` (repositorios).
- Todo se registra en el contenedor de DI de .NET con un método de extensión por módulo:
  `AddUsersModule()`, `AddRolesModule()`, `AddAcademicTermsModule()`, etc.
- Los controladores **nunca** instancian servicios ni repositorios.

## Nomenclatura en la API

| Elemento | Convención | Ejemplo |
|---|---|---|
| Clase, interfaz, método, propiedad | PascalCase (interfaces con `I`) | `UserService`, `IUserRepository`, `GetByIdAsync` |
| Variables locales y parámetros | **PascalCase** | `UserId`, `CancellationToken` |
| Campos privados | `_` + PascalCase | `_UserRepository` |
| Constantes | PascalCase | `MaxFailedAttempts` |
| Tablas y columnas | PascalCase | `Users.FirstName` |
| Rutas HTTP | minúsculas con guiones (estándar REST, no son identificadores) | `/api/v1/class-groups` |
| Propiedades JSON | PascalCase (`PropertyNamingPolicy = null`) | `{ "FirstName": "..." }` |
| Parámetros de consulta | PascalCase | `?Page=1&PageSize=20&Search=ana&SortBy=Username` |
| Permisos | `Module.Action` | `Users.Create`, `Grades.CloseReport` |
| Valores de catálogo | PascalCase en inglés | `Pending`, `InProgress` |

## Transversales

- **Autorización:** `[RequirePermission("Module.Action")]` valida el claim del JWT en cada acción.
- **Auditoría:** `IAuditService` se invoca desde los servicios en altas, cambios, bajas y acciones críticas.
- **Errores:** middleware global que traduce `BusinessRuleException`, `NotFoundException`,
  `ForbiddenException` y `ValidationException` a `ApiResponse` con el código HTTP correcto.
- **Borrado lógico:** filtro global de EF Core sobre `DeletedAt == null`.
- **Auditoría de campos:** `SaveChanges` llena `CreatedAt`, `CreatedBy`, `UpdatedAt` y `UpdatedBy`.
- **Transacciones:** unidad de trabajo (`IUnitOfWork`) en operaciones multi-tabla (inscripción, pagos).
- **Configuración:** `SystemSettings` cacheado y expuesto por `ISettingsProvider`.
