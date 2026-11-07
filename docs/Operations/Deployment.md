# Despliegue

## API

- Publicación: `dotnet publish api/USystem.Api -c Release -o Publish`.
- Hosting: servidor propio (IIS o Kestrel detrás de Nginx) o contenedor Docker.
- HTTPS obligatorio y CORS limitado al origen del desktop.
- Migraciones o scripts: aplicar en orden antes de levantar la versión nueva.

## Variables de entorno

La lista completa, qué va en local y qué en producción, y el bloque listo para pegar en Railway están en
[`Environments.md`](Environments.md). Los secretos no se guardan en el repositorio ni en `SystemSettings`
(ver [`Secrets.md`](Secrets.md)).

## Base de datos

- MySQL 8 con usuario de aplicación de permisos mínimos (`SELECT`, `INSERT`, `UPDATE`, `DELETE`).
- Respaldos automáticos (ver `Backups.md`).
- `lower_case_table_names=0` en todos los ambientes.
- `RunAll.sql` solo en instalaciones nuevas; en producción se aplican scripts incrementales.

### Instancia actual

MySQL 8.4.11 en Railway, base `railway`, compartida por local y producción. La API de producción se conecta
por la red interna con referencias `${{MySQL.…}}` (ver [`Environments.md`](Environments.md)). Los scripts se
aplican con `python tools/ApplyMySqlScripts.py --target-database railway`.

## Almacenamiento

- Bucket S3 privado; el detalle está en `Storage.md`.
- En desarrollo se puede usar MinIO con el mismo contrato S3.

## Desktop: instaladores

Se distribuye para **Windows, Linux y macOS** con `electron-builder`.

| Sistema | Comando | Salida |
|---|---|---|
| Windows | `npm run dist:win` | Instalador NSIS `.exe` |
| Linux | `npm run dist:linux` | `AppImage` y `.deb` |
| macOS | `npm run dist:mac` | `.dmg` (firmado y notarizado) |

Cada comando ejecuta `npm run version:bump` antes de compilar: sube la versión patch de `package.json`
(`scripts/BumpVersion.mjs`) y el artefacto lleva esa versión en el nombre. Para saltos grandes:
`npm run version:bump -- --minor` o `-- --major`.

- Variable `VITE_API_BASE_URL` por ambiente.
- Actualización automática opcional (`electron-updater`).
- Firma de código: certificado Authenticode en Windows, firma y notarización en macOS.
- El pipeline de CI compila los tres instaladores en una matriz.

## Ambientes

| Ambiente | Uso | Dónde corre | Base de datos |
|----------|-----|-------------|---------------|
| Development | Desarrollo local | `dotnet run` en tu computadora | MySQL de Railway (conexión pública) |
| Production | Operación de la escuela | Railway (Docker) | MySQL de Railway (red interna) |

No hay Staging ni QA. Detalle en [`Environments.md`](Environments.md).

## Orden de despliegue

1. Respaldo de la base de datos y del bucket.
2. Aplicar el script o la migración incremental.
3. Publicar la API y verificar `/health` (responde `Healthy` solo si llega a MySQL).
4. Publicar el desktop para los tres sistemas.
5. Prueba de humo: login, listado de alumnos y carga de un documento. Como no hay Staging y la base
   es compartida, evita pagos de prueba o bórralos al terminar.

> Swagger está apagado en producción; se enciende solo para demos con `Swagger__Enabled=true`.
