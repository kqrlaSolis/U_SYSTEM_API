-- =====================================================================
-- U SYSTEM | Datos de demostración para una base LOCAL
-- Requiere: 00-06, 99_SeedData.sql y 10_AdminUser.sql sobre una base vacía.
-- Contenido: personal con cada rol, 3 carreras (2 con malla completa de 9 cuatrimestres),
-- 7 periodos (2025-1 a 2027-1), 12 docentes, 27 alumnos en 3 generaciones, grupos y horarios,
-- inscripciones, calificaciones (incluye extraordinarios), becas, cobros, pagos, trámites,
-- notificaciones y material didáctico.
-- Todos los usuarios demo usan la contraseña: USystem#2026
-- "Hoy" para los datos demo es 2026-11-21 (periodo en curso: 2026-3).
-- SOLO PARA DESARROLLO LOCAL: nunca ejecutar contra la base de Railway.
-- =====================================================================
USE USystem;

SET @DemoPasswordHash = '$2a$12$azcpOAusfFoXq4cqt7kZ9e1BPCKzytSkA53edCYzJMX.Qw7b9WcEi';
SET @DemoToday = DATE('2026-11-21');

-- ---------------------------------------------------------------------
-- Personal administrativo (uno por rol)
-- ---------------------------------------------------------------------
INSERT INTO Users (Id, FirstName, PaternalLastName, MaternalLastName, Email, Username, PasswordHash, Phone, IsActive, MustChangePassword, LastLoginAt) VALUES
  (2, 'Laura', 'Méndez', 'Ríos', 'registrar@usystem.local', 'Registrar', @DemoPasswordHash, '6641000002', 1, 0, '2026-11-20 08:12:00'),
  (3, 'Jorge', 'Salinas', 'Vega', 'finance@usystem.local', 'Finance', @DemoPasswordHash, '6641000003', 1, 0, '2026-11-20 09:30:00'),
  (4, 'Patricia', 'Ochoa', 'Lara', 'coordinator@usystem.local', 'Coordinator', @DemoPasswordHash, '6641000004', 1, 0, '2026-11-19 16:45:00'),
  (5, 'Ricardo', 'Ruiz', 'Camacho', 'ricardo.ruiz@usystem.local', 'TeacherRuiz', @DemoPasswordHash, '6641000005', 1, 0, '2026-11-18 07:05:00'),
  (6, 'Mónica', 'Soto', 'Andrade', 'monica.soto@usystem.local', 'TeacherSoto', @DemoPasswordHash, '6641000006', 1, 0, '2026-11-17 10:20:00'),
  (7, 'Daniel', 'Fuentes', 'Ibarra', 'auditor@usystem.local', 'ReadOnly', @DemoPasswordHash, '6641000007', 1, 0, NULL),
  (8, 'Sergio', 'Navarro', 'Peña', 'sergio.navarro@usystem.local', 'SNavarro', @DemoPasswordHash, '6641000008', 0, 0, '2026-03-02 11:00:00');

INSERT INTO UserRoles (UserId, RoleId) VALUES
  (2, 2), (3, 3), (4, 4), (5, 5), (6, 5), (7, 6), (8, 2);

-- ---------------------------------------------------------------------
-- Carreras (Psicología queda sin plan para probar el alta de un plan desde la app)
-- ---------------------------------------------------------------------
INSERT INTO Careers (Id, Code, Name, Level, TermCount, TotalCredits, Modality, IsActive) VALUES
  (1, 'ISC', 'Ingeniería en Sistemas Computacionales', 'Bachelor', 9, 270, 'OnSite', 1),
  (2, 'LAE', 'Licenciatura en Administración de Empresas', 'Bachelor', 9, 270, 'OnSite', 1),
  (3, 'LPS', 'Licenciatura en Psicología', 'Bachelor', 9, 270, 'Hybrid', 1);

-- ---------------------------------------------------------------------
-- Materias: Inglés (compartido), ISC (101-130) y LAE (201-230)
-- Inglés: 5 créditos | Materias de carrera: 8 créditos -> 6*5 + 30*8 = 270 por plan
-- ---------------------------------------------------------------------
INSERT INTO Subjects (Id, Code, Name, Credits, TheoryHours, PracticeHours, IsActive) VALUES
  (1, 'ING101', 'Inglés I', 5, 2, 3, 1),
  (2, 'ING102', 'Inglés II', 5, 2, 3, 1),
  (3, 'ING103', 'Inglés III', 5, 2, 3, 1),
  (4, 'ING104', 'Inglés IV', 5, 2, 3, 1),
  (5, 'ING105', 'Inglés V', 5, 2, 3, 1),
  (6, 'ING106', 'Inglés VI', 5, 2, 3, 1),
  (101, 'ISC101', 'Fundamentos de Programación', 8, 3, 3, 1),
  (102, 'ISC102', 'Cálculo Diferencial', 8, 4, 1, 1),
  (103, 'ISC103', 'Matemáticas Discretas', 8, 4, 1, 1),
  (104, 'ISC104', 'Programación Orientada a Objetos', 8, 3, 3, 1),
  (105, 'ISC105', 'Cálculo Integral', 8, 4, 1, 1),
  (106, 'ISC106', 'Álgebra Lineal', 8, 4, 1, 1),
  (107, 'ISC107', 'Estructuras de Datos', 8, 3, 3, 1),
  (108, 'ISC108', 'Probabilidad y Estadística', 8, 4, 1, 1),
  (109, 'ISC109', 'Fundamentos de Bases de Datos', 8, 3, 3, 1),
  (110, 'ISC110', 'Taller de Bases de Datos', 8, 2, 4, 1),
  (111, 'ISC111', 'Sistemas Operativos', 8, 3, 2, 1),
  (112, 'ISC112', 'Arquitectura de Computadoras', 8, 3, 2, 1),
  (113, 'ISC113', 'Redes de Computadoras', 8, 3, 2, 1),
  (114, 'ISC114', 'Ingeniería de Software', 8, 3, 2, 1),
  (115, 'ISC115', 'Programación Web', 8, 2, 4, 1),
  (116, 'ISC116', 'Desarrollo de Aplicaciones Móviles', 8, 2, 4, 1),
  (117, 'ISC117', 'Inteligencia Artificial', 8, 3, 2, 1),
  (118, 'ISC118', 'Administración de Redes', 8, 2, 3, 1),
  (119, 'ISC119', 'Seguridad Informática', 8, 3, 2, 1),
  (120, 'ISC120', 'Computación en la Nube', 8, 2, 3, 1),
  (121, 'ISC121', 'Gestión de Proyectos de Software', 8, 3, 2, 1),
  (122, 'ISC122', 'Lenguajes y Autómatas', 8, 4, 1, 1),
  (123, 'ISC123', 'Ciencia de Datos', 8, 3, 2, 1),
  (124, 'ISC124', 'Sistemas Distribuidos', 8, 3, 2, 1),
  (125, 'ISC125', 'Taller de Investigación', 8, 2, 2, 1),
  (126, 'ISC126', 'Ética Profesional', 8, 3, 0, 1),
  (127, 'ISC127', 'Arquitectura de Software', 8, 3, 2, 1),
  (128, 'ISC128', 'Tópicos Avanzados de Programación', 8, 2, 4, 1),
  (129, 'ISC129', 'Residencia Profesional', 8, 0, 6, 1),
  (130, 'ISC130', 'Emprendimiento Tecnológico', 8, 3, 2, 1),
  (201, 'LAE201', 'Fundamentos de Administración', 8, 4, 1, 1),
  (202, 'LAE202', 'Contabilidad Financiera', 8, 3, 2, 1),
  (203, 'LAE203', 'Matemáticas para los Negocios', 8, 4, 1, 1),
  (204, 'LAE204', 'Proceso Administrativo', 8, 4, 1, 1),
  (205, 'LAE205', 'Contabilidad de Costos', 8, 3, 2, 1),
  (206, 'LAE206', 'Microeconomía', 8, 4, 1, 1),
  (207, 'LAE207', 'Mercadotecnia', 8, 3, 2, 1),
  (208, 'LAE208', 'Macroeconomía', 8, 4, 1, 1),
  (209, 'LAE209', 'Estadística para los Negocios', 8, 3, 2, 1),
  (210, 'LAE210', 'Derecho Mercantil', 8, 4, 0, 1),
  (211, 'LAE211', 'Comportamiento Organizacional', 8, 3, 1, 1),
  (212, 'LAE212', 'Matemáticas Financieras', 8, 3, 2, 1),
  (213, 'LAE213', 'Administración de Recursos Humanos', 8, 3, 2, 1),
  (214, 'LAE214', 'Investigación de Mercados', 8, 3, 2, 1),
  (215, 'LAE215', 'Finanzas Corporativas', 8, 3, 2, 1),
  (216, 'LAE216', 'Derecho Laboral', 8, 4, 0, 1),
  (217, 'LAE217', 'Administración de Operaciones', 8, 3, 2, 1),
  (218, 'LAE218', 'Sistemas de Información Gerencial', 8, 2, 3, 1),
  (219, 'LAE219', 'Planeación Estratégica', 8, 3, 2, 1),
  (220, 'LAE220', 'Comercio Internacional', 8, 3, 1, 1),
  (221, 'LAE221', 'Desarrollo de Emprendedores', 8, 2, 3, 1),
  (222, 'LAE222', 'Auditoría Administrativa', 8, 3, 2, 1),
  (223, 'LAE223', 'Gestión de la Calidad', 8, 3, 2, 1),
  (224, 'LAE224', 'Administración Financiera', 8, 3, 2, 1),
  (225, 'LAE225', 'Negociación y Liderazgo', 8, 2, 3, 1),
  (226, 'LAE226', 'Ética Empresarial', 8, 3, 0, 1),
  (227, 'LAE227', 'Consultoría Empresarial', 8, 2, 3, 1),
  (228, 'LAE228', 'Seminario de Tesis', 8, 2, 3, 1),
  (229, 'LAE229', 'Prácticas Profesionales', 8, 0, 6, 1),
  (230, 'LAE230', 'Responsabilidad Social Corporativa', 8, 3, 1, 1),
  (301, 'LPS301', 'Introducción a la Psicología', 8, 4, 1, 1),
  (302, 'LPS302', 'Bases Biológicas de la Conducta', 8, 4, 1, 1),
  (303, 'LPS303', 'Psicología del Desarrollo', 8, 4, 1, 1);

-- ---------------------------------------------------------------------
-- Periodos cuatrimestrales: 2025-1 a 2026-2 cerrados, 2026-3 en curso, 2027-1 planeado
-- ---------------------------------------------------------------------
INSERT INTO AcademicTerms (Id, Code, Name, AcademicYear, TermNumber, StartDate, EndDate, EnrollmentStartDate, EnrollmentEndDate,
                           ReEnrollmentStartDate, ReEnrollmentEndDate, ExamStartDate, ExamEndDate, GradeReportCloseDate, Status) VALUES
  (1, '2025-1', 'Enero - Abril 2025', 2025, 1, '2025-01-06', '2025-04-25', '2024-12-02', '2025-01-10', '2024-12-02', '2025-01-10', '2025-04-14', '2025-04-25', '2025-04-30', 'Closed'),
  (2, '2025-2', 'Mayo - Agosto 2025', 2025, 2, '2025-05-05', '2025-08-22', '2025-04-07', '2025-05-09', '2025-04-07', '2025-05-09', '2025-08-11', '2025-08-22', '2025-08-29', 'Closed'),
  (3, '2025-3', 'Septiembre - Diciembre 2025', 2025, 3, '2025-09-01', '2025-12-12', '2025-08-04', '2025-09-05', '2025-08-04', '2025-09-05', '2025-12-01', '2025-12-12', '2025-12-17', 'Closed'),
  (4, '2026-1', 'Enero - Abril 2026', 2026, 1, '2026-01-05', '2026-04-24', '2025-12-01', '2026-01-09', '2025-12-01', '2026-01-09', '2026-04-13', '2026-04-24', '2026-04-29', 'Closed'),
  (5, '2026-2', 'Mayo - Agosto 2026', 2026, 2, '2026-05-04', '2026-08-21', '2026-04-06', '2026-05-08', '2026-04-06', '2026-05-08', '2026-08-10', '2026-08-21', '2026-08-28', 'Closed'),
  (6, '2026-3', 'Septiembre - Diciembre 2026', 2026, 3, '2026-09-01', '2026-12-11', '2026-08-03', '2026-09-04', '2026-08-03', '2026-09-04', '2026-11-30', '2026-12-11', '2026-12-16', 'InProgress'),
  (7, '2027-1', 'Enero - Abril 2027', 2027, 1, '2027-01-04', '2027-04-23', '2026-11-23', '2027-01-08', '2026-11-23', '2027-01-08', '2027-04-12', '2027-04-23', '2027-04-28', 'Planned');

-- ---------------------------------------------------------------------
-- Planes de estudio vigentes y su malla (los créditos suman exactamente TotalCredits)
-- ---------------------------------------------------------------------
INSERT INTO StudyPlans (Id, CareerId, Code, Version, StartDate, EndDate, TotalCredits, IsCurrent) VALUES
  (1, 1, 'ISC-2024', '2024', '2024-01-01', NULL, 270, 1),
  (2, 2, 'LAE-2024', '2024', '2024-01-01', NULL, 270, 1);

-- Cuatrimestres 1-6: Inglés + 3 materias de carrera; 7-9: 4 materias de carrera
INSERT INTO StudyPlanSubjects (StudyPlanId, SubjectId, TermNumber, Type)
SELECT Plan.Id,
       Subject.Id,
       CASE WHEN Subject.Id - Plan.FirstSubjectId < 18 THEN (Subject.Id - Plan.FirstSubjectId) DIV 3 + 1
            ELSE 7 + (Subject.Id - Plan.FirstSubjectId - 18) DIV 4 END,
       IF(Subject.Id IN (128, 130, 230), 'Elective', 'Mandatory')
FROM (SELECT 1 AS Id, 101 AS FirstSubjectId UNION ALL SELECT 2, 201) AS Plan
JOIN Subjects AS Subject ON Subject.Id BETWEEN Plan.FirstSubjectId AND Plan.FirstSubjectId + 29
UNION ALL
SELECT Plan.Id, Subject.Id, Subject.Id, 'Mandatory'
FROM (SELECT 1 AS Id UNION ALL SELECT 2) AS Plan
JOIN Subjects AS Subject ON Subject.Id BETWEEN 1 AND 6;

INSERT INTO SubjectPrerequisites (StudyPlanId, SubjectId, PrerequisiteSubjectId) VALUES
  -- Inglés en cadena para ambos planes
  (1, 2, 1), (1, 3, 2), (1, 4, 3), (1, 5, 4), (1, 6, 5),
  (2, 2, 1), (2, 3, 2), (2, 4, 3), (2, 5, 4), (2, 6, 5),
  -- ISC
  (1, 104, 101), (1, 105, 102), (1, 107, 104), (1, 108, 105), (1, 110, 109),
  (1, 115, 107), (1, 116, 115), (1, 117, 108), (1, 118, 113), (1, 119, 118),
  (1, 123, 117), (1, 127, 114), (1, 129, 121),
  -- LAE
  (2, 204, 201), (2, 205, 202), (2, 208, 206), (2, 212, 203), (2, 214, 209),
  (2, 215, 212), (2, 224, 215), (2, 219, 204), (2, 229, 219);

-- ---------------------------------------------------------------------
-- Docentes y materias que pueden impartir
-- ---------------------------------------------------------------------
INSERT INTO Teachers (Id, EmployeeNumber, FirstName, PaternalLastName, MaternalLastName, Email, Phone, Specialty, UserId, IsActive) VALUES
  (1, 'EMP-0001', 'Ricardo', 'Ruiz', 'Camacho', 'ricardo.ruiz@usystem.local', '6642100001', 'Ingeniería de Software', 5, 1),
  (2, 'EMP-0002', 'Ana Lucía', 'Torres', 'Medina', 'ana.torres@usystem.local', '6642100002', 'Matemáticas', NULL, 1),
  (3, 'EMP-0003', 'Fernando', 'Castillo', 'Ortega', 'fernando.castillo@usystem.local', '6642100003', 'Redes y Sistemas Operativos', NULL, 1),
  (4, 'EMP-0004', 'Gabriela', 'Herrera', 'Luna', 'gabriela.herrera@usystem.local', '6642100004', 'Bases de Datos y Ciencia de Datos', NULL, 1),
  (5, 'EMP-0005', 'Mónica', 'Soto', 'Andrade', 'monica.soto@usystem.local', '6642100005', 'Administración', 6, 1),
  (6, 'EMP-0006', 'Luis Alberto', 'Moreno', 'Díaz', 'luis.moreno@usystem.local', '6642100006', 'Contaduría y Finanzas', NULL, 1),
  (7, 'EMP-0007', 'Claudia', 'Ramírez', 'Guerrero', 'claudia.ramirez@usystem.local', '6642100007', 'Economía', NULL, 1),
  (8, 'EMP-0008', 'Héctor', 'Delgado', 'Romero', 'hector.delgado@usystem.local', '6642100008', 'Derecho', NULL, 1),
  (9, 'EMP-0009', 'Jennifer', 'Collins', 'Ávila', 'jennifer.collins@usystem.local', '6642100009', 'Inglés', NULL, 1),
  (10, 'EMP-0010', 'Marco Antonio', 'Vargas', 'Rey', 'marco.vargas@usystem.local', '6642100010', 'Inglés', NULL, 1),
  (11, 'EMP-0011', 'Rosa Elena', 'Jiménez', 'Cruz', 'rosa.jimenez@usystem.local', '6642100011', 'Psicología', NULL, 1),
  (12, 'EMP-0012', 'Arturo', 'Medina', 'Solís', 'arturo.medina@usystem.local', '6642100012', 'Programación', NULL, 0);

INSERT INTO TeacherSubjects (TeacherId, SubjectId) VALUES
  (1, 101), (1, 104), (1, 107), (1, 114), (1, 115), (1, 116), (1, 121), (1, 127), (1, 128), (1, 130),
  (2, 102), (2, 103), (2, 105), (2, 106), (2, 108), (2, 122),
  (3, 111), (3, 112), (3, 113), (3, 118), (3, 119), (3, 120), (3, 124),
  (4, 109), (4, 110), (4, 117), (4, 123), (4, 125), (4, 126), (4, 129),
  (5, 201), (5, 204), (5, 211), (5, 213), (5, 217), (5, 219), (5, 221), (5, 222), (5, 223), (5, 225), (5, 227), (5, 229),
  (6, 202), (6, 203), (6, 205), (6, 212), (6, 215), (6, 224),
  (7, 206), (7, 207), (7, 208), (7, 209), (7, 214), (7, 218), (7, 220), (7, 228),
  (8, 210), (8, 216), (8, 226), (8, 230),
  (9, 1), (9, 2), (9, 3), (9, 4), (9, 5), (9, 6),
  (10, 1), (10, 2), (10, 3), (10, 4), (10, 5), (10, 6),
  (11, 301), (11, 302), (11, 303),
  (12, 101), (12, 104);

-- ---------------------------------------------------------------------
-- Alumnos: generación 2025-1 (hoy en 6.º), 2026-1 (hoy en 3.º), 2026-3 (hoy en 1.º) y aspirantes 2027-1
-- ---------------------------------------------------------------------
INSERT INTO Students (Id, StudentNumber, CareerId, StudyPlanId, EnrollmentTermId, FirstName, PaternalLastName, MaternalLastName, Curp, BirthDate,
                      Gender, Email, Phone, AddressLine, City, State, PostalCode, Status) VALUES
  (1, 'A20250001', 1, 1, 1, 'Diego', 'Hernández', 'Castro', 'HECD060314HBCRSGA4', '2006-03-14', 'M', 'diego.hernandez@alumnos.usystem.local', '6643000001', 'Av. Revolución 1201', 'Tijuana', 'Baja California', '22000', 'Active'),
  (2, 'A20250002', 1, 1, 1, 'Valeria', 'Gómez', 'Navarro', 'GONV060722MBCMVLA9', '2006-07-22', 'F', 'valeria.gomez@alumnos.usystem.local', '6643000002', 'Calle Ocampo 845', 'Tijuana', 'Baja California', '22010', 'Active'),
  (3, 'A20250003', 1, 1, 1, 'Emiliano', 'Rojas', 'Mejía', 'ROME051102HBCJJMA2', '2005-11-02', 'M', 'emiliano.rojas@alumnos.usystem.local', '6643000003', 'Blvd. Agua Caliente 4558', 'Tijuana', 'Baja California', '22420', 'Active'),
  (4, 'A20250004', 1, 1, 1, 'Sofía', 'Martínez', 'Lozano', 'MALS060130MBCRZFA7', '2006-01-30', 'F', 'sofia.martinez@alumnos.usystem.local', '6643000004', 'Calle Benito Juárez 32', 'Rosarito', 'Baja California', '22710', 'Inactive'),
  (5, 'A20250005', 2, 2, 1, 'Andrea', 'Cruz', 'Villalobos', 'CUVA060509MBCRLNA3', '2006-05-09', 'F', 'andrea.cruz@alumnos.usystem.local', '6643000005', 'Paseo de los Héroes 9350', 'Tijuana', 'Baja California', '22010', 'Active'),
  (6, 'A20250006', 2, 2, 1, 'Mateo', 'Sánchez', 'Ibarra', 'SAIM060918HBCNBTA5', '2006-09-18', 'M', 'mateo.sanchez@alumnos.usystem.local', '6643000006', 'Calle Hidalgo 210', 'Tecate', 'Baja California', '21400', 'Active'),
  (7, 'A20250007', 2, 2, 1, 'Camila', 'Flores', 'Pineda', 'FOPC051225MBCLNMA8', '2005-12-25', 'F', 'camila.flores@alumnos.usystem.local', '6643000007', 'Av. Internacional 1500', 'Tijuana', 'Baja California', '22320', 'Active'),
  (8, 'A20260001', 1, 1, 4, 'Santiago', 'López', 'Aguilar', 'LOAS070211HBCPGNA1', '2007-02-11', 'M', 'santiago.lopez@alumnos.usystem.local', '6643000008', 'Calle Segunda 1820', 'Tijuana', 'Baja California', '22000', 'Active'),
  (9, 'A20260002', 1, 1, 4, 'Regina', 'Ramírez', 'Ortiz', 'RAOR070603MBCMRGA6', '2007-06-03', 'F', 'regina.ramirez@alumnos.usystem.local', '6643000009', 'Blvd. Díaz Ordaz 12415', 'Tijuana', 'Baja California', '22410', 'Active'),
  (10, 'A20260003', 1, 1, 4, 'Leonardo', 'Torres', 'Valdez', 'TOVL070427HBCRLNA2', '2007-04-27', 'M', 'leonardo.torres@alumnos.usystem.local', '6643000010', 'Calle Mar Egeo 77', 'Ensenada', 'Baja California', '22800', 'Active'),
  (11, 'A20260004', 1, 1, 4, 'Ximena', 'Morales', 'Rivas', 'MORX070815MBCRVMA0', '2007-08-15', 'F', 'ximena.morales@alumnos.usystem.local', '6643000011', 'Av. Las Palmas 4410', 'Tijuana', 'Baja California', '22106', 'Active'),
  (12, 'A20260005', 1, 1, 4, 'Iker', 'Gutiérrez', 'Salas', 'GUSI071005HBCTLKA3', '2007-10-05', 'M', 'iker.gutierrez@alumnos.usystem.local', '6643000012', 'Calle Coahuila 920', 'Tijuana', 'Baja California', '22000', 'Dropout'),
  (13, 'A20260006', 2, 2, 4, 'Renata', 'Vázquez', 'Molina', 'VAMR070319MBCZLNA4', '2007-03-19', 'F', 'renata.vazquez@alumnos.usystem.local', '6643000013', 'Calle Lerdo 455', 'Mexicali', 'Baja California', '21100', 'Active'),
  (14, 'A20260007', 2, 2, 4, 'Sebastián', 'Domínguez', 'Reyes', 'DORS070108HBCMYBA7', '2007-01-08', 'M', 'sebastian.dominguez@alumnos.usystem.local', '6643000014', 'Blvd. Insurgentes 18000', 'Tijuana', 'Baja California', '22225', 'Active'),
  (15, 'A20260008', 2, 2, 4, 'Fernanda', 'Castro', 'León', 'CALF070929MBCSNRA9', '2007-09-29', 'F', 'fernanda.castro@alumnos.usystem.local', '6643000015', 'Calle Ensenada 303', 'Tijuana', 'Baja California', '22040', 'Active'),
  (16, 'A20260009', 2, 2, 4, 'Diego Alejandro', 'Peña', 'Rosales', 'PERD061212HBCXSGA5', '2006-12-12', 'M', 'diego.pena@alumnos.usystem.local', '6643000016', 'Calle Novena 1012', 'Tijuana', 'Baja California', '22000', 'Active'),
  (17, 'A20260010', 1, 1, 6, 'Valentina', 'Herrera', 'Campos', 'HECV080220MBCRMLA1', '2008-02-20', 'F', 'valentina.herrera@alumnos.usystem.local', '6643000017', 'Av. Tecnológico 2200', 'Tijuana', 'Baja California', '22414', 'Active'),
  (18, 'A20260011', 1, 1, 6, 'Gael', 'Mendoza', 'Paredes', 'MEPG080514HBCNRLA6', '2008-05-14', 'M', 'gael.mendoza@alumnos.usystem.local', '6643000018', 'Calle Río Grijalva 54', 'Tijuana', 'Baja California', '22015', 'Active'),
  (19, 'A20260012', 1, 1, 6, 'Isabella', 'Aguilar', 'Fuentes', 'AUFI080707MBCGNSA3', '2008-07-07', 'F', 'isabella.aguilar@alumnos.usystem.local', '6643000019', 'Calle Misión de San Javier 10', 'Tijuana', 'Baja California', '22010', 'Active'),
  (20, 'A20260013', 1, 1, 6, 'Adrián', 'Silva', 'Cervantes', 'SICA080303HBCLRDA8', '2008-03-03', 'M', 'adrian.silva@alumnos.usystem.local', '6643000020', 'Av. Popocatépetl 360', 'Tijuana', 'Baja California', '22110', 'Active'),
  (21, 'A20260014', 1, 1, 6, 'Mariana', 'Ortega', 'Bravo', 'OEBM081121MBCRRRA2', '2008-11-21', 'F', 'mariana.ortega@alumnos.usystem.local', '6643000021', 'Calle Guadalajara 77', 'Rosarito', 'Baja California', '22703', 'Active'),
  (22, 'A20260015', 2, 2, 6, 'Daniela', 'Ríos', 'Carrillo', 'RICD080416MBCSRNA0', '2008-04-16', 'F', 'daniela.rios@alumnos.usystem.local', '6643000022', 'Calle Madero 1505', 'Tijuana', 'Baja California', '22000', 'Active'),
  (23, 'A20260016', 2, 2, 6, 'Emilio', 'Navarro', 'Guzmán', 'NAGE080625HBCVZMA4', '2008-06-25', 'M', 'emilio.navarro@alumnos.usystem.local', '6643000023', 'Blvd. Fundadores 2900', 'Tijuana', 'Baja California', '22020', 'Active'),
  (24, 'A20260017', 2, 2, 6, 'Paula', 'Jiménez', 'Sandoval', 'JISP080830MBCMNLA7', '2008-08-30', 'F', 'paula.jimenez@alumnos.usystem.local', '6643000024', 'Calle Abasolo 18', 'Tecate', 'Baja California', '21400', 'Active'),
  (25, 'A20260018', 2, 2, 6, 'Rodrigo', 'Medina', 'Acosta', 'MEAR080117HBCDCDA1', '2008-01-17', 'M', 'rodrigo.medina@alumnos.usystem.local', '6643000025', 'Av. Sánchez Taboada 10488', 'Tijuana', 'Baja California', '22010', 'Active'),
  (26, 'A20260019', 1, 1, 7, 'Lucía', 'Estrada', 'Montes', 'EAML081010MBCSNCA5', '2008-10-10', 'F', 'lucia.estrada@correo.example', '6643000026', 'Calle Pípila 400', 'Tijuana', 'Baja California', '22000', 'Applicant'),
  (27, 'A20260020', 2, 2, 7, 'Tomás', 'Cabrera', 'Nieves', 'CANT080902HBCBVMA9', '2008-09-02', 'M', 'tomas.cabrera@correo.example', '6643000027', 'Calle Allende 66', 'Tecate', 'Baja California', '21400', 'Applicant');

-- Fecha de alta realista: inicio de inscripciones de su periodo de ingreso
UPDATE Students AS Student
JOIN AcademicTerms AS Term ON Term.Id = Student.EnrollmentTermId
SET Student.CreatedAt = TIMESTAMP(Term.EnrollmentStartDate, '09:30:00'), Student.CreatedBy = 2;

INSERT INTO StudentGuardians (StudentId, FullName, Relationship, Phone, Email)
SELECT Student.Id,
       CONCAT(ELT(1 + MOD(Student.Id, 4), 'María', 'José', 'Guadalupe', 'Roberto'), ' ', Student.PaternalLastName, ' ',
              ELT(1 + MOD(Student.Id, 5), 'García', 'López', 'Martínez', 'Hernández', 'Pérez')),
       ELT(1 + MOD(Student.Id, 4), 'Mother', 'Father', 'Mother', 'Father'),
       CONCAT('664', LPAD(2000000 + Student.Id * 137, 7, '0')),
       CONCAT('tutor', Student.Id, '@correo.example')
FROM Students AS Student
ORDER BY Student.Id;

-- ---------------------------------------------------------------------
-- Expediente: documentos obligatorios validados, con algunos casos pendientes o rechazados
-- ---------------------------------------------------------------------
INSERT INTO StudentDocuments (StudentId, DocumentTypeId, Status, FilePath, Notes, ValidatedBy, ValidatedAt)
SELECT Student.Id, DocumentType.Id, 'Validated', NULL, NULL, 2, TIMESTAMP(Term.EnrollmentEndDate, '12:00:00')
FROM Students AS Student
JOIN AcademicTerms AS Term ON Term.Id = Student.EnrollmentTermId
JOIN DocumentTypes AS DocumentType ON DocumentType.IsMandatory = 1
WHERE Student.Status <> 'Applicant'
ORDER BY Student.Id, DocumentType.Id;

-- Identificación oficial (opcional) solo para algunos
INSERT INTO StudentDocuments (StudentId, DocumentTypeId, Status, FilePath, Notes, ValidatedBy, ValidatedAt)
SELECT Student.Id, 6, 'Validated', NULL, NULL, 2, TIMESTAMP(Term.EnrollmentEndDate, '12:30:00')
FROM Students AS Student
JOIN AcademicTerms AS Term ON Term.Id = Student.EnrollmentTermId
WHERE Student.Status <> 'Applicant' AND MOD(Student.Id, 3) = 0;

UPDATE StudentDocuments SET Status = 'Delivered', ValidatedBy = NULL, ValidatedAt = NULL
WHERE StudentId = 20 AND DocumentTypeId IN (4, 5);

UPDATE StudentDocuments SET Status = 'Rejected', ValidatedBy = NULL, ValidatedAt = NULL, Notes = 'Documento ilegible, solicitar copia nueva'
WHERE StudentId = 23 AND DocumentTypeId = 3;

-- Aspirantes 2027-1 con expediente incompleto
INSERT INTO StudentDocuments (StudentId, DocumentTypeId, Status, FilePath, Notes, ValidatedBy, ValidatedAt) VALUES
  (26, 1, 'Validated', NULL, NULL, 2, '2026-11-20 10:00:00'),
  (26, 2, 'Validated', NULL, NULL, 2, '2026-11-20 10:05:00'),
  (26, 3, 'Delivered', NULL, NULL, NULL, NULL),
  (27, 1, 'Delivered', NULL, NULL, NULL, NULL);

-- ---------------------------------------------------------------------
-- Inscripciones: una por alumno y periodo desde su ingreso
-- Sofía (4) se dio de baja temporal tras 2025-3; Iker (12) canceló su reinscripción 2026-2
-- ---------------------------------------------------------------------
INSERT INTO Enrollments (StudentId, AcademicTermId, CareerId, Type, Status, Notes, CreatedAt, CreatedBy)
SELECT Student.Id,
       Term.Id,
       Student.CareerId,
       IF(Term.Id = Student.EnrollmentTermId, 'Enrollment', 'ReEnrollment'),
       'Active',
       NULL,
       TIMESTAMP(IF(Term.Id = Student.EnrollmentTermId, Term.EnrollmentStartDate, Term.ReEnrollmentStartDate) + INTERVAL MOD(Student.Id, 10) DAY, '10:00:00'),
       2
FROM Students AS Student
JOIN AcademicTerms AS Term
  ON Term.Id BETWEEN Student.EnrollmentTermId AND (CASE Student.Id WHEN 4 THEN 3 WHEN 12 THEN 5 ELSE 6 END)
WHERE Student.Status <> 'Applicant'
ORDER BY Term.Id, Student.Id;

UPDATE Enrollments SET Status = 'Cancelled', Notes = 'Baja voluntaria antes del inicio de clases'
WHERE StudentId = 12 AND AcademicTermId = 5;

-- ---------------------------------------------------------------------
-- Grupos: uno por materia de cada generación en cada periodo (+ oferta 2027-1 para reinscripción)
-- El código del grupo es <Carrera>-<Materia> y es único dentro del periodo
-- ---------------------------------------------------------------------
INSERT INTO ClassGroups (AcademicTermId, SubjectId, TeacherId, Code, Classroom, Capacity, EnrolledCount, Status)
SELECT Combo.AcademicTermId,
       PlanSubject.SubjectId,
       IF(PlanSubject.SubjectId <= 6, IF(Combo.CareerId = 1, 9, 10),
          (SELECT MIN(TeacherSubject.TeacherId) FROM TeacherSubjects AS TeacherSubject WHERE TeacherSubject.SubjectId = PlanSubject.SubjectId)),
       CONCAT(Career.Code, '-', Subject.Code),
       NULL,
       30,
       0,
       IF(Combo.AcademicTermId >= 6, 'Open', 'Closed')
FROM (
  SELECT DISTINCT Enrollment.AcademicTermId, Student.CareerId, Student.StudyPlanId,
         Enrollment.AcademicTermId - Student.EnrollmentTermId + 1 AS TermIndex
  FROM Enrollments AS Enrollment
  JOIN Students AS Student ON Student.Id = Enrollment.StudentId
  WHERE Enrollment.Status = 'Active'
  UNION
  SELECT DISTINCT 7, Student.CareerId, Student.StudyPlanId, 7 - Student.EnrollmentTermId + 1
  FROM Enrollments AS Enrollment
  JOIN Students AS Student ON Student.Id = Enrollment.StudentId
  WHERE Enrollment.Status = 'Active' AND Enrollment.AcademicTermId = 6
) AS Combo
JOIN Careers AS Career ON Career.Id = Combo.CareerId
JOIN StudyPlanSubjects AS PlanSubject ON PlanSubject.StudyPlanId = Combo.StudyPlanId AND PlanSubject.TermNumber = Combo.TermIndex
JOIN Subjects AS Subject ON Subject.Id = PlanSubject.SubjectId
ORDER BY Combo.AcademicTermId, Combo.CareerId, Combo.TermIndex, PlanSubject.SubjectId;

-- Cada generación tiene su turno y aula: 4 grupos por generación sin choques de horario
-- Slot 0: Lun/Mié bloque 1 | 1: Lun/Mié bloque 2 | 2: Mar/Jue bloque 1 | 3: Mar/Jue bloque 2
-- Turno (Slot DIV 4): 0 -> 07:00, 1 -> 11:00, 2 -> 15:00
UPDATE ClassGroups AS ClassGroup
JOIN (
  SELECT Id, ROW_NUMBER() OVER (PARTITION BY AcademicTermId, SUBSTRING_INDEX(Code, '-', 1) ORDER BY Id) - 1 AS Slot
  FROM ClassGroups
) AS Ranked ON Ranked.Id = ClassGroup.Id
SET ClassGroup.Classroom = CONCAT(IF(ClassGroup.Code LIKE 'ISC-%', 'A-', 'B-'), 101 + Ranked.Slot DIV 4);

INSERT INTO ClassGroupSchedules (ClassGroupId, DayOfWeek, StartTime, EndTime)
SELECT Ranked.Id,
       IF(MOD(Ranked.Slot, 4) < 2, 1, 2) + Day.Offset,
       SEC_TO_TIME((7 + 4 * (Ranked.Slot DIV 4) + 2 * MOD(Ranked.Slot, 2)) * 3600),
       SEC_TO_TIME((9 + 4 * (Ranked.Slot DIV 4) + 2 * MOD(Ranked.Slot, 2)) * 3600)
FROM (
  SELECT Id, ROW_NUMBER() OVER (PARTITION BY AcademicTermId, SUBSTRING_INDEX(Code, '-', 1) ORDER BY Id) - 1 AS Slot
  FROM ClassGroups
) AS Ranked
CROSS JOIN (SELECT 0 AS Offset UNION ALL SELECT 2) AS Day
ORDER BY Ranked.Id, Day.Offset;

INSERT INTO EnrollmentClassGroups (EnrollmentId, ClassGroupId)
SELECT Enrollment.Id, ClassGroup.Id
FROM Enrollments AS Enrollment
JOIN Students AS Student ON Student.Id = Enrollment.StudentId
JOIN Careers AS Career ON Career.Id = Student.CareerId
JOIN StudyPlanSubjects AS PlanSubject
  ON PlanSubject.StudyPlanId = Student.StudyPlanId
 AND PlanSubject.TermNumber = Enrollment.AcademicTermId - Student.EnrollmentTermId + 1
JOIN ClassGroups AS ClassGroup
  ON ClassGroup.AcademicTermId = Enrollment.AcademicTermId
 AND ClassGroup.SubjectId = PlanSubject.SubjectId
 AND ClassGroup.Code = CONCAT(Career.Code, '-', (SELECT Code FROM Subjects WHERE Id = PlanSubject.SubjectId))
WHERE Enrollment.Status = 'Active'
ORDER BY Enrollment.Id, ClassGroup.Id;

UPDATE ClassGroups AS ClassGroup
JOIN (SELECT ClassGroupId, COUNT(*) AS Total FROM EnrollmentClassGroups GROUP BY ClassGroupId) AS Counted
  ON Counted.ClassGroupId = ClassGroup.Id
SET ClassGroup.EnrolledCount = Counted.Total;

-- ---------------------------------------------------------------------
-- Calificaciones (deterministas): periodos cerrados con acta cerrada; 2026-3 con parciales 1 y 2
-- Final = promedio de parciales * 0.6 + ordinario * 0.4 | Aprobatoria >= 7
-- ---------------------------------------------------------------------
INSERT INTO Grades (EnrollmentId, ClassGroupId, Partial1, Partial2, Partial3, Ordinary, FinalScore, AttemptNumber,
                    EvaluationType, Status, IsReportClosed, ClosedAt, ClosedBy)
SELECT Scored.EnrollmentId,
       Scored.ClassGroupId,
       LEAST(10.00, ROUND(Scored.Base + 0.4 - MOD(Scored.Seed, 3) * 0.4, 1)),
       ROUND(Scored.Base, 1),
       IF(Scored.IsCurrent, NULL, LEAST(10.00, ROUND(Scored.Base - 0.2 + MOD(Scored.Seed, 2) * 0.4, 1))),
       IF(Scored.IsCurrent, NULL, ROUND(Scored.Base, 1)),
       NULL,
       1,
       'Ordinary',
       'InProgress',
       IF(Scored.IsCurrent, 0, 1),
       IF(Scored.IsCurrent, NULL, TIMESTAMP(Scored.GradeReportCloseDate, '18:00:00')),
       IF(Scored.IsCurrent, NULL, 4)
FROM (
  SELECT Link.EnrollmentId,
         Link.ClassGroupId,
         Term.GradeReportCloseDate,
         Term.Id = 6 AS IsCurrent,
         Enrollment.StudentId * 31 + ClassGroup.SubjectId * 17 AS Seed,
         LEAST(9.9, 6.3 + MOD(Enrollment.StudentId * 7, 5) * 0.3
                   + MOD(Enrollment.StudentId * 37 + ClassGroup.SubjectId * 11 + Term.Id * 5, 23) * 0.1) AS Base
  FROM EnrollmentClassGroups AS Link
  JOIN Enrollments AS Enrollment ON Enrollment.Id = Link.EnrollmentId
  JOIN AcademicTerms AS Term ON Term.Id = Enrollment.AcademicTermId
  JOIN ClassGroups AS ClassGroup ON ClassGroup.Id = Link.ClassGroupId
) AS Scored
ORDER BY Scored.EnrollmentId, Scored.ClassGroupId;

-- MySQL evalúa las asignaciones en orden: Status ya ve el FinalScore recién calculado
UPDATE Grades
SET FinalScore = ROUND((Partial1 + Partial2 + Partial3) / 3 * 0.6 + Ordinary * 0.4, 1),
    Status = IF(FinalScore >= 7, 'Passed', 'Failed')
WHERE Ordinary IS NOT NULL;

-- Extraordinario aprobado para los reprobados hasta 2026-1; los de 2026-2 siguen pendientes
INSERT INTO Grades (EnrollmentId, ClassGroupId, Partial1, Partial2, Partial3, Ordinary, FinalScore, AttemptNumber,
                    EvaluationType, Status, IsReportClosed, ClosedAt, ClosedBy)
SELECT Grade.EnrollmentId, Grade.ClassGroupId, NULL, NULL, NULL,
       7.5 + MOD(Grade.Id, 3) * 0.5, 7.5 + MOD(Grade.Id, 3) * 0.5,
       2, 'Extraordinary', 'Passed', 1, Grade.ClosedAt + INTERVAL 14 DAY, 4
FROM Grades AS Grade
JOIN Enrollments AS Enrollment ON Enrollment.Id = Grade.EnrollmentId
WHERE Grade.Status = 'Failed' AND Enrollment.AcademicTermId <= 4
ORDER BY Grade.Id;

-- ---------------------------------------------------------------------
-- Finanzas: concepto de reinscripción, becas, planes de pago, cobros y pagos
-- ---------------------------------------------------------------------
INSERT INTO PaymentConcepts (Id, Code, Name, BaseAmount, IsRecurring, IsActive) VALUES
  (5, 'ReEnrollment', 'Re-enrollment fee', 1800.00, 0, 1);

INSERT INTO Scholarships (Id, Name, Type, Value, IsActive) VALUES
  (1, 'Excelencia académica', 'Percentage', 30.00, 1),
  (2, 'Beca deportiva', 'Percentage', 20.00, 1),
  (3, 'Convenio empresarial', 'Percentage', 15.00, 1),
  (4, 'Apoyo socioeconómico', 'Amount', 500.00, 1),
  (5, 'Beca cultural', 'Percentage', 10.00, 0);

INSERT INTO StudentScholarships (StudentId, ScholarshipId, AcademicTermId, AssignedAt) VALUES
  (2, 1, 4, '2025-12-15 10:00:00'),
  (2, 1, 5, '2026-04-10 10:00:00'),
  (2, 1, 6, '2026-08-05 10:00:00'),
  (6, 3, 5, '2026-04-10 11:00:00'),
  (6, 3, 6, '2026-08-05 11:00:00'),
  (13, 2, 6, '2026-08-06 09:00:00'),
  (19, 4, 6, '2026-08-20 09:00:00');

INSERT INTO PaymentPlans (CareerId, AcademicTermId, Name, InstallmentCount, IsActive) VALUES
  (1, 6, 'Plan cuatrimestral ISC 2026-3', 4, 1),
  (2, 6, 'Plan cuatrimestral LAE 2026-3', 4, 1),
  (1, 7, 'Plan cuatrimestral ISC 2027-1', 4, 1),
  (2, 7, 'Plan cuatrimestral LAE 2027-1', 4, 1),
  (NULL, 7, 'Pago de contado 2027-1', 1, 1);

-- Inscripción o reinscripción (también para la reinscripción cancelada: se pagó antes de la baja)
INSERT INTO Charges (StudentId, AcademicTermId, PaymentConceptId, Amount, Discount, Surcharge, DueDate, Status, CreatedAt, CreatedBy)
SELECT Enrollment.StudentId, Enrollment.AcademicTermId,
       IF(Enrollment.Type = 'Enrollment', 1, 5),
       IF(Enrollment.Type = 'Enrollment', 2500.00, 1800.00),
       0, 0,
       IF(Enrollment.Type = 'Enrollment', Term.EnrollmentEndDate, Term.ReEnrollmentEndDate),
       'Paid',
       Enrollment.CreatedAt, 3
FROM Enrollments AS Enrollment
JOIN AcademicTerms AS Term ON Term.Id = Enrollment.AcademicTermId
ORDER BY Enrollment.AcademicTermId, Enrollment.StudentId;

-- Cuatro colegiaturas mensuales por periodo (día 10), con descuento si hay beca
INSERT INTO Charges (StudentId, AcademicTermId, PaymentConceptId, Amount, Discount, Surcharge, DueDate, Status, CreatedAt, CreatedBy)
SELECT Enrollment.StudentId, Enrollment.AcademicTermId, 2, 3200.00,
       ROUND(CASE Scholarship.Type
               WHEN 'Percentage' THEN 3200.00 * Scholarship.Value / 100
               WHEN 'Amount' THEN Scholarship.Value
               ELSE 0 END, 2),
       0,
       DATE(DATE_FORMAT(Term.StartDate, '%Y-%m-10')) + INTERVAL Installment.Number MONTH,
       'Pending',
       Enrollment.CreatedAt, 3
FROM Enrollments AS Enrollment
JOIN AcademicTerms AS Term ON Term.Id = Enrollment.AcademicTermId
CROSS JOIN (SELECT 0 AS Number UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3) AS Installment
LEFT JOIN StudentScholarships AS Assigned ON Assigned.StudentId = Enrollment.StudentId AND Assigned.AcademicTermId = Enrollment.AcademicTermId
LEFT JOIN Scholarships AS Scholarship ON Scholarship.Id = Assigned.ScholarshipId
WHERE Enrollment.Status = 'Active'
ORDER BY Enrollment.AcademicTermId, Enrollment.StudentId, Installment.Number;

-- Vencidas antes de "hoy": pagadas, salvo morosos (recargo 10 %) y un pago parcial
UPDATE Charges SET Status = 'Paid' WHERE PaymentConceptId = 2 AND DueDate < @DemoToday;

UPDATE Charges SET Status = 'Overdue', Surcharge = ROUND((Amount - Discount) * 0.10, 2)
WHERE PaymentConceptId = 2 AND DueDate = '2026-11-10' AND StudentId IN (9, 18, 24);

UPDATE Charges SET Status = 'Partial'
WHERE PaymentConceptId = 2 AND DueDate = '2026-11-10' AND StudentId = 14;

-- Examen extraordinario: un cobro pagado por cada intento extraordinario
INSERT INTO Charges (StudentId, AcademicTermId, PaymentConceptId, Amount, Discount, Surcharge, DueDate, Status, CreatedAt, CreatedBy)
SELECT Enrollment.StudentId, Enrollment.AcademicTermId, 3, 800.00, 0, 0, DATE(Grade.ClosedAt) - INTERVAL 7 DAY, 'Paid',
       Grade.ClosedAt - INTERVAL 10 DAY, 3
FROM Grades AS Grade
JOIN Enrollments AS Enrollment ON Enrollment.Id = Grade.EnrollmentId
WHERE Grade.EvaluationType = 'Extraordinary'
ORDER BY Grade.Id;

-- Constancias de trámites ya emitidos
INSERT INTO Charges (StudentId, AcademicTermId, PaymentConceptId, Amount, Discount, Surcharge, DueDate, Status, CreatedAt, CreatedBy) VALUES
  (2, 3, 4, 500.00, 0, 0, '2025-10-01', 'Paid', '2025-10-01 09:00:00', 3),
  (1, 6, 4, 500.00, 0, 0, '2026-09-15', 'Paid', '2026-09-15 09:00:00', 3),
  (5, 6, 4, 500.00, 0, 0, '2026-10-06', 'Paid', '2026-10-06 09:00:00', 3),
  (9, 6, 4, 500.00, 0, 0, '2026-11-25', 'Pending', '2026-11-18 12:00:00', 3);

-- Un pago por cobro pagado; el folio definitivo se asigna después en orden de captura
INSERT INTO Payments (StudentId, ChargeId, Amount, Method, Reference, PaymentDate, ReceiptNumber, UserId, Status, CreatedAt, CreatedBy)
SELECT Charge.StudentId, Charge.Id,
       Charge.Amount - Charge.Discount + Charge.Surcharge,
       ELT(1 + MOD(Charge.Id, 3), 'Cash', 'Card', 'Transfer'),
       CASE MOD(Charge.Id, 3)
         WHEN 1 THEN CONCAT('AUT-', LPAD(MOD(Charge.Id * 7919, 1000000), 6, '0'))
         WHEN 2 THEN CONCAT('SPEI-', LPAD(Charge.Id, 8, '0'))
         ELSE NULL END,
       TIMESTAMP(Charge.DueDate - INTERVAL MOD(Charge.Id, 5) DAY, MAKETIME(9 + MOD(Charge.Id, 8), MOD(Charge.Id * 7, 60), 0)),
       CONCAT('TMP-', Charge.Id),
       3, 'Applied',
       TIMESTAMP(Charge.DueDate - INTERVAL MOD(Charge.Id, 5) DAY, MAKETIME(9 + MOD(Charge.Id, 8), MOD(Charge.Id * 7, 60), 0)), 3
FROM Charges AS Charge
WHERE Charge.Status = 'Paid'
ORDER BY Charge.DueDate, Charge.Id;

-- Abono parcial (50 %) a la colegiatura de noviembre de Sebastián
INSERT INTO Payments (StudentId, ChargeId, Amount, Method, Reference, PaymentDate, ReceiptNumber, UserId, Status, CreatedAt, CreatedBy)
SELECT Charge.StudentId, Charge.Id, ROUND((Charge.Amount - Charge.Discount) / 2, 2), 'Cash', NULL,
       '2026-11-09 12:15:00', CONCAT('TMP-P', Charge.Id), 3, 'Applied', '2026-11-09 12:15:00', 3
FROM Charges AS Charge
WHERE Charge.Status = 'Partial';

-- Pago cancelado por captura errónea (el cobro se volvió a pagar correctamente)
INSERT INTO Payments (StudentId, ChargeId, Amount, Method, Reference, PaymentDate, ReceiptNumber, UserId, Status, CancelledAt, CancellationReason, CreatedAt, CreatedBy)
SELECT Charge.StudentId, Charge.Id, 2300.00, 'Cash', NULL, '2026-09-07 10:00:00', CONCAT('TMP-C', Charge.Id), 3, 'Cancelled',
       '2026-09-07 10:20:00', 'Monto capturado incorrectamente', '2026-09-07 10:00:00', 3
FROM Charges AS Charge
WHERE Charge.StudentId = 1 AND Charge.AcademicTermId = 6 AND Charge.PaymentConceptId = 2 AND Charge.DueDate = '2026-09-10';

UPDATE Payments AS Payment
JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY PaymentDate, Id) AS Sequence FROM Payments) AS Ordered ON Ordered.Id = Payment.Id
SET Payment.ReceiptNumber = CONCAT('REC-', YEAR(Payment.PaymentDate), '-', LPAD(Ordered.Sequence, 5, '0'));

-- ---------------------------------------------------------------------
-- Trámites (folios con el formato de la API: TRM<año><consecutivo>)
-- ---------------------------------------------------------------------
INSERT INTO Procedures (ProcedureTypeId, StudentId, FolioNumber, Status, RequestedAt, IssuedAt, FilePath, UserId, CreatedAt, CreatedBy) VALUES
  (1, 2, 'TRM20250001', 'Issued', '2025-10-01 09:10:00', '2025-10-03 13:00:00', NULL, 2, '2025-10-01 09:10:00', 2),
  (1, 1, 'TRM20260001', 'Issued', '2026-09-15 09:05:00', '2026-09-17 12:30:00', NULL, 2, '2026-09-15 09:05:00', 2),
  (2, 5, 'TRM20260002', 'Issued', '2026-10-06 11:40:00', '2026-10-08 10:00:00', NULL, 2, '2026-10-06 11:40:00', 2),
  (1, 9, 'TRM20260003', 'Requested', '2026-11-18 12:00:00', NULL, NULL, 2, '2026-11-18 12:00:00', 2),
  (3, 3, 'TRM20260004', 'Requested', '2026-11-20 16:20:00', NULL, NULL, 2, '2026-11-20 16:20:00', 2);

-- ---------------------------------------------------------------------
-- Notificaciones
-- ---------------------------------------------------------------------
INSERT INTO Notifications (TemplateId, StudentId, RecipientEmail, Status, SentAt, ErrorMessage, RetryCount, CreatedAt, CreatedBy)
SELECT 2, Student.Id, Student.Email, 'Sent', '2026-08-03 09:00:00', NULL, 0, '2026-08-03 08:55:00', 2
FROM Students AS Student
WHERE Student.EnrollmentTermId = 6
ORDER BY Student.Id;

INSERT INTO Notifications (TemplateId, StudentId, RecipientEmail, Status, SentAt, ErrorMessage, RetryCount, CreatedAt, CreatedBy)
SELECT 3, Enrollment.StudentId, Student.Email, 'Sent', '2026-08-29 10:00:00', NULL, 0, '2026-08-29 09:55:00', 4
FROM Enrollments AS Enrollment
JOIN Students AS Student ON Student.Id = Enrollment.StudentId
WHERE Enrollment.AcademicTermId = 5 AND Enrollment.Status = 'Active'
ORDER BY Enrollment.StudentId;

INSERT INTO Notifications (TemplateId, StudentId, RecipientEmail, Status, SentAt, ErrorMessage, RetryCount, CreatedAt, CreatedBy)
SELECT 1, Charge.StudentId, Guardian.Email, 'Pending', NULL, NULL, 0, '2026-11-15 08:00:00', 3
FROM Charges AS Charge
JOIN StudentGuardians AS Guardian ON Guardian.StudentId = Charge.StudentId
WHERE Charge.Status = 'Overdue'
ORDER BY Charge.StudentId;

INSERT INTO Notifications (TemplateId, StudentId, RecipientEmail, Status, SentAt, ErrorMessage, RetryCount, CreatedAt, CreatedBy) VALUES
  (1, 14, 'tutor14@correo.example', 'Failed', NULL, 'SMTP timeout after 30 seconds', 3, '2026-11-15 08:00:00', 3);

-- ---------------------------------------------------------------------
-- Material didáctico
-- ---------------------------------------------------------------------
INSERT INTO CourseMaterials (SubjectId, AcademicTermId, Type, Title, Description, Url, FilePath, IsActive) VALUES
  (101, NULL, 'Syllabus', 'Programa de Fundamentos de Programación', 'Temario oficial, criterios de evaluación y bibliografía.', NULL, NULL, 1),
  (101, 6, 'Link', 'Tutorial oficial de Python', 'Lectura recomendada para la unidad 1.', 'https://docs.python.org/3/tutorial/', NULL, 1),
  (107, 6, 'Video', 'Visualización de estructuras de datos', 'Animaciones de pilas, colas, árboles y grafos.', 'https://visualgo.net/', NULL, 1),
  (109, NULL, 'Syllabus', 'Programa de Fundamentos de Bases de Datos', 'Modelo entidad-relación, normalización y SQL.', NULL, NULL, 1),
  (109, 6, 'Link', 'Manual de referencia de MySQL 8', NULL, 'https://dev.mysql.com/doc/refman/8.0/en/', NULL, 1),
  (115, NULL, 'Reading', 'Guía de HTML y CSS', 'Material de apoyo para la primera unidad.', 'https://developer.mozilla.org/es/docs/Learn', NULL, 1),
  (1, NULL, 'Syllabus', 'Programa de Inglés I', 'Nivel A1 del Marco Común Europeo.', NULL, NULL, 1),
  (201, NULL, 'Syllabus', 'Programa de Fundamentos de Administración', NULL, NULL, NULL, 1),
  (202, 6, 'Reading', 'Normas de Información Financiera (resumen)', 'Lectura para la unidad 2.', NULL, NULL, 1),
  (207, 6, 'Other', 'Caso práctico: lanzamiento de producto', 'Caso para trabajo en equipo.', NULL, NULL, 0);

-- ---------------------------------------------------------------------
-- Bitácora: marca de que la base se generó con LocalSetup
-- ---------------------------------------------------------------------
INSERT INTO AuditLogs (UserId, Action, Module, Entity, EntityId, OldValue, NewValue, IpAddress) VALUES
  (1, 'Seed', 'System', 'Database', NULL, NULL, JSON_OBJECT('Source', 'database/LocalSetup', 'DemoToday', '2026-11-21'), '127.0.0.1');
