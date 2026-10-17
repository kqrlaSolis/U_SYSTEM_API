# Política de seguridad

## Autenticación

- JWT de vida corta más refresh token rotativo, guardado como hash.
- Contraseñas con BCrypt o Argon2; mínimo 8 caracteres con mayúscula, minúscula, número y símbolo.
- Bloqueo temporal por intentos fallidos (`Login.MaxFailedAttempts`, `Login.LockoutMinutes`).
- Recuperación de contraseña con token de un solo uso (30 min) y respuesta genérica.

## Autorización

- RBAC con permisos `Module.Action` incluidos como claims del JWT.
- Verificación **siempre en la API** con `[RequirePermission]`; la UI solo oculta opciones.
- Mínimo privilegio en los roles semilla (ver `RbacMatrix.md`).
- No se puede desactivar ni quitar el rol al último Administrator activo.
- No se eliminan roles de sistema ni roles con usuarios asignados.

## Datos

- Consultas parametrizadas con EF Core; sin SQL concatenado.
- Secretos fuera del repositorio (variables de entorno o *User Secrets*).
- HTTPS obligatorio; CORS restringido al origen del desktop.
- Respaldos cifrados y fuera del servidor de base de datos.
- Secretos gestionados por variables de entorno o gestor de secretos (ver `Operations/Secrets.md`).
- Los documentos se guardan en un bucket S3 **privado**; solo se almacena la referencia y se descargan
  con URLs firmadas de vida corta (ver `Operations/Storage.md`).

## Auditoría

- `AuditLogs` es solo de inserción: no se edita ni se borra.
- Se registra login y logout, altas, cambios y bajas; cambios de roles y permisos; captura y
  modificación de calificaciones; cierre y reapertura de actas; pagos, cancelaciones y descargas de
  documentos.

## Electron

- `contextIsolation: true` y `nodeIntegration: false`; preload mínimo.
- Sin `eval` ni carga de contenido remoto en la ventana principal.
- Tokens en memoria; el refresh token en el almacenamiento seguro del sistema operativo.

## Respuesta ante incidentes

1. Revocar los refresh tokens del usuario afectado.
2. Desactivar la cuenta si hay compromiso de credenciales.
3. Revisar `AuditLogs` del periodo afectado.
4. Rotar `Jwt:SecretKey` si se comprometió la firma.
5. Revisar accesos al bucket y rotar credenciales S3 si aplica.
6. Documentar el incidente y las acciones tomadas.
