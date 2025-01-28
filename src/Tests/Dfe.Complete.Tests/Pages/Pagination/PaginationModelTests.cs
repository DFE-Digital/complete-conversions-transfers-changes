using Dfe.Complete.Pages.Pagination;

namespace Dfe.Complete.Tests.Pages.Pagination;

public class PaginationModelTests
{
    [Fact]
    public void Constructor_WithValidInputs_InitialisesPropertiesCorrectly()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 2;
        var recordCount = 50;
        var pageSize = 10;
        var elementIdPrefix = "test-";

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize, elementIdPrefix);

        // Assert
        Assert.Equal(url, model.Url);
        Assert.Equal(pageNumber, model.CurrentPageNumber);
        Assert.Equal(recordCount, model.RecordCount);
        Assert.Equal(5, model.TotalPages); // 50 records with page size 10 = 5 pages
        Assert.True(model.HasPrevious);
        Assert.True(model.HasNext);
        Assert.Equal(pageNumber - 1, model.Previous);
        Assert.Equal(pageNumber + 1, model.Next);
        Assert.Equal(elementIdPrefix, model.ElementIdPrefix);
        Assert.Equal("test-", model.Prefix);
        Assert.Contains(1, model.PagesToDisplay);
        Assert.Contains(pageNumber, model.PagesToDisplay);
        Assert.Contains(5, model.PagesToDisplay);
        Assert.Contains(1, model.PagesToDisplay);
        Assert.Contains(3, model.PagesToDisplay);
    }

    [Fact]
    public void PaginationModel_OnFirstPage_HasCorrectProperties()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 1;
        var recordCount = 20;
        var pageSize = 5;

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize);

        // Assert
        Assert.False(model.HasPrevious);
        Assert.Null(model.Previous);
        Assert.Null(model.PreviousPageLink);
        Assert.True(model.HasNext);
        Assert.Equal(2, model.Next);
        Assert.Equal(4, model.TotalPages); // 20 records with page size 5 = 4 pages
        Assert.Equal(new List<int> { 1, 2, 4 }, model.PagesToDisplay);
    }

    [Fact]
    public void PaginationModel_OnLastPage_HasCorrectProperties()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 5;
        var recordCount = 50;
        var pageSize = 10;

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize);

        // Assert
        Assert.True(model.HasPrevious);
        Assert.Equal(4, model.Previous);
        Assert.False(model.HasNext);
        Assert.Null(model.Next);
        Assert.Null(model.NextPageLink);
        Assert.Equal(new List<int> { 1, 4, 5 }, model.PagesToDisplay);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Prefix_WhenElementIdPrefixIsNullOrEmpty_ReturnsEmptyString(string? elementPrefix)
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 1;
        var recordCount = 10;
        var pageSize = 5;

        // Act
        var modelWithPrefix = new PaginationModel(url, pageNumber, recordCount, pageSize, elementPrefix);

        // Assert
        Assert.Equal(string.Empty, modelWithPrefix.Prefix);
    }

    [Fact]
    public void NextPageLink_CorrectlyFormatsUrl()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 2;
        var recordCount = 50;
        var pageSize = 10;

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize);

        // Assert
        Assert.Equal("https://example.com?pageNumber=3", model.NextPageLink);
    }

    [Fact]
    public void PreviousPageLink_CorrectlyFormatsUrl()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 2;
        var recordCount = 50;
        var pageSize = 10;

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize);

        // Assert
        Assert.Equal("https://example.com?pageNumber=1", model.PreviousPageLink);
    }

    [Fact]
    public void PaginationContainerId_FormatsCorrectlyWithPrefix()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 1;
        var recordCount = 10;
        var pageSize = 5;
        var elementIdPrefix = "custom-";

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize, elementIdPrefix);

        // Assert
        Assert.Equal("custom-pagination-container", model.PaginationContainerId);
    }

    [Fact]
    public void NextButtonId_FormatsCorrectlyWithPrefix()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 1;
        var recordCount = 10;
        var pageSize = 5;
        var elementIdPrefix = "custom-";

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize, elementIdPrefix);

        // Assert
        Assert.Equal("custom-next-page", model.NextButtonId);
    }

    [Fact]
    public void PreviousButtonId_FormatsCorrectlyWithPrefix()
    {
        // Arrange
        var url = "https://example.com";
        var pageNumber = 1;
        var recordCount = 10;
        var pageSize = 5;
        var elementIdPrefix = "custom-";

        // Act
        var model = new PaginationModel(url, pageNumber, recordCount, pageSize, elementIdPrefix);

        // Assert
        Assert.Equal("custom-previous-page", model.PreviousButtonId);
    }
}
