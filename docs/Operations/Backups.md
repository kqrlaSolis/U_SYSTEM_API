# Respaldos

## Base de datos

- **Frecuencia mínima:** diario (completo) más binlog continuo para recuperación a un punto en el tiempo.
- **Retención sugerida:** 7 diarios, 4 semanales y 6 mensuales.
- **Ubicación:** fuera del servidor de base de datos (otro disco o nube cifrada).
- **Prueba de restauración:** al menos una vez por cuatrimestre.
- **Antes de cada cierre de periodo:** respaldo manual adicional.

## Archivos (S3)

- Activar **versionado** del bucket.
- Ciclo de vida: conservar versiones 90 días y mover a almacenamiento frío a los 30 días.
- El respaldo de la base de datos y el del bucket se coordinan para evitar referencias huérfanas.
- Probar la restauración de un documento desde una versión anterior.

## Restauración de la base de datos

```bash
gunzip -c USystem_YYYYMMDD_HHMM.sql.gz | mysql -u root -p USystem
```

## Automatización

Script de ejemplo: [`Scripts/BackupMysql.sh`](Scripts/BackupMysql.sh).

```cron
# Respaldo diario a las 02:00
0 2 * * * /opt/usystem/BackupMysql.sh >> /var/log/usystem-backup.log 2>&1
```

## Verificación del respaldo

- [ ] El archivo se generó y tiene un tamaño coherente.
- [ ] La restauración en un ambiente de prueba funciona.
- [ ] El respaldo está cifrado y fuera del servidor de base de datos.
- [ ] El versionado del bucket está activo.
- [ ] Se registró la fecha y el responsable en la bitácora de operación.
