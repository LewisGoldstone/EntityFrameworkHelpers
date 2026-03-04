namespace EntityFrameworkHelpers.Pagination;

public class PaginatedList<T>
{
    internal const int DefaultPageSize = 50;

    private const string UnequalDataCountExceptionMessage = "Data count must be equal";

    public PaginatedList(IEnumerable<T> data, int pageNumber, int pageSize, int count)
    {
        SetPageNumber(pageNumber);
        SetPageSize(pageSize);

        Count = count;

        Data = data.ToList();
    }

    /// <summary>
    /// Creates instance of PaginatedList with specified paginated result
    /// </summary>
    /// <param name="data">Data collection</param>
    /// <param name="pageNumber">Page Number (defaults to 1)</param>
    /// <param name="pageSize">Page Size (defaults to DefaultPageSize)</param>
    public PaginatedList(IEnumerable<T> data, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        SetPageNumber(pageNumber);
        SetPageSize(pageSize);

        Count = data.Count();

        Data = data
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
    }

    /// <summary>
    /// Creates instance of PaginatedList with specified paginated result
    /// </summary>
    /// <param name="data">Query</param>
    /// <param name="pageNumber">Page Number (defaults to 1)</param>
    /// <param name="pageSize">Page Size (defaults to DefaultPageSize)</param>
    public PaginatedList(IQueryable<T> data, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        SetPageNumber(pageNumber);
        SetPageSize(pageSize);

        Count = data.Count();

        Data = data
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
    }

    private PaginatedList() { }

    /// <summary>
    /// Creates empty Paginated List with no results and default values
    /// </summary>
    /// <returns>PaginatedList</returns>
    public static PaginatedList<T> Empty()
    {
        return new PaginatedList<T>()
        {
            Data = new List<T>(),
            PageNumber = 1,
            PageSize = DefaultPageSize,
            Count = 0
        };
    }

    /// <summary>
    /// Used by PaginatedListExtensions to map between layers
    /// </summary>
    /// <typeparam name="TOther">Type of existing PaginatedList object</typeparam>
    /// <param name="paginatedList">Existing PaginatedList object for pagination meta data</param>
    /// <param name="data">Data collection to create new PaginatedList object with</param>
    /// <returns>PaginatedList</returns>
    public static PaginatedList<T> CopyMetaData<TOther>(PaginatedList<TOther> paginatedList, IEnumerable<T> data)
    {
        if (paginatedList.Data.Count != data.Count())
            throw new InvalidOperationException(UnequalDataCountExceptionMessage);

        return new PaginatedList<T>
        {
            Data = data.ToList(),
            PageNumber = paginatedList.PageNumber,
            PageSize = paginatedList.PageSize,
            Count = paginatedList.Count
        };
    }

    public IReadOnlyList<T> Data { get; private set; } = [];

    public int Count { get; private set; }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }

    public int PageStart
    {
        get
        {
            if (Count == 0)
                return 0;

            return PageSize * (PageNumber - 1) + 1;
        }
    }


    public int PageEnd
    {
        get
        {
            if (IsLastPage)
                return Count;

            return PageSize * PageNumber;
        }
    }

    public int TotalPages => Count / PageSize + (Count % PageSize > 0 ? 1 : 0);

    public bool IsLastPage => TotalPages <= PageNumber;

    private void SetPageNumber(int pageNumber)
    {
        PageNumber = pageNumber >= 1 ? pageNumber : 1;
    }

    private void SetPageSize(int pageSize)
    {
        PageSize = pageSize >= 1 ? pageSize : DefaultPageSize;
    }
}
