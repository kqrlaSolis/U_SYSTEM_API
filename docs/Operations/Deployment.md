# Despliegue

## API

- Publicación: `dotnet publish api/USystem.Api -c Release -o Publish`.
- Hosting: servidor propio (IIS o Kestrel detrás de Nginx) o contenedor Docker.
- HTTPS obligatorio y CORS limitado al origen del desktop.
- Migraciones o scripts: aplicar en orden antes de levantar la versión nueva.

## Variables de entorno

| Variable | Uso |
|---|---|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión MySQL |
| `Jwt__SecretKey` | Firma del JWT |
| `Smtp__Host`, `Smtp__Port`, `Smtp__User`, `Smtp__Password`, `Smtp__From` | Correo (proveedor pendiente de definir) |
| `AWS_REGION` | `us-east-2` |
| `AWS_BUCKET_NAME` | `cfsp-s3-bucket-prod` |
| `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` | Credenciales del bucket (nunca en el repositorio) |
| `AWS_ENDPOINT_URL` | Opcional, para MinIO en desarrollo |

Los secretos no se guardan en el repositorio ni en `SystemSettings`. Ver [`Secrets.md`](Secrets.md).

## Base de datos

- MySQL 8 con usuario de aplicación de permisos mínimos (`SELECT`, `INSERT`, `UPDATE`, `DELETE`).
- Respaldos automáticos (ver `Backups.md`).
- `lower_case_table_names=0` en todos los ambientes.
- `RunAll.sql` solo en instalaciones nuevas; en producción se aplican scripts incrementales.

### Instancia actual de desarrollo

| Dato | Valor |
|---|---|
| Proveedor | Railway (MySQL 8.4.11) |
| Host / puerto | `iriguchi.proxy.rlwy.net` / `45128` |
| Base de datos | `railway` (37 tablas, 2 vistas tras `RunAll.sql` con `--target-database`) |
| Usuario | `root` |

La cadena está en `ConnectionStrings__DefaultConnection` de `.env` y documentada en
[`DataModel/README.md`](../DataModel/README.md#conexión). Los scripts se aplican con
`python tools/ApplyMySqlScripts.py`. La credencial viajó por chat: **rotarla antes de producción**.

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

| Ambiente | Uso | Base de datos |
|----------|-----|---------------|
| Development | Desarrollo local | `USystem` local |
| Staging | Pruebas y capacitación | `USystemStaging` |
| Production | Operación de la escuela | `USystem` |

## Orden de despliegue

1. Respaldo de la base de datos y del bucket.
2. Aplicar el script o la migración incremental.
3. Publicar la API y verificar `/health` (Swagger solo en Staging).
4. Publicar el desktop para los tres sistemas.
5. Prueba de humo: login, listado de alumnos, carga de un documento y un pago de prueba en Staging.

> Swagger se deshabilita o se protege en producción.
