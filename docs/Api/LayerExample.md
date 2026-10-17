# Ejemplo por capas — módulo Users

Todo el código en inglés y PascalCase; los comentarios en español.

## Domain — `USystem.Domain/Interfaces/IUserRepository.cs`

```csharp
using USystem.Domain.Common;
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long Id, CancellationToken Cancellation = default);
    Task<User?> GetByLoginAsync(string UsernameOrEmail, CancellationToken Cancellation = default);
    Task<bool> ExistsByEmailAsync(string Email, CancellationToken Cancellation = default);
    Task<PagedResult<User>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);
    Task AddAsync(User User, CancellationToken Cancellation = default);
    void Update(User User);
}
```

## Infrastructure — `USystem.Infrastructure/Repositories/UserRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly USystemDbContext _Context;

    public UserRepository(USystemDbContext Context) => _Context = Context;

    public Task<User?> GetByIdAsync(long Id, CancellationToken Cancellation = default)
        => _Context.Users.FirstOrDefaultAsync(User => User.Id == Id, Cancellation);

    public Task<User?> GetByLoginAsync(string UsernameOrEmail, CancellationToken Cancellation = default)
        => _Context.Users.FirstOrDefaultAsync(
            User => User.Username == UsernameOrEmail || User.Email == UsernameOrEmail, Cancellation);

    public Task<bool> ExistsByEmailAsync(string Email, CancellationToken Cancellation = default)
        => _Context.Users.AnyAsync(User => User.Email == Email, Cancellation);

    public async Task<PagedResult<User>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default)
    {
        var Source = _Context.Users.AsNoTracking();

        // Búsqueda por nombre, usuario o correo
        if (!string.IsNullOrWhiteSpace(Query.Search))
        {
            Source = Source.Where(User => User.Username.Contains(Query.Search)
                                       || User.Email.Contains(Query.Search)
                                       || User.FirstName.Contains(Query.Search));
        }

        var TotalCount = await Source.CountAsync(Cancellation);
        var Items = await Source
            .OrderBy(User => User.Username)
            .Skip((Query.Page - 1) * Query.PageSize)
            .Take(Query.PageSize)
            .ToListAsync(Cancellation);

        return new PagedResult<User>
        {
            Items = Items,
            TotalCount = TotalCount,
            Page = Query.Page,
            PageSize = Query.PageSize,
        };
    }

    public async Task AddAsync(User User, CancellationToken Cancellation = default)
        => await _Context.Users.AddAsync(User, Cancellation);

    public void Update(User User) => _Context.Users.Update(User);
}
```

## Application — `USystem.Application/Services/IUserService.cs`

```csharp
using USystem.Application.Dtos;
using USystem.Domain.Common;

namespace USystem.Application.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetPagedAsync(PagedQuery Query, CancellationToken Cancellation = default);
    Task<UserDto> GetByIdAsync(long Id, CancellationToken Cancellation = default);
    Task<UserDto> CreateAsync(CreateUserRequest Request, CancellationToken Cancellation = default);
}
```

## Application — `USystem.Application/Services/UserService.cs`

```csharp
using USystem.Application.Dtos;
using USystem.Application.Mappings;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _UserRepository;
    private readonly IPasswordHasher _PasswordHasher;
    private readonly IAuditService _AuditService;
    private readonly IUnitOfWork _UnitOfWork;

    // Todas las dependencias son interfaces (inversión de dependencias)
    public UserService(
        IUserRepository UserRepository,
        IPasswordHasher PasswordHasher,
        IAuditService AuditService,
        IUnitOfWork UnitOfWork)
    {
        _UserRepository = UserRepository;
        _PasswordHasher = PasswordHasher;
        _AuditService = AuditService;
        _UnitOfWork = UnitOfWork;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest Request, CancellationToken Cancellation = default)
    {
        // Regla de negocio: el correo debe ser único.
        // La excepción lleva el código y los valores; el middleware arma el mensaje traducido.
        if (await _UserRepository.ExistsByEmailAsync(Request.Email, Cancellation))
        {
            throw new BusinessRuleException(
                "Users.DuplicateEmail",
                new Dictionary<string, string> { ["Email"] = Request.Email });
        }

        var NewUser = new User
        {
            FirstName = Request.FirstName,
            PaternalLastName = Request.PaternalLastName,
            MaternalLastName = Request.MaternalLastName,
            Email = Request.Email,
            Username = Request.Username,
            // La contraseña temporal se guarda siempre con hash
            PasswordHash = _PasswordHasher.Hash(Request.TemporaryPassword),
            MustChangePassword = true,
            IsActive = true,
        };

        await _UserRepository.AddAsync(NewUser, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);
        await _AuditService.LogAsync(
            "Create", "Users", nameof(User), NewUser.Id.ToString(), null, NewUser, Cancellation);

        return UserMapper.ToDto(NewUser);
    }

    // GetPagedAsync y GetByIdAsync siguen el mismo patrón
}
```

## Application — `USystem.Application/Mappings/UserMapper.cs`

```csharp
using USystem.Application.Dtos;
using USystem.Domain.Entities;

namespace USystem.Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(User User) => new()
    {
        Id = User.Id,
        FirstName = User.FirstName,
        PaternalLastName = User.PaternalLastName,
        Email = User.Email,
        Username = User.Username,
        IsActive = User.IsActive,
    };
}
```

## Api — `USystem.Api/Attributes/RequirePermissionAttribute.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace USystem.Api.Attributes;

// Valida que el JWT incluya el permiso requerido, por ejemplo "Users.Create"
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _Permission;

    public RequirePermissionAttribute(string Permission) => _Permission = Permission;

    public void OnAuthorization(AuthorizationFilterContext Context)
    {
        var User = Context.HttpContext.User;
        if (User.Identity?.IsAuthenticated != true)
        {
            Context.Result = new UnauthorizedResult();
            return;
        }

        if (!User.HasClaim("Permission", _Permission))
        {
            Context.Result = new ForbidResult();
        }
    }
}
```

## Api — `USystem.Api/Controllers/UsersController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using USystem.Api.Attributes;
using USystem.Application.Dtos;
using USystem.Application.Services;
using USystem.Domain.Common;

namespace USystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _UserService;

    // El controlador solo conoce la interfaz del servicio
    public UsersController(IUserService UserService) => _UserService = UserService;

    [HttpGet]
    [RequirePermission("Users.View")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetPaged(
        [FromQuery] PagedQuery Query, CancellationToken Cancellation)
        => Ok(ApiResponse<PagedResult<UserDto>>.Ok(await _UserService.GetPagedAsync(Query, Cancellation)));

    [HttpGet("{Id:long}")]
    [RequirePermission("Users.View")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(long Id, CancellationToken Cancellation)
        => Ok(ApiResponse<UserDto>.Ok(await _UserService.GetByIdAsync(Id, Cancellation)));

    [HttpPost]
    [RequirePermission("Users.Create")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create(
        [FromBody] CreateUserRequest Request, CancellationToken Cancellation)
    {
        var CreatedUser = await _UserService.CreateAsync(Request, Cancellation);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = CreatedUser.Id },
            ApiResponse<UserDto>.Ok(CreatedUser));
    }
}
```

## Api — registro de DI por módulo

```csharp
using USystem.Application.Services;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Repositories;
using USystem.Infrastructure.Security;

namespace USystem.Api.Extensions;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection Services)
    {
        Services.AddScoped<IUserRepository, UserRepository>();
        Services.AddScoped<IUserService, UserService>();
        Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        return Services;
    }
}
```

## Api — `Program.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using USystem.Api.Extensions;
using USystem.Api.Middlewares;
using USystem.Infrastructure.Persistence;

namespace USystem.Api;

public static class Program
{
    public static void Main(string[] Args)
    {
        var Builder = WebApplication.CreateBuilder(Args);

        // Cadena de conexión desde appsettings o variables de entorno
        var ConnectionString = Builder.Configuration.GetConnectionString("DefaultConnection")!;
        Builder.Services.AddDbContext<USystemDbContext>(Options =>
            Options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)));

        // PascalCase en el JSON de la API
        Builder.Services.AddControllers()
            .AddJsonOptions(Options => Options.JsonSerializerOptions.PropertyNamingPolicy = null);

        Builder.Services.AddUsersModule();
        // Builder.Services.AddRolesModule(); ... un método por módulo

        var App = Builder.Build();

        App.UseMiddleware<ExceptionHandlingMiddleware>();
        App.UseAuthentication();
        App.UseAuthorization();
        App.MapControllers();
        App.Run();
    }
}
```

## `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=USystem;User=USystemApp;Password=CHANGE_ME;"
  },
  "Jwt": {
    "Issuer": "USystem",
    "Audience": "USystem.Desktop",
    "SecretKey": "SET_FROM_ENVIRONMENT"
  }
}
```
