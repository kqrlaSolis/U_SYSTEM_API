# Módulo 09 — Docentes

| | |
|---|---|
| **Fase** | Fase 2 — Operación académica |
| **Capa** | Operación |
| **Prefijo de permisos** | `Teachers.*` |
| **Depende de** | `Subjects`, `Users` |
| **Tablas** | `Teachers`, `TeacherSubjects` |
| **Controller** | `TeachersController` |

## 1. Objetivo

Perfil de docentes y materias que pueden impartir.

## 2. Funcionalidades

- CRUD de docentes.
- Asignar materias habilitadas.
- Vincular opcionalmente a un usuario del sistema.

## 3. Reglas de negocio

1. `EmployeeNumber` y `Email` únicos.
2. Un docente con grupos asignados solo se desactiva.
3. `UserId` solo si el docente tendrá acceso a U SYSTEM.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/03_AcademicOperations.sql`

Entidades C# (en `Api/Entities/`): `Teacher`, `TeacherSubject`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/teachers` | Listar docentes (paginado, búsqueda, orden) | `Teachers.View` |
| GET | `/api/v1/teachers/{Id}` | Detalle | `Teachers.View` |
| POST | `/api/v1/teachers` | Crear docentes | `Teachers.Create` |
| PUT | `/api/v1/teachers/{Id}` | Editar docentes | `Teachers.Edit` |
| DELETE | `/api/v1/teachers/{Id}` | Baja lógica | `Teachers.Delete` |
| PUT | `/api/v1/teachers/{Id}/subjects` | Asignar materias habilitadas | `Teachers.Edit` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Teachers/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Teacher.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/TeacherSubject.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/TeachersService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useTeacherListViewModel.ts` | Estado, validaciones y acciones de TeacherListPage |
| View | `view/TeacherListPage.tsx` | Listado |
| ViewModel | `viewmodel/useTeacherFormViewModel.ts` | Estado, validaciones y acciones de TeacherFormPage |
| View | `view/TeacherFormPage.tsx` | Datos y materias habilitadas |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `03_AcademicOperations` ejecutado(s) y revisado(s).
- [ ] Permisos `Teachers.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Teacher` (Entity) → `ITeacherRepository` / `TeacherRepository`
- [ ] `ITeacherService` / `TeacherService`; DTOs `TeacherDto`, `CreateTeacherRequest`, `UpdateTeacherRequest`; validadores
- [ ] `TeacherSubject` (Entity, tabla de unión) → `ITeacherSubjectRepository` / `TeacherSubjectRepository`; se gestiona desde el servicio del módulo
- [ ] `TeachersController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddTeachersModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `TeachersService`
- [ ] ViewModels (hooks) con validaciones y manejo de errores
- [ ] Views con AXZY UI; ruta y entrada de menú protegidas por permiso

### D. Pruebas
- [ ] Unitarias de servicios (reglas de negocio de la sección 3)
- [ ] Integración de repositorios contra MySQL de pruebas
- [ ] Pruebas de ViewModels
- [ ] Caso de prueba por cada criterio de aceptación

### E. Cierre
- [ ] Criterios de aceptación cumplidos
- [ ] Revisión de seguridad (permisos, datos sensibles)
- [ ] Documentación actualizada

## 8. Criterios de aceptación

- [ ] Se registra un docente con sus materias.
- [ ] No se eliminan docentes con grupos.
