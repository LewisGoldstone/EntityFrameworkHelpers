using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EntityFrameworkHelpers.MetaData;

public static class MetaDataHelper
{
    /// <summary>
    /// Get full table name with schema (if present) 
    /// </summary>
    /// <typeparam name="TEntity"Entity type></typeparam>
    /// <returns>Schema.Table</returns>
    public static string GetTableNameWithSchema<TEntity>()
        where TEntity : class
    {
        TableAttribute? tableAttribute = typeof(TEntity).GetCustomAttribute<TableAttribute>();

        string? schema = tableAttribute?.Schema;
        string table = tableAttribute?.Name ?? typeof(TEntity).Name;

        return !string.IsNullOrEmpty(schema) ? $"{schema}.{table}" : table;
    }

    /// <summary>
    /// Get column mapping between property name and table column name
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Key: Property name, Value: Column name</returns>
    public static Dictionary<string, string> GetColumnMappings<TEntity>()
        where TEntity : class
    {
        return GetColumnMappings(typeof(TEntity));
    }

    /// <summary>
    /// Get column mapping between property name and table column name
    /// </summary>
    /// <param name="type">Entity type</param>
    /// <returns>Key: Property name, Value: Column name</returns>
    public static Dictionary<string, string> GetColumnMappings(Type type)
    {
        return type
            .GetProperties()
            .Where(prop =>
                prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 &&
                prop.PropertyType.GetCustomAttributes(typeof(TableAttribute), false).Length == 0)
            .ToDictionary(
                property => property.Name,
                property => property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name
            );
    }
}
