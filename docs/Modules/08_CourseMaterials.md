# Módulo 08 — Material didáctico

| | |
|---|---|
| **Fase** | Fase 2 — Operación académica |
| **Capa** | Catálogo |
| **Prefijo de permisos** | `CourseMaterials.*` |
| **Depende de** | `Subjects`, `AcademicTerms` |
| **Tablas** | `CourseMaterials` |
| **Controller** | `CourseMaterialsController` |

## 1. Objetivo

Programas, temarios, bibliografía, archivos y enlaces asociados a una materia (y opcionalmente a un periodo).

## 2. Funcionalidades

- CRUD de material.
- Carga de archivos.
- Consulta por materia y periodo.

## 3. Reglas de negocio

1. Tipos permitidos: Syllabus, Outline, Bibliography, File, Link.
2. Formatos y tamaño máximo configurables.
3. En la base solo se guarda la referencia; el archivo va a almacenamiento configurable.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/02_AcademicCatalogs.sql`

Entidades C# (en `Api/Entities/`): `CourseMaterial`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/course-materials` | Listar material (paginado, búsqueda, orden) | `CourseMaterials.View` |
| GET | `/api/v1/course-materials/{Id}` | Detalle | `CourseMaterials.View` |
| POST | `/api/v1/course-materials` | Crear material | `CourseMaterials.Create` |
| PUT | `/api/v1/course-materials/{Id}` | Editar material | `CourseMaterials.Edit` |
| DELETE | `/api/v1/course-materials/{Id}` | Baja lógica | `CourseMaterials.Delete` |
| POST | `/api/v1/course-materials/{Id}/file` | Subir archivo | `CourseMaterials.Create` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/CourseMaterials/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/CourseMaterial.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/CourseMaterialsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useCourseMaterialListViewModel.ts` | Estado, validaciones y acciones de CourseMaterialListPage |
| View | `view/CourseMaterialListPage.tsx` | Listado por materia |
| ViewModel | `viewmodel/useCourseMaterialFormViewModel.ts` | Estado, validaciones y acciones de CourseMaterialFormPage |
| View | `view/CourseMaterialFormPage.tsx` | Alta con archivo o enlace |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `02_AcademicCatalogs` ejecutado(s) y revisado(s).
- [ ] Permisos `CourseMaterials.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `CourseMaterial` (Entity) → `ICourseMaterialRepository` / `CourseMaterialRepository`
- [ ] `ICourseMaterialService` / `CourseMaterialService`; DTOs `CourseMaterialDto`, `CreateCourseMaterialRequest`, `UpdateCourseMaterialRequest`; validadores
- [ ] `IFileStorage` / `S3FileStorage` para los archivos de material didáctico
- [ ] Validación de extensión y tamaño (`Storage.AllowedExtensions` y `Storage.MaxFileSizeMb`)
- [ ] `CourseMaterialsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddCourseMaterialsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `CourseMaterialsService`
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

- [ ] Se adjunta un archivo a una materia.
- [ ] Se rechazan formatos o tamaños no permitidos.
