using System.ComponentModel;

namespace EntityFrameworkHelpers.ContainsKey.Parameters;

public enum SqlOperator
{
    [Description("==")]
    Equals,

    [Description("!=")]
    NotEquals,

    [Description(">")]
    GreaterThan,

    [Description(">=")]
    GreaterThanOrEquals,

    [Description("<")]
    LessThan,

    [Description("<=")]
    LessThanOrEquals
}
