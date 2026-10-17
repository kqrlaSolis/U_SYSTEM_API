-- =====================================================================
-- U SYSTEM | Creación de la base de datos
-- Motor: MySQL 8.x | Regla del proyecto: solo los comentarios van en español
-- =====================================================================
CREATE DATABASE IF NOT EXISTS USystem
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

USE USystem;

-- Usuario de aplicación con privilegios mínimos (cambiar la contraseña antes de ejecutar)
-- CREATE USER 'USystemApp'@'localhost' IDENTIFIED BY 'CHANGE_ME';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON USystem.* TO 'USystemApp'@'localhost';
-- FLUSH PRIVILEGES;
