# Plan de refinamientos (UX + filtros + inscripción)

Objetivo: pulir la experiencia de U SYSTEM sobre lo ya construido (Fases 0–4 cerradas), sin
reabrir arquitectura. Cada bloque tiene criterios de aceptación y verificación igual que las fases.

## 1. Visual

### 1.1 Icono grande en el navbar y quitar el del sidebar
- `ITLayout` ya expone `topBar.logo` (ReactNode) y `logoText`; hoy el icono vive en `sidebar.brand`.
- Cambio en `app/layout/MainLayout.tsx`: `topBar.logo = <img src={UsystemIcon} className="USystemNavbarLogo" />`,
  y eliminar `sidebar.brand`.
- CSS: `.USystemNavbarLogo { width: 40px; height: 40px; border-radius: 12px; }` (alineado al top bar).
- Aceptación: logo grande al extremo izquierdo del navbar, sidebar sin marca, layout sin saltos al colapsar.
- Verificación: `tsc`, `build`, ajustar `MainLayout.test.tsx` si dependía del brand.

### 1.2 Resaltado del item en sidebar colapsado
- Síntoma: con el sidebar en modo icono, el fondo de énfasis cubre solo la mitad del icono.
- Causa: el padding/ancho del item activo de la librería no contempla el ancho del riel colapsado.
- Enfoque: inspeccionar clases reales del item en `@axzydev/axzy_ui_system/dist/index.css`, y en
  `styles/Global.css` (capa `components`) fijar `width: 100%`, `margin-inline: 0`, `padding` simétrico,
  `border-radius` y `justify-content: center` para el estado colapsado; usar `:where()` para no
  romper especificidad. Si el estado se controla por atributo (`data-collapsed`/`aria-*`), usarlo.
- Aceptación: el resaltado rodea completo el icono en colapsado, hover y activo; sin recortes ni
  desplazamientos; verificado en Electron (no solo en navegador).

### 1.3 Home page
- Hero: logo/banner + **“¡Hola, {Nombre}!”**, rol principal, fecha y **periodo en curso**.
- **Módulos más usados**: tarjetas con icono, título y descripción, filtradas por permisos y ordenadas
  por uso real (ver 3.1 contador). Enlaces a Dashboard, Alumnos, Inscripciones, Calificaciones,
  Trámites, Pagos y Catálogos.
- **Resumen rápido**: alumnos activos, grupos abiertos, cargos pendientes (monto) y trámites
  solicitados, reutilizando `GET /reports/dashboard` cuando el usuario tenga `Reports.View`.
- **Detalles técnicos** (colapsable): versión, URL del API, runtime e idioma (lo que hoy muestra la
  página de inicio).
- i18n: nuevas claves `Home.*`.

## 2. Filtros en tablas

### 2.1 API
- Extender los query DTOs (derivan de `PagedQuery`, con `[FromQuery]`):
  - `StudentQuery`: `CareerId`, `StudyPlanId`, `AcademicTermId`, `Status`.
  - `CareerQuery`: `Level`, `Modality`, `IsActive`.
  - `SubjectQuery`: `IsActive`, `StudyPlanId` (materias de un plan).
  - `StudyPlanQuery`: `CareerId`, `AcademicTermId`, `IsCurrent`.
- Repositorios: aplicar filtros combinados con `Search` y paginar después.
- Controladores: cambiar la firma a los nuevos DTOs sin romper compatibilidad (query opcional).

### 2.2 Desktop
- Componente compartido `shared/components/FilterBar.tsx` (selects + “Limpiar”, con chips de filtros activos).
- Hook `useFilterOptions` que carga carreras, periodos y planes (con cache por sesión).
- Los ViewModels de listado guardan filtros, los envían al servicio y **reinician la página** al cambiar.
- Aceptación: filtros combinables con búsqueda, opciones traducidas y valores por defecto vacíos.
- Verificación: pruebas de repositorio (integración gated) + pruebas de ViewModel + `tsc/build`.

## 3. Refinamientos

### 3.1 Dashboard con encabezado de periodo
- `DashboardDto` agrega: `CurrentTermId/Code/Name/Status/StartDate/EndDate`, `TermEnrollments`,
  `PendingChargesAmount`, `RequestedProcedures` (ya existe) y `ActiveStudents` (ya existe).
- La API toma el periodo en curso con `IAcademicTermRepository.GetCurrentAsync`/estatus `Open|InProgress`.
- Desktop: encabezado con **periodo en curso** (clave, nombre, estatus, rango de fechas) y aviso
  cuando no hay periodo abierto.
- Aceptación: con periodo abierto muestra datos; sin periodo muestra advertencia y agrega `0`.

### 3.2 Inscripción por pasos (documentación + pago)
Pasos: **1) Alumno → 2) Periodo y grupos → 3) Documentación → 4) Pago → 5) Confirmar**.

- API nueva:
  - `GET /students/{Id}/enrollment-checklist?academicTermId=` → documentos requeridos por
    `DocumentType` con estatus, resumen de cargos pendientes y grupos disponibles.
  - `POST /students/{Id}/documents` (tipo, estatus, ruta/URL, notas) reutilizando `StudentDocument`.
  - `POST /enrollments/{Id}/confirm` → exige documentos `Validated` y cargos del periodo saldados o
    autorizados; errores de negocio explícitos (`Enrollments.MissingDocuments`,
    `Enrollments.PaymentPending`).
  - `GET /charges?studentId&academicTermId` (aprovecha los filtros del punto 2).
- **Adjuntos**: el bucket S3 sigue en 403; para desarrollo se agrega `ILocalFileStorage`
  (carpeta `storage/`) detrás de `IFileStorage`, con S3 seleccionable por configuración.
- Desktop: asistente con pasos, validación por paso, resumen final y mensajes de la API.
- Aceptación: no se puede confirmar sin documentos validados ni pagos; el flujo se puede reanudar;
  auditoría de cada validación.

### 3.3 Mi perfil se queda cargando (bug)
- Causa: `useProfileViewModel` depende del objeto `Auth` completo; `ReloadCurrentUser` actualiza
  `CurrentUser`, el contexto cambia de identidad y el efecto vuelve a disparar `/auth/me` en bucle.
- Fix: depender solo de `Auth.ReloadCurrentUser` (memoizado) y proteger el montaje con `useRef`
  para StrictMode; mostrar el usuario desde el contexto.
- Aceptación: una sola llamada a `/auth/me`, sin parpadeo; prueba del ViewModel con mock del contexto.

## 4. Pendientes arrastrados (Fuera de este plan salvo lo indicado)

| Origen | Pendiente | Tipo |
|---|---|---|
| Fase 0 | Bucket S3 403 (política IAM) | Se mitiga con `LocalFileStorage` en 3.2 |
| Fase 0 | Refresh token en `localStorage` (falta `safeStorage`) | Seguridad |
| Fase 0 | Instaladores sin firma Authenticode | Distribución |
| Fase 0 | CI nunca ejecutado (sin repo git) | Proceso |
| Fase 0 | Esquema `USystem` de respaldo en Railway | Limpieza |
| Fase 1 | Pruebas de ViewModel de Periodos y Planes | Calidad |
| Fase 1 | Integración de repositorios contra MySQL de pruebas | Calidad |
| Fase 1 | Revisión de seguridad formal | Proceso |
| Fase 1 | Recuperación de contraseña sin SMTP | Producto |
| Fase 2 | Editor visual de horarios (hoy texto) | UX |
| Fase 2 | Selects dependientes (carrera/plan/periodo/grupos) | UX |
| Fase 2 | Botón de cancelar inscripción en desktop | Funcional |
| Fase 2 | Descontar `EnrolledCount` al cancelar inscripción | Regla |
| Fase 4 | Pantalla de planes de pago (solo API) | Funcional |
| Nuevo | `ELECTRON_RUN_AS_NODE`/IPv6 en `npm run dev` | ✅ resuelto |

## 5. Orden propuesto

| Sprint | Contenido | Verificación |
|---|---|---|
| **S0 (quick wins)** | 3.3 perfil, 1.1 logo navbar, 1.2 resaltado sidebar, 1.3 home | `tsc`, Vitest, `build`, prueba en Electron |
| **S1** | Filtros API + desktop (alumnos, carreras, materias, planes) | pruebas repo/VM, HTTP con filtros |
| **S2** | 3.1 dashboard con periodo en curso | pruebas API, HTTP, pantalla |
| **S3** | 3.2 inscripción por pasos (checklist, documentos, pago, confirmación) | pruebas de reglas, HTTP, pantalla |
| **S4** | Pendientes funcionales de Fase 2/4 (cancelar inscripción, cupo, selects, planes) | pruebas y pantallas |

Cada sprint cierra con: `dotnet build/test/format`, `npm tsc/lint/test/build` y smoke HTTP contra
MySQL `railway`; el `Backlog.md` se actualiza al terminar.
