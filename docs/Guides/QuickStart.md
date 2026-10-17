# Inicio rápido

## Requisitos

- Visual Studio 2022 (workloads ASP.NET y Node.js) y .NET 8 SDK.
- Node.js LTS y npm.
- MySQL 8.x.
- Bucket S3 o MinIO local para archivos.

## 1. Base de datos

La instancia de desarrollo está en Railway (MySQL 8.4): `iriguchi.proxy.rlwy.net:45128`, base `railway`.
La cadena completa, en formato URI y en formato .NET, está en `ConnectionStrings__DefaultConnection` de
`.env` y documentada en [`DataModel/README.md`](../DataModel/README.md#conexión).

Sin cliente `mysql` (recomendado en Windows):

```bash
python -m pip install pymysql
python tools/ApplyMySqlScripts.py --target-database railway
```

Con cliente `mysql`:

```bash
cd docs/DataModel/Scripts
mysql -h iriguchi.proxy.rlwy.net -P 45128 -u root -p < RunAll.sql
```

Al final, crear el usuario administrador inicial con el comando de la API (la contraseña se hashea con
BCrypt y nunca se guarda en texto plano):

```bash
# Define Seed__AdminPassword en .env (mínimo 8 con mayúscula, minúscula, número y símbolo)
dotnet run --project api/USystem.Api -- --seed-admin
```

El comando crea `Administrator` (`admin@usystem.local`) con `MustChangePassword = 1` y solo actúa si la
tabla `Users` está vacía. Verificar el login:

```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"UsernameOrEmail":"Administrator","Password":"<la de Seed__AdminPassword>"}'
```

### Usuarios de desarrollo

Cuentas demo creadas en la base de desarrollo (una por rol). **Son credenciales de desarrollo: se rotan o
se eliminan antes de producción.**

| Usuario | Contraseña | Rol | Permisos |
|---|---|---|---|
| `Administrator` | `Admin.2026` | `Administrator` | 78 |
| `registro` | `Registro.2026` | `Registrar` | 33 |
| `finanzas` | `Finanzas.2026` | `Finance` | 21 |
| `coordinador` | `Coordinador.2026` | `ProgramCoordinator` | 35 |
| `docente` | `Docente.2026` | `Teacher` | 9 |
| `consulta` | `Consulta.2026` | `ReadOnly` | 23 |

Se crean con `POST /api/v1/users` (contraseña temporal) y la API las marca con `MustChangePassword`; en
desarrollo se desactiva esa marca para entrar directo. El administrador puede restablecer cualquier
contraseña desde `POST /api/v1/users/{Id}/reset-password`.

## 2. Almacenamiento

En desarrollo, levantar MinIO con el mismo contrato S3:

```bash
docker run -p 9000:9000 -p 9001:9001 \
  -e MINIO_ROOT_USER=USystemDev \
  -e MINIO_ROOT_PASSWORD=CHANGE_ME \
  minio/minio server /data --console-address ":9001"
```

Configurar en `.env`:

```
AWS_REGION=us-east-2
AWS_BUCKET_NAME=cfsp-s3-bucket-prod
AWS_ACCESS_KEY_ID=
AWS_SECRET_ACCESS_KEY=
AWS_ENDPOINT_URL=http://localhost:9000   # solo MinIO local
```

`.env` no se versiona; la plantilla está en `.env.example`. El detalle está en `Operations/Storage.md` y
`Operations/Secrets.md`.

## 3. API

1. Abrir `USystem.sln` en Visual Studio (o usar `dotnet run --project api/USystem.Api`).
2. Verificar la cadena de conexión en `.env` (`ConnectionStrings__DefaultConnection`); la API carga el
   `.env` de la raíz del repositorio al arrancar.
3. Configurar `Jwt__SecretKey` (mínimo 32 caracteres) y las variables `Storage__*` por entorno.
4. Ejecutar `USystem.Api`; Swagger queda en `/swagger` y el chequeo de salud en `/health`.

## 4. Desktop

```bash
cd desktop
npm install
npm run dev        # Vite + Electron
```

- Configurar `VITE_API_BASE_URL=http://localhost:5000`.
- Idioma por defecto: español; el selector de idioma guarda la preferencia en `localStorage`.

## 5. Instaladores

| Sistema | Comando |
|---|---|
| Windows | `npm run dist:win` |
| Linux | `npm run dist:linux` |
| macOS | `npm run dist:mac` |

## 6. Verificación

- [ ] Swagger carga y `POST /api/v1/auth/login` responde.
- [ ] El desktop abre la pantalla de inicio de sesión.
- [ ] Tras iniciar sesión, el menú solo muestra los módulos permitidos.
- [ ] Un usuario sin permiso recibe 403 al invocar el endpoint directamente.
- [ ] Ninguna tabla de la base de datos usa snake_case.
- [ ] La interfaz cambia entre español e inglés sin reiniciar.
- [ ] Un documento de prueba se sube al bucket y se descarga con URL firmada.

## 7. Herramientas de calidad

- `dotnet format` aplica las reglas del `.editorconfig` (incluida la nomenclatura).
- `npm run lint` aplica ESLint, que valida PascalCase en TypeScript.
- `dotnet test` y `npm test` ejecutan las pruebas.
