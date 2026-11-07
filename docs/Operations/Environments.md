# Ambientes y variables de entorno

U SYSTEM tiene **dos ambientes**. No hay develop ni QA.

| | **Local** | **Producción** |
|---|---|---|
| Dónde corre | Tu computadora (`dotnet run`) | Railway (imagen Docker) |
| `ASPNETCORE_ENVIRONMENT` | `Development` (lo pone `launchSettings.json`) | `Production` (lo pone el `Dockerfile`) |
| De dónde salen los secretos | Archivo `.env` en la raíz del repo | Pestaña **Variables** del servicio de la API en Railway |
| Archivo de configuración extra | `appsettings.Development.json` | ninguno (solo `appsettings.json`) |
| Base de datos | MySQL de Railway por su **conexión pública** | El mismo MySQL de Railway por la **red interna** |
| Swagger | Encendido | Apagado (se enciende con `Swagger__Enabled=true`) |
| Logs | Consola + archivo `api/USystem.Api/logs/`, nivel Debug | Solo consola (Railway los guarda), nivel Information |

> ⚠️ **Local y producción comparten la misma base de datos.** Lo que hagas corriendo local (crear usuarios,
> borrar registros, el `--seed-admin`) se ve en producción. Prueba con cuidado.

## Cómo se arma la configuración

La API junta la configuración en este orden; **lo de abajo gana**:

1. `appsettings.json`: valores por defecto, sin secretos. Es lo que usa producción.
2. `appsettings.Development.json`: solo local; logs más detallados y a archivo.
3. Variables de entorno: el `.env` en local, el panel de Railway en producción.

El `.env` lo carga `EnvironmentFileLoader` al arrancar y **nunca pisa** una variable que ya exista en el
sistema. En la imagen Docker no hay `.env` (lo excluyen `.dockerignore` y `.railwayignore`).

El doble guion bajo es la forma de escribir una sección de `appsettings.json` como variable:
`Jwt__SecretKey` es lo mismo que `{ "Jwt": { "SecretKey": "..." } }`.

## Variables

Solo estas variables cambian algo en la API. Lo demás tiene un valor por defecto en `appsettings.json`.

| Variable | Local | Producción | Para qué |
|---|---|---|---|
| `ConnectionStrings__DefaultConnection` | **Requerida** | **Requerida** (sin ella la API no arranca) | Conexión a MySQL |
| `Jwt__SecretKey` | **Requerida** | **Requerida**, distinta a la local | Firma de los tokens de sesión (mínimo 32 caracteres) |
| `AWS_ACCESS_KEY_ID` | Para documentos | **Requerida** | Credencial de S3 |
| `AWS_SECRET_ACCESS_KEY` | Para documentos | **Requerida** | Credencial de S3 |
| `AWS_REGION` | `us-east-2` | `us-east-2` | Región del bucket |
| `AWS_BUCKET_NAME` | `cfsp-s3-bucket-prod` | `cfsp-s3-bucket-prod` | Bucket de archivos |
| `AWS_ENDPOINT_URL` | Opcional | — | Solo para MinIO u otro S3 compatible |
| `Seed__AdminPassword` | Solo para `--seed-admin` | — | Contraseña del primer administrador |
| `Seed__AdminUsername`, `Seed__AdminEmail` | Opcional | — | Por defecto `Administrator` / `admin@usystem.local` |
| `Swagger__Enabled` | — | Opcional (`true` para demos) | Publica `/swagger` en producción |
| `Cors__AllowedOrigins__0` | — | Opcional | Origen permitido extra; por defecto `http://localhost:5173` |
| `Database__ServerVersion` | Opcional | Opcional | Versión de MySQL para EF; por defecto `8.4.11-mysql` |

**No hace falta definir**: `PORT` (lo pone Railway), `ASPNETCORE_ENVIRONMENT` (lo ponen `launchSettings.json`
y el `Dockerfile`), ni `Jwt__Issuer`/`Jwt__Audience`/`Jwt__AccessTokenMinutes`/`Jwt__RefreshTokenDays`
(ya están en `appsettings.json`).

---

## Correr en local

**Requisitos:** .NET 8 SDK. Python 3 solo si vas a aplicar scripts SQL.

### 1. Crear tu `.env` (una sola vez)

```bash
cp .env.example .env
```

Llénalo así:

- **`ConnectionStrings__DefaultConnection`**: en Railway abre el servicio **MySQL** → **Variables** →
  `MYSQL_PUBLIC_URL`. Tiene la forma `mysql://USUARIO:CLAVE@HOST:PUERTO/BASE`; pásala al formato .NET:

  ```
  Server=HOST;Port=PUERTO;Database=BASE;User=USUARIO;Password=CLAVE;SslMode=Preferred;AllowPublicKeyRetrieval=True;
  ```

- **`Jwt__SecretKey`**: genera una clave nueva (no uses la de producción):

  ```bash
  openssl rand -base64 48
  ```

- **`AWS_*`**: las credenciales del usuario IAM del proyecto. Sin ellas la API arranca, pero no puede subir ni
  descargar documentos.

### 2. Arrancar la API

```bash
dotnet run --project api/USystem.Api
```

- Swagger: <http://localhost:5000/swagger>
- Salud (incluye MySQL): <http://localhost:5000/health>

En Visual Studio: abre `USystem.sln`, elige `USystem.Api` y el perfil **http**.

### 3. Pruebas

```bash
dotnet test USystem.sln
```

No necesitan MySQL ni `.env`.

### 4. Crear el primer administrador (solo si la tabla `Users` está vacía)

Pon `Seed__AdminPassword` en `.env` y corre:

```bash
dotnet run --project api/USystem.Api -- --seed-admin
```

Después borra el valor de `Seed__AdminPassword` del `.env`. Como la base es compartida, esto crea el
administrador **también para producción**.

### Probar la imagen de producción en tu compu (opcional)

```bash
docker build -t usystem-api .
docker run --rm -p 8080:8080 --env-file .env usystem-api
```

Corre con `Production`, igual que en Railway: sin Swagger y sin log a archivo. Abre
<http://localhost:8080/health>.

---

## Correr en producción (Railway)

El proyecto de Railway tiene dos servicios: **MySQL** y la **API**. La API se construye con el `Dockerfile`
(`railway.json`) y Railway revisa `/health` antes de dar por bueno el despliegue.

### 1. Variables del servicio de la API (una sola vez)

En Railway abre el servicio de la **API** → **Variables** → **Raw Editor** y pega esto. Cambia lo que está
en `<...>`:

```
ConnectionStrings__DefaultConnection=Server=${{MySQL.MYSQLHOST}};Port=${{MySQL.MYSQLPORT}};Database=${{MySQL.MYSQLDATABASE}};User=${{MySQL.MYSQLUSER}};Password=${{MySQL.MYSQLPASSWORD}};SslMode=Preferred;AllowPublicKeyRetrieval=True;
Jwt__SecretKey=<clave de producción: openssl rand -base64 48>
AWS_ACCESS_KEY_ID=<access key>
AWS_SECRET_ACCESS_KEY=<secret key>
AWS_REGION=us-east-2
AWS_BUCKET_NAME=cfsp-s3-bucket-prod
```

- `${{MySQL.…}}` son **referencias**: Railway pone los valores del servicio MySQL por la red interna. Si
  cambias la contraseña de MySQL, la API la toma sola en el siguiente despliegue. Si tu servicio de base de
  datos no se llama `MySQL`, cambia ese nombre en las referencias.
- Opcionales: `Swagger__Enabled=true` (solo para demos) y `Cors__AllowedOrigins__0=<origen>`.

### 2. Desplegar

Cualquiera de las dos:

- **Push a `main`**: si el repo está conectado a Railway, despliega solo.
- **Desde tu compu**, con el CLI de Railway enlazado al proyecto:

  ```bash
  railway up
  ```

### 3. Verificar

- `https://<tu-dominio>.up.railway.app/health` responde `Healthy`. Si dice `Unhealthy`, la API no puede
  conectarse a MySQL: revisa `ConnectionStrings__DefaultConnection`.
- `https://<tu-dominio>.up.railway.app/` muestra `"Environment": "Production"`.

### Si el despliegue falla

| Síntoma en los logs de Railway | Causa | Arreglo |
|---|---|---|
| `Configura ConnectionStrings:DefaultConnection...` | Falta la cadena | Agrega la variable del paso 1 |
| `Configura Jwt:SecretKey...` | Falta la clave o tiene menos de 32 caracteres | Agrega `Jwt__SecretKey` |
| El healthcheck no pasa | La API no llega a MySQL | Revisa las referencias `${{MySQL.…}}` y el nombre del servicio |

---

## Reglas

- Ningún secreto va en `appsettings*.json`, en `docs/` ni en commits. Solo en `.env` (local) o en Railway
  (producción).
- `.env.example` lleva los **nombres** de las variables con valores de ejemplo, nunca valores reales.
- `Jwt__SecretKey` es distinta en local y en producción.
- Si agregas una variable nueva: agrégala a `.env.example`, a la tabla de este documento y, si es de
  producción, al bloque del paso 1.
- Una credencial que pasó por chat, correo o captura se considera expuesta: rótala (ver `Secrets.md`).
