-- =====================================================================
-- U SYSTEM | Administrador inicial para una base LOCAL
-- Se ejecuta después de 99_SeedData.sql (necesita el rol Administrator).
-- Usuario: Administrator (o admin@usystem.local) | Contraseña: USystem#2026
-- El hash es BCrypt con costo 12, igual que BCryptPasswordHasher de la API.
-- SOLO PARA DESARROLLO LOCAL: nunca ejecutar contra la base de Railway.
-- =====================================================================
USE USystem;

INSERT INTO Users (Id, FirstName, PaternalLastName, MaternalLastName, Email, Username, PasswordHash, Phone, IsActive, MustChangePassword)
VALUES (1, 'System', 'Administrator', NULL, 'admin@usystem.local', 'Administrator',
        '$2a$12$azcpOAusfFoXq4cqt7kZ9e1BPCKzytSkA53edCYzJMX.Qw7b9WcEi', NULL, 1, 0);

INSERT INTO UserRoles (UserId, RoleId) VALUES (1, 1);
