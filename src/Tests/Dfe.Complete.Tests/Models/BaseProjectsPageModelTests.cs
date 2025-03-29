using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Tests.Models;

public class BaseProjectsPageModelTests
{
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetProjectSummaryUrl_ShouldReturnConversionTaskListUrl_WhenProjectTypeIsConversion(IFixture fixture)
    {
        // Arrange
        var project = fixture.Customize(new ListAllProjectResultModelCustomization
        {
            ProjectType = ProjectType.Conversion
        }).Create<ListAllProjectsResultModel>();

        string expectedUrl = string.Format(RouteConstants.ConversionProjectTaskList, project.ProjectId.Value);

        // Act
        var result = BaseProjectsPageModel.GetProjectSummaryUrl(project);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetProjectSummaryUrl_ShouldReturnTransferTaskListUrl_WhenProjectTypeIsTransfer(IFixture fixture)
    {
        // Arrange
        var project = fixture.Customize(new ListAllProjectResultModelCustomization
        {
            ProjectType = ProjectType.Transfer
        }).Create<ListAllProjectsResultModel>();

        string expectedUrl = string.Format(RouteConstants.TransferProjectTaskList, project.ProjectId.Value);

        // Act
        var result = BaseProjectsPageModel.GetProjectSummaryUrl(project);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Theory]
    [CustomAutoData(typeof(ProjectIdCustomization))]
    public void GetProjectSummaryUrlById_ShouldReturnConversionTaskListUrl_WhenProjectTypeIsConversion(IFixture fixture)
    {
        // Arrange
        var projectId = fixture.Customize(new ProjectIdCustomization()).Create<ProjectId>();

        string expectedUrl = string.Format(RouteConstants.ConversionProjectTaskList, projectId.Value);

        // Act
        var result = BaseProjectsPageModel.GetProjectSummaryUrl(ProjectType.Conversion, projectId);

        // Assert
        Assert.Equal(expectedUrl, result);
    }


    [Theory]
    [CustomAutoData(typeof(ProjectIdCustomization))]
    public void GetProjectSummaryUrlById_ShouldReturnConversionTaskListUrl_WhenProjectTypeIsTransfer(IFixture fixture)
    {
        // Arrange
        var projectId = fixture.Customize(new ProjectIdCustomization()).Create<ProjectId>();

        string expectedUrl = string.Format(RouteConstants.TransferProjectTaskList, projectId.Value);

        // Act
        var result = BaseProjectsPageModel.GetProjectSummaryUrl(ProjectType.Transfer, projectId);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

}