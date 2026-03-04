using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkHelpers.MetaData;
using FluentAssertions;
using Xunit;

namespace EntityFrameworkHelpers.Tests.MetaData;

public class MetaDataHelperTests
{
    [Fact]
    public void GetTableNameWithSchema_WithoutTableAttributes_ShouldReturnTableName()
    {
        // Arrange/Act
        string tableName = MetaDataHelper.GetTableNameWithSchema<FakeEntity>();

        // Assert
        tableName.Should().Be(nameof(FakeEntity));
    }

    [Fact]
    public void GetTableNameWithSchema_WithTableAttribute_ShouldReturnTableName()
    {
        // Arrange/Act
        string tableName = MetaDataHelper.GetTableNameWithSchema<FakeEntityWithAnnotation>();

        // Assert
        tableName.Should().Be("FakeSchema.FakeEntity");
    }

    [Fact]
    public void GetColumnMappings_ShouldNotReturnNotMappedProperties()
    {
        // Arrange/Act
        Dictionary<string, string> columns = MetaDataHelper.GetColumnMappings<FakeEntity>();

        // Assert
        columns.Should().NotContainKey(nameof(FakeEntity.IsActive));
    }

    [Fact]
    public void GetColumnMappings_ShouldReturnPropertyWithoutColumnAttribute()
    {
        // Arrange/Act
        Dictionary<string, string> columns = MetaDataHelper.GetColumnMappings<FakeEntity>();

        // Assert
        KeyValuePair<string, string> expectedKey = new(nameof(FakeEntity.Id), nameof(FakeEntity.Id));
        columns.Should().Contain(expectedKey);
    }

    [Fact]
    public void GetColumnMappings_ShouldReturnPropertyWithColumnAttribute()
    {
        // Arrange/Act
        Dictionary<string, string> columns = MetaDataHelper.GetColumnMappings<FakeEntity>();

        // Assert
        KeyValuePair<string, string> expectedKey = new(nameof(FakeEntity.IsActiveField), "IsActive");
        columns.Should().Contain(expectedKey);
    }

    private class FakeEntity() 
    { 
        public Guid Id { get; set; }

        [Column("IsActive")]
        public int IsActiveField { get; set; }

        [NotMapped]
        public bool IsActive => Convert.ToBoolean(IsActiveField);
    }

    [Table("FakeEntity", Schema = "FakeSchema")]
    private class FakeEntityWithAnnotation() { }
}
