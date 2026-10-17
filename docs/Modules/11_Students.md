# Módulo 11 — Alumnos y documentación

| | |
|---|---|
| **Fase** | Fase 2 — Operación académica |
| **Capa** | Operación |
| **Prefijo de permisos** | `Students.*` · `StudentDocuments.*` |
| **Depende de** | `Careers`, `StudyPlans`, `AcademicTerms` |
| **Tablas** | `Students`, `StudentGuardians`, `DocumentTypes`, `StudentDocuments` |
| **Controller** | `StudentsController` |

## 1. Objetivo

Expediente del alumno (datos personales, tutor, matrícula) y control de documentación. **El alumno es una entidad de negocio, no un usuario del sistema.**

## 2. Funcionalidades

- CRUD de alumnos con generación de matrícula (`StudentNumber`).
- Datos de tutores.
- Carga y validación de documentos por tipo.
- Cambio de estatus del alumno.
- Búsqueda por matrícula, CURP o nombre.

## 3. Reglas de negocio

1. `Curp` y `StudentNumber` únicos; la matrícula se genera con el formato `Students.NumberFormat`.
2. Estatus del alumno: Applicant, Active, TemporaryLeave, Dropped, Graduated, Titled.
3. Estatus del documento: Pending, Delivered, Validated, Rejected; el rechazo exige `Notes`.
4. El alumno conserva el `StudyPlanId` con el que ingresó.
5. El acceso a documentos está restringido por permiso (datos sensibles).

## 4. Modelo de datos

Script(s): `DataModel/Scripts/03_AcademicOperations.sql`

Entidades C# (en `Api/Entities/`): `Student`, `StudentGuardian`, `DocumentType`, `StudentDocument`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/students` | Listar alumnos (paginado, búsqueda, orden) | `Students.View` |
| GET | `/api/v1/students/{Id}` | Detalle | `Students.View` |
| POST | `/api/v1/students` | Crear alumnos | `Students.Create` |
| PUT | `/api/v1/students/{Id}` | Editar alumnos | `Students.Edit` |
| DELETE | `/api/v1/students/{Id}` | Baja lógica | `Students.Delete` |
| GET | `/api/v1/students/{Id}/documents` | Documentos del alumno | `StudentDocuments.View` |
| POST | `/api/v1/students/{Id}/documents/{DocumentTypeId}` | Cargar documento | `StudentDocuments.Upload` |
| POST | `/api/v1/students/{Id}/documents/{DocumentTypeId}/validate` | Validar documento | `StudentDocuments.Validate` |
| POST | `/api/v1/students/{Id}/documents/{DocumentTypeId}/reject` | Rechazar documento | `StudentDocuments.Reject` |
| PATCH | `/api/v1/students/{Id}/status` | Cambiar estatus | `Students.Edit` |
| GET | `/api/v1/document-types` | Catálogo de tipos de documento | `StudentDocuments.View` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Students/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/Student.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/StudentGuardian.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/StudentDocument.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/DocumentType.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/StudentsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useStudentListViewModel.ts` | Estado, validaciones y acciones de StudentListPage |
| View | `view/StudentListPage.tsx` | Listado y búsqueda |
| ViewModel | `viewmodel/useStudentFormViewModel.ts` | Estado, validaciones y acciones de StudentFormPage |
| View | `view/StudentFormPage.tsx` | Datos personales y tutor |
| ViewModel | `viewmodel/useStudentDocumentsViewModel.ts` | Estado, validaciones y acciones de StudentDocumentsPage |
| View | `view/StudentDocumentsPage.tsx` | Checklist y validación de documentos |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `03_AcademicOperations` ejecutado(s) y revisado(s).
- [ ] Permisos `Students.*` · `StudentDocuments.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `Student` (Entity) → `IStudentRepository` / `StudentRepository`
- [ ] `IStudentService` / `StudentService`; DTOs `StudentDto`, `CreateStudentRequest`, `UpdateStudentRequest`; validadores
- [ ] `StudentGuardian` (Entity) → `IStudentGuardianRepository` / `StudentGuardianRepository`; se gestiona desde el servicio del módulo (sin CRUD propio)
- [ ] `DocumentType` (Entity) → `IDocumentTypeRepository` / `DocumentTypeRepository`; solo consulta (sin CRUD)
- [ ] `StudentDocument` (Entity) → `IStudentDocumentRepository` / `StudentDocumentRepository`
- [ ] `IStudentDocumentService` / `StudentDocumentService`; DTOs `StudentDocumentDto`, `CreateStudentDocumentRequest`, `UpdateStudentDocumentRequest`; validadores
- [ ] La carga de documentos valida tipo y tamaño y guarda solo la referencia del archivo
- [ ] `IFileStorage` / `S3FileStorage` para guardar documentos en el bucket S3 y devolver URLs firmadas
- [ ] Validación de extensión y tamaño (`Storage.AllowedExtensions` y `Storage.MaxFileSizeMb`)
- [ ] `StudentsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddStudentsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `StudentsService`
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

- [ ] Se registra un alumno con matrícula automática.
- [ ] No se duplican CURP ni matrícula.
- [ ] Un documento rechazado exige observación.
