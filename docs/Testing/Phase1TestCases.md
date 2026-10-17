# Casos de prueba — Fase 1

| ID | Módulo | Caso | Precondición | Resultado esperado |
|---|---|---|---|---|
| TC-Users-001 | Users | Login correcto | Usuario activo con credenciales válidas | Recibe AccessToken y RefreshToken |
| TC-Users-002 | Users | Contraseña incorrecta | Usuario activo | Responde 401 y aumenta FailedAttempts |
| TC-Users-003 | Users | Bloqueo por intentos | 5 intentos fallidos consecutivos | LockedUntil se establece y el login se rechaza |
| TC-Users-004 | Users | Usuario inactivo | IsActive = 0 | Login rechazado |
| TC-Users-005 | Users | Recuperar contraseña | Correo existente | Respuesta genérica y token creado |
| TC-Users-006 | Users | Correo inexistente | Correo no registrado | Misma respuesta genérica; no se crea token |
| TC-Users-007 | Users | Token reutilizado | Token de recuperación ya usado | Rechazado con 400 |
| TC-Users-008 | Users | Correo duplicado | Crear usuario con Email existente | 409 Users.DuplicateEmail |
| TC-Users-009 | Users | Cambio obligatorio | MustChangePassword = 1 | El primer login exige cambio de contraseña |
| TC-Roles-010 | Roles | Acceso sin permiso | Usuario sin Users.Create | POST /api/v1/users responde 403 |
| TC-Roles-011 | Roles | Eliminar rol con usuarios | Rol con usuarios asignados | Rechazado |
| TC-Roles-012 | Roles | Eliminar rol de sistema | IsSystem = 1 | Rechazado |
| TC-Roles-013 | Roles | Último administrador | Un solo Administrator activo | No se desactiva ni se le quita el rol |
| TC-AuditLogs-014 | AuditLogs | Registro de acciones | Crear un usuario | AuditLog con OldValue nulo y NewValue |
| TC-Careers-015 | Careers | Clave duplicada | Code existente | 409 |
| TC-StudyPlans-016 | StudyPlans | Seriación circular | A requiere B y B requiere A | Rechazado |
| TC-StudyPlans-017 | StudyPlans | Plan vigente único | Marcar un plan como vigente | El anterior deja de ser vigente |
| TC-AcademicTerms-018 | AcademicTerms | Dos periodos en curso | Ya existe uno InProgress | Rechazado |
| TC-AcademicTerms-019 | AcademicTerms | Periodo cerrado | Status = Closed | No admite cambios |
| TC-Settings-020 | Settings | Cambio de parámetro | Editar Login.MaxFailedAttempts | Se aplica sin reiniciar y queda en bitácora |
