// Estados posibles de un periodo cuatrimestral.
namespace USystem.Domain.Constants;

public static class AcademicTermStatuses
{
    public const string Planned = "Planned";
    public const string Open = "Open";
    public const string InProgress = "InProgress";
    public const string Closed = "Closed";

    // Estados que cuentan como "periodo en curso" (solo puede haber uno)
    public static readonly IReadOnlyList<string> Active = new[] { Open, InProgress };
}
