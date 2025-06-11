using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Dfe.Complete.Tests.Models;

public class BaseProjectsPageModelTests
{
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetProjectSummaryUrl_ShouldReturnTaskListUrl(IFixture fixture)
    {
        // Arrange
        var project = fixture.Customize(new ListAllProjectResultModelCustomization
        {
            ProjectType = ProjectType.Conversion
        }).Create<ListAllProjectsResultModel>();

        string expectedUrl = string.Format(RouteConstants.ProjectTaskList, project.ProjectId.Value);

        // Act
        var result = BaseProjectsPageModel.GetProjectSummaryUrl(project);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Theory]
    [CustomAutoData(typeof(ProjectIdCustomization))]
    public void GetProjectSummaryUrlById_ShouldReturnTaskListUrl(IFixture fixture)
    {
        // Arrange
        var projectId = fixture.Customize(new ProjectIdCustomization()).Create<ProjectId>();

        string expectedUrl = string.Format(RouteConstants.ProjectTaskList, projectId.Value);

        // Act
        var result = BaseProjectsPageModel.GetProjectSummaryUrl( projectId);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public void BaseProjectsPageModel_HasCorrectConfiguration_WhenInstantiated(){
        // Arrange + Act
        var model = new TestBaseProjectsPageModel("my-navigation-page");
        var pagination = new PaginationModel("route", 1, 100, 20);
        model.Pagination = pagination;

        var pageSizeField = typeof(BaseProjectsPageModel)
            .GetField("PageSize", BindingFlags.Instance | BindingFlags.NonPublic);

        // Assert
        Assert.NotNull(pageSizeField);

        var value = (int)pageSizeField!.GetValue(model)!;

        Assert.Equal(pagination, model.Pagination);
        Assert.Equal("my-navigation-page", model.CurrentNavigationItem);
        Assert.Equal(1, model.PageNumber);
        Assert.Equal(20, value);
    }

    [Fact]
    public void HasPageFound_Returns404_WhenConditionIsTrue()
    {
        // Arrange + Act
        var model = new TestBaseProjectsPageModel("my-navigation-page");
        var pagination = new PaginationModel("route", 100, 100, 20);
        model.Pagination = pagination;

        var result = model.TestHasPageFound(pagination.IsOutOfRangePage, pagination.TotalPages);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(404, statusCodeResult.StatusCode);
    }

    [Fact]
    public void HasPageFound_ReturnsNull_WhenConditionIsFalse()
    {
        // Arrange + Act
        var model = new TestBaseProjectsPageModel("my-navigation-page");
        var pagination = new PaginationModel("route", 1, 100, 20);
        model.Pagination = pagination;

        var result = model.TestHasPageFound(pagination.IsOutOfRangePage, pagination.TotalPages);

        // Assert
        Assert.Null(result);
    }
}

public class TestBaseProjectsPageModel(string currentNav) : BaseProjectsPageModel(currentNav) {
    public IActionResult TestHasPageFound(bool condition, int totalPages) =>
       HasPageFound(condition, totalPages);
}