# Definición del esquema de U SYSTEM.
# Fuente única: de aquí se generan los scripts SQL, las entidades C# y el diccionario de datos.
# Identificadores en inglés y PascalCase; comentarios en español.

# Columnas de auditoría que llevan todas las tablas
AuditColumns = [
    ("CreatedAt", "DATETIME", "DEF:CURRENT_TIMESTAMP", "Fecha de creación"),
    ("CreatedBy", "BIGINT", "NULL", "Usuario que creó el registro"),
    ("UpdatedAt", "DATETIME", "NULL", "Fecha de última modificación"),
    ("UpdatedBy", "BIGINT", "NULL", "Usuario que modificó el registro"),
    ("DeletedAt", "DATETIME", "NULL", "Borrado lógico"),
]


def Table(Name, ClassName, Script, Columns, Unique=None, Indexes=None):
    # Unique: lista de (nombre del constraint, columnas)
    # Indexes: lista de (nombre del índice, columnas)
    return {
        "Name": Name,
        "Class": ClassName,
        "Script": Script,
        "Columns": [(Column[0], Column[1], Column[2] if len(Column) > 2 else "",
                     Column[3] if len(Column) > 3 else "") for Column in Columns],
        "Unique": Unique or [],
        "Indexes": Indexes or [],
    }


Tables = [
    # ------------------------------------------------------------------
    # 01_Security
    # ------------------------------------------------------------------
    Table("Users", "User", "01_Security", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("FirstName", "VARCHAR(100)", "", "Nombre(s)"),
        ("PaternalLastName", "VARCHAR(100)", "", "Apellido paterno"),
        ("MaternalLastName", "VARCHAR(100)", "NULL", "Apellido materno"),
        ("Email", "VARCHAR(150)", "UQ", "Correo institucional"),
        ("Username", "VARCHAR(50)", "UQ", "Nombre de usuario"),
        ("PasswordHash", "VARCHAR(255)", "", "Hash BCrypt/Argon2"),
        ("Phone", "VARCHAR(20)", "NULL", "Teléfono"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Usuario activo"),
        ("MustChangePassword", "TINYINT(1)", "DEF:1", "Cambio obligatorio en el primer ingreso"),
        ("FailedAttempts", "INT", "DEF:0", "Intentos fallidos consecutivos"),
        ("LockedUntil", "DATETIME", "NULL", "Bloqueo temporal"),
        ("LastLoginAt", "DATETIME", "NULL", "Último inicio de sesión"),
    ]),
    Table("RefreshTokens", "RefreshToken", "01_Security", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("UserId", "BIGINT", "FK:Users", "Usuario dueño del token"),
        ("TokenHash", "VARCHAR(255)", "", "Hash del refresh token"),
        ("ExpiresAt", "DATETIME", "", "Vigencia"),
        ("RevokedAt", "DATETIME", "NULL", "Revocación"),
        ("IpAddress", "VARCHAR(45)", "NULL", "IP de origen"),
        ("UserAgent", "VARCHAR(255)", "NULL", "Agente de usuario"),
    ], Indexes=[("IxRefreshTokensUserId", ["UserId"])]),
    Table("PasswordResetTokens", "PasswordResetToken", "01_Security", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("UserId", "BIGINT", "FK:Users", "Usuario"),
        ("TokenHash", "VARCHAR(255)", "", "Hash del token de recuperación"),
        ("ExpiresAt", "DATETIME", "", "Vigencia de 30 minutos"),
        ("UsedAt", "DATETIME", "NULL", "Uso único"),
    ], Indexes=[("IxPasswordResetTokensUserId", ["UserId"])]),
    Table("Roles", "Role", "01_Security", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Name", "VARCHAR(80)", "UQ", "Nombre del rol"),
        ("Description", "VARCHAR(255)", "NULL", "Descripción"),
        ("IsSystem", "TINYINT(1)", "DEF:0", "Rol de sistema no eliminable"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Rol activo"),
    ]),
    Table("Permissions", "Permission", "01_Security", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(100)", "UQ", "Código Module.Action"),
        ("Module", "VARCHAR(50)", "", "Módulo al que pertenece"),
        ("Description", "VARCHAR(255)", "", "Descripción"),
    ]),
    Table("RolePermissions", "RolePermission", "01_Security", [
        ("RoleId", "INT", "PK FK:Roles", "Rol"),
        ("PermissionId", "INT", "PK FK:Permissions", "Permiso"),
    ]),
    Table("UserRoles", "UserRole", "01_Security", [
        ("UserId", "BIGINT", "PK FK:Users", "Usuario"),
        ("RoleId", "INT", "PK FK:Roles", "Rol"),
    ]),
    Table("AuditLogs", "AuditLog", "01_Security", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("UserId", "BIGINT", "NULL FK:Users", "Usuario que ejecutó la acción"),
        ("Action", "VARCHAR(50)", "", "Acción (Create, Update, Delete, Login)"),
        ("Module", "VARCHAR(50)", "", "Módulo afectado"),
        ("Entity", "VARCHAR(50)", "", "Entidad afectada"),
        ("EntityId", "VARCHAR(50)", "NULL", "Identificador del registro"),
        ("OldValue", "JSON", "NULL", "Valores anteriores"),
        ("NewValue", "JSON", "NULL", "Valores nuevos"),
        ("IpAddress", "VARCHAR(45)", "NULL", "IP de origen"),
    ], Indexes=[("IxAuditLogsModuleCreatedAt", ["Module", "CreatedAt"]),
                ("IxAuditLogsUserId", ["UserId"])]),

    # ------------------------------------------------------------------
    # 02_AcademicCatalogs
    # ------------------------------------------------------------------
    Table("Careers", "Career", "02_AcademicCatalogs", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(20)", "UQ", "Clave de la carrera"),
        ("Name", "VARCHAR(150)", "", "Nombre"),
        ("Level", "VARCHAR(30)", "", "Bachelor, Engineering, Master"),
        ("TermCount", "TINYINT", "", "Duración en cuatrimestres"),
        ("TotalCredits", "SMALLINT", "", "Créditos totales"),
        ("Modality", "VARCHAR(20)", "", "OnSite, Online, Hybrid"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activa"),
    ]),
    Table("Subjects", "Subject", "02_AcademicCatalogs", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(20)", "UQ", "Clave de la materia"),
        ("Name", "VARCHAR(150)", "", "Nombre"),
        ("Credits", "TINYINT", "", "Créditos"),
        ("TheoryHours", "TINYINT", "", "Horas teóricas"),
        ("PracticeHours", "TINYINT", "", "Horas prácticas"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activa"),
    ]),
    Table("AcademicTerms", "AcademicTerm", "02_AcademicCatalogs", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(10)", "UQ", "Clave YYYY-N"),
        ("Name", "VARCHAR(100)", "", "Nombre del periodo"),
        ("AcademicYear", "SMALLINT", "", "Año académico"),
        ("TermNumber", "TINYINT", "", "1, 2 o 3"),
        ("StartDate", "DATE", "", "Inicio del periodo"),
        ("EndDate", "DATE", "", "Fin del periodo"),
        ("EnrollmentStartDate", "DATE", "", "Inicio de inscripciones"),
        ("EnrollmentEndDate", "DATE", "", "Fin de inscripciones"),
        ("ReEnrollmentStartDate", "DATE", "", "Inicio de reinscripciones"),
        ("ReEnrollmentEndDate", "DATE", "", "Fin de reinscripciones"),
        ("ExamStartDate", "DATE", "", "Inicio de exámenes"),
        ("ExamEndDate", "DATE", "", "Fin de exámenes"),
        ("GradeReportCloseDate", "DATE", "", "Fecha de cierre de actas"),
        ("Status", "VARCHAR(20)", "DEF:Planned", "Planned, Open, InProgress, Closed"),
    ], Indexes=[("IxAcademicTermsStatus", ["Status"])]),
    Table("StudyPlans", "StudyPlan", "02_AcademicCatalogs", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("CareerId", "INT", "FK:Careers", "Carrera"),
        ("Code", "VARCHAR(20)", "", "Clave del plan"),
        ("Version", "VARCHAR(20)", "", "Versión"),
        ("StartDate", "DATE", "", "Inicio de vigencia"),
        ("EndDate", "DATE", "NULL", "Fin de vigencia"),
        ("TotalCredits", "SMALLINT", "", "Créditos totales"),
        ("IsCurrent", "TINYINT(1)", "DEF:0", "Vigente para nuevos ingresos"),
    ], Unique=[("UqStudyPlansCareerCodeVersion", ["CareerId", "Code", "Version"])]),
    Table("StudyPlanSubjects", "StudyPlanSubject", "02_AcademicCatalogs", [
        ("StudyPlanId", "INT", "PK FK:StudyPlans", "Plan de estudios"),
        ("SubjectId", "INT", "PK FK:Subjects", "Materia"),
        ("TermNumber", "TINYINT", "", "Cuatrimestre dentro del plan"),
        ("Type", "VARCHAR(20)", "DEF:Mandatory", "Mandatory o Elective"),
    ]),
    Table("SubjectPrerequisites", "SubjectPrerequisite", "02_AcademicCatalogs", [
        ("StudyPlanId", "INT", "PK FK:StudyPlans", "Plan de estudios"),
        ("SubjectId", "INT", "PK FK:Subjects", "Materia"),
        ("PrerequisiteSubjectId", "INT", "PK FK:Subjects", "Materia prerrequisito"),
    ]),
    Table("CourseMaterials", "CourseMaterial", "02_AcademicCatalogs", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("SubjectId", "INT", "FK:Subjects", "Materia"),
        ("AcademicTermId", "INT", "NULL FK:AcademicTerms", "Periodo (opcional)"),
        ("Type", "VARCHAR(20)", "", "Syllabus, Outline, Bibliography, File, Link"),
        ("Title", "VARCHAR(200)", "", "Título"),
        ("Description", "TEXT", "NULL", "Descripción"),
        ("Url", "VARCHAR(500)", "NULL", "Enlace externo"),
        ("FilePath", "VARCHAR(500)", "NULL", "Ruta del archivo"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activo"),
    ]),

    # ------------------------------------------------------------------
    # 03_AcademicOperations
    # ------------------------------------------------------------------
    Table("Teachers", "Teacher", "03_AcademicOperations", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("EmployeeNumber", "VARCHAR(20)", "UQ", "Número de empleado"),
        ("FirstName", "VARCHAR(100)", "", "Nombre(s)"),
        ("PaternalLastName", "VARCHAR(100)", "", "Apellido paterno"),
        ("MaternalLastName", "VARCHAR(100)", "NULL", "Apellido materno"),
        ("Email", "VARCHAR(150)", "UQ", "Correo"),
        ("Phone", "VARCHAR(20)", "NULL", "Teléfono"),
        ("Specialty", "VARCHAR(150)", "NULL", "Especialidad"),
        ("UserId", "BIGINT", "NULL FK:Users", "Usuario del sistema (opcional)"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activo"),
    ]),
    Table("TeacherSubjects", "TeacherSubject", "03_AcademicOperations", [
        ("TeacherId", "INT", "PK FK:Teachers", "Docente"),
        ("SubjectId", "INT", "PK FK:Subjects", "Materia habilitada"),
    ]),
    Table("ClassGroups", "ClassGroup", "03_AcademicOperations", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("AcademicTermId", "INT", "FK:AcademicTerms", "Periodo"),
        ("SubjectId", "INT", "FK:Subjects", "Materia"),
        ("TeacherId", "INT", "FK:Teachers", "Docente"),
        ("Code", "VARCHAR(20)", "", "Clave del grupo"),
        ("Classroom", "VARCHAR(50)", "NULL", "Aula"),
        ("Capacity", "SMALLINT", "", "Cupo"),
        ("EnrolledCount", "SMALLINT", "DEF:0", "Inscritos"),
        ("Status", "VARCHAR(20)", "DEF:Open", "Open, Closed, Cancelled"),
    ], Unique=[("UqClassGroupsTermCode", ["AcademicTermId", "Code"])],
       Indexes=[("IxClassGroupsTeacherId", ["TeacherId"])]),
    Table("ClassGroupSchedules", "ClassGroupSchedule", "03_AcademicOperations", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("ClassGroupId", "INT", "FK:ClassGroups", "Grupo"),
        ("DayOfWeek", "TINYINT", "", "1 = lunes ... 7 = domingo"),
        ("StartTime", "TIME", "", "Hora de inicio"),
        ("EndTime", "TIME", "", "Hora de fin"),
    ]),
    Table("Students", "Student", "03_AcademicOperations", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentNumber", "VARCHAR(20)", "UQ", "Matrícula"),
        ("CareerId", "INT", "FK:Careers", "Carrera"),
        ("StudyPlanId", "INT", "FK:StudyPlans", "Plan con el que ingresó"),
        ("EnrollmentTermId", "INT", "FK:AcademicTerms", "Periodo de ingreso"),
        ("FirstName", "VARCHAR(100)", "", "Nombre(s)"),
        ("PaternalLastName", "VARCHAR(100)", "", "Apellido paterno"),
        ("MaternalLastName", "VARCHAR(100)", "NULL", "Apellido materno"),
        ("Curp", "VARCHAR(18)", "UQ", "CURP"),
        ("BirthDate", "DATE", "", "Fecha de nacimiento"),
        ("Gender", "VARCHAR(20)", "", "Género"),
        ("Email", "VARCHAR(150)", "NULL", "Correo"),
        ("Phone", "VARCHAR(20)", "NULL", "Teléfono"),
        ("AddressLine", "VARCHAR(200)", "NULL", "Calle y número"),
        ("City", "VARCHAR(100)", "NULL", "Ciudad"),
        ("State", "VARCHAR(100)", "NULL", "Estado"),
        ("PostalCode", "VARCHAR(10)", "NULL", "Código postal"),
        ("Status", "VARCHAR(20)", "DEF:Applicant", "Applicant, Active, TemporaryLeave, Dropped, Graduated, Titled"),
    ], Indexes=[("IxStudentsFullName", ["PaternalLastName", "MaternalLastName", "FirstName"]),
                ("IxStudentsCareerId", ["CareerId"])]),
    Table("StudentGuardians", "StudentGuardian", "03_AcademicOperations", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("FullName", "VARCHAR(150)", "", "Nombre completo del tutor"),
        ("Relationship", "VARCHAR(50)", "", "Parentesco"),
        ("Phone", "VARCHAR(20)", "NULL", "Teléfono"),
        ("Email", "VARCHAR(150)", "NULL", "Correo"),
    ]),
    Table("DocumentTypes", "DocumentType", "03_AcademicOperations", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Name", "VARCHAR(100)", "UQ", "Nombre del documento"),
        ("IsMandatory", "TINYINT(1)", "DEF:1", "Obligatorio"),
        ("AppliesToLevel", "VARCHAR(30)", "NULL", "Nivel educativo al que aplica"),
    ]),
    Table("StudentDocuments", "StudentDocument", "03_AcademicOperations", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("DocumentTypeId", "INT", "FK:DocumentTypes", "Tipo de documento"),
        ("Status", "VARCHAR(20)", "DEF:Pending", "Pending, Delivered, Validated, Rejected"),
        ("FilePath", "VARCHAR(500)", "NULL", "Ruta del archivo"),
        ("Notes", "VARCHAR(500)", "NULL", "Observaciones"),
        ("ValidatedBy", "BIGINT", "NULL FK:Users", "Usuario que validó"),
        ("ValidatedAt", "DATETIME", "NULL", "Fecha de validación"),
    ], Unique=[("UqStudentDocumentsStudentType", ["StudentId", "DocumentTypeId"])]),
    Table("Enrollments", "Enrollment", "03_AcademicOperations", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("AcademicTermId", "INT", "FK:AcademicTerms", "Periodo"),
        ("CareerId", "INT", "FK:Careers", "Carrera"),
        ("Type", "VARCHAR(20)", "DEF:Enrollment", "Enrollment o ReEnrollment"),
        ("Status", "VARCHAR(20)", "DEF:Active", "Active o Cancelled"),
        ("Notes", "VARCHAR(500)", "NULL", "Observaciones"),
    ], Unique=[("UqEnrollmentsStudentTerm", ["StudentId", "AcademicTermId"])],
       Indexes=[("IxEnrollmentsTermStatus", ["AcademicTermId", "Status"])]),
    Table("EnrollmentClassGroups", "EnrollmentClassGroup", "03_AcademicOperations", [
        ("EnrollmentId", "BIGINT", "PK FK:Enrollments", "Inscripción"),
        ("ClassGroupId", "INT", "PK FK:ClassGroups", "Grupo"),
    ]),

    # ------------------------------------------------------------------
    # 04_AcademicTracking
    # ------------------------------------------------------------------
    Table("Grades", "Grade", "04_AcademicTracking", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("EnrollmentId", "BIGINT", "FK:Enrollments", "Inscripción"),
        ("ClassGroupId", "INT", "FK:ClassGroups", "Grupo"),
        ("Partial1", "DECIMAL(5,2)", "NULL", "Parcial 1"),
        ("Partial2", "DECIMAL(5,2)", "NULL", "Parcial 2"),
        ("Partial3", "DECIMAL(5,2)", "NULL", "Parcial 3"),
        ("Ordinary", "DECIMAL(5,2)", "NULL", "Calificación ordinaria"),
        ("FinalScore", "DECIMAL(5,2)", "NULL", "Calificación final"),
        ("AttemptNumber", "TINYINT", "DEF:1", "Número de intento (ordinario, extraordinario, regularización)"),
        ("EvaluationType", "VARCHAR(20)", "DEF:Ordinary", "Ordinary, Extraordinary, Remedial"),
        ("Status", "VARCHAR(20)", "DEF:InProgress", "InProgress, Passed, Failed, NoRight, NotPresented"),
        ("IsReportClosed", "TINYINT(1)", "DEF:0", "Acta cerrada"),
        ("ClosedAt", "DATETIME", "NULL", "Fecha de cierre"),
        ("ClosedBy", "BIGINT", "NULL FK:Users", "Usuario que cerró el acta"),
    ], Unique=[("UqGradesEnrollmentGroupAttempt", ["EnrollmentId", "ClassGroupId", "AttemptNumber"])],
       Indexes=[("IxGradesClassGroupId", ["ClassGroupId"])]),

    # ------------------------------------------------------------------
    # 05_Finance
    # ------------------------------------------------------------------
    Table("PaymentConcepts", "PaymentConcept", "05_Finance", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(20)", "UQ", "Clave del concepto"),
        ("Name", "VARCHAR(150)", "", "Nombre"),
        ("BaseAmount", "DECIMAL(12,2)", "", "Monto base"),
        ("IsRecurring", "TINYINT(1)", "DEF:0", "Recurrente"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activo"),
    ]),
    Table("PaymentPlans", "PaymentPlan", "05_Finance", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("CareerId", "INT", "NULL FK:Careers", "Carrera (opcional)"),
        ("AcademicTermId", "INT", "FK:AcademicTerms", "Periodo"),
        ("Name", "VARCHAR(150)", "", "Nombre del plan"),
        ("InstallmentCount", "TINYINT", "", "Número de colegiaturas"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activo"),
    ]),
    Table("Scholarships", "Scholarship", "05_Finance", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Name", "VARCHAR(150)", "UQ", "Nombre de la beca"),
        ("Type", "VARCHAR(20)", "DEF:Percentage", "Percentage o Amount"),
        ("Value", "DECIMAL(12,2)", "", "Valor de la beca"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activa"),
    ]),
    Table("StudentScholarships", "StudentScholarship", "05_Finance", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("ScholarshipId", "INT", "FK:Scholarships", "Beca"),
        ("AcademicTermId", "INT", "FK:AcademicTerms", "Periodo"),
        ("AssignedAt", "DATETIME", "DEF:CURRENT_TIMESTAMP", "Fecha de asignación"),
    ], Unique=[("UqStudentScholarshipsStudentTerm", ["StudentId", "AcademicTermId"])]),
    Table("Charges", "Charge", "05_Finance", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("AcademicTermId", "INT", "FK:AcademicTerms", "Periodo"),
        ("PaymentConceptId", "INT", "FK:PaymentConcepts", "Concepto"),
        ("Amount", "DECIMAL(12,2)", "", "Monto"),
        ("Discount", "DECIMAL(12,2)", "DEF:0", "Descuento"),
        ("Surcharge", "DECIMAL(12,2)", "DEF:0", "Recargo"),
        ("DueDate", "DATE", "", "Fecha de vencimiento"),
        ("Status", "VARCHAR(20)", "DEF:Pending", "Pending, Partial, Paid, Overdue, Cancelled"),
    ], Indexes=[("IxChargesStudentStatus", ["StudentId", "Status"]),
                ("IxChargesDueDate", ["DueDate"])]),
    Table("Payments", "Payment", "05_Finance", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("ChargeId", "BIGINT", "FK:Charges", "Cargo"),
        ("Amount", "DECIMAL(12,2)", "", "Monto pagado"),
        ("Method", "VARCHAR(20)", "", "Cash, Transfer, Card, Deposit"),
        ("Reference", "VARCHAR(100)", "NULL", "Referencia"),
        ("PaymentDate", "DATETIME", "DEF:CURRENT_TIMESTAMP", "Fecha del pago"),
        ("ReceiptNumber", "VARCHAR(30)", "UQ", "Folio de recibo"),
        ("UserId", "BIGINT", "FK:Users", "Usuario de caja"),
        ("Status", "VARCHAR(20)", "DEF:Applied", "Applied o Cancelled"),
        ("CancelledAt", "DATETIME", "NULL", "Fecha de cancelación"),
        ("CancellationReason", "VARCHAR(255)", "NULL", "Motivo de cancelación"),
    ]),

    # ------------------------------------------------------------------
    # 06_Extras
    # ------------------------------------------------------------------
    Table("ProcedureTypes", "ProcedureType", "06_Extras", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(20)", "UQ", "Clave del trámite"),
        ("Name", "VARCHAR(150)", "", "Nombre"),
        ("PaymentConceptId", "INT", "NULL FK:PaymentConcepts", "Cobro asociado (opcional)"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activo"),
    ]),
    Table("Procedures", "Procedure", "06_Extras", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("ProcedureTypeId", "INT", "FK:ProcedureTypes", "Tipo de trámite"),
        ("StudentId", "BIGINT", "FK:Students", "Alumno"),
        ("FolioNumber", "VARCHAR(30)", "UQ", "Folio consecutivo"),
        ("Status", "VARCHAR(20)", "DEF:Requested", "Requested, InProcess, Issued, Delivered, Cancelled"),
        ("RequestedAt", "DATETIME", "DEF:CURRENT_TIMESTAMP", "Fecha de solicitud"),
        ("IssuedAt", "DATETIME", "NULL", "Fecha de emisión"),
        ("FilePath", "VARCHAR(500)", "NULL", "Ruta del PDF emitido"),
        ("UserId", "BIGINT", "FK:Users", "Usuario que atiende"),
    ]),
    Table("NotificationTemplates", "NotificationTemplate", "06_Extras", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("Code", "VARCHAR(50)", "UQ", "Clave de la plantilla"),
        ("SubjectLine", "VARCHAR(200)", "", "Asunto"),
        ("Body", "TEXT", "", "Cuerpo con variables {{StudentName}}"),
        ("IsActive", "TINYINT(1)", "DEF:1", "Activa"),
    ]),
    Table("Notifications", "Notification", "06_Extras", [
        ("Id", "BIGINT", "PK AI", "Llave primaria"),
        ("TemplateId", "INT", "FK:NotificationTemplates", "Plantilla"),
        ("StudentId", "BIGINT", "NULL FK:Students", "Alumno (opcional)"),
        ("RecipientEmail", "VARCHAR(150)", "", "Destinatario"),
        ("Status", "VARCHAR(20)", "DEF:Pending", "Pending, Sent, Error"),
        ("SentAt", "DATETIME", "NULL", "Fecha de envío"),
        ("ErrorMessage", "VARCHAR(500)", "NULL", "Error de SMTP"),
        ("RetryCount", "TINYINT", "DEF:0", "Reintentos"),
    ], Indexes=[("IxNotificationsStatus", ["Status"])]),
    Table("SystemSettings", "SystemSetting", "06_Extras", [
        ("Id", "INT", "PK AI", "Llave primaria"),
        ("SettingKey", "VARCHAR(100)", "UQ", "Clave del parámetro"),
        ("SettingValue", "VARCHAR(500)", "", "Valor"),
        ("ValueType", "VARCHAR(20)", "DEF:String", "String, Int, Decimal, Bool"),
        ("Category", "VARCHAR(50)", "", "Categoría"),
        ("Description", "VARCHAR(255)", "NULL", "Descripción"),
    ]),
]

# Índice por nombre de tabla y por clase
TableByName = {Table["Name"]: Table for Table in Tables}
TableByClass = {Table["Class"]: Table for Table in Tables}

# Scripts en el orden en que deben ejecutarse
ScriptOrder = [
    ("00_CreateDatabase", "Base de datos y usuario de aplicación"),
    ("01_Security", "Seguridad, roles, permisos y auditoría"),
    ("02_AcademicCatalogs", "Catálogos académicos"),
    ("03_AcademicOperations", "Operación académica"),
    ("04_AcademicTracking", "Seguimiento académico y vistas"),
    ("05_Finance", "Finanzas"),
    ("06_Extras", "Trámites, notificaciones y configuración"),
    ("99_SeedData", "Datos semilla"),
]
