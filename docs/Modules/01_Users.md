# Módulo 01 — Usuarios y autenticación

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Base |
| **Prefijo de permisos** | `Users.*` |
| **Depende de** | — |
| **Tablas** | `Users`, `RefreshTokens`, `PasswordResetTokens` |
| **Controller** | `AuthController`, `UsersController` |

## Estado (Fase 1 en curso)

- [x] Scripts `01_Security` y permisos `Users.*` verificados contra MySQL.
- [x] Login con usuario o correo, refresh con rotación, logout y `/auth/me`.
- [x] Bloqueo temporal por intentos fallidos con `Login.MaxFailedAttempts` y `Login.LockoutMinutes`.
- [x] CRUD de usuarios con roles, activar/desactivar y contraseña temporal forzada.
- [x] Cambio de contraseña, recuperación con token de un solo uso y `MustChangePassword`.
- [x] Auditoría de login correcto y fallido en `AuditLogs`.
- [x] `AddUsersModule()` y 27 pruebas nuevas (75 en total en la solución).
- [x] Desktop: login real contra `/api/v1/auth/login` (`UsernameOrEmail`) y contrato con `Roles`.
- [x] Desktop: perfil (`/profile`), cambio de contraseña (`/change-password`) y formulario de usuarios con roles, estado y contraseña temporal.
- [x] Pantallas de recuperación de contraseña (/forgot-password y /reset-password) en el desktop.
- [ ] Envío real del correo de recuperación (proveedor SMTP pendiente).

## 1. Objetivo

Gestionar las cuentas del personal de la escuela que opera U SYSTEM y su autenticación. **Un usuario es personal administrativo; el alumno NO es usuario.**

## 2. Funcionalidades

- Login con usuario o correo y contraseña (access token JWT + refresh token).
- Alta de usuarios hecha por un administrador (no hay auto-registro público).
- Recuperación de contraseña por correo con token de un solo uso.
- Cambio de contraseña y cambio obligatorio en el primer ingreso.
- Bloqueo temporal por intentos fallidos.
- Cierre de sesión con revocación del refresh token.
- CRUD de usuarios, activar o desactivar y perfil propio.

## 3. Reglas de negocio

1. `Email` y `Username` únicos.
2. Contraseña: mínimo 8 caracteres con mayúscula, minúscula, número y símbolo; se guarda con hash (BCrypt o Argon2).
3. Token de recuperación: vigencia 30 min, un solo uso; la respuesta de “olvidé mi contraseña” es genérica.
4. Bloqueo tras `Login.MaxFailedAttempts` intentos por `Login.LockoutMinutes` minutos.
5. Un usuario inactivo no puede iniciar sesión ni renovar tokens.
6. No se puede desactivar ni quitar el rol al último Administrator activo.
7. Todo login (exitoso o fallido) se registra en `AuditLogs`.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/01_Security.sql`

Entidades C# (en `Api/Entities/`): `User`, `RefreshToken`, `PasswordResetToken`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Iniciar sesión | `Public` |
| POST | `/api/v1/auth/refresh` | Renovar token | `Public` |
| POST | `/api/v1/auth/logout` | Cerrar sesión | `Authenticated` |
| POST | `/api/v1/auth/forgot-password` | Solicitar recuperación | `Public` |
| POST | `/api/v1/auth/reset-password` | Restablecer con token | `Public` |
| POST | `/api/v1/auth/change-password` | Cambiar contraseña | `Authenticated` |
| GET | `/api/v1/auth/me` | Datos y permisos del usuario actual | `Authenticated` |
| GET | `/api/v1/users` | Listar usuarios (paginado) | `Users.View` |
| GET | `/api/v1/users/{Id}` | Detalle | `Users.View` |
| POST | `/api/v1/users` | Crear usuario | `Users.Create` |
| PUT | `/api/v1/users/{Id}` | Editar usuario | `Users.Edit` |
| PATCH | `/api/v1/users/{Id}/status` | Activar o desactivar | `Users.Edit` |
| POST | `/api/v1/users/{Id}/reset-password` | Forzar contraseña temporal | `Users.Edit` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Users/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/User.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AuthSession.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/LoginRequest.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/CreateUserRequest.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/UpdateUserRequest.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/AuthService.ts` | Llamadas HTTP al controlador del módulo |
| Model | `model/UsersService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useLoginViewModel.ts` | Estado, validaciones y acciones de LoginPage |
| View | `view/LoginPage.tsx` | Inicio de sesión |
| ViewModel | `viewmodel/useForgotPasswordViewModel.ts` | Estado, validaciones y acciones de ForgotPasswordPage |
| View | `view/ForgotPasswordPage.tsx` | Solicitud de recuperación |
| ViewModel | `viewmodel/useResetPasswordViewModel.ts` | Estado, validaciones y acciones de ResetPasswordPage |
| View | `view/ResetPasswordPage.tsx` | Restablecer contraseña |
| ViewModel | `viewmodel/useChangePasswordViewModel.ts` | Estado, validaciones y acciones de ChangePasswordPage |
| View | `view/ChangePasswordPage.tsx` | Cambio obligatorio en el primer ingreso |
| ViewModel | `viewmodel/useUserListViewModel.ts` | Estado, validaciones y acciones de UserListPage |
| View | `view/UserListPage.tsx` | Listado con filtros por rol y estado |
| ViewModel | `viewmodel/useUserFormViewModel.ts` | Estado, validaciones y acciones de UserFormPage |
| View | `view/UserFormPage.tsx` | Crear o editar y asignar rol |
| ViewModel | `viewmodel/useProfileViewModel.ts` | Estado, validaciones y acciones de ProfilePage |
| View | `view/ProfilePage.tsx` | Perfil propio |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `01_Security` ejecutado(s) y revisado(s).
- [ ] Permisos `Users.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `User` (Entity) → `IUserRepository` / `UserRepository`
- [ ] `IUserService` / `UserService`; DTOs `UserDto`, `CreateUserRequest`, `UpdateUserRequest`; validadores
- [ ] `RefreshToken` (Entity) → `IRefreshTokenRepository` / `RefreshTokenRepository`; se gestiona desde el servicio del módulo (sin CRUD propio)
- [ ] `PasswordResetToken` (Entity) → `IPasswordResetTokenRepository` / `PasswordResetTokenRepository`; se gestiona desde el servicio del módulo (sin CRUD propio)
- [ ] `IAuthService` / `AuthService` (login, refresh, recuperación y cierre de sesión); DTOs `LoginRequest`, `LoginResponse`
- [ ] `IPasswordHasher` (BCrypt o Argon2) y `ITokenService` para firmar el JWT
- [ ] `AuthController` con `[RequirePermission]` en cada acción
- [ ] `UsersController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddUsersModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `UsersService`
- [ ] ViewModels (hooks) con validaciones y manejo de errores
- [ ] Views con AXZY UI; ruta y entrada de menú protegidas por permiso

### D. Pruebas
- [ ] Unitarias de servicios (reglas de negocio de la sección 3)
- [ ] Integración de repositorios contra MySQL de pruebas
- [ ] Pruebas de ViewModels
- [ ] Caso de prueba por cada criterio de aceptación

### E. Cierre
- [ ] Criterios de aceptación cumplidos
- [ ] Revisión de seguridad (permisos, datos sensibles)
- [ ] Documentación actualizada

## 8. Criterios de aceptación

- [ ] Un usuario activo con credenciales correctas obtiene tokens y entra al sistema.
- [ ] Tras 5 intentos fallidos la cuenta queda bloqueada el tiempo configurado.
- [ ] El token de recuperación funciona una sola vez y expira.
- [ ] Ninguna contraseña ni token se guarda en texto plano.
- [ ] Un usuario sin permiso no ve ni puede invocar la funcionalidad (UI y API).
