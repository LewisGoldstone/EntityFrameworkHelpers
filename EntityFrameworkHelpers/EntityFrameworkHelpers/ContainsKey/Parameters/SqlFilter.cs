using EntityFrameworkHelpers.Utils.Enums.Extensions;
using EntityFrameworkHelpers.Utils.Expressions.Extensions;
using System.ComponentModel;
using System.Linq.Expressions;

namespace EntityFrameworkHelpers.ContainsKey.Parameters;

public class SqlFilter<TEntity>
{
    public static SqlFilter<TEntity> FromLambda<TProp>(
        Expression<Func<TEntity, TProp>> property,
        SqlOperator @operator,
        TProp value
        )
    {
        return new SqlFilter<TEntity>
        {
            Column = property.GetPropertyName(),
            Operator = @operator.GetAttribute<DescriptionAttribute>().Description,
            Value = FormatSqlValue(value)
        };
    }

    internal string Column { get; init; } = "";

    internal string Operator { get; init; } = "";

    internal object Value { get; init; } = "";

    private static object FormatSqlValue(object? value)
    {
        return value switch
        {
            null => DBNull.Value,
            Enum e => Convert.ToInt32(e),
            _ => value
        };
    }
}
