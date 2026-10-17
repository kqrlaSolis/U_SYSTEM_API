# Convenciones de código

## Regla principal

> **Todo el código va en inglés y PascalCase.** Variables, parámetros, clases, interfaces, métodos,
> propiedades, constantes, enums, tablas, columnas, permisos y nombres de archivo.
> **Solo los comentarios van en español.**

Esta regla aplica a C#, TypeScript, SQL, JSON y scripts de apoyo. No se usa snake_case ni camelCase.

## Tabla de convenciones

| Elemento | Convención | Ejemplo correcto | Ejemplo incorrecto |
|---|---|---|---|
| Clase / entidad | PascalCase | `User`, `ClassGroup` | `user`, `class_group` |
| Interfaz | `I` + PascalCase | `IUserRepository` | `UserRepositoryInterface` |
| Método | PascalCase; `Async` al final si es asíncrono | `GetByIdAsync` | `getById` |
| Propiedad | PascalCase | `FirstName` | `first_name` |
| Variable local | **PascalCase** | `PageSize`, `CurrentUser` | `pageSize`, `current_user` |
| Parámetro | **PascalCase** | `UserId`, `Cancellation` | `userId`, `ct` |
| Campo privado | `_` + PascalCase | `_UserRepository` | `_userRepository` |
| Constante | PascalCase | `DefaultPageSize` | `DEFAULT_PAGE_SIZE` |
| Enum y sus valores | PascalCase | `StudentStatus.Active` | `student_status.active` |
| Archivo C# | PascalCase igual al tipo | `UserService.cs` | `user-service.cs` |
| Archivo TS/TSX | PascalCase igual al export | `UserListPage.tsx` | `user-list-page.tsx` |
| Script SQL | Prefijo numérico + PascalCase | `01_Security.sql` | `01_security.sql` |
| Tabla | PascalCase, plural | `ClassGroups` | `class_groups` |
| Columna | PascalCase | `PaternalLastName` | `paternal_last_name` |
| Llave / restricción | Prefijo + tabla + columna | `UqUsersEmail`, `FkGradesEnrollmentId` | `users_email_key` |
| Permiso | `Module.Action` en PascalCase | `Users.Create` | `users.create` |
| Propiedad JSON | PascalCase | `"FirstName"` | `"firstName"` |
| Parámetro de consulta | PascalCase | `?Page=1&PageSize=20` | `?page=1&page_size=20` |
| Ruta HTTP | minúsculas con guiones (no es un identificador) | `/api/v1/class-groups` | `/api/v1/ClassGroups` |
| Hook / ViewModel | `use` + PascalCase | `useUserListViewModel` | `UserListViewModel` |

## Variables y parámetros

```csharp
// Correcto: identificadores en inglés y PascalCase; comentario en español
public async Task<UserDto> CreateAsync(CreateUserRequest Request, CancellationToken Cancellation)
{
    var NewUser = new User { FirstName = Request.FirstName };

    // El correo debe ser único antes de guardar
    if (await _UserRepository.ExistsByEmailAsync(Request.Email, Cancellation))
    {
        throw new BusinessRuleException("Users.DuplicateEmail", "The email is already registered.");
    }

    await _UserRepository.AddAsync(NewUser, Cancellation);
    return UserMapper.ToDto(NewUser);
}
```

```csharp
// Incorrecto: camelCase en variables y parámetros
public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
{
    var newUser = new User { FirstName = request.FirstName };
}
```

Recomendaciones:

- No nombres una variable igual que un tipo (`User`, `Grade`). Usa un sufijo: `NewUser`, `CurrentGrade`.
- No nombres un parámetro igual que su tipo. En lugar de `CancellationToken CancellationToken`, usa
  `CancellationToken Cancellation`.
- Los nombres son en inglés, completos y sin abreviaturas innecesarias (`StudentNumber`, no `StNum`).

## TypeScript y React

```ts
// Correcto: variables, funciones y constantes en PascalCase
const DefaultPageSize = 20;

export function useUserListViewModel() {
  const [Items, SetItems] = useState<User[]>([]);
  const [IsLoading, SetIsLoading] = useState(false);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    const Result = await UsersService.GetPaged(1, DefaultPageSize, '');
    SetItems(Result.Items);
    SetIsLoading(false);
  }, []);

  return { Items, IsLoading, Reload: Load };
}
```

```tsx
// Incorrecto: camelCase en variables y funciones
const defaultPageSize = 20;
const [items, setItems] = useState([]);
const loadUsers = async () => { /* ... */ };
```

## Excepciones que imponen los frameworks

Estas excepciones están permitidas y documentadas; no rompen la regla.

| Caso | Por qué | Ejemplo |
|---|---|---|
| Hooks de React | React exige que el nombre empiece con `use` en minúscula | `useUserListViewModel`, `usePermissions` |
| Props de React y AXZY UI | Son la API de una librería externa; no se renombran | `onClick`, `value`, `key`, `className` |
| Funciones de librería | Son la API del framework o de AXZY UI | `useState`, `useTranslation`, `t`, `formatCurrency` |
| Sufijo `Async` | Convención del estándar de C# | `SaveChangesAsync` |
| Interfaces con `I` | Convención del estándar de C# | `IRoleService` |
| Variables de entorno | Convención del sistema operativo y de .NET | `VITE_API_BASE_URL`, `ConnectionStrings__DefaultConnection` |
| Rutas HTTP | Estándar REST: minúsculas con guiones | `/api/v1/class-groups` |
| Miembros de librerías | No se pueden renombrar (`DateTime.Now`, `console.log`) | — |

Todo lo demás —incluidas variables locales, parámetros, constantes y nombres de archivo— es PascalCase.

## Comentarios en español

```csharp
// Regla: no se puede cerrar el acta dos veces
public async Task CloseReportAsync(int ClassGroupId, CancellationToken Cancellation)
{
    // ...
}
```

```sql
-- Los pagos no se borran: se cancelan con motivo
SELECT ...
```

```ts
// El permiso se evalúa en la UI, pero la API siempre es la autoridad
const CanCreate = Can('Users.Create');
```

Los comentarios explican el **por qué**, no repiten el código.

## Aplicación de la regla

- **C#:** `.editorconfig` en la raíz de la solución declara `pascal_case` para variables locales,
  parámetros y campos privados (`_` + PascalCase). El compilador no lo exige, pero el analizador marca
  la desviación como advertencia en el IDE y en CI.
- **TypeScript:** regla `@typescript-eslint/naming-convention` con `PascalCase` para variables,
  funciones y constantes, salvo los hooks (`use` + PascalCase).
- **SQL:** revisión en el script y en las migraciones; los nombres se generan desde el modelo de datos.
- **Revisión de código:** el checklist del Pull Request incluye verificar que no haya camelCase ni
  snake_case en identificadores nuevos.

## Checklist del Pull Request

- [ ] Identificadores nuevos en inglés.
- [ ] Sin camelCase ni snake_case (salvo las excepciones documentadas).
- [ ] Archivos nuevos con nombre PascalCase.
- [ ] Comentarios en español y solo donde aportan contexto.
- [ ] Permisos nuevos con formato `Module.Action` y agregados a `99_SeedData.sql`.
- [ ] Tablas o columnas nuevas en PascalCase y documentadas en `DataModel/Entities/`.
