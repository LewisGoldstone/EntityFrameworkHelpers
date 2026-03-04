using System.Linq.Expressions;
using System.Reflection;
using EntityFrameworkHelpers.Configuration;
using EntityFrameworkHelpers.ContainsKey.Parameters;
using EntityFrameworkHelpers.MetaData;
using EntityFrameworkHelpers.Pagination;
using EntityFrameworkHelpers.Utils.Expressions.Extensions;
using EntityFrameworkHelpers.Utils.Groupings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkHelpers.ContainsKey.Extensions;

public static class ContainsKeyExtensions
{
    /// <summary>
    /// Enables filtering against composite key with optional pagination - all inside the database.
    /// Key = Composite key
    /// Value = PaginatedList of entities
    /// </summary>
    /// <remarks>
    /// Entity must be configured within context using RegisterHelpers
    /// </remarks>
    /// <typeparam name="TEntity">Our entity</typeparam>
    /// <typeparam name="TKey">Composite key</typeparam>
    /// <param name="source">Our entity source</param>
    /// <param name="containKeys">Key values to query against</param>
    /// <param name="groupBy">Group by logic</param>
    /// <param name="orderBy">Column that'll be unique amongst a group to order by against</param>
    /// <param name="filters">Any filtering</param>
    /// <param name="includes">LINQ includes on TEntity</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Grouped collection of paginated result set</returns>
    internal static IEnumerable<IGrouping<TKey, PaginatedList<TEntity>>> ContainsKey<TEntity, TKey>(
        this DbSet<TEntity> source,
        IEnumerable<TKey> containKeys,
        Func<TEntity, TKey> groupBy,
        Expression<Func<TEntity, object>>? orderBy = null,
        IEnumerable<SqlFilter<TEntity>>? filters = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>[]? includes = null,
        int? pageNumber = null,
        int? pageSize = null)
        where TEntity : class
    {
        if (!containKeys.Any())
            return [];

        // Set pagination values to return full data set if not defined
        pageNumber = pageNumber >= 1 ? pageNumber.Value : 1;
        pageSize = pageSize ?? int.MaxValue;

        // Build parameterised SQL query
        var (sql, parameters) = BuildQuery(
            containKeys,
            pageNumber.Value,
            pageSize.Value,
            orderBy,
            filters);

        IQueryable<TEntity> query = source
            .FromSqlRaw(sql, parameters.ToArray());

        // include our includes 
        if (includes != null)
            foreach (Func<IQueryable<TEntity>, IQueryable<TEntity>> include in includes)
                query = include(query);

        // Execute and cast to PaginatedRow
        List<PaginatedRow<TEntity>> result = query
            .AsNoTracking()
            .Select(row => new PaginatedRow<TEntity>
            {
                Row = row,
                Count = EF.Property<int>(row, nameof(PaginatedRow<TEntity>.Count))
            })
            .ToList();

        // Group rows by key values and create PaginatedList
        return result
            .GroupBy(r => groupBy(r.Row))
            .Select(g => new Grouping<TKey, PaginatedList<TEntity>>(
                g.Key,
                [
                    new PaginatedList<TEntity>(
                        g.Select(i => i.Row),
                        pageNumber.Value,
                        pageSize.Value,
                        g.First().Count
                    )
                ]
            ));
    }

    private static (string Sql, List<SqlParameter> Parameters) BuildQuery<TEntity, TKey>(
        IEnumerable<TKey> containKeys,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, object>>? orderBy = null,
        IEnumerable<SqlFilter<TEntity>>? filters = null)
        where TEntity : class
    {
        List<SqlParameter> parameters = [];
        int parameterIndex = 0;

        int skip = (pageNumber - 1) * pageSize;

        // Get meta-data
        string tableName = MetaDataHelper.GetTableNameWithSchema<TEntity>();
        Dictionary<string, string> columns = MetaDataHelper.GetColumnMappings<TEntity>();

        PropertyInfo[] keyProperties = typeof(TKey).GetProperties();
        string[] containColumns = keyProperties.Select(p => p.Name).ToArray();

        string selectColumns = string.Join(", ", columns.Values);
        string partitionByColumns = string.Join(", ", containColumns.Select(c => columns[c]));

        string countColumn = nameof(PaginatedRow<TEntity>.Count);

        // Build collection of contain predicates i.e (key1 = @p1 AND key2 = @p2) 
        List<string> containPredicates = [];

        foreach (TKey key in containKeys.Distinct())
        {
            List<string> contain = [];

            for (int i = 0; i < keyProperties.Length; i++)
            {
                PropertyInfo property = keyProperties[i];

                string parameterName = $"@p{parameterIndex++}";
                contain.Add($"{columns[containColumns[i]]} = {parameterName}");
                parameters.Add(new SqlParameter(parameterName, property.GetValue(key) ?? DBNull.Value));
            }

            containPredicates.Add($"({string.Join(" AND ", contain)})");
        }

        // (key1 = @p1 AND key2 = @p2) OR (key1 = @p3 AND key2 = @p4) ...
        string whereClause = string.Join(" OR ", containPredicates);

        // Build order by
        string orderBySql = "";
        if (orderBy is not null)
        {
            orderBySql = $"ORDER BY {columns[orderBy.GetPropertyName()]}";
        }

        // Build filtering
        string filterSql = "";

        if (filters?.Any() == true)
        {
            List<string> filterSqls = [];

            foreach (SqlFilter<TEntity> filter in filters)
            {
                string filterParameter = $"@p{parameterIndex++}";
                filterSqls.Add($"{columns[filter.Column]} {filter.Operator} {filterParameter}");
                parameters.Add(new SqlParameter(filterParameter, filter.Value));
            }

            filterSql = "AND " + string.Join(" AND ", filterSqls);
        }

        // Finally build SQL
        string sql = $@"
        SELECT
        {selectColumns},
        CAST({countColumn} AS INT) AS {countColumn}
        FROM (
            SELECT
            {selectColumns},
            ROW_NUMBER() OVER (
                PARTITION BY {partitionByColumns}
                {orderBySql}
            ) AS RowNumber,
            COUNT(*) OVER (
                PARTITION BY {partitionByColumns})
            AS {countColumn}
            FROM {tableName}
            WHERE ({whereClause})
            {filterSql}
        ) AS PagedResults
        WHERE RowNumber > {skip}
        AND RowNumber <= {skip + pageSize}";

        return (sql, parameters);
    }
}
