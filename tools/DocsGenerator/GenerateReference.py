# Genera la matriz RBAC, la capacitación por rol y los casos de prueba de la fase 1.
# Los identificadores van en inglés y PascalCase; la documentación va en español.

from Paths import WriteDocument
from Rbac import PermissionModules, Roles, RolePermissions

RoleSpanish = {Name: Spanish for Name, Spanish, _ in Roles}
RoleDescriptionSpanish = {
    "Administrator": "Acceso total, gestión de usuarios y roles.",
    "Registrar": "Alumnos, documentación, inscripciones, reinscripciones, kardex y trámites.",
    "Finance": "Cargos, pagos, becas, recibos y estados de cuenta.",
    "ProgramCoordinator": "Carreras, planes, materias, docentes, grupos y avance académico.",
    "Teacher": "Captura de calificaciones de sus grupos.",
    "ReadOnly": "Solo lectura.",
}


def CellFor(Module, Actions, RoleName):
    # Leyenda: View cuando solo tiene lectura, Full cuando tiene todas las acciones
    Codes = RolePermissions[RoleName]
    Granted = [Action for Action, _ in Actions if f"{Module}.{Action}" in Codes]
    if not Granted:
        return "—"
    if Granted == ["View"]:
        return "View"
    if len(Granted) == len(Actions):
        return "Full"
    return ", ".join(Granted)


def WriteRbacMatrix():
    RoleNames = [Name for Name, _, _ in Roles]
    Lines = ["# Matriz RBAC", "",
             "Generada desde el catálogo semilla (`99_SeedData.sql`). Leyenda: "
             "**Full** = todas las acciones · **View** = solo lectura · lista = acciones concedidas · — = sin acceso.", "",
             "| Módulo | " + " | ".join(RoleSpanish[Name] for Name in RoleNames) + " |",
             "|---|" + "---|" * len(RoleNames)]
    for Module, Actions in PermissionModules:
        Cells = [CellFor(Module, Actions, RoleName) for RoleName in RoleNames]
        Lines.append(f"| `{Module}` | " + " | ".join(Cells) + " |")
    Lines += ["", "## Roles", "", "| Rol | Equivalente | Descripción |", "|---|---|---|"]
    Lines += [f"| `{Name}` | {RoleSpanish[Name]} | {RoleDescriptionSpanish[Name]} |" for Name, _, _ in Roles]
    Lines += ["", "## Catálogo de permisos", "", "| Código | Descripción |", "|---|---|"]
    for Module, Actions in PermissionModules:
        for Action, Description in Actions:
            Lines.append(f"| `{Module}.{Action}` | {Description} |")
    WriteDocument("Security/RbacMatrix.md", "\n".join(Lines) + "\n")


TrainingTopics = {
    "Administrator": ["Inicio de sesión y cambio de contraseña",
                      "Alta de usuarios y asignación de roles (módulos Users y Roles)",
                      "Edición de roles y permisos",
                      "Consulta de la bitácora (AuditLogs)",
                      "Parámetros del sistema (Settings)",
                      "Creación y cierre de periodos"],
    "Registrar": ["Alta de alumnos y matrícula",
                  "Carga y validación de documentos",
                  "Inscripción de nuevo ingreso",
                  "Reinscripción y excepciones",
                  "Kardex y avance académico",
                  "Trámites y constancias"],
    "Finance": ["Conceptos, planes de pago y becas",
                "Generación de cargos por periodo",
                "Registro de pagos y recibos",
                "Cancelación y condonación",
                "Estado de cuenta y morosidad"],
    "ProgramCoordinator": ["Carreras, planes y materias",
                           "Grupos, horarios y docentes",
                           "Seguimiento del avance académico",
                           "Reportes de reprobación"],
    "Teacher": ["Inicio de sesión",
                "Captura de calificaciones por grupo",
                "Consulta del acta de calificaciones"],
    "ReadOnly": ["Consulta de catálogos y expedientes",
                 "Uso de filtros, búsqueda y paginación",
                 "Exportación de reportes permitidos"],
}


def WriteTraining():
    Lines = ["# Plan de capacitación", "",
             "Capacitación por rol en el ambiente **Staging** con datos ficticios.", "",
             "| Rol | Documento | Duración sugerida |", "|---|---|---|"]
    Durations = {"Administrator": "3 h", "Registrar": "4 h", "Finance": "3 h",
                 "ProgramCoordinator": "2 h", "Teacher": "1 h", "ReadOnly": "1 h"}
    for Name, _, _ in Roles:
        Lines.append(f"| {RoleSpanish[Name]} | [{Name}.md]({Name}.md) | {Durations[Name]} |")
    Lines += ["", "## Método", "",
              "1. Demostración guiada por módulo.",
              "2. Ejercicios prácticos con casos ficticios.",
              "3. Evaluación breve y entrega de la guía rápida.",
              "4. Acompañamiento durante el primer periodo de uso.", ""]
    WriteDocument("Training/README.md", "\n".join(Lines))

    for Name, _, _ in Roles:
        Topics = TrainingTopics.get(Name, [])
        Lines = [f"# Capacitación — {RoleSpanish[Name]} (`{Name}`)", "", "## Temario", ""]
        Lines += [f"{Index}. {Topic}" for Index, Topic in enumerate(Topics, 1)]
        Lines += ["", "## Permisos del rol", "",
                  f"Ver `Security/RbacMatrix.md` (columna {RoleSpanish[Name]}).", "",
                  "## Evaluación", "",
                  "- [ ] Ejecutó el flujo principal sin ayuda.",
                  "- [ ] Identifica qué hacer ante un error de permisos (403).",
                  "- [ ] Conoce el procedimiento de soporte.", ""]
        WriteDocument(f"Training/{Name}.md", "\n".join(Lines))


TestCases = [
    ("Users", "Login correcto", "Usuario activo con credenciales válidas", "Recibe AccessToken y RefreshToken"),
    ("Users", "Contraseña incorrecta", "Usuario activo", "Responde 401 y aumenta FailedAttempts"),
    ("Users", "Bloqueo por intentos", "5 intentos fallidos consecutivos", "LockedUntil se establece y el login se rechaza"),
    ("Users", "Usuario inactivo", "IsActive = 0", "Login rechazado"),
    ("Users", "Recuperar contraseña", "Correo existente", "Respuesta genérica y token creado"),
    ("Users", "Correo inexistente", "Correo no registrado", "Misma respuesta genérica; no se crea token"),
    ("Users", "Token reutilizado", "Token de recuperación ya usado", "Rechazado con 400"),
    ("Users", "Correo duplicado", "Crear usuario con Email existente", "409 Users.DuplicateEmail"),
    ("Users", "Cambio obligatorio", "MustChangePassword = 1", "El primer login exige cambio de contraseña"),
    ("Roles", "Acceso sin permiso", "Usuario sin Users.Create", "POST /api/v1/users responde 403"),
    ("Roles", "Eliminar rol con usuarios", "Rol con usuarios asignados", "Rechazado"),
    ("Roles", "Eliminar rol de sistema", "IsSystem = 1", "Rechazado"),
    ("Roles", "Último administrador", "Un solo Administrator activo", "No se desactiva ni se le quita el rol"),
    ("AuditLogs", "Registro de acciones", "Crear un usuario", "AuditLog con OldValue nulo y NewValue"),
    ("Careers", "Clave duplicada", "Code existente", "409"),
    ("StudyPlans", "Seriación circular", "A requiere B y B requiere A", "Rechazado"),
    ("StudyPlans", "Plan vigente único", "Marcar un plan como vigente", "El anterior deja de ser vigente"),
    ("AcademicTerms", "Dos periodos en curso", "Ya existe uno InProgress", "Rechazado"),
    ("AcademicTerms", "Periodo cerrado", "Status = Closed", "No admite cambios"),
    ("Settings", "Cambio de parámetro", "Editar Login.MaxFailedAttempts", "Se aplica sin reiniciar y queda en bitácora"),
]


def WriteTestCases():
    Lines = ["# Casos de prueba — Fase 1", "",
             "| ID | Módulo | Caso | Precondición | Resultado esperado |",
             "|---|---|---|---|---|"]
    for Index, (Module, Case, Precondition, Expected) in enumerate(TestCases, 1):
        Lines.append(f"| TC-{Module}-{Index:03d} | {Module} | {Case} | {Precondition} | {Expected} |")
    WriteDocument("Testing/Phase1TestCases.md", "\n".join(Lines) + "\n")


def Main():
    WriteRbacMatrix()
    WriteTraining()
    WriteTestCases()
    print("Reference documents generated")


if __name__ == "__main__":
    Main()
