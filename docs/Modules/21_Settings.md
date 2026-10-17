# Módulo 21 — Configuración del sistema

| | |
|---|---|
| **Fase** | Fase 1 — MVP: seguridad y catálogos |
| **Capa** | Base |
| **Prefijo de permisos** | `Settings.*` |
| **Depende de** | `Roles` |
| **Tablas** | `SystemSettings` |
| **Controller** | `SettingsController` |

## Estado (Fase 1 en curso)

- [x] Listado de los 21 parámetros de la semilla agrupados por categoría.
- [x] Edición por clave con validación de tipo y rango (`SettingRules`) y respuesta traducida.
- [x] Caché en memoria invalidada al guardar: el cambio aplica sin reiniciar la API.
- [x] Auditoría de cada cambio con valor anterior y nuevo.
- [x] Pantalla de configuración en el desktop (`/settings`, agrupada por categoría y con guardado por fila).

## 1. Objetivo

Parámetros ajustables sin recompilar (intentos de login, escala de calificación, recargos y límites).

## 2. Funcionalidades

- Listado y edición de parámetros.
- Idioma por defecto y idiomas disponibles (`Localization.*`).
- Caché en memoria con invalidación al editar.

## 3. Reglas de negocio

1. Las claves son fijas (semilla); solo se edita el valor.
2. Se valida el tipo y el rango por clave.
3. Todo cambio queda en bitácora.

## 4. Modelo de datos

Script(s): `DataModel/Scripts/06_Extras.sql`

Entidades C# (en `Api/Entities/`): `SystemSetting`

Diccionario de campos en `DataModel/Entities/` (un archivo por tabla).

## 5. Endpoints

| Método | Ruta | Descripción | Permiso |
|---|---|---|---|
| GET | `/api/v1/settings` | Listar parámetros | `Settings.View` |
| PUT | `/api/v1/settings/{Key}` | Editar valor | `Settings.Edit` |

## 6. Desktop (Electron + React TS, patrón MVVM)

Carpeta: `desktop/src/modules/Settings/`

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Model | `model/SystemSetting.ts` | Tipo o DTO que refleja el contrato de la API |
| Model | `model/SettingsService.ts` | Llamadas HTTP al controlador del módulo |
| ViewModel | `viewmodel/useSettingsViewModel.ts` | Estado, validaciones y acciones de SettingsPage |
| View | `view/SettingsPage.tsx` | Edición agrupada por categoría |

> La View solo renderiza y delega eventos; el ViewModel (hook) contiene el estado y la lógica de presentación; el Model contiene los tipos y los servicios. Ver `Architecture/DesktopMvvm.md`.

## 7. Plan de construcción

### A. Base de datos
- [ ] Script(s) `06_Extras` ejecutado(s) y revisado(s).
- [ ] Permisos `Settings.*` presentes en `99_SeedData.sql` y asignados a roles.

### B. API (.NET 8, capas)
- [ ] `SystemSetting` (Entity) → `ISystemSettingRepository` / `SystemSettingRepository`
- [ ] `ISystemSettingService` / `SystemSettingService`; DTOs `SystemSettingDto`, `CreateSystemSettingRequest`, `UpdateSystemSettingRequest`; validadores
- [ ] `SettingsController` con `[RequirePermission]` en cada acción
- [ ] Registro de DI en `AddSettingsModule()`
- [ ] Eventos de auditoría (`IAuditService`) donde aplique
- [ ] Documentación Swagger y ejemplos

### C. Desktop (MVVM)
- [ ] Model: tipos y `SettingsService`
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

- [ ] Un cambio de parámetro se aplica sin reiniciar la API.
- [ ] El cambio queda en bitácora.
