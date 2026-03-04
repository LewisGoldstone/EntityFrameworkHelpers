namespace EntityFrameworkHelpers.Pagination.Extensions;

public static class PaginatedListExtensions
{
    /// <summary>
    /// Creates new instance of PaginatedList - for use with queries
    /// </summary>
    /// <typeparam name="T">Collection type</typeparam>
    /// <param name="data">Collection to be paginated</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>New PaginatedList instance</returns>
    public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> data, int pageNumber = 1, int pageSize = PaginatedList<T>.DefaultPageSize)
    {
        return new PaginatedList<T>(data, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates new instance of PaginatedList - for use with queries
    /// </summary>
    /// <typeparam name="T">Query type</typeparam>
    /// <param name="data">Query to be paginated</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>New PaginatedList instance</returns>
    public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> data, int pageNumber = 1, int pageSize = PaginatedList<T>.DefaultPageSize)
    {
        return new PaginatedList<T>(data, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates new instance of PaginatedList with original object's meta data but with new mapped data result - for mapping between layers
    /// </summary>
    /// <typeparam name="T">New data type</typeparam>
    /// <typeparam name="TOther">Existing PaginatedList type</typeparam>
    /// <param name="paginatedList">Existing PaginatedList object</param>
    /// <param name="mapper">Mapper delegate for mapping between existing data and new</param>
    /// <returns>New PaginatedList instance</returns>
    public static PaginatedList<T> MapData<T, TOther>(this PaginatedList<TOther> paginatedList, Func<TOther, T> mapper)
    {
        IEnumerable<T> mappedData = paginatedList.Data.Select(mapper);

        return PaginatedList<T>.CopyMetaData(paginatedList, mappedData);
    }
}
