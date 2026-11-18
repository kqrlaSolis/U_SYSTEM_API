#!/usr/bin/env bash
# Respaldo diario de la base de datos USystem.
# Uso: ./BackupMysql.sh   (configurar las variables por entorno)
set -euo pipefail

DatabaseName="${DatabaseName:-USystem}"
BackupDirectory="${BackupDirectory:-/var/backups/usystem}"
RetentionDays="${RetentionDays:-14}"
Timestamp="$(date +%Y%m%d_%H%M)"

mkdir -p "$BackupDirectory"

# --single-transaction evita bloquear tablas InnoDB durante el respaldo
mysqldump --single-transaction --routines --triggers \
  --defaults-extra-file="$HOME/.usystem.cnf" \
  "$DatabaseName" | gzip > "$BackupDirectory/${DatabaseName}_${Timestamp}.sql.gz"

# Elimina respaldos antiguos
find "$BackupDirectory" -name "${DatabaseName}_*.sql.gz" -mtime +"$RetentionDays" -delete

echo "Backup completed: ${DatabaseName}_${Timestamp}.sql.gz"
