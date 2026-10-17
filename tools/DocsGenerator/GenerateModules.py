# Genera la documentación de los 21 módulos, los endpoints por fase y los criterios de aceptación.
# Los identificadores van en inglés y PascalCase; la documentación va en español.

from Paths import WriteDocument
from Schema import TableByName
from Modules import Modules, ModuleByFile, PhaseNames


def PageViewModelName(PageName):
    Base = PageName[:-4] if PageName.endswith("Page") else PageName
    if Base.endswith("Dialog"):
        Base = Base[:-6]
    return f"use{Base}ViewModel"


def PermissionPrefixes(Module):
    # Prefijos reales usados por los endpoints (un módulo puede tocar dos catálogos)
    Result = []
    for Method, Path, Description, Permission in Module["Endpoints"]:
        if Permission in ("Public", "Authenticated"):
            continue
        Prefix = Permission.split(".")[0]
        if Prefix not in Result:
            Result.append(Prefix)
    return Result or [Module["Permission"]]


def PermissionText(Module):
    return " · ".join(f"`{Prefix}.*`" for Prefix in PermissionPrefixes(Module))


def Controllers(Module):
    return Module.get("Controllers", [Module["Controller"]])


def Services(Module):
    return Module.get("Services", [Module["File"]])


def EntityClasses(Module):
    return [TableByName[Name]["Class"] for Name in Module["Tables"]]


# Entidades que se administran desde el servicio de otra entidad o que son de solo consulta
ManagedClasses = {"RefreshToken", "PasswordResetToken", "ClassGroupSchedule", "StudentGuardian", "StudentScholarship"}
ReadOnlyClasses = {"AuditLog", "DocumentType"}


def JoinTables(Module):
    Result = set()
    for Name in Module["Tables"]:
        Table = TableByName[Name]
        PrimaryKeys = [Column for Column in Table["Columns"] if "PK" in Column[2].split()]
        if len(PrimaryKeys) > 1:
            Result.add(Table["Class"])
    return Result


def ModuleDocument(Module):
    Classes = EntityClasses(Module)
    Join = JoinTables(Module)
    Scripts = sorted({TableByName[Name]["Script"] for Name in Module["Tables"]})
    ModuleName = Module["File"]

    Lines = [f"# Módulo {Module['Number']:02d} — {Module['Title']}", "",
             "| | |", "|---|---|",
             f"| **Fase** | {PhaseNames[Module['Phase']]} |",
             f"| **Capa** | {Module['Layer']} |",
             f"| **Prefijo de permisos** | {PermissionText(Module)} |",
             f"| **Depende de** | {', '.join(f'`{Dependency}`' for Dependency in Module['Depends']) or '—'} |",
             f"| **Tablas** | {', '.join(f'`{Name}`' for Name in Module['Tables']) or '—'}"
             + (f" · Vistas: {', '.join(f'`{View}`' for View in Module['Views'])}" if Module.get("Views") else "")
             + " |",
             f"| **Controller** | {', '.join(f'`{Name}`' for Name in Controllers(Module))} |", "",
             "## 1. Objetivo", "", Module["Objective"], "",
             "## 2. Funcionalidades", ""]
    Lines += [f"- {Feature}" for Feature in Module["Features"]]
    Lines += ["", "## 3. Reglas de negocio", ""]
    Lines += [f"{Index}. {Rule}" for Index, Rule in enumerate(Module["Rules"], 1)]

    Lines += ["", "## 4. Modelo de datos", ""]
    if Scripts:
        Lines.append("Script(s): " + ", ".join(f"`DataModel/Scripts/{Script}.sql`" for Script in Scripts))
        Lines.append("")
        Lines.append("Entidades C# (en `Api/Entities/`): " + ", ".join(f"`{Class}`" for Class in Classes))
        if Module.get("Reuses"):
            Lines.append("")
            Lines.append(f"Este módulo **reutiliza el modelo de {Module['Reuses']}**; no crea tablas nuevas.")
        Lines.append("")
        Lines.append("Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).")
    else:
        Lines.append("Este módulo no define tablas propias; consume tablas y vistas de otros módulos.")
        if Module.get("Views"):
            Lines.append("")
            Lines.append("Vistas: " + ", ".join(f"`{View}`" for View in Module["Views"])
                         + " (script `04_AcademicTracking.sql`).")

    Lines += ["", "## 5. Endpoints", "", "| Método | Ruta | Descripción | Permiso |", "|---|---|---|---|"]
    Lines += [f"| {Method} | `{Path}` | {Description} | `{Permission}` |"
              for Method, Path, Description, Permission in Module["Endpoints"]]

    Lines += ["", "## 6. Desktop (Electron + React TS, patrón MVVM)", "",
              f"Carpeta: `desktop/src/modules/{ModuleName}/`", "",
              "| Capa | Archivo | Responsabilidad |", "|---|---|---|"]
    for Model in Module["Models"]:
        Lines.append(f"| Model | `model/{Model}.ts` | Tipo o DTO que refleja el contrato de la API |")
    for ServiceName in Services(Module):
        Lines.append(f"| Model | `model/{ServiceName}Service.ts` | Llamadas HTTP al controlador del módulo |")
    for Page, Description in Module["Pages"]:
        Lines.append(f"| ViewModel | `viewmodel/{PageViewModelName(Page)}.ts` | Estado, validaciones y acciones de {Page} |")
        Lines.append(f"| View | `view/{Page}.tsx` | {Description} |")
    Lines += ["",
              "> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; "
              "el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.",
              "", "## 7. Plan de construcción", ""]

    Lines += ["### A. Base de datos"]
    if Scripts:
        Lines.append("- [ ] Script(s) " + ", ".join(f"`{Script}`" for Script in Scripts)
                     + " ejecutado(s) y revisado(s).")
    elif Module.get("Views"):
        Lines.append("- [ ] Vistas `" + "`, `".join(Module["Views"])
                     + "` creadas por `04_AcademicTracking.sql` y revisadas.")
    else:
        Lines.append("- [ ] Sin cambios de esquema; solo consume datos de otros módulos.")
    Lines += [f"- [ ] Permisos {PermissionText(Module)} presentes en `99_SeedData.sql` y asignados a roles.", "",
              "### B. API (.NET 8, capas)"]

    if Module["Number"] == 13:
        Lines += ["- [ ] Reutiliza `Enrollment`, `IEnrollmentRepository` y `EnrollmentClassGroup` del módulo 12 (sin entidades nuevas)",
                  "- [ ] `IReEnrollmentService` / `ReEnrollmentService` (seriación, adeudos, cupo y empalmes); "
                  "DTOs `ReEnrollmentDto`, `CreateReEnrollmentRequest`"]
        Lines += [f"- [ ] {Note}" for Note in Module.get("ApiNotes", [])]
    else:
        for Class in Classes:
            if Class in Join:
                Lines.append(f"- [ ] `{Class}` (Entity, tabla de unión) → `I{Class}Repository` / `{Class}Repository`; "
                             f"se gestiona desde el servicio del módulo")
            elif Class in ManagedClasses:
                Lines.append(f"- [ ] `{Class}` (Entity) → `I{Class}Repository` / `{Class}Repository`; "
                             f"se gestiona desde el servicio del módulo (sin CRUD propio)")
            elif Class in ReadOnlyClasses:
                Lines.append(f"- [ ] `{Class}` (Entity) → `I{Class}Repository` / `{Class}Repository`; "
                             f"solo consulta (sin CRUD)")
            else:
                Lines += [f"- [ ] `{Class}` (Entity) → `I{Class}Repository` / `{Class}Repository`",
                          f"- [ ] `I{Class}Service` / `{Class}Service`; DTOs `{Class}Dto`, "
                          f"`Create{Class}Request`, `Update{Class}Request`; validadores"]
        if not Classes:
            Lines += [f"- [ ] `I{ModuleName}Repository` / `{ModuleName}Repository` (consultas de lectura y exportación)",
                      f"- [ ] `I{ModuleName}Service` / `{ModuleName}Service` y DTOs de respuesta"]
        if Module["Number"] == 11:
            Lines.append("- [ ] La carga de documentos valida tipo y tamaño y guarda solo la referencia del archivo")
        Lines += [f"- [ ] {Note}" for Note in Module.get("ApiNotes", [])]

    Lines += [f"- [ ] `{Name}` con `[RequirePermission]` en cada acción" for Name in Controllers(Module)]
    Lines += [f"- [ ] Registro de DI en `Add{ModuleName}Module()`",
              "- [ ] Eventos de auditoría (`IAuditService`) donde aplique",
              "- [ ] Documentación Swagger y ejemplos", "",
              "### C. Desktop (MVVM)",
              f"- [ ] Model: tipos y `{ModuleName}Service`",
              "- [ ] ViewModels (hooks) con validaciones y manejo de errores",
              "- [ ] Views con AXZY UI; ruta y entrada de menú protegidas por permiso", "",
              "### D. Pruebas",
              "- [ ] Unitarias de servicios (reglas de negocio de la sección 3)",
              "- [ ] Integración de repositorios contra MySQL de pruebas",
              "- [ ] Pruebas de ViewModels",
              "- [ ] Caso de prueba por cada criterio de aceptación", "",
              "### E. Cierre",
              "- [ ] Criterios de aceptación cumplidos",
              "- [ ] Revisión de seguridad (permisos, datos sensibles)",
              "- [ ] Documentación actualizada", "",
              "## 8. Criterios de aceptación", ""]
    Lines += [f"- [ ] {Criterion}" for Criterion in Module["Accept"]]
    return "\n".join(Lines) + "\n"


def WriteModuleIndex():
    Lines = ["# Módulos de U SYSTEM", "",
             "Cada módulo tiene su documento con objetivo, reglas, datos, endpoints, "
             "estructura MVVM del desktop, plan de construcción y criterios de aceptación.", "",
             "| # | Módulo | Capa | Fase | Permisos | Documento |",
             "|---|---|---|---|---|---|"]
    for Module in Modules:
        Lines.append(f"| {Module['Number']} | {Module['Title']} | {Module['Layer']} | {Module['Phase']} | "
                     f"{PermissionText(Module)} | [{Module['Number']:02d}_{Module['File']}.md]"
                     f"({Module['Number']:02d}_{Module['File']}.md) |")
    Lines += ["", "## Orden por fase", ""]
    for Phase in range(1, 5):
        Lines.append(f"**{PhaseNames[Phase]}:** " + ", ".join(
            Module["File"] for Module in Modules if Module["Phase"] == Phase))
        Lines.append("")
    Lines += ["## Dependencias", "", "```",
              "Users → Roles → Settings",
              "Careers → StudyPlans ← Subjects",
              "AcademicTerms + Subjects + Teachers → ClassGroups",
              "Students + StudentDocuments + Payments → Enrollments",
              "ClassGroups + Enrollments → ReEnrollments → Grades → Transcript → AcademicProgress",
              "Payments <-> ReEnrollments (un adeudo vencido bloquea la reinscripción)",
              "```", ""]
    WriteDocument("Modules/README.md", "\n".join(Lines))


def WriteEndpointsByPhase():
    for Phase in range(1, 5):
        Lines = [f"# Endpoints — {PhaseNames[Phase]}", "",
                 "Prefijo `/api/v1`. Respuesta estándar `ApiResponse<T>` (ver `Api/Conventions.md`).", ""]
        for Module in [Module for Module in Modules if Module["Phase"] == Phase]:
            Lines += [f"## {Module['Number']:02d}. {Module['Title']} — `{Module['Controller']}`", "",
                      "| Método | Ruta | Descripción | Permiso |", "|---|---|---|---|"]
            Lines += [f"| {Method} | `{Path}` | {Description} | `{Permission}` |"
                      for Method, Path, Description, Permission in Module["Endpoints"]]
            Lines.append("")
        WriteDocument(f"Api/EndpointsPhase{Phase}.md", "\n".join(Lines))


def WriteAcceptanceCriteria():
    Lines = ["# Criterios de aceptación por módulo", "",
             "Cada criterio debe tener al menos un caso de prueba (ver `Templates/TestCaseTemplate.md`).", ""]
    for Module in Modules:
        Lines += [f"## {Module['Number']:02d}. {Module['Title']} (Fase {Module['Phase']})", ""]
        Lines += [f"- [ ] {Criterion}" for Criterion in Module["Accept"]]
        Lines.append("")
    WriteDocument("Testing/AcceptanceCriteria.md", "\n".join(Lines))


def WritePhases():
    Lines = ["# Fases de desarrollo", "",
             "Orden recomendado dentro de cada fase: **base de datos → API → desktop (MVVM) → pruebas → cierre**.", "",
             "## Fase 0 — Cimientos", "",
             "- [ ] Solución de Visual Studio `USystem.sln` con proyectos `USystem.Api`, `USystem.Application`, `USystem.Domain`, `USystem.Infrastructure` y `USystem.Tests`.",
             "- [ ] Conexión a MySQL por cadena de conexión (`ConnectionStrings:DefaultConnection`), Pomelo + EF Core 8.",
             "- [ ] Scripts `00_CreateDatabase` y `99_SeedData` ejecutados; estructura de capas con DI por módulo.",
             "- [ ] Middleware global de errores, `ApiResponse<T>`, paginación, Swagger y Serilog.",
             "- [ ] JWT y atributo `[RequirePermission]`.",
             "- [ ] Almacenamiento S3 (MinIO en desarrollo) y catálogos i18n base (`es`/`en`) de la API y del desktop.",
             "- [ ] Proyecto `desktop/` con Electron + React TS + AXZY UI, layout, router, cliente HTTP con interceptores y estructura MVVM.",
             "- [ ] Pipeline de build y convenciones de código (ver `Guides/CodingConventions.md`).", ""]
    for Phase in range(1, 5):
        Lines += [f"## {PhaseNames[Phase]}", "", "| Módulo | Documento | Entregable |", "|---|---|---|"]
        for Module in [Module for Module in Modules if Module["Phase"] == Phase]:
            Lines.append(f"| {Module['Title']} | [{Module['Number']:02d}_{Module['File']}]"
                         f"(../Modules/{Module['Number']:02d}_{Module['File']}.md) | {Module['Features'][0]} |")
        Lines.append("")
    Lines += ["## Futuro (fuera de alcance)", "", "App del alumno. Ver `Architecture/StudentAppReadiness.md`."]
    WriteDocument("Architecture/Phases.md", "\n".join(Lines) + "\n")


def Main():
    for Module in Modules:
        WriteDocument(f"Modules/{Module['Number']:02d}_{Module['File']}.md", ModuleDocument(Module))
    WriteModuleIndex()
    WriteEndpointsByPhase()
    WriteAcceptanceCriteria()
    WritePhases()
    print(f"Module documents generated: {len(Modules)}")


if __name__ == "__main__":
    Main()
