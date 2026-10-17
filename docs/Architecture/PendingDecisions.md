# Decisiones del proyecto

## Cerradas

| Tema | Decisión |
|---|---|
| **Idioma del código** | Todo en inglés: variables, clases, constantes, archivos, tablas y columnas. Solo los comentarios en español. |
| **Estilo de nombres** | PascalCase en todo el código, incluidas variables locales y parámetros. Nada de snake_case ni camelCase. |
| **Base de datos** | MySQL 8 con tablas, columnas y restricciones en PascalCase. |
| **JSON de la API** | PascalCase. |
| **Patrón del desktop** | MVVM con React + TypeScript (Model, ViewModel, View). |
| **Llaves primarias** | `BIGINT` en alumnos y transacciones; `INT` en catálogos. |
| **Borrado** | Lógico en catálogos y expedientes; los pagos solo se cancelan. |
| **Matrícula** | Formato `YYYY-CCC-NNNN` (ej. `2026-ISC-0001`), configurable en `Students.NumberFormat`. |
| **Escala de calificación** | 0–10 con mínimo aprobatorio **7** (`Grades.MinPassingScore`). |
| **Reprobación** | Recursar hasta **2** veces, **2** extraordinarios y **1** regularización. Cada intento se guarda con `AttemptNumber`. |
| **Colegiaturas** | Esquema **mensual: 4 pagos por cuatrimestre** más el cargo de inscripción. |
| **Recargos** | **10% del saldo** al vencer (`Payments.SurchargeType = Percentage`, `SurchargeValue = 10`). |
| **Almacenamiento de archivos** | **Nube S3 compatible**: bucket `cfsp-s3-bucket-prod`, región `us-east-2`; en la base solo la referencia. |
| **Instalador** | **Windows, Linux y macOS** (electron-builder: nsis, AppImage/deb y dmg). |
| **Acceso del docente** | **Sí**, con rol `Teacher` y captura de calificaciones de sus grupos. |
| **Roles y permisos** | Aprobado el catálogo de **6 roles y 78 permisos** de `Security/RbacMatrix.md`. |
| **AXZY UI System** | Documentación oficial copiada en `docs/AxzyUiSystem/`; los componentes reales se mapean al construir la primera pantalla. |
| **Idioma de la interfaz** | **i18n en API y desktop**: español por defecto e inglés disponible. Ver `Architecture/Internationalization.md`. |

## Abiertas

- [ ] **Proveedor SMTP** para recuperación de contraseña y notificaciones. Queda **pendiente** (no fuera de
      alcance): la API ya lee `Smtp__Host`, `Smtp__Port`, `Smtp__User`, `Smtp__Password` y
      `Smtp__From`, así que solo falta elegir proveedor y cargar credenciales.
- [ ] **Montos oficiales de los conceptos de pago**: inscripción, colegiatura, extraordinario y constancia.
      Los valores del seed (`2500.00`, `3200.00`, `800.00`, `500.00`) son de ejemplo.
- [ ] **Permisos S3 del usuario `axzydev`**: las credenciales son válidas (STS responde), pero
      `head_bucket` devuelve `403`. Falta adjuntar la política de identidad y, si el bucket está en otra
      cuenta, la bucket policy (ver `Operations/Secrets.md` y `tools/CheckS3.py`).
- [ ] **Remitente y dominio de correo** para SPF/DKIM, cuando se defina el proveedor SMTP.

> Los secretos (cadena de conexión, JWT, SMTP y S3) nunca viven en la base de datos ni en el repositorio:
> se cargan por variables de entorno o *User Secrets*.
