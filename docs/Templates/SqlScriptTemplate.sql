-- =====================================================================
-- U SYSTEM | <Descripción del script>
-- Motor: MySQL 8.x | Convención: PascalCase en inglés
-- =====================================================================
USE USystem;

CREATE TABLE IF NOT EXISTS TableName (
  Id BIGINT NOT NULL AUTO_INCREMENT,
  Name VARCHAR(100) NOT NULL,
  IsActive TINYINT(1) NOT NULL DEFAULT 1,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CreatedBy BIGINT NULL,
  UpdatedAt DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
  UpdatedBy BIGINT NULL,
  DeletedAt DATETIME NULL,
  PRIMARY KEY (Id),
  UNIQUE KEY UqTableNameName (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
