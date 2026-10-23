// Parámetros de paginación y filtros compartidos por los listados de la API.
namespace USystem.Domain.Common;

public class PagedQuery
{
    // Tope duro: evita que un cliente pida listados gigantes
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    private int _Page = 1;
    private int _PageSize = DefaultPageSize;

    public int Page
    {
        get => _Page;
        set => _Page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _PageSize;
        set => _PageSize = value < 1 ? 1 : (value > MaxPageSize ? MaxPageSize : value);
    }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }

    // --- Filtros opcionales: cada repositorio usa los que le aplican ---
    public int? CareerId { get; set; }

    public int? StudyPlanId { get; set; }

    public int? AcademicTermId { get; set; }

    public string? Status { get; set; }

    public string? Level { get; set; }

    public string? Modality { get; set; }

    public bool? IsActive { get; set; }

    // Solo planes de estudio: vigente o no
    public bool? IsCurrent { get; set; }

    // Desplazamiento que usan los repositorios al consultar
    public int Skip => (Page - 1) * PageSize;
}
