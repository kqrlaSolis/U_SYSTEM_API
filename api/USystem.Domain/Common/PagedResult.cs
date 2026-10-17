// Resultado paginado que devuelve la API dentro de ApiResponse.
namespace USystem.Domain.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();

    public int TotalCount { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = PagedQuery.DefaultPageSize;

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasNextPage => Page < TotalPages;

    public bool HasPreviousPage => Page > 1;

    public static PagedResult<T> Create(List<T> Items, int TotalCount, PagedQuery Query) => new()
    {
        Items = Items,
        TotalCount = TotalCount,
        Page = Query.Page,
        PageSize = Query.PageSize,
    };
}
