# Convenciones de la API

## Versionado y rutas

- Prefijo `/api/v1`.
- Recursos en plural, minúsculas y guiones: `/api/v1/class-groups`.
- Acciones que no son CRUD van como subrecurso con verbo: `POST /api/v1/academic-terms/{Id}/close`.
- Los parámetros de ruta y de consulta se escriben en PascalCase: `{Id}`, `?Page=1&PageSize=20`.

## Respuesta estándar

```csharp
// Envoltura común de todas las respuestas
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ApiError> Errors { get; set; } = new();
    public string? TraceId { get; set; }

    // Respuesta exitosa con datos
    public static ApiResponse<T> Ok(T Data, string Message = "OK")
        => new() { Success = true, Message = Message, Data = Data };

    // Respuesta de error con detalle
    public static ApiResponse<T> Fail(string Message, params ApiError[] Errors)
        => new() { Success = false, Message = Message, Errors = Errors.ToList() };
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;   // Ej. "Users.DuplicateEmail"
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
```

JSON en **PascalCase**:

```json
{
  "Success": true,
  "Message": "OK",
  "Data": { "Id": 1, "FirstName": "Ana" },
  "Errors": [],
  "TraceId": "00-8f6c1e0f4f0d9a3b-1-00"
}
```

Configuración:

```csharp
// Evita que ASP.NET Core convierta las propiedades a camelCase
Builder.Services.AddControllers()
    .AddJsonOptions(Options => Options.JsonSerializerOptions.PropertyNamingPolicy = null);
```

## Paginación

```csharp
public class PagedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;      // máximo 100
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

Ejemplo: `GET /api/v1/students?Page=2&PageSize=20&Search=ana&SortBy=PaternalLastName&Descending=false`.

## Códigos HTTP

| Código | Uso |
|--------|-----|
| 200 / 201 / 204 | Éxito / creado / sin contenido |
| 400 | Validación o regla de negocio (`Errors` con el detalle) |
| 401 | No autenticado o token vencido |
| 403 | Autenticado sin permiso |
| 404 | Recurso inexistente |
| 409 | Conflicto (duplicados, estado inválido) |
| 500 | Error inesperado (se registra con `TraceId`) |

## Seguridad

- JWT en `Authorization: Bearer`.
- Claims: `sub` (UserId), `username`, `role` y un claim `Permission` por cada permiso.
- HTTPS obligatorio fuera de desarrollo; CORS restringido al origen del desktop.
- Contraseñas con BCrypt o Argon2; los tokens de recuperación y los refresh tokens se guardan como hash.

## Errores de dominio

| Excepción | Código HTTP | Datos |
|---|---|---|
| `ValidationException` | 400 | `Code`, `Field`, valores |
| `BusinessRuleException` | 400 o 409 | `Code`, valores |
| `NotFoundException` | 404 | `Code`, valores |
| `ForbiddenException` | 403 | `Code` |

Las excepciones llevan un **código estable** (`Users.DuplicateEmail`) y los valores para los
placeholders; el middleware los traduce con `IErrorMessageProvider` según `Accept-Language`.
Ver `Architecture/Internationalization.md`.

```csharp
// El código es estable; los valores llenan los placeholders del catálogo
throw new BusinessRuleException(
    "Users.DuplicateEmail",
    new Dictionary<string, string> { ["Email"] = Request.Email });
```

## Nomenclatura

Todas las variables, parámetros, clases y constantes en PascalCase; solo los comentarios en español.
El detalle y las excepciones de frameworks están en `Guides/CodingConventions.md`.
