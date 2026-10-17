# Cómo agregar un módulo

Sigue este orden. Cada módulo ya tiene su plan detallado en `Modules/`.

1. **Leer** el documento del módulo (`Modules/NN_Name.md`): reglas, endpoints y criterios de aceptación.
2. **Base de datos:** confirmar las tablas en `DataModel/Scripts/`; agregar permisos en `99_SeedData.sql`
   y en `tools/DocsGenerator/Rbac.py`.
3. **Entidades:** regenerar con `python tools/DocsGenerator/GenerateEntities.py` (escribe en
   `docs/Api/Entities/` y en `api/USystem.Domain/Entities/`) y registrar el `DbSet` en
   `USystemDbContext`.
4. **Repositorio:** interfaz en `USystem.Domain/Interfaces`, implementación en
   `USystem.Infrastructure/Repositories`.
5. **Servicio:** interfaz e implementación en `USystem.Application/Services`; DTOs en `Dtos/` y
   validadores en `Validators/`.
6. **Controller:** `[Route("api/v1/<resource>")]` con `[RequirePermission]` en cada acción.
7. **DI:** crear `Add<Module>Module()` en `api/USystem.Api/Extensions` (molde:
   `SecurityModuleExtensions.cs`) y llamarlo desde `AddUSystemModules` en `ServiceCollectionExtensions.cs`.
8. **Auditoría:** invocar `IAuditService` en altas, cambios, bajas y acciones críticas.
9. **Desktop (MVVM):** crear `desktop/src/modules/<Module>/` con `model/`, `viewmodel/` y `view/`;
   la pantalla se renderiza dentro del `ITLayout` de `app/layout/MainLayout.tsx` y usa `ITPage` para la
   cabecera y sus estados (nunca un `PageHeader` propio). Agregar la ruta y la entrada de menú en
   `app/layout/NavigationModules.ts`, protegidas por permiso.
10. **i18n:** agregar las claves de la interfaz en `Es.ts` y `En.ts`, y los códigos de error nuevos en
    `Api/Resources/ErrorMessagesEs.json` y `ErrorMessagesEn.json`.
11. **Pruebas:** unitarias de servicio, integración de repositorio, pruebas de ViewModel y un caso por
    criterio de aceptación.
12. **Cierre:** marcar el checklist del módulo y actualizar la documentación.

## Checklist de nomenclatura

- [ ] Nombres de clases, métodos, variables y parámetros en inglés y PascalCase.
- [ ] Archivos nuevos con nombre PascalCase.
- [ ] Permisos con formato `Module.Action`.
- [ ] Tablas y columnas en PascalCase.
- [ ] Comentarios en español.
- [ ] Textos visibles en los catálogos i18n (nada hardcodeado).
- [ ] Error nuevo con código estable y mensajes es/en.

Plantillas útiles en `Templates/`.
