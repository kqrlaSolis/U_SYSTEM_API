// Entidad de la tabla ClassGroupSchedules.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class ClassGroupSchedule : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Grupo
    public int ClassGroupId { get; set; }

    // 1 = lunes ... 7 = domingo
    public byte DayOfWeek { get; set; }

    // Hora de inicio
    public TimeOnly StartTime { get; set; }

    // Hora de fin
    public TimeOnly EndTime { get; set; }
}
