# Ciclo operativo de un cuatrimestre

| Etapa | Estado del periodo (`AcademicTerms.Status`) | Acciones |
|-------|--------------------------------------------|----------|
| 1. Planeación | `Planned` | Crear el periodo y su calendario; crear grupos y asignar docentes y horarios (módulos 7 y 10) |
| 2. Inscripciones | `Open` | Inscripción de nuevo ingreso y reinscripciones; validación de documentos; generación de cargos |
| 3. Cursado | `InProgress` | Captura de parciales; cobro de colegiaturas; extraordinarios al final |
| 4. Cierre de actas | `InProgress` (fecha `GradeReportCloseDate`) | Captura final y cierre de actas por grupo |
| 5. Cierre | `Closed` | Verificación del checklist, bloqueo de cambios y respaldo |

## Periodos del año

| `TermNumber` | Meses |
|---|---|
| 1 | enero–abril |
| 2 | mayo–agosto |
| 3 | septiembre–diciembre |

El `Code` del periodo tiene el formato `YYYY-N` (por ejemplo `2026-1`).

## Reglas de estado

- Solo un periodo `InProgress` a la vez.
- Un periodo `Closed` es de solo lectura: no admite grupos, inscripciones ni calificaciones nuevas.
- Abrir y cerrar requiere permisos específicos (`AcademicTerms.Open`, `AcademicTerms.Close`).
- Las fechas de dos periodos no se traslapan.
