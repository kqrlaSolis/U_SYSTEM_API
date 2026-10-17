# Catálogo de roles y permisos de U SYSTEM.
# Los códigos de permiso usan el formato Module.Action en inglés y PascalCase.

PermissionModules = [
    ("Users", [
        ("View", "View users"),
        ("Create", "Create users"),
        ("Edit", "Edit users and activate or deactivate accounts"),
    ]),
    ("Roles", [
        ("View", "View roles and permissions"),
        ("Create", "Create roles"),
        ("Edit", "Edit roles and their permissions"),
        ("Delete", "Delete or deactivate roles"),
        ("Assign", "Assign roles to users"),
    ]),
    ("AuditLogs", [
        ("View", "View the audit log"),
    ]),
    ("Careers", [
        ("View", "View careers"),
        ("Create", "Create careers"),
        ("Edit", "Edit careers"),
        ("Delete", "Delete or deactivate careers"),
    ]),
    ("StudyPlans", [
        ("View", "View study plans"),
        ("Create", "Create study plans"),
        ("Edit", "Edit study plans, curriculum and prerequisites"),
        ("Delete", "Delete or deactivate study plans"),
    ]),
    ("Subjects", [
        ("View", "View subjects"),
        ("Create", "Create subjects"),
        ("Edit", "Edit subjects"),
        ("Delete", "Delete or deactivate subjects"),
    ]),
    ("AcademicTerms", [
        ("View", "View academic terms"),
        ("Create", "Create academic terms"),
        ("Edit", "Edit academic terms"),
        ("Open", "Open an academic term for enrollment"),
        ("Close", "Close an academic term"),
    ]),
    ("CourseMaterials", [
        ("View", "View course materials"),
        ("Create", "Create course materials"),
        ("Edit", "Edit course materials"),
        ("Delete", "Delete or deactivate course materials"),
    ]),
    ("Teachers", [
        ("View", "View teachers"),
        ("Create", "Create teachers"),
        ("Edit", "Edit teachers and their enabled subjects"),
        ("Delete", "Delete or deactivate teachers"),
    ]),
    ("ClassGroups", [
        ("View", "View class groups and schedules"),
        ("Create", "Create class groups"),
        ("Edit", "Edit class groups and schedules"),
        ("Delete", "Delete or cancel class groups"),
    ]),
    ("Students", [
        ("View", "View students"),
        ("Create", "Create students"),
        ("Edit", "Edit students and change their status"),
        ("Delete", "Delete or deactivate students"),
    ]),
    ("StudentDocuments", [
        ("View", "View student documents"),
        ("Upload", "Upload student documents"),
        ("Validate", "Validate student documents"),
        ("Reject", "Reject student documents"),
    ]),
    ("Enrollments", [
        ("View", "View enrollments"),
        ("Create", "Create enrollments"),
        ("Cancel", "Cancel enrollments"),
    ]),
    ("ReEnrollments", [
        ("View", "View re-enrollments"),
        ("Create", "Create re-enrollments"),
        ("Cancel", "Cancel re-enrollments"),
        ("AuthorizeException", "Authorize re-enrollment exceptions"),
    ]),
    ("Grades", [
        ("View", "View grades"),
        ("Capture", "Capture grades"),
        ("Modify", "Modify grades after capture"),
        ("CloseReport", "Close a grade report"),
        ("ReopenReport", "Reopen a grade report"),
    ]),
    ("Transcript", [
        ("View", "View transcripts"),
        ("Export", "Export transcripts to PDF"),
    ]),
    ("AcademicProgress", [
        ("View", "View academic progress"),
    ]),
    ("Payments", [
        ("View", "View charges and payments"),
        ("Register", "Register payments and generate charges"),
        ("Cancel", "Cancel payments"),
        ("Waive", "Waive charges"),
    ]),
    ("Scholarships", [
        ("View", "View scholarships"),
        ("Create", "Create scholarships"),
        ("Edit", "Edit scholarships"),
        ("Assign", "Assign scholarships to students"),
    ]),
    ("Procedures", [
        ("View", "View procedures"),
        ("Request", "Request procedures"),
        ("Issue", "Issue procedure documents"),
    ]),
    ("Notifications", [
        ("View", "View notifications and templates"),
        ("Send", "Send notifications and edit templates"),
    ]),
    ("Reports", [
        ("View", "View reports and the dashboard"),
        ("Export", "Export reports"),
    ]),
    ("Settings", [
        ("View", "View system settings"),
        ("Edit", "Edit system settings"),
    ]),
]

Roles = [
    ("Administrator", "Administrador", "Full access to every module, users and roles included."),
    ("Registrar", "Control escolar", "Students, documents, enrollments, re-enrollments, transcripts and procedures."),
    ("Finance", "Finanzas / Caja", "Charges, payments, scholarships, receipts and account statements."),
    ("ProgramCoordinator", "Coordinador de carrera", "Careers, study plans, subjects, teachers, class groups and academic progress."),
    ("Teacher", "Docente", "Grade capture for assigned class groups."),
    ("ReadOnly", "Consulta", "Read-only access to every module."),
]


def AllCodes():
    Codes = []
    for Module, Actions in PermissionModules:
        for Action, _ in Actions:
            Codes.append(f"{Module}.{Action}")
    return Codes


def Expand(Module, Actions):
    # Convierte un módulo y una lista de acciones en códigos completos
    return [f"{Module}.{Action}" for Action in Actions]


RolePermissions = {
    "Administrator": AllCodes(),
    "Registrar": (
        Expand("Careers", ["View"]) + Expand("StudyPlans", ["View"]) + Expand("Subjects", ["View"])
        + Expand("AcademicTerms", ["View"]) + Expand("CourseMaterials", ["View"])
        + Expand("Teachers", ["View", "Create", "Edit"]) + Expand("ClassGroups", ["View"])
        + Expand("Students", ["View", "Create", "Edit", "Delete"])
        + Expand("StudentDocuments", ["View", "Upload", "Validate", "Reject"])
        + Expand("Enrollments", ["View", "Create", "Cancel"])
        + Expand("ReEnrollments", ["View", "Create", "Cancel"])
        + Expand("Grades", ["View"])
        + Expand("Transcript", ["View", "Export"]) + Expand("AcademicProgress", ["View"])
        + Expand("Payments", ["View"]) + Expand("Scholarships", ["View"])
        + Expand("Procedures", ["View", "Request", "Issue"])
        + Expand("Reports", ["View"])
    ),
    "Finance": (
        Expand("Careers", ["View"]) + Expand("AcademicTerms", ["View"])
        + Expand("Students", ["View"]) + Expand("StudentDocuments", ["View"])
        + Expand("Enrollments", ["View"]) + Expand("ReEnrollments", ["View"])
        + Expand("Grades", ["View"]) + Expand("Transcript", ["View"]) + Expand("AcademicProgress", ["View"])
        + Expand("Payments", ["View", "Register", "Cancel", "Waive"])
        + Expand("Scholarships", ["View", "Create", "Edit", "Assign"])
        + Expand("Procedures", ["View"]) + Expand("Notifications", ["View"])
        + Expand("Reports", ["View", "Export"])
    ),
    "ProgramCoordinator": (
        Expand("Careers", ["View", "Create", "Edit", "Delete"])
        + Expand("StudyPlans", ["View", "Create", "Edit", "Delete"])
        + Expand("Subjects", ["View", "Create", "Edit", "Delete"])
        + Expand("AcademicTerms", ["View"])
        + Expand("CourseMaterials", ["View", "Create", "Edit", "Delete"])
        + Expand("Teachers", ["View", "Create", "Edit", "Delete"])
        + Expand("ClassGroups", ["View", "Create", "Edit", "Delete"])
        + Expand("Students", ["View"]) + Expand("StudentDocuments", ["View"])
        + Expand("Enrollments", ["View"]) + Expand("ReEnrollments", ["View"])
        + Expand("Grades", ["View"]) + Expand("Transcript", ["View"]) + Expand("AcademicProgress", ["View"])
        + Expand("Notifications", ["View"]) + Expand("Reports", ["View", "Export"])
    ),
    "Teacher": (
        Expand("Careers", ["View"]) + Expand("Subjects", ["View"])
        + Expand("AcademicTerms", ["View"]) + Expand("CourseMaterials", ["View"])
        + Expand("Teachers", ["View"]) + Expand("ClassGroups", ["View"])
        + Expand("Students", ["View"])
        + Expand("Grades", ["View", "Capture"])
    ),
    # Consulta: todas las acciones de solo lectura del catálogo
    "ReadOnly": [Code for Code in AllCodes() if Code.endswith(".View")]
        + Expand("Transcript", ["View"]),
}

RolePermissions = {Role: sorted(set(Codes)) for Role, Codes in RolePermissions.items()}

# Verificación de integridad del catálogo semilla
KnownCodes = set(AllCodes())
for RoleName, Codes in RolePermissions.items():
    UnknownCodes = [Code for Code in Codes if Code not in KnownCodes]
    if UnknownCodes:
        raise ValueError(f"Unknown permission codes for {RoleName}: {UnknownCodes}")
