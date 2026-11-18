# Guía paso a paso: bajar y correr U SYSTEM por primera vez

Esta guía es para alguien que **nunca ha abierto el proyecto**. Síguela en orden y no te saltes pasos.
Está pensada para **Windows con PowerShell**. En Mac o Linux los comandos son los mismos, salvo donde se
indica.

> Cada bloque de código es **un comando**: cópialo, pégalo en la terminal y presiona Enter. Espera a que
> termine antes de pegar el siguiente.

---

## Paso 0. Lo que necesitas pedirle al equipo

Antes de empezar, pídele a alguien del equipo (por mensaje privado, **nunca** por un grupo público ni en
un commit):

- [ ] Acceso al repositorio de GitHub `kqrlaSolis/U_SYSTEM_API`.
- [ ] La **cadena de conexión de MySQL** (empieza con `Server=...`).
- [ ] Las credenciales de **AWS S3** (solo si vas a trabajar con subida de archivos).
- [ ] Un **usuario y contraseña** para entrar a la app (ver
      [`QuickStart.md`](QuickStart.md#usuarios-de-desarrollo)).

Sin la cadena de conexión la API no arranca, así que consíguela primero.

---

## Paso 1. Instalar los programas (una sola vez)

Instala estos cuatro programas. Si ya los tienes, solo revisa la versión con el comando de la derecha.

| Programa | Dónde se descarga | Para revisar que quedó |
|---|---|---|
| **Git** | <https://git-scm.com/downloads> | `git --version` |
| **.NET 8 SDK** | <https://dotnet.microsoft.com/download/dotnet/8.0> (elige "SDK", no "Runtime") | `dotnet --list-sdks` (debe salir una línea que empiece con `8.`) |
| **Node.js 22 LTS** | <https://nodejs.org> (botón LTS) | `node --version` (debe decir `v22...`) |
| Un editor | VS Code o Visual Studio 2022 | — |

**Muy importante:** después de instalar, **cierra la terminal y abre una nueva**. Si no, los comandos
nuevos no se reconocen y sale el error "no se reconoce como nombre de un cmdlet".

> Si ya tienes un .NET más nuevo (9 o 10), igual instala el 8: el proyecto y sus pruebas corren sobre
> .NET 8.

---

## Paso 2. Bajar el repositorio

1. Abre PowerShell.
2. Ve a la carpeta donde guardas tus proyectos. Por ejemplo:

   ```powershell
   cd C:\UNI\DEV
   ```

3. Clona el repo (esto crea la carpeta `USystem`):

   ```powershell
   git clone https://github.com/kqrlaSolis/U_SYSTEM_API.git USystem
   ```

4. Entra a la carpeta:

   ```powershell
   cd USystem
   ```

A partir de aquí, **a esta carpeta le llamamos "la raíz"** (`C:\UNI\DEV\USystem`). Si en algún paso
dice "desde la raíz", es esta carpeta.

---

## Paso 3. Crear el archivo `.env` de la API

La API lee sus contraseñas de un archivo llamado `.env` en la raíz. Ese archivo **no se sube a GitHub**,
por eso cada quien crea el suyo.

1. Desde la raíz, copia la plantilla:

   ```powershell
   Copy-Item .env.example .env
   ```

2. Abre `.env` con tu editor y llena:

   - `ConnectionStrings__DefaultConnection=` → pega la cadena de MySQL que te pasó el equipo.
   - `Jwt__SecretKey=` → cualquier texto de **32 caracteres o más**. Para generar uno aleatorio:

     ```powershell
     [Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }))
     ```

     Copia lo que salga y pégalo después del `=`.
   - `AWS_ACCESS_KEY_ID` / `AWS_SECRET_ACCESS_KEY` → solo si te los pasaron. Si no, déjalos vacíos.

3. Guarda el archivo.

> ⚠️ **La base de datos es la misma que usa producción.** Todo lo que crees, edites o borres desde tu
> computadora cae en datos reales. Ten cuidado.

> ❌ **Nunca** subas el `.env` a GitHub ni lo mandes por un chat de grupo.

---

## Paso 4. Correr la API

Desde la raíz:

```powershell
dotnet run --project api/USystem.Api
```

La primera vez tarda un rato porque descarga paquetes. Sabrás que ya arrancó cuando veas algo como
`Now listening on: http://localhost:5000`.

**Comprueba que funciona:** abre en el navegador <http://localhost:5000/health>. Debe decir `Healthy`.
Si dice `Unhealthy`, revisa la cadena de conexión del `.env`.

También puedes ver todos los endpoints en <http://localhost:5000/swagger>.

**Deja esta terminal abierta.** Si la cierras, la API se apaga.

---

## Paso 5. Correr la app de escritorio

Abre **otra** terminal (la de la API sigue corriendo) y ve a la carpeta `desktop`:

```powershell
cd C:\UNI\DEV\USystem\desktop
```

1. Instala las dependencias. Solo hace falta la primera vez y cada vez que alguien cambie el
   `package.json`:

   ```powershell
   npm install
   ```

2. Crea el `.env` del desktop:

   ```powershell
   Copy-Item .env.example .env
   ```

   No hay que cambiarle nada: ya apunta a `http://localhost:5000`, que es tu API del paso 4.

3. Arranca la app:

   ```powershell
   npm run dev
   ```

Después de unos segundos se abre una ventana de **U SYSTEM** con la pantalla de inicio de sesión. Entra
con el usuario que te pasó el equipo.

---

## Paso 6. Correr las pruebas

Comprueba que todo quedó bien corriendo las pruebas. **No necesitan base de datos ni la API prendida.**

**API**, desde la raíz:

```powershell
cd C:\UNI\DEV\USystem
```

```powershell
dotnet test
```

Al final debe decir `Passed!` y `Failed: 0`. Una prueba sale como `Skipped`: es la que se conecta a la
base de datos real y está apagada a propósito.

**Desktop**, desde la carpeta `desktop`:

```powershell
cd C:\UNI\DEV\USystem\desktop
```

```powershell
npm test
```

Al final debe decir `Test Files ... passed` sin ningún `failed`.

✅ Si llegaste aquí, ya tienes todo funcionando.

---

## Día a día: traer los cambios de los demás

Cada vez que vayas a trabajar:

1. Desde la raíz, guarda o descarta lo que tengas a medias (`git status` te dice si hay algo pendiente).
2. Cámbiate a `main` y trae lo nuevo:

   ```powershell
   git checkout main
   ```

   ```powershell
   git pull
   ```

3. Si cambió algo del desktop, vuelve a instalar dependencias (no pasa nada si no hacía falta):

   ```powershell
   cd desktop
   ```

   ```powershell
   npm install
   ```

4. Para trabajar en algo nuevo, **crea tu propia rama**. No trabajes directo en `main`:

   ```powershell
   git checkout -b fix/Grades-ReportClose
   ```

   El formato de los nombres de rama y de los commits está en [`Conventions/Git.md`](../Conventions/Git.md).

---

## Si algo sale mal

| Error | Qué significa | Cómo se arregla |
|---|---|---|
| `MSB1003: Specify a project or solution file` | Corriste `dotnet` en una carpeta sin `.sln` ni `.csproj` (por ejemplo `api/` o `UnitTests/`). | Corre `dotnet test` o `dotnet run --project api/USystem.Api` **desde la raíz**. |
| `npm ERR! enoent ... package.json` | Corriste `npm` fuera de `desktop`. | `cd desktop` y vuelve a correrlo. |
| `dotnet`, `node` o `git` "no se reconoce como nombre de un cmdlet" | No está instalado, o la terminal se abrió antes de instalarlo. | Instálalo (paso 1), **cierra y abre la terminal**. |
| `You must install or update .NET to run this application` / falta `Microsoft.NETCore.App 8.0` | Tienes otra versión de .NET pero no la 8. | Instala el **.NET 8 SDK** (paso 1). |
| La API se cierra al arrancar con un error de conexión, o `/health` dice `Unhealthy` | El `.env` no tiene bien la cadena de MySQL. | Revisa `ConnectionStrings__DefaultConnection` en el `.env` de la raíz (paso 3). |
| Error sobre `Jwt` o `SecretKey` | Falta `Jwt__SecretKey` o tiene menos de 32 caracteres. | Genera uno nuevo (paso 3). |
| `Port 5173 is already in use` | Ya tienes otro `npm run dev` abierto. | Cierra la otra terminal o presiona `Ctrl + C` en ella. |
| `address already in use ... 5000` | Ya tienes otra API corriendo. | Cierra la otra terminal de la API. |
| La app abre pero el login dice que no se puede conectar | La API no está corriendo. | Revisa que la terminal del paso 4 siga abierta y que <http://localhost:5000/health> responda. |
| `git pull` dice que tus cambios se sobrescribirían | Tienes cambios sin guardar que chocan con los nuevos. | Haz commit en tu rama, o guárdalos temporalmente con `git stash`, haz `git pull` y luego `git stash pop`. |

Si nada de esto lo resuelve, copia el error **completo** (no solo la última línea) y mándaselo al equipo.

---

Más detalle técnico: [`QuickStart.md`](QuickStart.md) y
[`../Operations/Environments.md`](../Operations/Environments.md).
