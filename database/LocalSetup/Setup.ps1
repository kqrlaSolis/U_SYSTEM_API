<#
.SYNOPSIS
  Crea desde cero la base de datos USystem en un MySQL 8 LOCAL, con datos semilla y datos demo.

.DESCRIPTION
  1. Busca mysql.exe (PATH, instalación de MySQL Server, XAMPP o Laragon).
  2. Borra la base USystem si ya existe (pide confirmación salvo con -Force).
  3. Aplica en orden los scripts de database/ (00-06 y 99_SeedData) y los de esta carpeta
     (10_AdminUser y 20_DemoData).
  4. Crea el usuario de aplicación (USystemApp) con permisos mínimos sobre USystem.
  5. Verifica el resultado e imprime la cadena de conexión para el .env de la API.

  Nunca lo apuntes al MySQL de Railway: está pensado para un servidor local.

.EXAMPLE
  .\database\LocalSetup\Setup.ps1
  .\database\LocalSetup\Setup.ps1 -RootPassword 'MiClave' -Force -WriteEnv
  .\database\LocalSetup\Setup.ps1 -Port 3307 -NoDemoData
#>
[CmdletBinding()]
param(
    # Servidor MySQL local
    [string]$Server = '127.0.0.1',
    [int]$Port = 3306,
    [string]$RootUser = 'root',
    # Si no se pasa, se pide de forma oculta (Enter = sin contraseña)
    [string]$RootPassword,

    # Usuario con el que se conectará la API
    [string]$AppUser = 'USystemApp',
    [string]$AppPassword = 'USystemLocal#2026',

    # Solo esquema, catálogos y administrador (sin alumnos, grupos, pagos, etc.)
    [switch]$NoDemoData,
    # No pregunta antes de borrar la base existente
    [switch]$Force,
    # Escribe la cadena de conexión (y un Jwt__SecretKey si falta) en el .env de la raíz del repo
    [switch]$WriteEnv,
    # Ruta explícita a mysql.exe si no se encuentra sola
    [string]$MySqlPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$DatabaseName = 'USystem'
$SetupDirectory = $PSScriptRoot
$DatabaseDirectory = Split-Path -Parent $SetupDirectory
$RepositoryRoot = Split-Path -Parent $DatabaseDirectory

function Write-Step([string]$Message) {
    Write-Host ''
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Write-Ok([string]$Message) {
    Write-Host "    OK  $Message" -ForegroundColor Green
}

function Stop-Setup([string]$Message) {
    Write-Host ''
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit 1
}

# Busca el cliente mysql en las ubicaciones habituales de Windows
function Find-MySqlClient {
    if ($MySqlPath) {
        if (Test-Path $MySqlPath -PathType Leaf) { return (Resolve-Path $MySqlPath).Path }
        $Candidate = Join-Path $MySqlPath 'mysql.exe'
        if (Test-Path $Candidate) { return $Candidate }
        Stop-Setup "No existe mysql.exe en '$MySqlPath'."
    }

    $FromPath = Get-Command mysql -ErrorAction SilentlyContinue
    if ($FromPath) { return $FromPath.Source }

    $Patterns = @(
        "$env:ProgramFiles\MySQL\MySQL Server *\bin\mysql.exe",
        "${env:ProgramFiles(x86)}\MySQL\MySQL Server *\bin\mysql.exe",
        "$env:ProgramFiles\MySQL\MySQL Workbench *\mysql.exe",
        'C:\xampp\mysql\bin\mysql.exe',
        'C:\laragon\bin\mysql\*\bin\mysql.exe'
    )
    foreach ($Pattern in $Patterns) {
        $Found = Get-ChildItem -Path $Pattern -ErrorAction SilentlyContinue |
            Sort-Object FullName -Descending |
            Select-Object -First 1
        if ($Found) { return $Found.FullName }
    }

    Stop-Setup ("No se encontró mysql.exe. Instala MySQL 8 (https://dev.mysql.com/downloads/installer/), " +
        "agrega su carpeta bin al PATH o pasa -MySqlPath 'C:\ruta\a\mysql.exe'. " +
        'Alternativa sin instalar MySQL: docker compose (ver README.md de esta carpeta).')
}

# Escapa un valor para el archivo de opciones de MySQL ([client] password="...")
function ConvertTo-OptionValue([string]$Value) {
    return '"' + ($Value -replace '\\', '\\' -replace '"', '\"') + '"'
}

# Escapa un literal de cadena SQL entre comillas simples
function ConvertTo-SqlLiteral([string]$Value) {
    return "'" + ($Value -replace '\\', '\\' -replace "'", "''") + "'"
}

function Write-Utf8File([string]$Path, [string]$Content) {
    [System.IO.File]::WriteAllText($Path, $Content, (New-Object System.Text.UTF8Encoding $false))
}

# Ejecuta una consulta y devuelve la salida sin encabezados
function Invoke-MySqlQuery([string]$Query) {
    # En Windows PowerShell 5.1 una línea en stderr con ErrorAction Stop aborta: se revisa el código de salida
    $PreviousPreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $Output = & $MySqlClient "--defaults-extra-file=$OptionFile" --batch --skip-column-names -e $Query 2>&1
    }
    finally {
        $ErrorActionPreference = $PreviousPreference
    }
    if ($LASTEXITCODE -ne 0) {
        throw ($Output | Out-String).Trim()
    }
    return ($Output | Out-String).Trim()
}

# Ejecuta un archivo .sql pasándolo por stdin byte a byte (sin recodificar acentos)
function Invoke-MySqlFile([string]$FilePath) {
    $Arguments = "--defaults-extra-file=`"$OptionFile`" --batch"
    $Process = Start-Process -FilePath $MySqlClient -ArgumentList $Arguments `
        -RedirectStandardInput $FilePath -NoNewWindow -Wait -PassThru
    if ($Process.ExitCode -ne 0) {
        Stop-Setup "Falló el script $(Split-Path -Leaf $FilePath) (código $($Process.ExitCode)). Revisa el mensaje de MySQL de arriba."
    }
}

Write-Host ''
Write-Host 'U SYSTEM | Base de datos local' -ForegroundColor White
Write-Host '------------------------------' -ForegroundColor White

if ($Server -match 'rlwy\.net|railway' ) {
    Stop-Setup 'Este script es solo para un MySQL local. No lo apuntes a Railway.'
}

$Scripts = @(
    (Join-Path $DatabaseDirectory '00_CreateDatabase.sql'),
    (Join-Path $DatabaseDirectory '01_Security.sql'),
    (Join-Path $DatabaseDirectory '02_AcademicCatalogs.sql'),
    (Join-Path $DatabaseDirectory '03_AcademicOperations.sql'),
    (Join-Path $DatabaseDirectory '04_AcademicTracking.sql'),
    (Join-Path $DatabaseDirectory '05_Finance.sql'),
    (Join-Path $DatabaseDirectory '06_Extras.sql'),
    (Join-Path $DatabaseDirectory '99_SeedData.sql'),
    (Join-Path $SetupDirectory '10_AdminUser.sql')
)
if (-not $NoDemoData) {
    $Scripts += (Join-Path $SetupDirectory '20_DemoData.sql')
}
foreach ($Script in $Scripts) {
    if (-not (Test-Path $Script)) { Stop-Setup "Falta el script $Script" }
}

Write-Step 'Buscando el cliente mysql'
$MySqlClient = Find-MySqlClient
Write-Ok $MySqlClient

if (-not $PSBoundParameters.ContainsKey('RootPassword')) {
    $Secure = Read-Host "Contraseña de '$RootUser' en $Server`:$Port (Enter si no tiene)" -AsSecureString
    $Pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Secure)
    try { $RootPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($Pointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($Pointer) }
}

# Archivo de opciones temporal: evita pasar la contraseña por la línea de comandos
$OptionFile = Join-Path ([System.IO.Path]::GetTempPath()) ("usystem-" + [guid]::NewGuid().ToString('N') + '.cnf')
$OptionContent = @(
    '[client]',
    "user=$(ConvertTo-OptionValue $RootUser)",
    "password=$(ConvertTo-OptionValue $RootPassword)",
    "host=$Server",
    "port=$Port",
    'protocol=TCP',
    'default-character-set=utf8mb4'
) -join "`n"
Write-Utf8File $OptionFile ($OptionContent + "`n")

$AppUserFile = $null
try {
    Write-Step "Conectando a MySQL en $Server`:$Port"
    try {
        $Version = Invoke-MySqlQuery 'SELECT VERSION()'
    }
    catch {
        Stop-Setup ("No se pudo conectar: $($_.Exception.Message)`n" +
            '       Revisa que el servicio MySQL esté corriendo (services.msc -> MySQL80), el puerto y la contraseña de root.')
    }
    Write-Ok "MySQL $Version"

    if ($Version -match 'MariaDB') {
        Stop-Setup 'Se detectó MariaDB. Los scripts requieren MySQL 8.0 o superior (usan utf8mb4_0900_ai_ci).'
    }
    $Major = [int]($Version.Split('.')[0])
    if ($Major -lt 8) {
        Stop-Setup "Se requiere MySQL 8.0 o superior (detectado $Version)."
    }

    $Exists = Invoke-MySqlQuery "SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = '$DatabaseName'"
    if ($Exists -ne '0') {
        Write-Host ''
        Write-Host "    La base '$DatabaseName' ya existe en este servidor y se BORRARÁ por completo." -ForegroundColor Yellow
        if (-not $Force) {
            $Answer = Read-Host "    Escribe SI para continuar"
            if ($Answer -ne 'SI') { Stop-Setup 'Cancelado por el usuario. No se modificó nada.' }
        }
        Write-Step "Borrando la base $DatabaseName"
        Invoke-MySqlQuery "DROP DATABASE $DatabaseName" | Out-Null
        Write-Ok 'Base anterior eliminada'
    }

    Write-Step 'Aplicando scripts'
    foreach ($Script in $Scripts) {
        $Started = Get-Date
        Invoke-MySqlFile $Script
        $Elapsed = [math]::Round(((Get-Date) - $Started).TotalSeconds, 1)
        Write-Ok "$(Split-Path -Leaf $Script) ($Elapsed s)"
    }

    if ($AppUser -ne $RootUser) {
        Write-Step "Creando el usuario de aplicación $AppUser"
        $User = ConvertTo-SqlLiteral $AppUser
        $Password = ConvertTo-SqlLiteral $AppPassword
        $AppUserSql = @"
CREATE USER IF NOT EXISTS $User@'localhost' IDENTIFIED BY $Password;
CREATE USER IF NOT EXISTS $User@'%' IDENTIFIED BY $Password;
ALTER USER $User@'localhost' IDENTIFIED BY $Password;
ALTER USER $User@'%' IDENTIFIED BY $Password;
GRANT SELECT, INSERT, UPDATE, DELETE ON $DatabaseName.* TO $User@'localhost';
GRANT SELECT, INSERT, UPDATE, DELETE ON $DatabaseName.* TO $User@'%';
FLUSH PRIVILEGES;
"@
        $AppUserFile = Join-Path ([System.IO.Path]::GetTempPath()) ("usystem-appuser-" + [guid]::NewGuid().ToString('N') + '.sql')
        Write-Utf8File $AppUserFile $AppUserSql
        Invoke-MySqlFile $AppUserFile
        Write-Ok "$AppUser con SELECT, INSERT, UPDATE, DELETE sobre $DatabaseName"
    }

    Write-Step 'Verificando'
    $Summary = Invoke-MySqlQuery @"
SELECT CONCAT_WS('|',
  (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '$DatabaseName' AND table_type = 'BASE TABLE'),
  (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = '$DatabaseName'),
  (SELECT COUNT(*) FROM $DatabaseName.Permissions),
  (SELECT COUNT(*) FROM $DatabaseName.Users),
  (SELECT COUNT(*) FROM $DatabaseName.Students),
  (SELECT COUNT(*) FROM $DatabaseName.ClassGroups),
  (SELECT COUNT(*) FROM $DatabaseName.Grades),
  (SELECT COUNT(*) FROM $DatabaseName.Charges),
  (SELECT COUNT(*) FROM $DatabaseName.Payments),
  (SELECT COUNT(*) FROM $DatabaseName.TranscriptView),
  (SELECT COUNT(*) FROM $DatabaseName.AcademicProgressView))
"@
    $Values = $Summary.Split('|')
    if ([int]$Values[0] -ne 37 -or [int]$Values[1] -ne 2 -or [int]$Values[2] -ne 78) {
        Stop-Setup "La base quedó incompleta: $($Values[0]) tablas (esperadas 37), $($Values[1]) vistas (esperadas 2), $($Values[2]) permisos (esperados 78)."
    }
    Write-Ok "$($Values[0]) tablas, $($Values[1]) vistas, $($Values[2]) permisos"
    Write-Ok "$($Values[3]) usuarios, $($Values[4]) alumnos, $($Values[5]) grupos, $($Values[6]) calificaciones"
    Write-Ok "$($Values[7]) cobros, $($Values[8]) pagos, $($Values[9]) filas de kardex, $($Values[10]) alumnos con avance"

    $ConnectionUser = if ($AppUser -ne $RootUser) { $AppUser } else { $RootUser }
    $ConnectionPassword = if ($AppUser -ne $RootUser) { $AppPassword } else { $RootPassword }
    $ConnectionString = "Server=$Server;Port=$Port;Database=$DatabaseName;User=$ConnectionUser;Password=$ConnectionPassword;SslMode=Preferred;AllowPublicKeyRetrieval=True;"

    if ($WriteEnv) {
        Write-Step 'Actualizando el .env de la raíz del repositorio'
        $EnvFile = Join-Path $RepositoryRoot '.env'
        $EnvExample = Join-Path $RepositoryRoot '.env.example'
        if (Test-Path $EnvFile) {
            $Backup = "$EnvFile.bak-$(Get-Date -Format 'yyyyMMddHHmmss')"
            Copy-Item $EnvFile $Backup
            Write-Ok "Respaldo: $(Split-Path -Leaf $Backup)"
        }
        elseif (Test-Path $EnvExample) {
            Copy-Item $EnvExample $EnvFile
            Write-Ok '.env creado a partir de .env.example'
        }
        else {
            Write-Utf8File $EnvFile ''
        }

        $Lines = [System.Collections.Generic.List[string]]::new()
        foreach ($Line in [System.IO.File]::ReadAllLines($EnvFile)) { $Lines.Add($Line) }

        $Replaced = $false
        for ($Index = 0; $Index -lt $Lines.Count; $Index++) {
            if ($Lines[$Index] -match '^\s*ConnectionStrings__DefaultConnection\s*=') {
                $Lines[$Index] = "ConnectionStrings__DefaultConnection=$ConnectionString"
                $Replaced = $true
            }
        }
        if (-not $Replaced) { $Lines.Add("ConnectionStrings__DefaultConnection=$ConnectionString") }
        Write-Ok 'ConnectionStrings__DefaultConnection apunta a la base local'

        # La API no arranca sin una clave JWT de al menos 32 caracteres
        $JwtIndex = -1
        for ($Index = 0; $Index -lt $Lines.Count; $Index++) {
            if ($Lines[$Index] -match '^\s*Jwt__SecretKey\s*=') { $JwtIndex = $Index }
        }
        $NeedsJwt = $JwtIndex -lt 0 -or ($Lines[$JwtIndex] -replace '^\s*Jwt__SecretKey\s*=\s*', '').Trim().Length -lt 32
        if ($NeedsJwt) {
            $Bytes = New-Object byte[] 48
            [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($Bytes)
            $JwtLine = 'Jwt__SecretKey=' + [Convert]::ToBase64String($Bytes)
            if ($JwtIndex -ge 0) { $Lines[$JwtIndex] = $JwtLine } else { $Lines.Add($JwtLine) }
            Write-Ok 'Jwt__SecretKey generado (no había uno válido)'
        }

        Write-Utf8File $EnvFile (($Lines -join "`n") + "`n")
    }

    Write-Host ''
    Write-Host 'Listo. La base USystem local está creada.' -ForegroundColor Green
    Write-Host ''
    if (-not $WriteEnv) {
        Write-Host 'Pon esta línea en el .env de la raíz del repo (o vuelve a correr con -WriteEnv):' -ForegroundColor White
        Write-Host "  ConnectionStrings__DefaultConnection=$ConnectionString" -ForegroundColor Yellow
        Write-Host ''
    }
    Write-Host 'Inicio de sesión en la app:' -ForegroundColor White
    Write-Host '  Administrator / USystem#2026   (admin@usystem.local)'
    if (-not $NoDemoData) {
        Write-Host '  Registrar, Finance, Coordinator, TeacherRuiz, TeacherSoto, ReadOnly / USystem#2026'
    }
    Write-Host ''
    Write-Host 'Siguiente paso:  dotnet run --project api/USystem.Api   ->  http://localhost:5000/health' -ForegroundColor White
}
finally {
    Remove-Item $OptionFile -Force -ErrorAction SilentlyContinue
    if ($AppUserFile) { Remove-Item $AppUserFile -Force -ErrorAction SilentlyContinue }
}
