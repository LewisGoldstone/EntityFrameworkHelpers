using System.Reflection;
using EntityFrameworkHelpers.ContainsKey.Parameters;
using FluentAssertions;
using Xunit;

namespace EntityFrameworkHelpers.Tests.ContainsKey.Parameters;

public class SqlFilterTests
{
    [Fact]
    public void FormatSqlValue_WhenNull_ShouldReturnDbNull()
    {
        // Arrange
        object? value = null;

        // Act
        object result = FormatSqlValue(value);

        // Assert
        result.Should().Be(DBNull.Value);
    }

    [Theory]
    [InlineData(FakeEnum.NotNone)]
    [InlineData(FakeEnum.None)]
    public void FormatSqlValue_WhenEnum_ShouldReturnInteger(FakeEnum value)
    {
        // Act
        object result = FormatSqlValue(value);

        // Assert
        result.Should().Be((int)value);
    }

    [Theory]
    [InlineData("test")]
    [InlineData(1)]
    [InlineData(true)]
    public void FormatSqlValue_WhenOtherDataTypes_ShouldReturnValue(object value)
    {
        // Act
        object result = FormatSqlValue(value);

        // Assert
        result.Should().Be(value);
    }

    // Private helper for reflection access
    private static object FormatSqlValue(object? value)
    {
        MethodInfo method = typeof(SqlFilter<object>)
            .GetMethod("FormatSqlValue", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("FormatSqlValue not found");

        return (object)method.Invoke(null, [value])!;
    }

    public enum FakeEnum
    {
        None = 0,
        NotNone = 1
    }
}
