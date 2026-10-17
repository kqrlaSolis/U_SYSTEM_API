# Genera los scripts SQL del modelo de datos y el README del modelo.
# Los identificadores van en inglés y PascalCase; los comentarios en español.

from Paths import WriteDocument
from Schema import Tables, TableByName, AuditColumns, ScriptOrder
from Rbac import PermissionModules, Roles, RolePermissions, AllCodes

DatabaseName = "USystem"

Header = """-- =====================================================================
-- U SYSTEM | {Title}
-- Motor: MySQL 8.x | Tablas y columnas en PascalCase (inglés)
-- Regla del proyecto: solo los comentarios van en español
-- =====================================================================
USE {Database};
"""


def FormatDefault(Value):
    if Value == "CURRENT_TIMESTAMP":
        return "CURRENT_TIMESTAMP"
    if Value.replace(".", "", 1).isdigit():
        return Value
    return f"'{Value}'"


def ColumnSql(Name, Type, Flags):
    Tokens = Flags.split()
    Parts = [Name, Type]
    IsPrimary = "PK" in Tokens
    Parts.append("NULL" if "NULL" in Tokens and not IsPrimary else "NOT NULL")
    if "AI" in Tokens:
        Parts.append("AUTO_INCREMENT")
    # UpdatedAt se actualiza automáticamente en MySQL
    if Name == "UpdatedAt":
        return " ".join([Name, "DATETIME", "NULL", "ON UPDATE CURRENT_TIMESTAMP"])
    for Token in Tokens:
        if Token.startswith("DEF:"):
            Parts.append(f"DEFAULT {FormatDefault(Token[4:])}")
    return " ".join(Parts)


def ForeignKeySql(TableName, ColumnName, ReferencedTable):
    return (f"  CONSTRAINT Fk{TableName}{ColumnName} FOREIGN KEY ({ColumnName}) "
            f"REFERENCES {ReferencedTable} (Id) ON DELETE RESTRICT ON UPDATE CASCADE")


def TableSql(Table):
    Name = Table["Name"]
    Lines = []
    PrimaryKeys = []
    Constraints = []

    for Column in Table["Columns"]:
        ColumnName, Type, Flags, _ = Column
        Tokens = Flags.split()
        if "PK" in Tokens:
            PrimaryKeys.append(ColumnName)
        Lines.append(f"  {ColumnSql(ColumnName, Type, Flags)}")
        for Token in Tokens:
            if Token.startswith("FK:"):
                Constraints.append(ForeignKeySql(Name, ColumnName, Token[3:]))

    # Auditoría estándar (todas las tablas)
    for ColumnName, Type, Flags, _ in AuditColumns:
        Lines.append(f"  {ColumnSql(ColumnName, Type, Flags)}")

    Lines.append(f"  PRIMARY KEY ({', '.join(PrimaryKeys)})")

    # Llaves únicas declaradas en la columna
    for Column in Table["Columns"]:
        if "UQ" in Column[2].split():
            Lines.append(f"  UNIQUE KEY Uq{Name}{Column[0]} ({Column[0]})")

    for ConstraintName, Columns in Table["Unique"]:
        Lines.append(f"  UNIQUE KEY {ConstraintName} ({', '.join(Columns)})")

    ForeignKeyColumns = set()
    for Column in Table["Columns"]:
        for Token in Column[2].split():
            if Token.startswith("FK:"):
                ForeignKeyColumns.add(Column[0])

    for IndexName, Columns in Table["Indexes"]:
        # Un índice de una sola columna FK ya lo crea MySQL
        if len(Columns) == 1 and Columns[0] in ForeignKeyColumns:
            continue
        Lines.append(f"  KEY {IndexName} ({', '.join(Columns)})")

    Lines.extend(Constraints)
    Body = ",\n".join(Lines)
    return (f"CREATE TABLE IF NOT EXISTS {Name} (\n{Body}\n) "
            f"ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;\n")


def WriteScripts():
    WriteDocument("DataModel/Scripts/00_CreateDatabase.sql", f"""-- =====================================================================
-- U SYSTEM | Creación de la base de datos
-- Motor: MySQL 8.x | Regla del proyecto: solo los comentarios van en español
-- =====================================================================
CREATE DATABASE IF NOT EXISTS {DatabaseName}
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

USE {DatabaseName};

-- Usuario de aplicación con privilegios mínimos (cambiar la contraseña antes de ejecutar)
-- CREATE USER 'USystemApp'@'localhost' IDENTIFIED BY 'CHANGE_ME';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON {DatabaseName}.* TO 'USystemApp'@'localhost';
-- FLUSH PRIVILEGES;
""")

    for ScriptName, Title in ScriptOrder:
        if ScriptName in ("00_CreateDatabase", "99_SeedData", "04_AcademicTracking"):
            continue
        TablesInScript = [Table for Table in Tables if Table["Script"] == ScriptName]
        Parts = [Header.format(Title=Title, Database=DatabaseName)]
        for Table in TablesInScript:
            Parts.append(TableSql(Table))
        WriteDocument(f"DataModel/Scripts/{ScriptName}.sql", "\n".join(Parts))

    # 04 incluye la tabla Grades y las dos vistas
    GradesTable = TableByName["Grades"]
    Tracking = [Header.format(Title="Seguimiento académico y vistas", Database=DatabaseName), TableSql(GradesTable)]
    Tracking.append("""
-- Kardex por alumno: una fila por materia cursada y evaluación
CREATE OR REPLACE VIEW TranscriptView AS
SELECT
  Student.Id AS StudentId,
  Student.StudentNumber,
  CONCAT_WS(' ', Student.FirstName, Student.PaternalLastName, Student.MaternalLastName) AS FullName,
  Career.Code AS CareerCode,
  AcademicTerm.Code AS AcademicTermCode,
  Subject.Code AS SubjectCode,
  Subject.Name AS SubjectName,
  Subject.Credits,
  Grade.FinalScore,
  Grade.EvaluationType,
  Grade.AttemptNumber,
  Grade.Status,
  Grade.IsReportClosed
FROM Grades AS Grade
JOIN Enrollments AS Enrollment ON Enrollment.Id = Grade.EnrollmentId
JOIN Students AS Student ON Student.Id = Enrollment.StudentId
JOIN Careers AS Career ON Career.Id = Student.CareerId
JOIN AcademicTerms AS AcademicTerm ON AcademicTerm.Id = Enrollment.AcademicTermId
JOIN ClassGroups AS ClassGroup ON ClassGroup.Id = Grade.ClassGroupId
JOIN Subjects AS Subject ON Subject.Id = ClassGroup.SubjectId
WHERE Enrollment.Status = 'Active'
  AND Enrollment.DeletedAt IS NULL
  AND Grade.DeletedAt IS NULL;

-- Avance académico: cada materia cuenta una sola vez con su mejor calificación
CREATE OR REPLACE VIEW AcademicProgressView AS
SELECT
  Student.Id AS StudentId,
  Student.StudentNumber,
  Student.CareerId,
  StudyPlan.TotalCredits AS PlanTotalCredits,
  COALESCE(SUM(CASE WHEN BestAttempt.IsPassed = 1 THEN BestAttempt.Credits ELSE 0 END), 0) AS ApprovedCredits,
  ROUND(COALESCE(SUM(CASE WHEN BestAttempt.IsPassed = 1 THEN BestAttempt.Credits ELSE 0 END), 0)
        / NULLIF(StudyPlan.TotalCredits, 0) * 100, 2) AS ProgressPercentage,
  COUNT(CASE WHEN BestAttempt.IsPassed = 1 THEN BestAttempt.SubjectId END) AS ApprovedSubjects,
  ROUND(AVG(BestAttempt.BestScore), 2) AS GradePointAverage
FROM Students AS Student
JOIN StudyPlans AS StudyPlan ON StudyPlan.Id = Student.StudyPlanId
LEFT JOIN (
  SELECT
    Enrollment.StudentId,
    Subject.Id AS SubjectId,
    Subject.Credits,
    MAX(Grade.FinalScore) AS BestScore,
    MAX(CASE WHEN Grade.Status = 'Passed' THEN 1 ELSE 0 END) AS IsPassed
  FROM Grades AS Grade
  JOIN Enrollments AS Enrollment ON Enrollment.Id = Grade.EnrollmentId AND Enrollment.Status = 'Active'
  JOIN ClassGroups AS ClassGroup ON ClassGroup.Id = Grade.ClassGroupId
  JOIN Subjects AS Subject ON Subject.Id = ClassGroup.SubjectId
  GROUP BY Enrollment.StudentId, Subject.Id, Subject.Credits
) AS BestAttempt ON BestAttempt.StudentId = Student.Id
WHERE Student.DeletedAt IS NULL
  AND Student.Status = 'Active'
GROUP BY Student.Id, Student.StudentNumber, Student.CareerId, StudyPlan.TotalCredits;
""")
    WriteDocument("DataModel/Scripts/04_AcademicTracking.sql", "\n".join(Tracking))

    WriteSeedData()


def WriteSeedData():
    Parts = [Header.format(Title="Datos semilla", Database=DatabaseName)]
    Parts.append("-- Roles del sistema\n")
    Parts.append("INSERT INTO Roles (Id, Name, Description, IsSystem, IsActive) VALUES")
    RoleRows = []
    for Index, (RoleName, _, Description) in enumerate(Roles, 1):
        RoleRows.append(f"  ({Index}, '{RoleName}', '{Description}', 1, 1)")
    Parts.append(",\n".join(RoleRows) + ";\n")

    Parts.append("-- Catálogo de permisos (definido por el sistema, no se crea desde la UI)\n")
    Parts.append("INSERT INTO Permissions (Id, Code, Module, Description) VALUES")
    PermissionRows = []
    PermissionId = 0
    for Module, Actions in PermissionModules:
        for Action, Description in Actions:
            PermissionId += 1
            PermissionRows.append(f"  ({PermissionId}, '{Module}.{Action}', '{Module}', '{Description}')")
    Parts.append(",\n".join(PermissionRows) + ";\n")

    Parts.append("-- Matriz de permisos por rol\n")
    Parts.append("INSERT INTO RolePermissions (RoleId, PermissionId) VALUES")
    MatrixRows = []
    RoleIds = {Name: Index for Index, (Name, _, _) in enumerate(Roles, 1)}
    CodesById = {}
    CodeId = 0
    for Module, Actions in PermissionModules:
        for Action, _ in Actions:
            CodeId += 1
            CodesById[f"{Module}.{Action}"] = CodeId
    for RoleName, _, _ in Roles:
        for Code in RolePermissions[RoleName]:
            MatrixRows.append(f"  ({RoleIds[RoleName]}, {CodesById[Code]})")
    Parts.append(",\n".join(MatrixRows) + ";\n")

    Parts.append("""-- Tipos de documento del expediente
INSERT INTO DocumentTypes (Id, Name, IsMandatory, AppliesToLevel) VALUES
  (1, 'Birth Certificate', 1, 'Bachelor'),
  (2, 'CURP', 1, 'Bachelor'),
  (3, 'High School Certificate', 1, 'Bachelor'),
  (4, 'Photographs', 1, 'Bachelor'),
  (5, 'Proof of Address', 1, 'Bachelor'),
  (6, 'Official Identification', 0, 'Bachelor');

-- Conceptos de pago
INSERT INTO PaymentConcepts (Id, Code, Name, BaseAmount, IsRecurring, IsActive) VALUES
  (1, 'Enrollment', 'Enrollment fee', 2500.00, 0, 1),
  (2, 'Tuition', 'Tuition', 3200.00, 1, 1),
  (3, 'ExtraordinaryExam', 'Extraordinary exam', 800.00, 0, 1),
  (4, 'Certificate', 'Certificate', 500.00, 0, 1);

-- Tipos de trámite
INSERT INTO ProcedureTypes (Id, Code, Name, PaymentConceptId, IsActive) VALUES
  (1, 'StudyCertificate', 'Certificate of studies', 4, 1),
  (2, 'TranscriptCopy', 'Academic transcript copy', 4, 1),
  (3, 'CompletionLetter', 'Completion letter', 4, 1);

-- Plantillas de notificación
INSERT INTO NotificationTemplates (Id, Code, SubjectLine, Body, IsActive) VALUES
  (1, 'PaymentReminder', 'Upcoming payment', 'Dear {{GuardianName}}, the payment for {{StudentName}} is due on {{DueDate}}.', 1),
  (2, 'TermOpen', 'Enrollment period open', 'The enrollment period {{TermCode}} is now open.', 1),
  (3, 'GradesPublished', 'Grades published', 'The grades for {{StudentName}} are available.', 1);

-- Parámetros del sistema
INSERT INTO SystemSettings (Id, SettingKey, SettingValue, ValueType, Category, Description) VALUES
  (1, 'Login.MaxFailedAttempts', '5', 'Int', 'Security', 'Failed attempts before locking the account'),
  (2, 'Login.LockoutMinutes', '15', 'Int', 'Security', 'Lockout duration in minutes'),
  (3, 'Password.MinLength', '8', 'Int', 'Security', 'Minimum password length'),
  (4, 'Jwt.AccessTokenMinutes', '15', 'Int', 'Security', 'Access token lifetime'),
  (5, 'Jwt.RefreshTokenDays', '7', 'Int', 'Security', 'Refresh token lifetime'),
  (6, 'PasswordReset.ExpirationMinutes', '30', 'Int', 'Security', 'Password reset token lifetime'),
  (7, 'Grades.MaxScore', '10', 'Decimal', 'Grades', 'Maximum grade'),
  (8, 'Grades.MinPassingScore', '7', 'Decimal', 'Grades', 'Minimum passing grade'),
  (9, 'Grades.MaxRecurrenceAttempts', '2', 'Int', 'Grades', 'Maximum attempts to retake a failed subject'),
  (10, 'Grades.MaxExtraordinaryAttempts', '2', 'Int', 'Grades', 'Maximum extraordinary attempts'),
  (11, 'Grades.MaxRemedialAttempts', '1', 'Int', 'Grades', 'Maximum remedial attempts'),
  (12, 'ReEnrollment.MaxSubjects', '8', 'Int', 'Academic', 'Maximum subjects per term'),
  (13, 'Payments.SurchargeType', 'Percentage', 'String', 'Finance', 'Surcharge type: Percentage or Amount'),
  (14, 'Payments.SurchargeValue', '10', 'Decimal', 'Finance', 'Surcharge value (10% of the outstanding balance)'),
  (15, 'Payments.InstallmentsPerTerm', '4', 'Int', 'Finance', 'Monthly installments per term'),
  (16, 'Students.NumberFormat', 'YYYY-CCC-NNNN', 'String', 'Students', 'Student number format'),
  (17, 'Storage.Provider', 'S3', 'String', 'Storage', 'File storage provider: Local or S3'),
  (18, 'Storage.MaxFileSizeMb', '10', 'Int', 'Storage', 'Maximum upload size in megabytes'),
  (19, 'Storage.AllowedExtensions', 'pdf,jpg,jpeg,png', 'String', 'Storage', 'Allowed file extensions'),
  (20, 'Localization.DefaultLanguage', 'es', 'String', 'Localization', 'Default interface language'),
  (21, 'Localization.AvailableLanguages', 'es,en', 'String', 'Localization', 'Languages available in the interface');

-- Usuario administrador inicial.
-- El hash BCrypt se genera desde la API; nunca se guarda la contraseña en texto plano.
-- INSERT INTO Users (FirstName, PaternalLastName, Email, Username, PasswordHash, MustChangePassword)
-- VALUES ('System', 'Administrator', 'admin@usystem.local', 'Administrator', '<BCryptHash>', 1);
-- INSERT INTO UserRoles (UserId, RoleId) VALUES (LAST_INSERT_ID(), 1);
""")
    WriteDocument("DataModel/Scripts/99_SeedData.sql", "\n".join(Parts))

    SourceLines = ["-- =====================================================================",
                   "-- U SYSTEM | Ejecuta todos los scripts en orden",
                   "-- Ejecutar desde la carpeta Scripts: mysql -u root -p < RunAll.sql",
                   "-- ====================================================================="]
    for ScriptName, _ in ScriptOrder:
        SourceLines.append(f"SOURCE {ScriptName}.sql;")
    WriteDocument("DataModel/Scripts/RunAll.sql", "\n".join(SourceLines) + "\n")


ScriptGroups = [
    ("01_Security", "Seguridad, roles, permisos y auditoría", 1, "Base"),
    ("02_AcademicCatalogs", "Catálogos académicos", 1, "Catálogo"),
    ("03_AcademicOperations", "Operación académica", 2, "Operación"),
    ("04_AcademicTracking", "Seguimiento académico", 3, "Seguimiento"),
    ("05_Finance", "Finanzas", 4, "Finanzas"),
    ("06_Extras", "Trámites, notificaciones y configuración", 4, "Complementario"),
]


def WriteDataModelReadme():
    Lines = ["# Modelo de datos", "",
             "- Motor: **MySQL 8**, InnoDB, `utf8mb4`.",
             f"- Base de datos: `{DatabaseName}`.",
             "- **PascalCase** en tablas y columnas, en inglés (ver `Conventions.md`).",
             "- Scripts en `Scripts/` (ejecutar con `RunAll.sql` o uno por uno en orden numérico).",
             "- Diccionario de campos en `Entities/` (un archivo por tabla).", "",
             "## Scripts", "", "| Script | Contenido | Fase |", "|---|---|---|"]
    for ScriptName, Title, Phase, _ in ScriptGroups:
        Lines.append(f"| `{ScriptName}.sql` | {Title} | {Phase} |")
    Lines += ["", "## Tablas por script", ""]
    for ScriptName, Title, _, _ in ScriptGroups:
        TablesInScript = [Table for Table in Tables if Table["Script"] == ScriptName]
        Lines.append(f"### `{ScriptName}.sql`")
        Lines.append("")
        Lines.append("| Tabla | Clase C# | Descripción |")
        Lines.append("|---|---|---|")
        for Table in TablesInScript:
            Lines.append(f"| [`{Table['Name']}`](Entities/{Table['Class']}.md) | `{Table['Class']}` | {Table['Name']} |")
        Lines.append("")
    Lines += ["## Vistas", "",
              "- `TranscriptView`: kardex por alumno (materias cursadas, calificación, créditos y estatus).",
              "- `AcademicProgressView`: créditos aprobados, porcentaje de avance y promedio por alumno.", "",
              "## Diagrama del núcleo", "", "```mermaid", "erDiagram",
              "    Users ||--o{ UserRoles : has", "    Roles ||--o{ UserRoles : has",
              "    Roles ||--o{ RolePermissions : grants", "    Permissions ||--o{ RolePermissions : in",
              "    Careers ||--o{ StudyPlans : has", "    StudyPlans ||--o{ StudyPlanSubjects : contains",
              "    Subjects ||--o{ StudyPlanSubjects : in", "    Subjects ||--o{ ClassGroups : taught_as",
              "    AcademicTerms ||--o{ ClassGroups : offers", "    Teachers ||--o{ ClassGroups : teaches",
              "    Careers ||--o{ Students : enrolls", "    StudyPlans ||--o{ Students : follows",
              "    Students ||--o{ StudentDocuments : submits", "    DocumentTypes ||--o{ StudentDocuments : of",
              "    Students ||--o{ Enrollments : makes", "    AcademicTerms ||--o{ Enrollments : in",
              "    Enrollments ||--o{ EnrollmentClassGroups : includes", "    ClassGroups ||--o{ EnrollmentClassGroups : in",
              "    Enrollments ||--o{ Grades : produces", "    ClassGroups ||--o{ Grades : evaluates",
              "    Students ||--o{ Charges : owes", "    PaymentConcepts ||--o{ Charges : of",
              "    Charges ||--o{ Payments : settled_by", "    Students ||--o{ Procedures : requests",
              "```", "",
              f"Total: **{len(Tables)} tablas** y **2 vistas**.", ""]
    WriteDocument("DataModel/README.md", "\n".join(Lines))


def Main():
    WriteScripts()
    WriteDataModelReadme()
    print(f"Data model generated: {len(Tables)} tables")


if __name__ == "__main__":
    Main()
