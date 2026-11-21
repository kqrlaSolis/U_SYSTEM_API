# Base de datos local (LocalSetup)

Crea **desde cero** la base `USystem` completa en tu computadora: las 37 tablas, las 2 vistas, los
catálogos del sistema, un administrador listo para entrar y un conjunto de **datos demo realistas** para
probar todos los módulos de la app sin tocar la base de Railway.

> **Úsalo solo contra un MySQL local.** El script borra la base `USystem` si ya existe. La base de
> Railway (la que usa producción) se llama `railway` y el script se niega a apuntar a hosts de Railway.

## Qué necesitas

- **MySQL 8.0 o superior** (8.4 recomendado). MariaDB no sirve: el esquema usa `utf8mb4_0900_ai_ci`.
  - Windows: [MySQL Installer](https://dev.mysql.com/downloads/installer/) → "Server only" o "Developer Default".
  - macOS: `brew install mysql && brew services start mysql`
  - Linux: `sudo apt install mysql-server`
  - **O nada de eso** si tienes Docker (opción C).
- La contraseña de `root` (o de otro usuario con permiso para crear bases y usuarios).

## Opción A · Windows (PowerShell)

Desde la raíz del repositorio:

```powershell
.\database\LocalSetup\Setup.ps1 -WriteEnv
```

Te pide la contraseña de `root`, crea todo, crea el usuario `USystemApp` y escribe la cadena de conexión
en tu `.env` (hace un respaldo `.env.bak-<fecha>` antes de tocarlo, y genera `Jwt__SecretKey` si falta).

Si PowerShell bloquea el script ("la ejecución de scripts está deshabilitada"):

```powershell
powershell -ExecutionPolicy Bypass -File .\database\LocalSetup\Setup.ps1 -WriteEnv
```

| Parámetro | Para qué |
|---|---|
| `-Port 3307` | Si tu MySQL no está en 3306 |
| `-RootPassword 'clave'` | No preguntar la contraseña (útil en scripts) |
| `-Force` | No pedir confirmación antes de borrar la base existente |
| `-NoDemoData` | Solo esquema, catálogos y administrador (base "limpia") |
| `-WriteEnv` | Actualiza `ConnectionStrings__DefaultConnection` en el `.env` de la raíz |
| `-AppPassword 'otra'` | Contraseña del usuario `USystemApp` (por defecto `USystemLocal#2026`) |
| `-MySqlPath 'C:\...\mysql.exe'` | Si no encuentra `mysql.exe` (busca en PATH, MySQL Server, XAMPP y Laragon) |

## Opción B · macOS / Linux / Git Bash

```bash
bash database/LocalSetup/Setup.sh --write-env
```

Mismas opciones en formato largo: `--port`, `--password`, `--force`, `--no-demo`, `--write-env`,
`--app-password`, `--mysql`. Ver `bash database/LocalSetup/Setup.sh --help`.

## Opción C · Docker (sin instalar MySQL)

```bash
cd database/LocalSetup
docker compose up -d
```

Levanta MySQL 8.4 en el **puerto 3307** y crea la base completa en el primer arranque (~30 s; revisa con
`docker compose logs -f mysql`). Luego pon esto en el `.env` de la raíz:

```
ConnectionStrings__DefaultConnection=Server=127.0.0.1;Port=3307;Database=USystem;User=USystemApp;Password=USystemLocal#2026;SslMode=Preferred;AllowPublicKeyRetrieval=True;
```

Para empezar de cero: `docker compose down -v && docker compose up -d`.

## Después: correr la API y la app

```bash
dotnet run --project api/USystem.Api        # http://localhost:5000/health debe decir Healthy
npm --prefix desktop run dev                # desktop (VITE_API_BASE_URL=http://localhost:5000)
```

El `.env` también necesita `Jwt__SecretKey` (32+ caracteres). `-WriteEnv` / `--write-env` lo genera si
no existe; sin eso, créalo con `openssl rand -base64 48`.

## Usuarios para entrar

Todos con la contraseña **`USystem#2026`** (no piden cambio de contraseña en el primer ingreso):

| Usuario | Rol | Qué puede probar |
|---|---|---|
| `Administrator` (`admin@usystem.local`) | Administrator | Todo, incluidos usuarios, roles y configuración |
| `Registrar` | Registrar | Alumnos, expedientes, inscripciones, kardex, trámites |
| `Finance` | Finance | Cobros, pagos, becas, estados de cuenta |
| `Coordinator` | ProgramCoordinator | Carreras, planes, materias, docentes, grupos |
| `TeacherRuiz`, `TeacherSoto` | Teacher | Captura de calificaciones |
| `ReadOnly` | ReadOnly | Consulta de todos los módulos |
| `SNavarro` | Registrar (inactivo) | Comprobar que un usuario inactivo no puede entrar |

Con `-NoDemoData` solo existe `Administrator`.

## Qué trae la base demo

"Hoy" para los datos es **21 de noviembre de 2026**: el periodo en curso es **2026-3** (Sep-Dic).

| Área | Contenido |
|---|---|
| Carreras | ISC y LAE con plan 2024 completo (9 cuatrimestres, 36 materias, 270 créditos, prerrequisitos). LPS sin plan, para probar el alta |
| Periodos | 2025-1 a 2026-2 cerrados, **2026-3 en curso**, 2027-1 planeado (inscripciones abiertas a partir del 23 de noviembre) |
| Docentes | 12 (dos ligados a usuarios Teacher, uno inactivo) con sus materias habilitadas |
| Alumnos | 27: generación 2025-1 (6.º cuatrimestre), 2026-1 (3.º), 2026-3 (1.º), una baja temporal, una baja definitiva y 2 aspirantes 2027-1 con expediente incompleto |
| Grupos | 104 con aula y horario sin choques; los de 2027-1 están abiertos para probar la reinscripción |
| Calificaciones | Actas cerradas de periodos anteriores (~9 % reprobados, extraordinarios aprobados hasta 2026-1, reprobadas de 2026-2 pendientes); 2026-3 con parciales 1 y 2 |
| Finanzas | Inscripción/reinscripción y 4 colegiaturas por periodo, becas con descuento, 3 morosos con recargo, un abono parcial, un pago cancelado; 356 recibos |
| Otros | Trámites emitidos y pendientes, notificaciones enviadas/pendientes/fallidas, material didáctico |

Los documentos del expediente no tienen archivo (`FilePath` vacío): para subir archivos necesitas las
variables `AWS_*` en el `.env`.

## Archivos

| Archivo | Qué hace |
|---|---|
| `Setup.ps1` / `Setup.sh` | Instaladores. Aplican `../00`-`../06`, `../99_SeedData.sql` y los de esta carpeta, en ese orden |
| `10_AdminUser.sql` | Administrador inicial con hash BCrypt (costo 12, igual que la API) |
| `20_DemoData.sql` | Datos demo (se omite con `-NoDemoData`) |
| `30_AppUser.sql` | Usuario `USystemApp`, solo para Docker (los instaladores generan el suyo) |
| `docker-compose.yml` | MySQL 8.4 con todo lo anterior montado en `docker-entrypoint-initdb.d` |

El esquema **no se duplica aquí**: siempre se toma de `database/`, así que esta carpeta sigue funcionando
cuando cambian las tablas. Si agregas una columna obligatoria, actualiza `20_DemoData.sql`.

## Problemas comunes

| Síntoma | Solución |
|---|---|
| `No se encontró mysql.exe` | Pasa `-MySqlPath` o agrega `C:\Program Files\MySQL\MySQL Server 8.x\bin` al PATH |
| `Access denied for user 'root'` | Contraseña de root incorrecta. En Windows se definió al instalar MySQL |
| `Can't connect to MySQL server` | El servicio no está corriendo: `services.msc` → `MySQL80` → Iniciar (o `brew services start mysql`) |
| `Unknown collation: 'utf8mb4_0900_ai_ci'` | Es MariaDB o MySQL 5.7: instala MySQL 8 o usa Docker |
| La API dice `Unable to connect` | Revisa puerto y contraseña en `ConnectionStrings__DefaultConnection` del `.env` |
| La API no arranca: `Configura Jwt:SecretKey...` | Falta `Jwt__SecretKey` (32+ caracteres) en el `.env`; `-WriteEnv` lo genera |
