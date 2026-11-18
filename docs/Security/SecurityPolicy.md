# Seguridad

## Autenticación

- JWT de vida corta más refresh token rotativo, guardado como hash.
- Contraseñas con BCrypt; mínimo 8 caracteres con mayúscula, minúscula, número y símbolo.
- Bloqueo temporal por intentos fallidos (`Login.MaxFailedAttempts`, `Login.LockoutMinutes`).
- Recuperación de contraseña con token de un solo uso (30 min) y respuesta genérica.

## Autorización

- RBAC con permisos `Module.Action` incluidos como claims del JWT. Matriz completa en [`Rbac.md`](Rbac.md).
- Verificación **siempre en la API** con `[RequirePermission]`; la UI solo oculta opciones.
- No se puede desactivar ni quitar el rol al último Administrator activo.
- No se eliminan roles de sistema ni roles con usuarios asignados.

## Datos

- Consultas parametrizadas con EF Core; sin SQL concatenado.
- Secretos fuera del repositorio ([`Operations/Secrets.md`](../Operations/Secrets.md)).
- HTTPS obligatorio; CORS restringido al origen del desktop.
- No se registran contraseñas, tokens ni datos sensibles en los logs.

### Datos personales de alumnos

CURP, domicilio, documentos oficiales y situación económica.

- Acceso restringido por `Students.View`, `StudentDocuments.View` y `StudentDocuments.Validate`.
  Finanzas ve el estado de cuenta, no los documentos.
- Los archivos van en un bucket S3 **privado**; en `StudentDocuments` solo se guarda `FilePath`. Se
  descargan con URL firmada de 5 minutos ([`Operations/Storage.md`](../Operations/Storage.md)).
- En pruebas se usan datos ficticios, nunca datos reales de alumnos.

## Auditoría

- `AuditLogs` es solo de inserción: no tiene endpoints para editar ni borrar.
- Se registra: login y logout; altas, cambios y bajas; cambios de roles y permisos; captura y
  modificación de calificaciones; cierre y reapertura de actas; pagos, cancelaciones y descargas de
  documentos.

## Electron

- `contextIsolation: true` y `nodeIntegration: false`; preload mínimo.
- Sin `eval` ni carga de contenido remoto en la ventana principal.

## Antes de producción

- [ ] `Jwt:SecretKey` fuerte y fuera del repositorio.
- [ ] Usuario de base de datos de aplicación sin privilegios DDL (no `root`).
- [ ] Contraseña del administrador inicial cambiada.
- [ ] Swagger deshabilitado o protegido.
- [ ] Respaldos probados y cifrados.
- [ ] Bucket S3 privado, con versionado y cifrado en reposo.
- [ ] Credenciales AWS con permisos mínimos.
- [ ] Dependencias auditadas: `dotnet list package --vulnerable` y `npm audit`.
- [ ] Instaladores firmados.

## Si hay un incidente

1. Revocar los refresh tokens del usuario afectado y desactivar la cuenta si hay credenciales comprometidas.
2. Revisar `AuditLogs` del periodo afectado.
3. Rotar `Jwt:SecretKey` si se comprometió la firma, y las credenciales de S3 si aplica.
