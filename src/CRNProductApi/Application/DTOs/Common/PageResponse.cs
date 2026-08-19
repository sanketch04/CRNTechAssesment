namespace CRNProductApi.Application.DTOs.Common;

public class PageResponse<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }

    public PageResponse() { }

    public PageResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? 10 : pageSize;
        TotalPages = PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        HasPreviousPage = PageNumber > 1;
        HasNextPage = PageNumber < TotalPages;
    }
}
