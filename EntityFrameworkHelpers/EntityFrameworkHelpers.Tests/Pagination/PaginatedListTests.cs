using EntityFrameworkHelpers.Pagination;
using FluentAssertions;
using Xunit;

namespace EntityFrameworkHelpers.Tests.Pagination;

public class PaginatedListTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void PaginatedList_WhenPageNumberIsLessThanOne_PageNumberShouldEqualOne(int pageNumber)
    {
        // Arrange
        List<object> data = [];
        int pageSize = 50;

        // Act
        PaginatedList<object> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PaginatedList_ShouldSetPageNumber()
    {
        // Arrange
        List<object> data = [];
        int pageNumber = 10;
        int pageSize = 20;

        // Act
        PaginatedList<object> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageNumber.Should().Be(pageNumber);
    }

    [Fact]
    public void PaginatedList_ShouldSetPageSize()
    {
        // Arrange
        List<object> data = [];
        int pageNumber = 10;
        int pageSize = 20;

        // Act
        PaginatedList<object> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public void PaginatedList_ShouldSetCount()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 }
        };

        int pageNumber = 1;
        int pageSize = 20;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.Count.Should().Be(data.Count);
    }

    [Fact]
    public void PaginatedList_ShouldSetData()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 }, // Take these two
            new { Id = 4 }, // Take these two
            new { Id = 5 },
            new { Id = 6 }
        };

        int pageNumber = 2;
        int pageSize = 2;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        List<dynamic> expectedData = new()
        {
            data[2],
            data[3]
        };
        paginatedList.Data.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public void TotalPages_WhenRoundNumber_ShouldCalculate()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 }
        };

        int pageNumber = 2;
        int pageSize = 2;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_WhenNotRoundNumber_ShouldCalculate()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 }
        };

        int pageNumber = 2;
        int pageSize = 5;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(2);
    }

    [Fact]
    public void TotalPages_WhenComplicated_ShouldCalculate()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 },
            new { Id = 7 },
            new { Id = 8 },
            new { Id = 9 },
            new { Id = 10 },
            new { Id = 11 },
            new { Id = 12 },
            new { Id = 13 },
            new { Id = 14 },
            new { Id = 15 },
            new { Id = 16 }
        };

        int pageNumber = 2;
        int pageSize = 5;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(4);
    }

    [Fact]
    public void IsLastPage_WhenNotLastPage_ShouldBeFalse()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 }
        };

        int pageNumber = 2;
        int pageSize = 2;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.IsLastPage.Should().BeFalse();
    }

    [Fact]
    public void IsLastPage_WhenLastPage_ShouldBeTrue()
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 }
        };

        int pageNumber = 3;
        int pageSize = 2;

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.IsLastPage.Should().BeTrue();
    }

    [Fact]
    public void CopyMetaData_WhenDataCountIsNotEqual_ShouldThrowException()
    {
        // Arrange
        List<dynamic> paginatedListData = new()
        {
            new { Id = 1 }
        };
        PaginatedList<dynamic> paginatedList = new(paginatedListData, 1, 2);

        List<dynamic> newData = new()
        {
            new { Id = 1 },
            new { Id = 2 }
        };

        // Act/Assert
        Assert.Multiple(() =>
        {
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => PaginatedList<dynamic>.CopyMetaData(paginatedList, newData));
            exception.Message.Should().Be("Data count must be equal");
        });
    }

    [Fact]
    public void CopyMetaData_ShouldCreatePaginatedList()
    {
        // Arrange
        List<dynamic> paginatedListData = new()
        {
            new { Id = 1 }
        };
        PaginatedList<dynamic> originalPaginatedList = new(paginatedListData, 1, 2);

        List<dynamic> newData = new()
        {
            new { NewId = 1 } // Change in object proves equal assertion is correct
        };

        // Act
        PaginatedList<dynamic> paginatedList = PaginatedList<dynamic>.CopyMetaData(originalPaginatedList, newData);

        // Assert
        Assert.Multiple(() =>
        {
            paginatedList.Data.Should().BeEquivalentTo(newData);
            paginatedList.PageNumber.Should().Be(originalPaginatedList.PageNumber);
            paginatedList.PageSize.Should().Be(originalPaginatedList.PageSize);
            paginatedList.Count.Should().Be(originalPaginatedList.Count);
        });
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 3)]
    [InlineData(2, 9)]
    public void PageStart_WhenCountIsZero_ShouldEqualZero(int pageNumber, int pageSize)
    {
        // Arrange
        List<dynamic> data = new();

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageStart.Should().Be(0);
    }

    [Theory]
    [InlineData(1, 5, 1)]
    [InlineData(2, 3, 4)]
    [InlineData(2, 9, 10)]
    public void PageStart_ShouldCalculate(int pageNumber, int pageSize, int expectedResult)
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 },
            new { Id = 7 },
            new { Id = 8 },
            new { Id = 9 },
            new { Id = 10 }
        };

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageStart.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, 5, 5)]
    [InlineData(2, 3, 6)]
    [InlineData(3, 3, 9)]
    public void PageEnd_ShouldCalculate(int pageNumber, int pageSize, int expectedResult)
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 },
            new { Id = 7 },
            new { Id = 8 },
            new { Id = 9 },
            new { Id = 10 }
        };

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageEnd.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(2, 5)]
    [InlineData(4, 3)]
    [InlineData(1, 10)]
    public void PageEnd_WhenLastPage_ShouldEqualCount(int pageNumber, int pageSize)
    {
        // Arrange
        List<dynamic> data = new()
        {
            new { Id = 1 },
            new { Id = 2 },
            new { Id = 3 },
            new { Id = 4 },
            new { Id = 5 },
            new { Id = 6 },
            new { Id = 7 },
            new { Id = 8 },
            new { Id = 9 },
            new { Id = 10 }
        };

        // Act
        PaginatedList<dynamic> paginatedList = new(data, pageNumber, pageSize);

        // Assert
        paginatedList.PageEnd.Should().Be(data.Count);
    }
}

