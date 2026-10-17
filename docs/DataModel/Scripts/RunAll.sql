-- =====================================================================
-- U SYSTEM | Ejecuta todos los scripts en orden
-- Ejecutar desde la carpeta Scripts: mysql -u root -p < RunAll.sql
-- =====================================================================
SOURCE 00_CreateDatabase.sql;
SOURCE 01_Security.sql;
SOURCE 02_AcademicCatalogs.sql;
SOURCE 03_AcademicOperations.sql;
SOURCE 04_AcademicTracking.sql;
SOURCE 05_Finance.sql;
SOURCE 06_Extras.sql;
SOURCE 99_SeedData.sql;
