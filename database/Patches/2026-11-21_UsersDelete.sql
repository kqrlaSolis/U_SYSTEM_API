-- =====================================================================
-- U SYSTEM | Parche: permiso Users.Delete (baja lógica de usuarios)
-- Para bases ya creadas antes de este cambio; las nuevas lo traen en 99_SeedData.sql.
-- Se puede ejecutar más de una vez sin duplicar datos.
-- =====================================================================
USE USystem;

INSERT IGNORE INTO Permissions (Id, Code, Module, Description) VALUES
  (79, 'Users.Delete', 'Users', 'Delete users (soft delete)');

-- Solo Administrator recibe el permiso; los demás roles se ajustan desde la pantalla de roles
INSERT IGNORE INTO RolePermissions (RoleId, PermissionId)
SELECT Roles.Id, Permissions.Id
FROM Roles
JOIN Permissions ON Permissions.Code = 'Users.Delete'
WHERE Roles.Name = 'Administrator';
