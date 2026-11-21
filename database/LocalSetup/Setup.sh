#!/usr/bin/env bash
# =====================================================================
# U SYSTEM | Crea desde cero la base USystem en un MySQL 8 LOCAL (macOS, Linux o Git Bash).
# Equivalente a Setup.ps1. Nunca lo apuntes al MySQL de Railway.
#
# Uso:
#   ./database/LocalSetup/Setup.sh                       # pide la contraseña de root
#   ./database/LocalSetup/Setup.sh --password 'MiClave' --force --write-env
#   ./database/LocalSetup/Setup.sh --port 3307 --no-demo
#
# Opciones:
#   --host H            Servidor (127.0.0.1)
#   --port P            Puerto (3306)
#   --user U            Usuario administrador de MySQL (root)
#   --password P        Contraseña del usuario administrador (si no, se pide; o usa MYSQL_ROOT_PASSWORD)
#   --app-user U        Usuario de la API (USystemApp)
#   --app-password P    Contraseña del usuario de la API (USystemLocal#2026)
#   --no-demo           Solo esquema, catálogos y administrador
#   --force             No pregunta antes de borrar la base existente
#   --write-env         Escribe la cadena de conexión (y Jwt__SecretKey si falta) en el .env del repo
#   --mysql RUTA        Ruta al cliente mysql
# =====================================================================
set -euo pipefail

Server="127.0.0.1"
Port="3306"
RootUser="root"
RootPassword="${MYSQL_ROOT_PASSWORD-}"
PasswordGiven="${MYSQL_ROOT_PASSWORD+yes}"
AppUser="USystemApp"
AppPassword="USystemLocal#2026"
NoDemo="no"
Force="no"
WriteEnv="no"
MySqlClient=""
DatabaseName="USystem"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --host) Server="$2"; shift 2 ;;
    --port) Port="$2"; shift 2 ;;
    --user) RootUser="$2"; shift 2 ;;
    --password) RootPassword="$2"; PasswordGiven="yes"; shift 2 ;;
    --app-user) AppUser="$2"; shift 2 ;;
    --app-password) AppPassword="$2"; shift 2 ;;
    --no-demo) NoDemo="yes"; shift ;;
    --force) Force="yes"; shift ;;
    --write-env) WriteEnv="yes"; shift ;;
    --mysql) MySqlClient="$2"; shift 2 ;;
    -h|--help) sed -n '2,25p' "$0"; exit 0 ;;
    *) echo "Opción desconocida: $1" >&2; exit 1 ;;
  esac
done

SetupDirectory="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DatabaseDirectory="$(dirname "$SetupDirectory")"
RepositoryRoot="$(dirname "$DatabaseDirectory")"

Step() { printf '\n\033[36m==> %s\033[0m\n' "$1"; }
Ok() { printf '    \033[32mOK\033[0m  %s\n' "$1"; }
Fail() { printf '\n\033[31mERROR: %s\033[0m\n' "$1" >&2; exit 1; }

printf '\nU SYSTEM | Base de datos local\n------------------------------\n'

if [[ "$Server" =~ rlwy\.net|railway ]]; then
  Fail "Este script es solo para un MySQL local. No lo apuntes a Railway."
fi

Scripts=(
  "$DatabaseDirectory/00_CreateDatabase.sql"
  "$DatabaseDirectory/01_Security.sql"
  "$DatabaseDirectory/02_AcademicCatalogs.sql"
  "$DatabaseDirectory/03_AcademicOperations.sql"
  "$DatabaseDirectory/04_AcademicTracking.sql"
  "$DatabaseDirectory/05_Finance.sql"
  "$DatabaseDirectory/06_Extras.sql"
  "$DatabaseDirectory/99_SeedData.sql"
  "$SetupDirectory/10_AdminUser.sql"
)
[[ "$NoDemo" == "yes" ]] || Scripts+=("$SetupDirectory/20_DemoData.sql")
for Script in "${Scripts[@]}"; do
  [[ -f "$Script" ]] || Fail "Falta el script $Script"
done

Step "Buscando el cliente mysql"
if [[ -z "$MySqlClient" ]]; then
  if command -v mysql >/dev/null 2>&1; then
    MySqlClient="$(command -v mysql)"
  else
    for Candidate in /opt/homebrew/bin/mysql /usr/local/bin/mysql /usr/local/mysql/bin/mysql \
                     /opt/homebrew/opt/mysql-client/bin/mysql /usr/local/opt/mysql-client/bin/mysql \
                     "/c/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe" "/c/Program Files/MySQL/MySQL Server 8.0/bin/mysql.exe"; do
      if [[ -x "$Candidate" ]]; then MySqlClient="$Candidate"; break; fi
    done
  fi
fi
[[ -n "$MySqlClient" ]] || Fail "No se encontró el cliente mysql. Instálalo (brew install mysql-client / apt install mysql-client) o usa --mysql RUTA. Alternativa: docker compose (ver README.md)."
Ok "$MySqlClient"

if [[ "$PasswordGiven" != "yes" ]]; then
  read -r -s -p "Contraseña de '$RootUser' en $Server:$Port (Enter si no tiene): " RootPassword
  echo
fi

# Archivo de opciones temporal: evita pasar la contraseña por la línea de comandos
OptionFile="$(mktemp "${TMPDIR:-/tmp}/usystem-XXXXXX")"
AppUserFile="$(mktemp "${TMPDIR:-/tmp}/usystem-appuser-XXXXXX")"
trap 'rm -f "$OptionFile" "$AppUserFile"' EXIT
chmod 600 "$OptionFile" "$AppUserFile"

OptionValue() { local Value="${1//\\/\\\\}"; Value="${Value//\"/\\\"}"; printf '"%s"' "$Value"; }
SqlLiteral() { local Value="${1//\\/\\\\}"; Value="${Value//\'/\'\'}"; printf "'%s'" "$Value"; }

{
  echo "[client]"
  echo "user=$(OptionValue "$RootUser")"
  echo "password=$(OptionValue "$RootPassword")"
  echo "host=$Server"
  echo "port=$Port"
  echo "protocol=TCP"
  echo "default-character-set=utf8mb4"
} > "$OptionFile"

Query() { "$MySqlClient" --defaults-extra-file="$OptionFile" --batch --skip-column-names -e "$1"; }
RunFile() { "$MySqlClient" --defaults-extra-file="$OptionFile" --batch < "$1" || Fail "Falló el script $(basename "$1"). Revisa el mensaje de MySQL de arriba."; }

Step "Conectando a MySQL en $Server:$Port"
Version="$(Query 'SELECT VERSION()' 2>&1)" || Fail "No se pudo conectar: $Version"
Ok "MySQL $Version"
[[ "$Version" != *MariaDB* ]] || Fail "Se detectó MariaDB. Los scripts requieren MySQL 8.0 o superior."
[[ "${Version%%.*}" -ge 8 ]] || Fail "Se requiere MySQL 8.0 o superior (detectado $Version)."

Exists="$(Query "SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = '$DatabaseName'")"
if [[ "$Exists" != "0" ]]; then
  printf "\n    \033[33mLa base '%s' ya existe en este servidor y se BORRARÁ por completo.\033[0m\n" "$DatabaseName"
  if [[ "$Force" != "yes" ]]; then
    read -r -p "    Escribe SI para continuar: " Answer
    [[ "$Answer" == "SI" ]] || Fail "Cancelado por el usuario. No se modificó nada."
  fi
  Step "Borrando la base $DatabaseName"
  Query "DROP DATABASE $DatabaseName"
  Ok "Base anterior eliminada"
fi

Step "Aplicando scripts"
for Script in "${Scripts[@]}"; do
  RunFile "$Script"
  Ok "$(basename "$Script")"
done

if [[ "$AppUser" != "$RootUser" ]]; then
  Step "Creando el usuario de aplicación $AppUser"
  User="$(SqlLiteral "$AppUser")"
  Password="$(SqlLiteral "$AppPassword")"
  cat > "$AppUserFile" <<EOF
CREATE USER IF NOT EXISTS $User@'localhost' IDENTIFIED BY $Password;
CREATE USER IF NOT EXISTS $User@'%' IDENTIFIED BY $Password;
ALTER USER $User@'localhost' IDENTIFIED BY $Password;
ALTER USER $User@'%' IDENTIFIED BY $Password;
GRANT SELECT, INSERT, UPDATE, DELETE ON $DatabaseName.* TO $User@'localhost';
GRANT SELECT, INSERT, UPDATE, DELETE ON $DatabaseName.* TO $User@'%';
FLUSH PRIVILEGES;
EOF
  RunFile "$AppUserFile"
  Ok "$AppUser con SELECT, INSERT, UPDATE, DELETE sobre $DatabaseName"
  ConnectionUser="$AppUser"; ConnectionPassword="$AppPassword"
else
  ConnectionUser="$RootUser"; ConnectionPassword="$RootPassword"
fi

Step "Verificando"
Summary="$(Query "SELECT CONCAT_WS(' ',
  (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '$DatabaseName' AND table_type = 'BASE TABLE'),
  (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = '$DatabaseName'),
  (SELECT COUNT(*) FROM $DatabaseName.Permissions),
  (SELECT COUNT(*) FROM $DatabaseName.Users),
  (SELECT COUNT(*) FROM $DatabaseName.Students),
  (SELECT COUNT(*) FROM $DatabaseName.ClassGroups),
  (SELECT COUNT(*) FROM $DatabaseName.Grades),
  (SELECT COUNT(*) FROM $DatabaseName.Charges),
  (SELECT COUNT(*) FROM $DatabaseName.Payments))")"
read -r Tables Views Permissions Users Students Groups Grades Charges Payments <<< "$Summary"
if [[ "$Tables" != "37" || "$Views" != "2" || "$Permissions" != "78" ]]; then
  Fail "La base quedó incompleta: $Tables tablas (37), $Views vistas (2), $Permissions permisos (78)."
fi
Ok "$Tables tablas, $Views vistas, $Permissions permisos"
Ok "$Users usuarios, $Students alumnos, $Groups grupos, $Grades calificaciones, $Charges cobros, $Payments pagos"

ConnectionString="Server=$Server;Port=$Port;Database=$DatabaseName;User=$ConnectionUser;Password=$ConnectionPassword;SslMode=Preferred;AllowPublicKeyRetrieval=True;"

if [[ "$WriteEnv" == "yes" ]]; then
  Step "Actualizando el .env de la raíz del repositorio"
  EnvFile="$RepositoryRoot/.env"
  if [[ -f "$EnvFile" ]]; then
    Backup="$EnvFile.bak-$(date +%Y%m%d%H%M%S)"
    cp "$EnvFile" "$Backup"
    Ok "Respaldo: $(basename "$Backup")"
  elif [[ -f "$RepositoryRoot/.env.example" ]]; then
    cp "$RepositoryRoot/.env.example" "$EnvFile"
    Ok ".env creado a partir de .env.example"
  else
    : > "$EnvFile"
  fi

  TemporaryEnv="$(mktemp "${TMPDIR:-/tmp}/usystem-env-XXXXXX")"
  ConnectionLine="ConnectionStrings__DefaultConnection=$ConnectionString" \
    awk 'BEGIN { Line = ENVIRON["ConnectionLine"] }
         /^[[:space:]]*ConnectionStrings__DefaultConnection[[:space:]]*=/ { print Line; Found = 1; next }
         { print }
         END { if (!Found) print Line }' "$EnvFile" > "$TemporaryEnv"
  Ok "ConnectionStrings__DefaultConnection apunta a la base local"

  # La API no arranca sin una clave JWT de al menos 32 caracteres
  CurrentJwt="$(grep -E '^[[:space:]]*Jwt__SecretKey[[:space:]]*=' "$TemporaryEnv" | tail -n 1 | sed -E 's/^[[:space:]]*Jwt__SecretKey[[:space:]]*=[[:space:]]*//' || true)"
  if [[ ${#CurrentJwt} -lt 32 ]]; then
    NewJwt="$(head -c 48 /dev/urandom | base64 | tr -d '\n')"
    JwtLine="Jwt__SecretKey=$NewJwt" awk 'BEGIN { Line = ENVIRON["JwtLine"] }
         /^[[:space:]]*Jwt__SecretKey[[:space:]]*=/ { print Line; Found = 1; next }
         { print }
         END { if (!Found) print Line }' "$TemporaryEnv" > "$TemporaryEnv.jwt"
    mv "$TemporaryEnv.jwt" "$TemporaryEnv"
    Ok "Jwt__SecretKey generado (no había uno válido)"
  fi
  mv "$TemporaryEnv" "$EnvFile"
fi

printf '\n\033[32mListo. La base USystem local está creada.\033[0m\n\n'
if [[ "$WriteEnv" != "yes" ]]; then
  echo "Pon esta línea en el .env de la raíz del repo (o vuelve a correr con --write-env):"
  printf '  \033[33mConnectionStrings__DefaultConnection=%s\033[0m\n\n' "$ConnectionString"
fi
echo "Inicio de sesión en la app:"
echo "  Administrator / USystem#2026   (admin@usystem.local)"
[[ "$NoDemo" == "yes" ]] || echo "  Registrar, Finance, Coordinator, TeacherRuiz, TeacherSoto, ReadOnly / USystem#2026"
echo
echo "Siguiente paso:  dotnet run --project api/USystem.Api   ->  http://localhost:5000/health"
