# Preparación para la app del alumno (futuro)

No se desarrolla ahora. El diseño actual debe permitirla sin reescritura.

- La API se mantiene **agnóstica del cliente** (REST + JWT).
- `Users` representa **personal**. Para el alumno se agregará después un mecanismo de acceso
  independiente (por ejemplo `StudentAccounts`) con su propia autenticación.
- Los permisos están definidos por acción, lo que permitirá un perfil `Student` restringido a consultar
  **solo sus propios datos**.
- Los endpoints actuales viven bajo `/api/v1/`; los del portal del alumno usarán `/api/v1/portal/`.
- Las notificaciones por correo ya llegan al alumno y al tutor sin necesidad de acceso al sistema.
- El kardex y el avance académico ya se calculan en el servidor, así que el portal solo consumirá datos.
