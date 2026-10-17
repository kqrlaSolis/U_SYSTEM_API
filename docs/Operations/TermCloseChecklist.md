# Checklist de cierre de periodo

- [ ] Todos los grupos tienen calificaciones capturadas.
- [ ] Todas las actas están cerradas (`Grades.IsReportClosed = 1`).
- [ ] Extraordinarios y regularizaciones capturados.
- [ ] Cargos vencidos revisados y recargos aplicados.
- [ ] Respaldo de base de datos realizado.
- [ ] Reporte de reprobación y deserción generado.
- [ ] Cierre ejecutado: `POST /api/v1/academic-terms/{Id}/close` (permiso `AcademicTerms.Close`).
- [ ] Siguiente periodo creado y en estado `Planned`.
- [ ] Calendario del siguiente periodo publicado al personal.
- [ ] Bitácora revisada (cierres, reaperturas y cancelaciones del periodo).
