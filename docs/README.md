# U SYSTEM — Documentación

Sistema **administrativo** de gestión escolar universitaria (modelo cuatrimestral).

> **El alumno NO tiene acceso a la aplicación.** U SYSTEM es de uso exclusivo del personal de la escuela.
> La app del alumno es un proyecto futuro (ver `Architecture/StudentAppReadiness.md`).

## Reglas del proyecto

| # | Regla |
|---|-------|
| 1 | El sistema se llama **U SYSTEM**. |
| 2 | **Todo el código en inglés**: variables, clases, constantes, funciones, tablas, columnas, rutas de permisos y nombres de archivo. **Solo los comentarios van en español.** |
| 3 | **PascalCase en todo el código**, también en variables locales y parámetros. **Nada de snake_case ni camelCase.** En MySQL, tablas y columnas en PascalCase. |
| 4 | API: C# .NET 8 en capas (Controller → Service → Repository → Entity) con inversión e inyección de dependencias. |
| 5 | Desktop: Electron + React TS + AXZY UI System con patrón **MVVM** (Model, ViewModel, View). |
| 6 | Base de datos: MySQL 8 con cadena de conexión configurable. |

La regla 2 y la 3 se detallan, con las únicas excepciones que imponen los frameworks, en
[`Guides/CodingConventions.md`](Guides/CodingConventions.md).

## Estructura de esta carpeta

```
docs/
├── README.md
├── Api/              Convenciones, ejemplo por capas, endpoints por fase y entidades C#
├── Architecture/     Visión, stack, solución, capas de la API, MVVM del desktop, i18n, fases
├── AxzyUiSystem/     Documentación oficial de AXZY UI System (63 componentes)
├── DataModel/        Scripts SQL, convenciones y diccionario de datos por tabla
├── Guides/           Inicio rápido, convenciones de código, Git, cómo agregar un módulo, glosario
├── Modules/          Un documento por módulo (21 módulos)
├── Operations/       Despliegue, respaldos, secretos, almacenamiento, configuración y calendario
├── Security/         Política, matriz RBAC, datos personales y checklist
├── Templates/        Plantillas de módulo, entidad, endpoint, historia, caso de prueba, SQL y ADR
├── Testing/          Estrategia, criterios de aceptación y casos de prueba
└── Training/         Plan de capacitación por rol
```

## Por dónde empezar

1. [`Architecture/Overview.md`](Architecture/Overview.md) y [`Architecture/Phases.md`](Architecture/Phases.md).
2. [`Guides/QuickStart.md`](Guides/QuickStart.md) para levantar el ambiente.
3. [`DataModel/README.md`](DataModel/README.md) para crear la base de datos.
4. [`Modules/README.md`](Modules/README.md) para el detalle de cada módulo.
5. [`Guides/HowToAddAModule.md`](Guides/HowToAddAModule.md) antes de construir el primer módulo.
6. [`Architecture/Internationalization.md`](Architecture/Internationalization.md) y
   [`Operations/Storage.md`](Operations/Storage.md) para i18n y archivos.

## Fases

- **Fase 0 — Cimientos:** solución, capas, DI, MySQL, shell de Electron + React + AXZY UI.
- **Fase 1 — MVP:** Users, Roles, AuditLogs, Careers, StudyPlans, Subjects, AcademicTerms, Settings.
- **Fase 2 — Operación:** CourseMaterials, Teachers, ClassGroups, Students, Enrollments.
- **Fase 3 — Seguimiento:** ReEnrollments, Grades, Transcript, AcademicProgress.
- **Fase 4 — Finanzas y complementarios:** Payments, Procedures, Notifications, Reports.

## Pendientes de definición

Ver [`Architecture/PendingDecisions.md`](Architecture/PendingDecisions.md).
