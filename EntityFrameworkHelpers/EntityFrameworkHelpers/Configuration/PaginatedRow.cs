namespace EntityFrameworkHelpers.Configuration;

internal class PaginatedRow<TEntity>
    where TEntity : class
{
    public TEntity Row { get; set; } = default!;

    public int Count { get; set; }
}
