# Parámetros de configuración (`SystemSettings`)

Se editan desde el módulo **Settings** (permiso `Settings.Edit`) sin recompilar. Cada cambio queda en
`AuditLogs` y se aplica sin reiniciar la API.

| SettingKey | Valor inicial | Tipo | Categoría | Descripción |
|-----------|---------------|------|-----------|-------------|
| `Login.MaxFailedAttempts` | 5 | Int | Security | Intentos fallidos antes de bloquear |
| `Login.LockoutMinutes` | 15 | Int | Security | Minutos de bloqueo |
| `Password.MinLength` | 8 | Int | Security | Longitud mínima de contraseña |
| `Jwt.AccessTokenMinutes` | 15 | Int | Security | Vida del access token |
| `Jwt.RefreshTokenDays` | 7 | Int | Security | Vida del refresh token |
| `PasswordReset.ExpirationMinutes` | 30 | Int | Security | Vigencia del token de recuperación |
| `Grades.MaxScore` | 10 | Decimal | Grades | Calificación máxima |
| `Grades.MinPassingScore` | 7 | Decimal | Grades | Calificación mínima aprobatoria |
| `Grades.MaxRecurrenceAttempts` | 2 | Int | Grades | Intentos para recursar una materia reprobada |
| `Grades.MaxExtraordinaryAttempts` | 2 | Int | Grades | Intentos de extraordinario |
| `Grades.MaxRemedialAttempts` | 1 | Int | Grades | Intentos de regularización |
| `ReEnrollment.MaxSubjects` | 8 | Int | Academic | Máximo de materias por cuatrimestre |
| `Payments.SurchargeType` | Percentage | String | Finance | `Percentage` o `Amount` |
| `Payments.SurchargeValue` | 10 | Decimal | Finance | Recargo del 10% sobre el saldo |
| `Payments.InstallmentsPerTerm` | 4 | Int | Finance | Colegiaturas mensuales por cuatrimestre |
| `Students.NumberFormat` | YYYY-CCC-NNNN | String | Students | Formato de matrícula |
| `Storage.Provider` | S3 | String | Storage | `Local` o `S3` |
| `Storage.MaxFileSizeMb` | 10 | Int | Storage | Tamaño máximo de archivo |
| `Storage.AllowedExtensions` | pdf,jpg,jpeg,png | String | Storage | Extensiones permitidas |
| `Localization.DefaultLanguage` | es | String | Localization | Idioma por defecto de la interfaz y de los mensajes |
| `Localization.AvailableLanguages` | es,en | String | Localization | Idiomas disponibles |

## Reglas

1. Las claves son fijas (semilla); solo se edita el valor.
2. El servicio valida el tipo y el rango según `ValueType` antes de guardar.
3. La caché en memoria se invalida al guardar.
4. Los secretos (cadena de conexión, JWT, SMTP y S3) **no** viven aquí: se manejan por variables de
   entorno.

## Valores de ejemplo

Los montos de los conceptos (`PaymentConcepts`) y de las becas son valores de ejemplo hasta que la
escuela defina los oficiales (ver `Architecture/PendingDecisions.md`).
