# Estructura en capas — U SYSTEM

> **Entregable:** documentación de la estructura en capas del proyecto final.
> **Aplica a:** solución `USystem.sln` (API .NET 8) + aplicación de escritorio `desktop/` (Electron + React TS) + base de datos MySQL 8.

## Ficha del documento

| Dato | Valor |
|---|---|
| Proyecto | **U SYSTEM** — sistema administrativo de gestión escolar universitaria (modelo cuatrimestral) |
| Tipo de producto | Aplicación de escritorio para el personal de la escuela (el alumno **no** es usuario) |
| Arquitectura | Cliente–servidor en capas: presentación (desktop) → API en 4 capas (Controller → Service → Repository → Entity) → MySQL 8 |
| Versión del documento | 1.0 |
| Fecha | 4 de noviembre de 2026 |
| Código fuente | `api/` · `desktop/` · `docs/DataModel/` |
| Solución | `USystem.sln` — 5 proyectos .NET 8 |

## Cómo se responde la tarea

| Punto de la tarea | Sección de este documento | Evidencia en el repositorio |
|---|---|---|
| 1. Bibliotecas de clases por capa | [§2](#2-bibliotecas-de-clases-por-capa) | `*.csproj`, `desktop/package.json` |
| 2. Clases de cada capa según la funcionalidad | [§3](#3-clases-de-cada-capa) | Carpetas `Entities/`, `Services/`, `Repositories/`, `Controllers/`, `modules/` |
| 3. Objetos de datos según la base de datos | [§4](#4-objetos-de-datos) | `docs/DataModel/Scripts/`, `docs/DataModel/Entities/`, `Dtos/` |
| 4. Referencias entre capas | [§5](#5-referencias-entre-capas) | `ProjectReference` de cada `.csproj`, `Program.cs`, `HttpClient.ts` |
| 5. Esquema de la interfaz gráfica | [§6](#6-esquema-de-la-interfaz-gráfica-capa-de-presentación) | `desktop/src/app/Router.tsx`, `NavigationModules.ts`, `modules/*/view/` |

---

## 1. Vista general de la arquitectura

U SYSTEM se divide en **seis piezas de software** y **una base de datos**. Cuatro de ellas son capas
lógicas del servidor, una es la capa de presentación y una agrupa las pruebas.

```
PRESENTACIÓN      U SYSTEM Desktop · Electron + React + TS · MVVM
                        │  HTTP /api/v1 · JSON PascalCase · JWT
                        ▼
API               USystem.Api · Controllers · Permisos · Middleware
                        │  interfaces de servicio
                        ▼
APLICACIÓN        USystem.Application · Services · DTOs · Validadores
                        │  interfaces de repositorio
                        ▼
DOMINIO           USystem.Domain · 37 entidades · 28 interfaces
                        ▲  implementa esas interfaces
                        │
INFRAESTRUCTURA   USystem.Infrastructure · 22 repositorios · EF Core
                        │  EF Core 8 + Pomelo
                        ▼
DATOS             MySQL 8 · 37 tablas · 2 vistas
```

| Capa | Responsabilidad | Biblioteca (proyecto) | Artefactos principales |
|---|---|---|---|
| **Presentación** | Capturar y mostrar información; navegar; validar formato; respetar permisos en la UI | `desktop/` (aplicación, no biblioteca .NET) | 22 módulos MVVM, 43 pantallas, 21 ViewModels, `HttpClient` |
| **API / Controller** | Recibir HTTP, autorizar por permiso, validar el contrato y responder `ApiResponse<T>` | `USystem.Api` | 30 controllers, 10 registros de DI, middleware, filtros |
| **Aplicación / Service** | Reglas de negocio, transacciones, auditoría y mapeo Entity ↔ DTO | `USystem.Application` | 24 servicios, 58 archivos de DTO, 29 validadores |
| **Dominio / Entity** | Modelo del negocio y contratos de persistencia | `USystem.Domain` | 37 entidades, 28 interfaces, 5 excepciones, 78 permisos |
| **Infraestructura / Repository** | Acceso a datos y servicios técnicos (hash, JWT, archivos) | `USystem.Infrastructure` | 22 repositorios, `USystemDbContext`, `UnitOfWork` |
| **Pruebas** | Verificar reglas, servicios y endpoints | `USystem.Tests` | 35 archivos (20 unitarias, 12 de servicio, 3 de integración) |
| **Datos** | Persistencia relacional y vistas de consulta | MySQL 8 (`docs/DataModel/Scripts/`) | 37 tablas, 2 vistas, 6 scripts + semilla |

**Flujo de una petición**

```
HTTP → Controller → IService → Service → IRepository → Repository → EF Core → MySQL
                     (DTOs)              (Entities)
```

**Principios que ordenan las capas**

1. **Las dependencias apuntan hacia adentro:** `Api → Application → Domain` y `Infrastructure → Domain`.
   El dominio no conoce a nadie.
2. **La API es la autoridad:** la UI oculta opciones por permiso, pero cada endpoint vuelve a validarlo.
3. **Las reglas de negocio viven en el servicio:** controllers y repositorios no deciden reglas.
4. **Inversión de dependencias:** se inyectan interfaces (`IUserService`, `IUserRepository`), nunca clases
   concretas entre capas.
5. **Todo el código en inglés y PascalCase**, con comentarios en español (ver `Guides/CodingConventions.md`).

---

## 2. Bibliotecas de clases por capa

### 2.1 Bibliotecas de la API (.NET 8)

| # | Biblioteca de clases | Tipo de proyecto | TFM | Paquetes NuGet | Responsabilidad |
|---|---|---|---|---|---|
| 1 | `USystem.Domain` | `Microsoft.NET.Sdk` (classlib) | `net8.0` | **Ninguno** (dependencia cero) | Entidades, interfaces de repositorio, excepciones de negocio, constantes y permisos |
| 2 | `USystem.Application` | `Microsoft.NET.Sdk` (classlib) | `net8.0` | FluentValidation 11.12.0 · FluentValidation.DependencyInjectionExtensions 11.12.0 · Microsoft.Extensions.Logging.Abstractions 8.0.2 | Servicios de caso de uso, DTOs, validadores, mappers y contratos de respuesta |
| 3 | `USystem.Infrastructure` | `Microsoft.NET.Sdk` (classlib) | `net8.0` | Pomelo.EntityFrameworkCore.MySql 8.0.3 · Microsoft.EntityFrameworkCore.Design 8.0.11 · BCrypt.Net-Next 4.0.3 · System.IdentityModel.Tokens.Jwt 7.1.2 · AWSSDK.S3 3.7.511.8 · Microsoft.Extensions.Caching.Memory 8.0.1 · Microsoft.Extensions.Http 8.0.1 · Microsoft.Extensions.Options.ConfigurationExtensions 8.0.0 | Implementación de repositorios, `DbContext`, unidad de trabajo, hash de contraseñas, tokens JWT y almacenamiento de archivos |
| 4 | `USystem.Api` | `Microsoft.NET.Sdk.Web` | `net8.0` | Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11 · Serilog.AspNetCore 8.0.3 · Serilog.Sinks.Console 6.0.0 · Serilog.Sinks.File 6.0.0 · Swashbuckle.AspNetCore 6.9.0 | Punto de entrada HTTP: controllers, filtros de autorización y validación, middleware de errores, localización, DI y Swagger |
| 5 | `USystem.Tests` | `Microsoft.NET.Sdk` (`IsTestProject`) | `net8.0` | xunit 2.5.3 · xunit.runner.visualstudio 2.5.3 · Microsoft.NET.Test.Sdk 17.8.0 · Moq 4.20.72 · Microsoft.AspNetCore.Mvc.Testing 8.0.11 · Microsoft.EntityFrameworkCore.InMemory 8.0.11 · Microsoft.Extensions.Diagnostics.HealthChecks 8.0.11 · coverlet.collector 6.0.0 | Pruebas unitarias de servicios y utilidades, y de integración de endpoints y base de datos |

### 2.2 Por qué cada dependencia externa

| Paquete | Para qué se usa | Capa que lo declara |
|---|---|---|
| **Pomelo.EntityFrameworkCore.MySql** | Proveedor de EF Core 8 para MySQL 8 (mapeo, migraciones, funciones) | Infrastructure |
| **Microsoft.EntityFrameworkCore.Design** | Herramientas de diseño (`dotnet ef`); `PrivateAssets=all` | Infrastructure |
| **BCrypt.Net-Next** | Hash de contraseñas con `BCryptPasswordHasher` | Infrastructure |
| **System.IdentityModel.Tokens.Jwt** | Generación y firma de access tokens (`JwtTokenGenerator`) | Infrastructure |
| **AWSSDK.S3** | Almacenamiento de documentos y material didáctico (`S3FileStorage`, compatible con MinIO/Backblaze) | Infrastructure |
| **Microsoft.Extensions.Caching.Memory** | Caché de `SystemSettings` (`SettingsProvider`) | Infrastructure |
| **FluentValidation (+ DI)** | Validación declarativa de los request DTO, registrada en `AddApplication()` | Application |
| **Microsoft.Extensions.Logging.Abstractions** | Contratos de log en servicios sin acoplarlos a un sink | Application |
| **JwtBearer** | Autenticación del esquema Bearer en la API | Api |
| **Serilog.AspNetCore + sinks** | Bitácora estructurada a consola y archivo | Api |
| **Swashbuckle.AspNetCore** | Documentación OpenAPI/Swagger de los endpoints | Api |
| **xunit + Moq + Mvc.Testing + EF InMemory** | Pruebas unitarias, de integración y dobles de repositorio | Tests |

### 2.3 Capa de presentación (desktop)

La capa de presentación **no es una biblioteca .NET**: es una aplicación Electron + React + TypeScript.
Su "biblioteca" son los paquetes npm declarados en `desktop/package.json`.

| Grupo | Paquete | Versión | Uso |
|---|---|---|---|
| Shell | `electron` | ^38.0.0 | Ventana de escritorio, menú, IPC, CSP |
| Empaquetado | `electron-builder` | ^26.0.0 | Instalador Windows/Linux/macOS (`dist:win`, `dist:linux`, `dist:mac`) |
| UI | `react` / `react-dom` | ^19.0.0 | Render del árbol de componentes |
| UI | `@axzydev/axzy_ui_system` | ^1.6.0 | Chasis `ITLayout`, `ITPage`, tablas, formularios, botones (63 componentes) |
| Navegación | `react-router-dom` | ^7.0.0 | `HashRouter`, 54 rutas privadas + 3 públicas |
| Estado de idioma | `i18next` / `react-i18next` | ^25.0.0 / ^16.0.0 | Catálogos `Es.ts` y `En.ts` |
| Animación | `lottie-react` | ^3.1.2 | Pantallas vacías y de bienvenida |
| Estilos | `tailwindcss` + `@tailwindcss/vite` | ^4.3.3 | Capas CSS declaradas en `styles/Global.css` |
| Compilación | `typescript`, `vite`, `@vitejs/plugin-react` | ^5.9.0 / ^7.0.0 / ^5.0.0 | Tipado estricto y build del renderer |
| Pruebas | `vitest`, `@testing-library/react`, `jsdom` | ^3.0.0 / ^16.0.0 / ^26.0.0 | Pruebas de ViewModels y componentes |

### 2.4 Lo que no aplica (y por qué)

- **No existe una biblioteca de clases para la presentación .NET** (WinForms/WPF): la capa de
  presentación es una app Electron. Se documenta con sus paquetes y su estructura de módulos.
- **No hay biblioteca separada de "acceso a datos"**: los repositorios son la capa de acceso a datos y
  viven en `USystem.Infrastructure` junto con los servicios técnicos.
- **No hay proyecto web, móvil ni portal del alumno**: el alumno no inicia sesión en U SYSTEM
  (`Architecture/StudentAppReadiness.md` lo deja como proyecto futuro).
- **La base de datos no se genera por ORM**: los scripts SQL de `docs/DataModel/Scripts/` son la fuente
  de verdad y las entidades solo se mapean contra ellos.

---

## 3. Clases de cada capa

### 3.1 Resumen del inventario

| Capa | Biblioteca | Clases y archivos | Detalle |
|---|---|---|---|
| Dominio | `USystem.Domain` | 38 archivos de entidad · 28 interfaces · 5 excepciones · 4 tipos comunes · 3 constantes | §3.2 |
| Aplicación | `USystem.Application` | 58 archivos de DTO (88 clases) · 24 servicios, todos con interfaz · 29 validadores · 5 mappers | §3.3 |
| Infraestructura | `USystem.Infrastructure` | 22 repositorios · 6 archivos de persistencia · 3 de seguridad · 2 de almacenamiento | §3.4 |
| API | `USystem.Api` | 30 controllers · 10 extensiones de DI · 1 atributo · 1 filtro · 1 middleware · 2 contratos | §3.5 |
| Pruebas | `USystem.Tests` | 35 archivos de prueba | §3.6 |
| Presentación | `desktop/src` | 22 módulos · 43 pantallas · 21 ViewModels · ≈60 archivos de modelo · ≈35 piezas compartidas | §3.7 |

### 3.2 Capa de dominio — `USystem.Domain`

Es el corazón del sistema: no depende de ninguna otra biblioteca ni de frameworks externos.

| Carpeta | Clases | Propósito |
|---|---|---|
| `Entities/` | `AuditableEntity` (base) + 37 entidades | Modelo del negocio mapeado 1:1 con las tablas |
| `Interfaces/` | 22 `I<Entidad>Repository` + `IRepository<T>` genérico + `IUnitOfWork`, `IPasswordHasher`, `ITokenGenerator`, `IFileStorage`, `ICurrentUser` | Contratos que implementa la infraestructura; permiten invertir dependencias |
| `Exceptions/` | `USystemException` (base con `ErrorCode` y `StatusCode`), `BusinessRuleException`, `NotFoundException`, `ForbiddenException`, `ValidationException` | Errores de negocio que el middleware traduce a HTTP |
| `Common/` | `PagedQuery`, `PagedResult<T>`, `AuditLogQuery`, `AccessToken` | Tipos transversales de paginación y resultados |
| `Constants/` | `PermissionCodes` (78 permisos `Módulo.Acción`), `AcademicTermStatuses`, `StudyPlanSubjectTypes` | Valores de catálogo sin números ni cadenas mágicas |

**Entidades por área funcional (37)**

| Área | Entidades |
|---|---|
| Seguridad y auditoría (8) | `User`, `Role`, `Permission`, `RolePermission`, `UserRole`, `RefreshToken`, `PasswordResetToken`, `AuditLog` |
| Catálogos académicos (7) | `Career`, `StudyPlan`, `StudyPlanSubject`, `Subject`, `SubjectPrerequisite`, `AcademicTerm`, `CourseMaterial` |
| Operación académica (10) | `Teacher`, `TeacherSubject`, `ClassGroup`, `ClassGroupSchedule`, `Student`, `StudentGuardian`, `DocumentType`, `StudentDocument`, `Enrollment`, `EnrollmentClassGroup` |
| Seguimiento (1) | `Grade` |
| Finanzas (6) | `PaymentConcept`, `PaymentPlan`, `Scholarship`, `StudentScholarship`, `Charge`, `Payment` |
| Complementarios (5) | `ProcedureType`, `Procedure`, `NotificationTemplate`, `Notification`, `SystemSetting` |

**Ejemplo — `User` (extracto de `Entities/User.cs`)**

```csharp
namespace USystem.Domain.Entities;

// Personal de la escuela con acceso al sistema (el alumno no es usuario)
public class User : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string PaternalLastName { get; set; } = string.Empty;
    public string? MaternalLastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }
}
```

`AuditableEntity` aporta `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy` y `DeletedAt` a todas
las entidades (borrado lógico y auditoría de campos de forma automática en `SaveChanges`).

### 3.3 Capa de aplicación — `USystem.Application`

| Carpeta | Contenido | Responsabilidad |
|---|---|---|
| `Services/` | 24 servicios + 11 interfaces | Reglas de negocio, transacciones, auditoría y mapeo a DTO |
| `Dtos/` | 58 archivos (88 clases) | Contratos de entrada y salida de la API |
| `Validators/` | 29 validadores FluentValidation | Formato y obligatoriedad de los request |
| `Mappings/` | `UserMapper`, `RoleMapper`, `CatalogMapper`, `StudyPlanMapper`, `TeacherMapper` | Conversión Entity → DTO |
| `Common/` | `ApiResponse<T>`, `ApiError`, `PasswordPolicy`, `SecureToken`, `SettingRules` | Envoltura de respuesta y reglas compartidas |
| `Interfaces/` | `IAuditService`, `IErrorMessageProvider`, `ISettingsProvider` | Servicios transversales que implementa otra capa |

**Servicios por módulo (24)**

`AuthService`, `UserService`, `RoleService`, `CareerService`, `SubjectService`, `AcademicTermService`,
`StudyPlanService`, `CourseMaterialService`, `TeacherService`, `ClassGroupService`, `StudentService`,
`EnrollmentService`, `EnrollmentChecklistService`, `ReEnrollmentService`, `GradeService`,
`TranscriptService`, `AcademicProgressService`, `PaymentService`, `ProcedureService`,
`NotificationService`, `ReportService`, `SettingsService`, `AuditService`, `AuditLogService`.

**Interfaces de servicio (24)**

Los 24 servicios se inyectan a través de su interfaz. Once se declaran en un archivo propio
(`IUserService.cs`, `IRoleService.cs`, `IAuthService.cs`, `ICareerService.cs`, `ISubjectService.cs`,
`IAcademicTermService.cs`, `IStudyPlanService.cs`, `ITeacherService.cs`, `IStudentService.cs`,
`ISettingsService.cs`, `IAuditLogService.cs`), doce se declaran dentro del archivo de su implementación
(`IClassGroupService`, `IEnrollmentService`, `IGradeService`, …) y `IAuditService` vive en
`Interfaces/` por ser un servicio transversal.

**Familias de DTO**

| Familia | Ejemplos |
|---|---|
| Identidad y acceso | `LoginRequest`, `LoginResponse`, `RefreshTokenRequest`, `ChangePasswordRequest`, `ForgotPasswordRequest`, `ResetPasswordRequest` |
| Usuarios y roles | `UserDto`, `CreateUserRequest`, `UpdateUserRequest`, `CreatedUserResponse`, `TemporaryPasswordResponse`, `RoleDto`, `CreateRoleRequest`, `AssignRolesRequest`, `PermissionDto` |
| Catálogos | `CareerDto`, `CreateCareerRequest`, `SubjectDto`, `AcademicTermDto`, `StudyPlanDto`, `StudyPlanSubjectDto`, `SubjectPrerequisiteDto`, `CourseMaterialDto` |
| Operación | `TeacherDto`, `ClassGroupDto`, `StudentDto`, `CreateStudentRequest`, `EnrollmentDto`, `EnrollmentChecklistDto`, `CancelEnrollmentRequest`, `ReEnrollmentProposalDto` |
| Seguimiento | `GradeDto`, `TranscriptDto`, `AcademicProgressDto` |
| Finanzas y trámites | `PaymentConceptDto`, `PaymentPlanDto`, `ProcedureDto` |
| Complementarios | `NotificationDto`, `SystemSettingDto`, `UpdateSettingRequest`, `DashboardDto`, `AuditLogDto` |

**Reglas de negocio en el servicio (ejemplo real, `UserService`)**

```csharp
// El correo debe ser único: la excepción lleva código y valores, no el texto final
if (await _UserRepository.ExistsByEmailAsync(Request.Email, Cancellation))
{
    throw new BusinessRuleException(
        "Users.DuplicateEmail",
        new Dictionary<string, string> { ["Email"] = Request.Email });
}

var NewUser = new User
{
    Email = Request.Email,
    PasswordHash = _PasswordHasher.Hash(Request.TemporaryPassword),
    MustChangePassword = true,
    IsActive = true,
};

await _UserRepository.AddAsync(NewUser, Cancellation);
await _UnitOfWork.SaveChangesAsync(Cancellation);
await _AuditService.LogAsync("Create", "Users", nameof(User), NewUser.Id.ToString(), null, NewUser, Cancellation);
```

### 3.4 Capa de infraestructura — `USystem.Infrastructure`

| Carpeta | Clases | Responsabilidad |
|---|---|---|
| `Persistence/` | `USystemDbContext`, `UnitOfWork`, `SettingsProvider`, `AdminUserSeeder`, `MySqlVersionResolver`, `USystemDbContextFactory` | Mapeo EF Core, `DbSet<>`, filtro global de borrado lógico, auditoría en `SaveChanges`, semilla del administrador |
| `Repositories/` | 22 repositorios (`UserRepository`, `RoleRepository`, `CareerRepository`, `SubjectRepository`, `AcademicTermRepository`, `StudyPlanRepository`, `CourseMaterialRepository`, `TeacherRepository`, `ClassGroupRepository`, `StudentRepository`, `EnrollmentRepository`, `EnrollmentChecklistRepository`, `GradeRepository`, `TranscriptRepository`, `ReportRepository`, `PaymentRepository`, `ProcedureRepository`, `NotificationRepository`, `SystemSettingRepository`, `AuditLogRepository`, `RefreshTokenRepository`, `PasswordResetTokenRepository`) | Consultas, paginación, filtros y persistencia; sin reglas de negocio |
| `Security/` | `BCryptPasswordHasher`, `JwtTokenGenerator`, `JwtOptions` | Hash de contraseñas y emisión de tokens |
| `Storage/` | `S3FileStorage`, `StorageOptions` | Carga y descarga de archivos en almacenamiento S3 compatible |
| Raíz | `InfrastructureExtensions` (`AddInfrastructure`) | Registra `DbContext`, repositorios y servicios técnicos |

### 3.5 Capa de API — `USystem.Api`

| Carpeta | Clases | Responsabilidad |
|---|---|---|
| `Controllers/` | 30 controllers | Un controller por módulo; recibe HTTP, autoriza y responde |
| `Extensions/` | `ServiceCollectionExtensions`, `UsersModuleExtensions`, `RolesModuleExtensions`, `SecurityModuleExtensions`, `CareCatalogModuleExtensions`, `StudyPlansModuleExtensions`, `AcademicOperationsModuleExtensions`, `AuditLogsModuleExtensions`, `SettingsModuleExtensions`, `HealthModuleExtensions` | Un método `Add<Módulo>Module()` por módulo para registrar sus dependencias |
| `Attributes/` | `RequirePermissionAttribute` | Valida el claim `Permission` del JWT (`Users.Create`, `Grades.CloseReport`, …) |
| `Filters/` | `ValidationFilter` | Ejecuta FluentValidation y devuelve 400 con `ApiError.Campo` |
| `Middlewares/` | `ExceptionHandlingMiddleware` | Traduce las excepciones de dominio a `ApiResponse` con su código HTTP |
| `Localization/` | `JsonErrorMessageProvider`, `LocalizationOptions` | Mensajes de error en `es`/`en` desde `Resources/ErrorMessages*.json` |
| `Security/` | `CurrentUserService` | Expone el usuario autenticado a las capas internas (`ICurrentUser`) |
| `Contracts/` | `HealthResponse`, `SessionResponse` | Contratos específicos de endpoints de infraestructura |
| Raíz | `Program.cs`, `Common/ApiJson.cs`, `Configuration/EnvironmentFileLoader.cs` | Composición de la aplicación, serialización PascalCase y carga de `.env` |

**Controllers y rutas base**

| Controller | Ruta base | Controller | Ruta base |
|---|---|---|---|
| `AuthController` | `/api/v1/auth` | `SessionController` | `/api/v1/session` |
| `UsersController` | `/api/v1/users` | `RolesController` | `/api/v1/roles` |
| `PermissionsController` | `/api/v1/permissions` | `AuditLogsController` | `/api/v1/audit-logs` |
| `CareersController` | `/api/v1/careers` | `SubjectsController` | `/api/v1/subjects` |
| `AcademicTermsController` | `/api/v1/academic-terms` | `StudyPlansController` | `/api/v1/study-plans` |
| `CourseMaterialsController` | `/api/v1/course-materials` | `TeachersController` | `/api/v1/teachers` |
| `ClassGroupsController` | `/api/v1/class-groups` | `StudentsController` | `/api/v1/students` |
| `EnrollmentsController` | `/api/v1/enrollments` | `EnrollmentChecklistController` | `/api/v1/students/{Id}/enrollment-eligibility` |
| `ReEnrollmentsController` | `/api/v1/re-enrollments` | `GradesController` | `/api/v1/grades` |
| `TranscriptsController` | `/api/v1/transcripts` | `AcademicProgressController` | `/api/v1/academic-progress` |
| `ChargesController` | `/api/v1/charges` | `PaymentConceptsController` | `/api/v1/payment-concepts` |
| `PaymentPlansController` | `/api/v1/payment-plans` | `ProceduresController` | `/api/v1/procedures` |
| `ProcedureTypesController` | `/api/v1/procedure-types` | `NotificationsController` | `/api/v1/notifications` |
| `NotificationTemplatesController` | `/api/v1/notification-templates` | `ReportsController` | `/api/v1/reports` |
| `SettingsController` | `/api/v1/settings` | `HealthController` | `/api/v1/health` |

### 3.6 Capa de pruebas — `USystem.Tests`

| Grupo | Archivos | Qué verifica |
|---|---|---|
| `UnitTests/` (20) | `AuthServiceTests`, `UserServiceTests`, `RoleServiceTests`, `CareerServiceTests`, `SubjectServiceTests`, `AcademicTermServiceTests`, `StudyPlanServiceTests`, `SettingsServiceTests`, `AuditServiceTests`, `AuditLogServiceTests`, `PasswordPolicyTests`, `PagedQueryTests`, `ApiResponseTests`, `PermissionCodesTests`, `JwtTokenGeneratorTests`, `BCryptPasswordHasherTests`, `RequirePermissionAttributeTests`, `ExceptionHandlingMiddlewareTests`, `JsonErrorMessageProviderTests`, `USystemDbContextTests` | Reglas de negocio y utilidades con Moq |
| `Services/` (12) | `ClassGroupServiceTests`, `TeacherServiceTests`, `StudentServiceTests`, `EnrollmentServiceTests`, `ReEnrollmentServiceTests`, `GradeServiceTests`, `TranscriptServiceTests`, `AcademicProgressServiceTests`, `PaymentServiceTests`, `ProcedureServiceTests`, `NotificationServiceTests`, `CourseMaterialServiceTests` | Reglas de los módulos de operación, seguimiento y finanzas |
| `IntegrationTests/` (3) | `HealthEndpointTests`, `SessionEndpointTests`, `DatabaseConnectivityTests` | Endpoints reales con `WebApplicationFactory` y conexión a MySQL |

### 3.7 Capa de presentación — `desktop/src` (MVVM)

| Elemento MVVM | Implementación | Regla |
|---|---|---|
| **Model** | `modules/<Módulo>/model/*.ts` — tipos y `...Service.ts` | Refleja el contrato JSON de la API en PascalCase; no conoce React |
| **ViewModel** | `modules/<Módulo>/viewmodel/use...ViewModel.ts` | Estado, validaciones, permisos, paginación y errores; **sin JSX** |
| **View** | `modules/<Módulo>/view/...Page.tsx` | Solo renderiza con AXZY UI y delega eventos al ViewModel |

| Carpeta | Contenido | Responsabilidad |
|---|---|---|
| `app/` | `Router.tsx`, `Providers.tsx`, `Theme.ts`, `layout/` | Rutas, providers (i18n, auth, router), tema y chasis |
| `modules/` | 22 módulos (`Auth`, `Home`, `Users`, `Roles`, `AuditLogs`, `Careers`, `Subjects`, `AcademicTerms`, `StudyPlans`, `CourseMaterials`, `Teachers`, `Students`, `ClassGroups`, `Enrollments`, `ReEnrollments`, `Grades`, `Tracking`, `Payments`, `Procedures`, `Notifications`, `Reports`, `Settings`) | Un módulo por módulo funcional de la API |
| `shared/components/` | `AppButton`, `DataTable`, `FilterBar`, `SearchBox`, `FormField`, `ConfirmDialog`, `LottieAnimation`, `ComingSoonPage` | Envoltorios de AXZY UI con props en PascalCase |
| `shared/crud/` | `CrudListPage`, `CrudFormPage`, `CrudTypes`, `useCrudListViewModel`, `useCrudFormViewModel` | CRUD genérico para catálogos: el módulo solo declara columnas, campos y llamadas |
| `shared/hooks/` | `usePermissions`, `usePagedQuery`, `useFilterOptions` | Permisos efectivos, paginación y catálogos de filtros |
| `shared/http/` | `HttpClient`, `ApiResponse`, `ApiErrorException`, `GetErrorMessage` | JWT, refresh en 401, `Accept-Language` y errores localizados |
| `shared/auth/` | `AuthContext`, `RequireAuth`, `AuthPaths` | Sesión, guardas de ruta y rutas públicas |
| `shared/i18n/` | `I18nProvider`, `Config`, `Es.ts`, `En.ts` | Traducción de toda la interfaz |
| `shared/utils/` | `Format.ts` | Fechas, moneda y números |
| `electron/` | `Main.ts`, `Preload.ts`, `Types.d.ts` | Ventana, menú, CSP, IPC seguro |
| `styles/` | `Global.css` | Capas de Tailwind + CSS de AXZY UI |

**Ejemplo — el mismo módulo en las tres piezas**

| Pieza | Archivo | Contenido |
|---|---|---|
| Model | `modules/Users/model/User.ts` | `interface User { Id: number; FirstName: string; … }` |
| Model | `modules/Users/model/UsersService.ts` | `HttpClient.Get<PagedResult<User>>('/api/v1/users', { Page, PageSize, Search })` |
| ViewModel | `modules/Users/viewmodel/useUserListViewModel.ts` | `Items`, `Page`, `Search`, `IsLoading`, `ErrorMessage`, `CanCreate`, `Reload()` |
| View | `modules/Users/view/UserListPage.tsx` | `<ITPage …><DataTable …/></ITPage>` |

### 3.8 Recorrido completo de una clase por todas las capas

Caso: *crear un usuario* (`Users.Create`).

```
UserFormPage.tsx            View: captura el formulario y llama a Save()
  └─ useUserFormViewModel   ViewModel: valida campos y permisos, arma CreateUserRequest
      └─ UsersService       Model: HttpClient.Post('/api/v1/users', Request)
          └─ HTTP POST ──────────────────────────────►  UsersController.Create
                                                            └─ IUserService.CreateAsync   (Application)
                                                                └─ UserService             reglas + hash + auditoría
                                                                    ├─ IUserRepository     (Domain)
                                                                    │   └─ UserRepository  (Infrastructure)
                                                                    │       └─ USystemDbContext → MySQL
                                                                    └─ IUnitOfWork / IAuditService
                                                                ◄── UserDto
                                                        ◄── ApiResponse<UserDto> (JSON PascalCase)
```

---

## 4. Objetos de datos

### 4.1 Fuente de verdad

| Aspecto | Definición |
|---|---|
| Motor | MySQL 8, `InnoDB`, `utf8mb4_0900_ai_ci` |
| Base de datos | `USystem` |
| Scripts | `docs/DataModel/Scripts/01_Security.sql` … `06_Extras.sql` + `99_SeedData.sql` |
| Diccionario | Un archivo por tabla en `docs/DataModel/Entities/` |
| Volumen | **37 tablas** y **2 vistas** |
| Convención | Tablas y columnas en inglés y PascalCase; `Id` como PK; `<Entidad>Id` como FK; restricciones `Uq`, `Fk`, `Ix` |
| Auditoría | `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt` (borrado lógico) |
| Dinero | `DECIMAL(12,2)` |

### 4.2 Objetos de datos (tablas) y su clase en C#

| Script | Tabla | Clase C# | Módulo |
|---|---|---|---|
| `01_Security` | `Users` | `User` | Users |
| `01_Security` | `RefreshTokens` | `RefreshToken` | Auth |
| `01_Security` | `PasswordResetTokens` | `PasswordResetToken` | Auth |
| `01_Security` | `Roles` | `Role` | Roles |
| `01_Security` | `Permissions` | `Permission` | Roles |
| `01_Security` | `RolePermissions` | `RolePermission` | Roles |
| `01_Security` | `UserRoles` | `UserRole` | Roles |
| `01_Security` | `AuditLogs` | `AuditLog` | AuditLogs |
| `02_AcademicCatalogs` | `Careers` | `Career` | Careers |
| `02_AcademicCatalogs` | `Subjects` | `Subject` | Subjects |
| `02_AcademicCatalogs` | `AcademicTerms` | `AcademicTerm` | AcademicTerms |
| `02_AcademicCatalogs` | `StudyPlans` | `StudyPlan` | StudyPlans |
| `02_AcademicCatalogs` | `StudyPlanSubjects` | `StudyPlanSubject` | StudyPlans |
| `02_AcademicCatalogs` | `SubjectPrerequisites` | `SubjectPrerequisite` | Subjects |
| `02_AcademicCatalogs` | `CourseMaterials` | `CourseMaterial` | CourseMaterials |
| `03_AcademicOperations` | `Teachers` | `Teacher` | Teachers |
| `03_AcademicOperations` | `TeacherSubjects` | `TeacherSubject` | Teachers |
| `03_AcademicOperations` | `ClassGroups` | `ClassGroup` | ClassGroups |
| `03_AcademicOperations` | `ClassGroupSchedules` | `ClassGroupSchedule` | ClassGroups |
| `03_AcademicOperations` | `Students` | `Student` | Students |
| `03_AcademicOperations` | `StudentGuardians` | `StudentGuardian` | Students |
| `03_AcademicOperations` | `DocumentTypes` | `DocumentType` | Students |
| `03_AcademicOperations` | `StudentDocuments` | `StudentDocument` | Students |
| `03_AcademicOperations` | `Enrollments` | `Enrollment` | Enrollments |
| `03_AcademicOperations` | `EnrollmentClassGroups` | `EnrollmentClassGroup` | Enrollments |
| `04_AcademicTracking` | `Grades` | `Grade` | Grades |
| `05_Finance` | `PaymentConcepts` | `PaymentConcept` | Payments |
| `05_Finance` | `PaymentPlans` | `PaymentPlan` | Payments |
| `05_Finance` | `Scholarships` | `Scholarship` | Payments |
| `05_Finance` | `StudentScholarships` | `StudentScholarship` | Payments |
| `05_Finance` | `Charges` | `Charge` | Payments |
| `05_Finance` | `Payments` | `Payment` | Payments |
| `06_Extras` | `ProcedureTypes` | `ProcedureType` | Procedures |
| `06_Extras` | `Procedures` | `Procedure` | Procedures |
| `06_Extras` | `NotificationTemplates` | `NotificationTemplate` | Notifications |
| `06_Extras` | `Notifications` | `Notification` | Notifications |
| `06_Extras` | `SystemSettings` | `SystemSetting` | Settings |

**Vistas de solo lectura**

| Vista | Contenido | Se consume como |
|---|---|---|
| `TranscriptView` | Kardex por alumno: materias, calificación, créditos y estatus | `TranscriptDto` |
| `AcademicProgressView` | Créditos aprobados, porcentaje de avance y promedio por alumno | `AcademicProgressDto` |

### 4.3 Las tres representaciones del mismo dato

El dato existe tres veces, con un propósito distinto por capa:

| Representación | Dónde vive | Propósito | Ejemplo de `Email` |
|---|---|---|---|
| **Tabla** | MySQL `Users` | Persistencia, integridad, índices | `Email VARCHAR(150) NOT NULL` + `UqUsersEmail` |
| **Entity** | `USystem.Domain/Entities/User.cs` | Modelo del negocio mapeado por EF Core | `public string Email { get; set; }` |
| **DTO** | `USystem.Application/Dtos/UserDto.cs` | Contrato de la API (no expone `PasswordHash`) | `public string Email { get; set; }` |
| **Model TS** | `desktop/src/modules/Users/model/User.ts` | Contrato que consume la interfaz | `Email: string;` |

La entidad nunca sale de la API: el servicio la proyecta a DTO. Así `PasswordHash`, `FailedAttempts` y
`LockedUntil` no viajan al cliente.

**Ejemplo completo — `User`**

```csharp
// Entity: espejo de la tabla Users (incluye columnas de auditoría heredadas)
public class User : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string PaternalLastName { get; set; } = string.Empty;
    public string? MaternalLastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
}
```

```csharp
// DTO de salida: lo que el cliente puede ver
public class UserDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;   // nombre ya armado para la UI
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new(); // solo en detalle, login y perfil
}
```

```ts
// Model TS: mismo contrato, en PascalCase
export interface User {
  Id: number;
  FullName: string;
  Email: string;
  Username: string;
  IsActive: boolean;
}
```

### 4.4 Contrato de transporte

Toda respuesta de la API usa la misma envoltura (`USystem.Application/Common/ApiResponse.cs`):

```json
{
  "Success": true,
  "Message": "Usuario creado correctamente",
  "Data": { "Id": 42, "FullName": "Ana Pérez López", "Email": "ana@usystem.edu", "IsActive": true },
  "Errors": [],
  "TraceId": "0HN7..."
}
```

Los listados transportan `PagedResult<T>`: `Items`, `TotalCount`, `Page`, `PageSize`. Las propiedades
JSON van en PascalCase (`PropertyNamingPolicy = null`), igual que en C# y TypeScript.

### 4.5 Reglas de modelado

1. **Una tabla = una entidad = un `DbSet<>`** en `USystemDbContext`; el nombre de la tabla es el plural
   de la entidad (`Students`, `ClassGroups`).
2. **`Groups` es palabra reservada en MySQL 8**: la tabla se llama `ClassGroups`.
3. **Llaves:** `INT` en catálogos, `BIGINT` en alumnos y transacciones; FK `<Entidad>Id`.
4. **Estados y tipos** con `VARCHAR` y valores en inglés PascalCase (`Pending`, `InProgress`,
   `Closed`); en C# viven en `Constants/`.
5. **Borrado lógico** con `DeletedAt` y filtro global en EF Core; los pagos solo se cancelan
   (`Payments.Status = Cancelled`).
6. **Integridad:** las relaciones usan `ON DELETE RESTRICT ON UPDATE CASCADE`; el historial no se pierde.
7. **Tablas de unión explícitas** (`RolePermissions`, `UserRoles`, `StudyPlanSubjects`,
   `TeacherSubjects`, `EnrollmentClassGroups`) en lugar de muchos-a-muchos ocultos.
8. **Lo sensible no se expone:** contraseñas como hash, tokens en tablas aparte con expiración.

---

## 5. Referencias entre capas

### 5.1 Matriz de referencias de proyecto

| Proyecto ↓ referencia a → | Domain | Application | Infrastructure | Api | Tests |
|---|:---:|:---:|:---:|:---:|:---:|
| `USystem.Domain` | — | ✗ | ✗ | ✗ | ✗ |
| `USystem.Application` | ✔ | — | ✗ | ✗ | ✗ |
| `USystem.Infrastructure` | ✔ | ✔ | — | ✗ | ✗ |
| `USystem.Api` | ✗ (transitiva) | ✔ | ✔ | — | ✗ |
| `USystem.Tests` | ✔ | ✔ | ✔ | ✔ | — |
| `desktop/` (npm) | ✗ | ✗ | ✗ | ✗ (solo HTTP) | ✗ |

Declaración real en los `.csproj`:

```xml
<!-- USystem.Application.csproj -->
<ProjectReference Include="..\USystem.Domain\USystem.Domain.csproj" />

<!-- USystem.Infrastructure.csproj -->
<ProjectReference Include="..\USystem.Application\USystem.Application.csproj" />
<ProjectReference Include="..\USystem.Domain\USystem.Domain.csproj" />

<!-- USystem.Api.csproj -->
<ProjectReference Include="..\USystem.Application\USystem.Application.csproj" />
<ProjectReference Include="..\USystem.Infrastructure\USystem.Infrastructure.csproj" />
```

### 5.2 Regla de dependencias

```
        ┌───────────────────────┐
        │      USystem.Api      │  Controllers · Middleware · DI
        └───────┬───────────────┘
                │ usa interfaces de servicio
        ┌───────▼───────────────┐
        │  USystem.Application  │  Services · DTOs · Validators
        └───────┬───────────────┘
                │ usa interfaces de repositorio
        ┌───────▼───────────────┐
        │    USystem.Domain     │  Entities · Interfaces · Exceptions
        └───────▲───────────────┘
                │ implementa esas interfaces
        ┌───────┴───────────────┐
        │ USystem.Infrastructure│  Repositories · EF Core · JWT · S3
        └───────────────────────┘
```

- Las **interfaces** se declaran en las capas internas (`Domain` para repositorios, `Application` para
  servicios) y las **implementaciones** viven en las externas (`Infrastructure`, `Application`).
- `USystem.Api` referencia `Infrastructure` **solo** para componer el contenedor de dependencias en
  `Program.cs`; ningún controller conoce una clase de infraestructura.

### 5.3 Composición e inyección de dependencias

`Program.cs` encadena el registro de cada capa; después cada módulo registra lo suyo:

```csharp
var Builder = WebApplication.CreateBuilder(Args);

Builder.Services.AddDbContext<USystemDbContext>(Options =>
    Options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)));

Builder.Services.AddInfrastructure();     // repositorios, hash, JWT, almacenamiento
Builder.Services.AddApplication();        // servicios, validadores, mappers
Builder.Services.AddSecurityModule();     // autenticación y autorización
Builder.Services.AddUsersModule();        // IUserService → UserService, IUserRepository → UserRepository
Builder.Services.AddRolesModule();
// … un método por módulo

var App = Builder.Build();
App.UseMiddleware<ExceptionHandlingMiddleware>();
App.UseAuthentication();
App.UseAuthorization();
App.MapControllers();
App.Run();
```

Cada módulo expone su extensión (`UsersModuleExtensions.AddUsersModule()`) y así el módulo se puede
registrar, probar o retirar sin tocar los demás.

### 5.4 Referencias prohibidas

| Desde | No puede referenciar | Motivo |
|---|---|---|
| `Domain` | EF Core, ASP.NET, BCrypt, AWS, FluentValidation | El dominio debe sobrevivir a cualquier tecnología |
| `Application` | `Infrastructure`, `Api`, EF Core | Las reglas se prueban sin base de datos |
| `Infrastructure` | `Api` | La infraestructura no conoce HTTP |
| `Controllers` | Clases de `Infrastructure` o `DbContext` | La API solo habla con interfaces de servicio |
| `desktop/` | C#, MySQL, `DbContext` | Solo consume HTTP; la base de datos no se expone |
| `View` (`.tsx`) | Servicios HTTP directos | Todo pasa por el ViewModel |

### 5.5 Referencias internas de la capa de presentación

```
modules/Users/view/UserListPage.tsx
        │ importa
        ▼
modules/Users/viewmodel/useUserListViewModel.ts
        │ importa
        ▼
modules/Users/model/UsersService.ts ──► shared/http/HttpClient.ts ──► API
        │ importa tipos de
        ▼
modules/Users/model/User.ts

shared/  ◄── usado por todos los módulos (components, crud, hooks, http, i18n, auth, utils)
app/     ◄── compone: Router, Providers, Theme, MainLayout
electron/◄── no se importa desde React: se comunica por preload + IPC
```

- Un módulo **nunca** importa de otro módulo: lo compartido se promueve a `shared/`.
- `view → viewmodel → model → shared/http` es la única dirección permitida.

### 5.6 Comunicación desktop ↔ API

| Aspecto | Definición |
|---|---|
| Protocolo | HTTP + JSON sobre `/api/v1` |
| URL base | `VITE_API_BASE_URL` (por defecto `http://localhost:5000`) |
| Autenticación | `Authorization: Bearer <access token>`; refresh automático en 401 contra `/api/v1/auth/refresh` |
| Idioma | `Accept-Language: es|en` para que la API devuelva mensajes traducidos |
| Envoltura | `ApiResponse<T>` → el `HttpClient` desenvuelve `Data` y lanza `ApiErrorException` si falla |
| Tiempo de espera | 20 s (`DefaultTimeoutMs`) |
| Seguridad Electron | `contextIsolation: true`, `nodeIntegration: false`, `sandbox: true`, CSP en producción |

---

## 6. Esquema de la interfaz gráfica (capa de presentación)

### 6.1 Modelo de ventana

| Elemento | Definición |
|---|---|
| Tipo | Aplicación de escritorio de **una sola ventana** (`BrowserWindow`) |
| Tamaño | 1440 × 900, mínimo 1180 × 720; una instancia (`requestSingleInstanceLock`) |
| Título / icono | `U SYSTEM` |
| Navegación | `HashRouter` (compatible con `file://` del build empaquetado) |
| Menú | Menú mínimo nativo (`fileMenu`, `editMenu`, `viewMenu`, `windowMenu`) |
| Seguridad | Enlaces externos al navegador; `will-navigate` bloqueado; CSP en producción |
| Chasis | `MainLayout.tsx` → `ITLayout` (barra superior + menú lateral) + `<Outlet />` |
| Contenido | Cada ruta autenticada es un `ITPage` (título, descripción, acciones, `loading`, `error`, `empty`) |
| Excepción | `LoginPage` vive fuera del chasis por ser pública |

### 6.2 Rutas

| Tipo | Rutas | Cantidad |
|---|---|---|
| Públicas | `/login`, `/forgot-password`, `/reset-password` | 3 |
| Privadas | `/`, `/profile`, `/change-password` y un par listado/formulario por módulo | 54 |
| Comodín | `*` → redirige a `/` | 1 |

### 6.3 Mapa de pantallas por módulo

| # | Módulo | Pantalla(s) | Ruta(s) | ViewModel | Permiso |
|---|---|---|---|---|---|
| 1 | Auth | Iniciar sesión | `/login` | `useLoginViewModel` | público |
| 2 | Auth | Recuperar contraseña | `/forgot-password` | `useForgotPasswordViewModel` | público |
| 3 | Auth | Restablecer contraseña | `/reset-password` | `useResetPasswordViewModel` | público (token) |
| 4 | Home | Panel de inicio | `/` | `useDashboardViewModel` | sesión |
| 5 | Auth | Perfil | `/profile` | `useProfileViewModel` | sesión |
| 6 | Auth | Cambiar contraseña | `/change-password` | `useChangePasswordViewModel` | sesión |
| 7 | Reports | Dashboard de indicadores | `/dashboard` | `useDashboardViewModel` | `Reports.View` |
| 8 | Users | Listado | `/users` | `useUserListViewModel` | `Users.View` |
| 9 | Users | Alta / edición | `/users/new`, `/users/:Id` | `useUserFormViewModel` | `Users.Create` / `Users.Edit` |
| 10 | Roles | Listado de roles | `/roles` | `CrudListPage` + `RoleConfig` | `Roles.View` |
| 11 | Roles | Alta / edición con matriz de permisos | `/roles/new`, `/roles/:Id` | `useRoleFormViewModel` | `Roles.Create` / `Roles.Edit` |
| 12 | AuditLogs | Bitácora | `/audit-logs` | `useAuditLogListViewModel` | `AuditLogs.View` |
| 13 | Careers | Listado / alta / edición | `/careers`, `/careers/new`, `/careers/:Id` | `CrudListPage` / `CrudFormPage` + `CareerConfig` | `Careers.View`, `.Create`, `.Edit` |
| 14 | Subjects | Listado / alta / edición | `/subjects`, `/subjects/new`, `/subjects/:Id` | `CrudListPage` / `CrudFormPage` + `SubjectConfig` | `Subjects.*` |
| 15 | AcademicTerms | Listado / alta / edición | `/academic-terms`, `/new`, `/:Id` | `useAcademicTermListViewModel`, `useAcademicTermFormViewModel` | `AcademicTerms.*` |
| 16 | StudyPlans | Listado / alta / edición con malla | `/study-plans`, `/new`, `/:Id` | `useStudyPlanListViewModel`, `useStudyPlanFormViewModel` | `StudyPlans.*` |
| 17 | CourseMaterials | Listado / alta / edición | `/course-materials`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `CourseMaterialConfig` | `CourseMaterials.*` |
| 18 | Teachers | Listado / alta / edición | `/teachers`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `TeacherConfig` | `Teachers.*` |
| 19 | Students | Listado / alta / edición | `/students`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `StudentConfig` | `Students.*` |
| 20 | ClassGroups | Listado / alta / edición y horarios | `/class-groups`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `ClassGroupConfig` | `ClassGroups.*` |
| 21 | Enrollments | Listado / alta / cancelación | `/enrollments`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `EnrollmentConfig` | `Enrollments.View`, `.Create`, `.Cancel` |
| 22 | Enrollments | Asistente de inscripción (requisitos → grupos → confirmación) | `/enrollment-wizard` | `useEnrollmentWizardViewModel` | `Enrollments.Create` |
| 23 | Grades | Captura de calificaciones por grupo | `/grades` | `useGradesViewModel` | `Grades.View`, `.Capture` |
| 24 | Tracking | Kardex y avance académico | `/tracking` | `useTrackingViewModel` | `Grades.View` |
| 25 | ReEnrollments | Reinscripción con propuesta de materias | `/re-enrollments` | `useReEnrollmentViewModel` | `Enrollments.Create` |
| 26 | Payments | Cargos y pagos | `/charges` | `useChargeListViewModel` | `Payments.View`, `.Register`, `.Cancel` |
| 27 | Payments | Conceptos de pago | `/payment-concepts`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `PaymentConceptConfig` | `Payments.View`, `.Register` |
| 28 | Procedures | Trámites | `/procedures` | `useProcedureListViewModel` | `Procedures.View`, `.Request`, `.Issue` |
| 29 | Procedures | Tipos de trámite | `/procedure-types`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `ProcedureTypeConfig` | `Procedures.*` |
| 30 | Notifications | Plantillas y envíos | `/notifications`, `/new`, `/:Id` | `CrudListPage` / `CrudFormPage` + `NotificationTemplateConfig` | `Notifications.View`, `.Send` |
| 31 | Settings | Configuración del sistema | `/settings` | `useSettingsViewModel` | `Settings.View`, `.Edit` |

### 6.4 Navegación y permisos

- El menú lateral se genera desde `app/layout/NavigationModules.ts`: **22 entradas** con `Path`,
  `LabelKey`, `Icon` y `Permission`. `useNavigationItems()` filtra por los permisos efectivos del JWT.
- `Inicio` (`/`) siempre es visible; el resto exige el permiso `.View` de su módulo.
- `RequireAuth` protege todas las rutas privadas: sin sesión redirige a `/login`.
- **La UI oculta, la API decide:** cada endpoint vuelve a validar con `[RequirePermission]`.

| Rol | Módulos visibles en el menú |
|---|---|
| `Administrator` | Todos |
| `Registrar` | Inicio, Dashboard, Carreras, Materias, Periodos, Planes, Docentes, Alumnos, Materiales, Inscripciones, Grupos, Calificaciones, Seguimiento, Reinscripción, Trámites, Reportes |
| `Finance` | Inicio, Dashboard, Carreras, Periodos, Alumnos, Pagos, Trámites, Reportes |
| `ProgramCoordinator` | Inicio, Dashboard, Carreras, Materias, Periodos, Planes, Docentes, Alumnos, Materiales, Grupos, Calificaciones, Seguimiento, Notificaciones, Reportes |
| `Teacher` | Inicio, Carreras, Materias, Periodos, Docentes, Alumnos, Materiales, Grupos, Calificaciones, Seguimiento |
| `ReadOnly` | Todos en modo consulta (sin acciones de alta, edición ni borrado) |

### 6.5 Patrones de pantalla

| Patrón | Estructura | Usado en |
|---|---|---|
| **Listado** | `ITPage` + `SearchBox` + `FilterBar` + `DataTable` paginada + acción "Nuevo" | Catálogos, usuarios, bitácora, trámites |
| **Formulario** | `ITPage` + secciones `ITCard` + `FormField` + barra `Cancelar / Guardar` | Altas y ediciones |
| **Asistente** | Pasos numerados con validación por paso y resumen final | Inscripción, reinscripción |
| **Tablero** | `ITGrid` de indicadores + tablas de resumen | Dashboard y Home |
| **Configuración** | Lista de parámetros por sección con guardado individual | Settings |
| **Consulta** | Igual al listado sin botones de acción | Rol `ReadOnly`, bitácora |

**Estados de pantalla (los aporta `ITPage`)**

| Estado | Cuándo | Representación |
|---|---|---|
| `loading` | Consulta en curso y sin datos | Esqueleto/spinner del componente |
| `error` | Fallo de red o error de la API | Mensaje localizado + botón "Reintentar" (`onRetry`) |
| `empty` | Consulta exitosa sin registros | Ilustración Lottie + texto de `EmptyKey` |
| `forbidden` | Sin permiso | Menú y acción ocultos; la API responde 403 si se fuerza |

### 6.6 Bocetos de pantalla

**Login (pantalla pública, fuera del chasis)**

```
┌────────────────────────────────────────────────────────────────────┐
│                                                                    │
│                   ┌──────────────────────────────┐                 │
│                   │  [logo]  U S Y S T E M       │                 │
│                   │  Sistema administrativo      │                 │
│                   │                              │                 │
│                   │  Usuario                     │                 │
│                   │  [________________________]  │                 │
│                   │  Contraseña                  │                 │
│                   │  [________________________]  │                 │
│                   │  ¿Olvidaste tu contraseña?   │                 │
│                   │  [        Entrar         ]   │                 │
│                   └──────────────────────────────┘                 │
└────────────────────────────────────────────────────────────────────┘
```

**Listado (chasis + `ITPage`)**

```
┌──────────────────────────────────────────────────────────────────────────────┐
│ [logo] U SYSTEM                                    Buscar…     Ana P.  ▾     │ topBar
├────────────────┬─────────────────────────────────────────────────────────────┤
│ Inicio         │ Usuarios                                      [+ Nuevo]      │
│ Dashboard      │ Personal de la escuela con acceso al sistema                │
│ ▸ Usuarios     │ ┌─ [Buscar usuario…]  Filtro: [Activo ▾] ────────────────┐  │
│ Carreras       │ │ Usuario     │ Correo             │ Nombre    │ Activo  │  │
│ Materias       │ │ ana.perez   │ ana@usystem.edu    │ Ana P.    │  Sí     │  │
│ Periodos       │ │ luis.gomez  │ luis@usystem.edu   │ Luis G.   │  Sí     │  │
│ Planes         │ │ rosa.mendez │ rosa@usystem.edu   │ Rosa M.   │  No     │  │
│ Docentes       │ └─────────────────────────────────────────────────────────┘  │
│ Alumnos        │                    ‹ 1  2  3 ›    40 registros              │
│ Inscripciones  │                                                             │
│ Grupos         │                                                             │
│ Calificaciones │                                                             │
│ Pagos          │                                                             │
│ Configuración  │                                                             │
└────────────────┴─────────────────────────────────────────────────────────────┘
```

**Formulario**

```
Usuarios / Nuevo usuario                                        [Cancelar] [Guardar]
─────────────────────────────────────────────────────────────────────────────────────
┌─ Datos generales ────────────────────────┐  ┌─ Acceso ──────────────────────────┐
│ Nombre(s) *            [_______________] │  │ Usuario *   [__________________] │
│ Apellido paterno *     [_______________] │  │ Correo *    [__________________] │
│ Apellido materno       [_______________] │  │ Contraseña temporal * [_______]  │
│ Teléfono               [_______________] │  │ Roles       [Seleccionar ▾]      │
└──────────────────────────────────────────┘  └──────────────────────────────────┘
* Campos obligatorios: el error se muestra debajo del campo
```

**Asistente de inscripción**

```
Inscripción            ●──────────○──────────○
                       1 Requisitos   2 Grupos   3 Confirmación

┌─ Requisitos del alumno ─────────────────────────────────────────┐
│ Documentos obligatorios      ✔ 4 de 4 validados                 │
│ Pago de inscripción          ✔ Registrado el 12/09/2026         │
│ Periodo abierto              ✔ 2026-3 (sep–dic)                 │
└─────────────────────────────────────────────────────────────────┘
                                                    [Atrás] [Continuar]
```

**Tablero**

```
┌─ Alumnos activos ─┐ ┌─ Inscritos al periodo ─┐ ┌─ Por cobrar ─┐ ┌─ Grupos abiertos ─┐
│       1,284       │ │          312           │ │  $1,450,000  │ │         86        │
└───────────────────┘ └────────────────────────┘ └──────────────┘ └───────────────────┘
┌─ Ocupación por carrera ─────────────┐ ┌─ Próximos vencimientos ─────────────────┐
│ Ingeniería en Sistemas  ███████ 78% │ │ 30/11  Colegiatura 3  ·  48 alumnos     │
│ Administración          █████   62% │ │ 15/12  Colegiatura 4  ·  12 alumnos     │
└─────────────────────────────────────┘ └─────────────────────────────────────────┘
```

### 6.7 Librería de componentes y convenciones visuales

| Grupo | Componentes | Nota |
|---|---|---|
| Chasis | `ITLayout`, `ITPage`, `ITStack`, `ITGrid`, `ITCard` | Se usan directo de AXZY UI, sin envoltorio |
| Datos | `DataTable`, `FilterBar`, `SearchBox` | Envoltorios en `shared/components` con props PascalCase |
| Formulario | `FormField`, `AppButton`, `ConfirmDialog` | Validación y confirmación centralizadas |
| Retroalimentación | `LottieAnimation`, `ComingSoonPage` | Estados vacíos y módulos futuros |
| Tema | `ITThemeProvider` + `app/Theme.ts` | Un único provider en `main.tsx`; Tailwind v4 con capas en `Global.css` |

Convenciones: todo texto visible sale de `t('Módulo.Sección.Clave')` (nunca hardcodeado); fechas y
moneda se formatean con `shared/utils/Format.ts`; los errores muestran el `Message` localizado que
devuelve la API.

### 6.8 Lo que la interfaz no incluye

- Portal o app del alumno (proyecto futuro, `Architecture/StudentAppReadiness.md`).
- Versión web o móvil: el cliente es de escritorio y consume la API por HTTP.
- Pasarela de pagos en línea ni facturación electrónica (CFDI).
- Pantallas fuera de los 21 módulos: cualquier módulo nuevo se agrega con `Guides/HowToAddAModule.md`.

---

## 7. Correspondencia con los entregables

| Punto solicitado | Respuesta en este documento | Estado |
|---|---|---|
| Bibliotecas de clases por capa | 5 proyectos .NET 8 (Domain, Application, Infrastructure, Api, Tests) + paquetes npm de la presentación, con sus dependencias y versiones (§2) | ✔ |
| Clases de cada capa según la funcionalidad | Inventario por carpeta con nombres reales: 37 entidades, 24 servicios, 22 repositorios, 30 controllers, 43 pantallas (§3) | ✔ |
| Objetos de datos según la base de datos | 37 tablas + 2 vistas, mapeadas a entidad, DTO y modelo TS; convenciones y contrato JSON (§4) | ✔ |
| Referencias entre capas | Matriz de `ProjectReference`, regla de dependencias, DI y referencias prohibidas (§5) | ✔ |
| Esquema de la interfaz gráfica | Modelo de ventana, 57 rutas, mapa de 31 pantallas, navegación por permisos, patrones y bocetos (§6) | ✔ |

### Observaciones de cumplimiento (mejoras detectadas)

1. **Ubicación de contratos:** 12 de las 24 interfaces de servicio se declaran dentro del archivo de su
   implementación; moverlas a `Services/I<Módulo>Service.cs` facilita ver el contrato de la capa.
2. **`IRepository<T>` genérico:** existe en `Domain`, pero los 22 repositorios implementan su propia
   interfaz; homogeneizar el contrato reduciría duplicación de firmas CRUD.
3. **Cobertura de integración:** hay 3 pruebas de integración (salud, sesión y conectividad); conviene
   ampliarlas a los endpoints de la fase 1.

---

## 8. Documentos relacionados

| Documento | Contenido |
|---|---|
| [SolutionStructure.md](SolutionStructure.md) | Árbol de carpetas de la solución |
| [ApiArchitecture.md](ApiArchitecture.md) | Flujo de petición, responsabilidades y nomenclatura de la API |
| [DesktopMvvm.md](DesktopMvvm.md) | Patrón MVVM en React, chasis y reglas de la UI |
| [TechStack.md](TechStack.md) | Stack tecnológico completo |
| [Phases.md](Phases.md) | Fases de construcción |
| [Api/LayerExample.md](../Api/LayerExample.md) | Ejemplo de código por las cuatro capas (módulo Users) |
| [Api/Conventions.md](../Api/Conventions.md) | Contrato de endpoints, paginación y errores |
| [DataModel/README.md](../DataModel/README.md) | Tablas, scripts y diagrama ER |
| [DataModel/Conventions.md](../DataModel/Conventions.md) | Convenciones de base de datos |
| [Modules/README.md](../Modules/README.md) | Los 21 módulos funcionales |
| [Security/RbacMatrix.md](../Security/RbacMatrix.md) | Roles y 78 permisos |
| [Guides/CodingConventions.md](../Guides/CodingConventions.md) | Convenciones de código |
