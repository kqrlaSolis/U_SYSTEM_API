# Criterios de aceptación por módulo

Cada criterio debe tener al menos un caso de prueba (ver `Templates/TestCaseTemplate.md`).

## 01. Usuarios y autenticación (Fase 1)

- [ ] Un usuario activo con credenciales correctas obtiene tokens y entra al sistema.
- [ ] Tras 5 intentos fallidos la cuenta queda bloqueada el tiempo configurado.
- [ ] El token de recuperación funciona una sola vez y expira.
- [ ] Ninguna contraseña ni token se guarda en texto plano.
- [ ] Un usuario sin permiso no ve ni puede invocar la funcionalidad (UI y API).

## 02. Roles y permisos (RBAC) (Fase 1)

- [ ] Un usuario solo accede a lo que sus roles permiten (403 en la API sin permiso).
- [ ] Se crea un rol nuevo y se le asignan permisos desde la UI.
- [ ] No se elimina un rol de sistema ni uno con usuarios asignados.
- [ ] Los cambios de permisos quedan en bitácora.

## 03. Auditoría y bitácora (Fase 1)

- [ ] Cada acción crítica genera un registro con usuario, fecha e IP.
- [ ] No existe endpoint para modificar o borrar registros.

## 04. Carreras y programas (Fase 1)

- [ ] Se crea una carrera con clave única.
- [ ] No se puede eliminar una carrera con planes o alumnos.

## 05. Planes de estudio (Fase 1)

- [ ] Se arma una malla por cuatrimestre con seriación válida.
- [ ] Al marcar un plan vigente, el anterior deja de serlo.
- [ ] Se rechaza la seriación circular.

## 06. Materias (Fase 1)

- [ ] Se crea una materia con clave única.
- [ ] No se elimina una materia en uso.

## 07. Periodos cuatrimestrales (Fase 1)

- [ ] Se crea el calendario de un cuatrimestre.
- [ ] Un periodo cerrado no admite cambios.
- [ ] No se permiten dos periodos en curso.

## 08. Material didáctico (Fase 2)

- [ ] Se adjunta un archivo a una materia.
- [ ] Se rechazan formatos o tamaños no permitidos.

## 09. Docentes (Fase 2)

- [ ] Se registra un docente con sus materias.
- [ ] No se eliminan docentes con grupos.

## 10. Grupos y horarios (Fase 2)

- [ ] Se detecta el empalme de horario de un docente.
- [ ] No se excede el cupo.

## 11. Alumnos y documentación (Fase 2)

- [ ] Se registra un alumno con matrícula automática.
- [ ] No se duplican CURP ni matrícula.
- [ ] Un documento rechazado exige observación.

## 12. Inscripción (primer ingreso) (Fase 2)

- [ ] No se inscribe sin documentos validados ni pago.
- [ ] Cancelar libera el cupo del grupo.

## 13. Reinscripción (Fase 3)

- [ ] Se bloquea la reinscripción con adeudo vencido.
- [ ] Se bloquea una materia sin prerrequisito aprobado.
- [ ] La excepción autorizada queda en bitácora.

## 14. Calificaciones (Fase 3)

- [ ] Un acta cerrada no se modifica sin reapertura.
- [ ] El cálculo final respeta la ponderación y el mínimo aprobatorio.

## 15. Kardex (Fase 3)

- [ ] El kardex coincide con las calificaciones capturadas.
- [ ] El PDF se genera con el formato institucional.

## 16. Avance académico (Fase 3)

- [ ] El porcentaje de avance coincide con los créditos aprobados.
- [ ] Las materias repetidas no duplican créditos.

## 17. Pagos y colegiaturas (Fase 4)

- [ ] Se registra un pago y se emite recibo con folio único.
- [ ] Un pago cancelado conserva su historial.
- [ ] Un adeudo vencido bloquea la reinscripción.

## 18. Trámites y constancias (Fase 4)

- [ ] Se emite una constancia con folio único.
- [ ] No se emite un trámite con cobro pendiente.

## 19. Notificaciones (Fase 4)

- [ ] Se envía un correo con la plantilla y las variables resueltas.
- [ ] Un error de SMTP deja el registro en estatus Error con reintento.

## 20. Reportes y dashboard (Fase 4)

- [ ] Los indicadores coinciden con los datos operativos.
- [ ] La exportación respeta permisos.

## 21. Configuración del sistema (Fase 1)

- [ ] Un cambio de parámetro se aplica sin reiniciar la API.
- [ ] El cambio queda en bitácora.
