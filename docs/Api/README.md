# API

| Archivo | Contenido |
|---------|-----------|
| [`Conventions.md`](Conventions.md) | Respuesta estándar, paginación, errores, versionado y seguridad |
| [`LayerExample.md`](LayerExample.md) | Ejemplo completo por capas con el módulo `Users` (C#) |
| [`EndpointsPhase1.md`](EndpointsPhase1.md) … [`EndpointsPhase4.md`](EndpointsPhase4.md) | Endpoints por fase (generados desde los módulos) |
| [`Entities/`](Entities/README.md) | Clases C# de las 37 entidades (`AuditableEntity` + una clase por tabla) |
| [`Resources/`](Resources/ErrorMessagesEs.json) | Mensajes de error en español e inglés (`ErrorMessagesEs.json`, `ErrorMessagesEn.json`) |

Las clases de `Entities/` van en `USystem.Domain/Entities` (namespace `USystem.Domain.Entities`).

## Estructura de una petición

```
HTTP → Controller → I...Service → Service → I...Repository → Repository → MySQL
                        (DTOs)                (Entities)
```

Reglas:

- Ningún controlador contiene lógica de negocio.
- Ningún repositorio contiene reglas de negocio.
- Los servicios reciben y devuelven DTOs; las entidades no salen de la capa de datos.
- Cada acción declara su permiso con `[RequirePermission("Module.Action")]`.
