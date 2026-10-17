// Filtros de la bitácora: heredan la paginación común y agregan el filtro por acción, módulo, entidad,
// usuario y rango de fechas.
namespace USystem.Domain.Common;

public class AuditLogQuery : PagedQuery
{
    public string? Action { get; set; }

    public string? Module { get; set; }

    public string? Entity { get; set; }

    public long? UserId { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}
