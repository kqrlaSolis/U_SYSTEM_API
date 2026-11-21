-- =====================================================================
-- U SYSTEM | Usuario de aplicación para la base LOCAL creada con Docker
-- Setup.ps1 y Setup.sh generan su propia versión con la contraseña que elijas;
-- este archivo solo lo usa docker-compose.yml (contraseña fija de desarrollo).
-- =====================================================================
CREATE USER IF NOT EXISTS 'USystemApp'@'%' IDENTIFIED BY 'USystemLocal#2026';
GRANT SELECT, INSERT, UPDATE, DELETE ON USystem.* TO 'USystemApp'@'%';
FLUSH PRIVILEGES;
