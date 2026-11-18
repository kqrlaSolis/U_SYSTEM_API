# Cómo agregar un módulo

Sigue este orden. Usa como molde un módulo que ya exista, por ejemplo `Careers`.

1. **Base de datos:** crear las tablas en el script que corresponda de `database/`; agregar permisos en `99_SeedData.sql`
   y en `USystem.Domain/Constants/PermissionCodes.cs`.
2. **Entidades:** crear la clase en `api/USystem.Domain/Entities/` (hereda de `AuditableEntity`),
   agregarla a `docs/Database/README.md` y registrar el `DbSet` en `USystemDbContext`.
3. **Repositorio:** interfaz en `USystem.Domain/Interfaces`, implementación en
   `USystem.Infrastructure/Repositories`.
4. **Servicio:** interfaz e implementación en `USystem.Application/Services`; DTOs en `Dtos/` y
   validadores en `Validators/`.
5. **Controller:** `[Route("api/v1/<resource>")]` con `[RequirePermission]` en cada acción.
6. **DI:** crear `Add<Module>Module()` en `api/USystem.Api/Extensions` (molde:
   `SecurityModuleExtensions.cs`) y llamarlo desde `AddUSystemModules` en `ServiceCollectionExtensions.cs`.
7. **Auditoría:** invocar `IAuditService` en altas, cambios, bajas y acciones críticas.
8. **Desktop (MVVM):** crear `desktop/src/modules/<Module>/` con `model/`, `viewmodel/` y `view/`;
   la pantalla se renderiza dentro del `ITLayout` de `app/layout/MainLayout.tsx` y usa `ITPage` para la
   cabecera y sus estados (nunca un `PageHeader` propio). Agregar la ruta y la entrada de menú en
   `app/layout/NavigationModules.ts`, protegidas por permiso.
9. **i18n:** agregar las claves de la interfaz en `Es.ts` y `En.ts`, y los códigos de error nuevos en
    `api/USystem.Api/Resources/ErrorMessagesEs.json` y `ErrorMessagesEn.json`.
10. **Pruebas:** unitarias de servicio, integración de repositorio y pruebas de ViewModel.
11. **Cierre:** actualizar la documentación que cambie (permisos en `Security/Rbac.md`, tablas en `Database/README.md`).

## Checklist de nomenclatura

- [ ] Nombres de clases, métodos, variables y parámetros en inglés y PascalCase.
- [ ] Archivos nuevos con nombre PascalCase.
- [ ] Permisos con formato `Module.Action`.
- [ ] Tablas y columnas en PascalCase.
- [ ] Comentarios en español.
- [ ] Textos visibles en los catálogos i18n (nada hardcodeado).
- [ ] Error nuevo con código estable y mensajes es/en.
