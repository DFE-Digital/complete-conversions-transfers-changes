using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.List;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Tests.Pages.Projects.List;

public class AllProjectsModelTests
{
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetTrustProjectsUrl_ShouldReturnCorrectMatUrl_When_IdentifierIsTrustReference(IFixture fixture)
    {
        var trust = fixture.Build<ListTrustsWithProjectsResultModel>()
            .With(x => x.identifier, "TR00001")
            .Create();

        string expectedUrl = string.Format(RouteConstants.TrustMATProjects, trust.identifier);

        // Act
        var result = AllProjectsModel.GetTrustProjectsUrl(trust);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
    
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetTrustProjectsUrl_ShouldReturnCorrectUrl_When_IdentifierIsNotTrustReference(IFixture fixture)
    {
        var trust = fixture.Build<ListTrustsWithProjectsResultModel>()
            .With(x => x.identifier, "10035415")
            .Create();

        string expectedUrl = string.Format(RouteConstants.TrustProjects, trust.identifier);

        // Act
        var result = AllProjectsModel.GetTrustProjectsUrl(trust);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
    
    [Theory]
    [InlineData(ProjectType.Conversion)]
    [InlineData(ProjectType.Transfer)]

    public void GetProjectByMonthUrl_ShouldReturnCorrectUrl_DependantOnProjectType(ProjectType projectType)
    {
        DateTime date = DateTime.Now.AddMonths(1);
        string month = date.Month.ToString("0");
        string year = date.Year.ToString("0000");
        
        // Arrange
        string expectedUrl = string.Format(projectType == ProjectType.Conversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth, month, year);
            
        // Act
        var result = AllProjectsModel.GetProjectByMonthUrl(projectType);
        
        // Assert
        Assert.Equal(result, expectedUrl);
    }
    
    [Theory]
    [InlineData(ProjectType.Conversion, ProjectTeam.DataConsumers, false, true)] 
    [InlineData(ProjectType.Conversion, ProjectTeam.NorthWest, true, true)] 
    [InlineData(ProjectType.Transfer, ProjectTeam.NorthWest, false, false)]
    public void GetProjectByMonthsUrl_ShouldReturnCorrectUrl(ProjectType projectType, ProjectTeam team, bool managesTeam, bool expectedToSeeDataConsumerUrl)
    {
        // Arrange
        DateTime date = DateTime.Now;
        int month = date.Month;
        int year = date.Year;

        string expectedUrl = string.Format(
            expectedToSeeDataConsumerUrl
                ? (projectType == ProjectType.Conversion ? RouteConstants.ConversionProjectsByMonths : RouteConstants.TransfersProjectsByMonths)
                : (projectType == ProjectType.Conversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth),
            month, year, month, year);

        var user = new UserDto
        {
            ManageTeam = managesTeam,
            Team = team.ToString()
        };

        // Act
        var result = AllProjectsModel.GetProjectByMonthsUrl(projectType, user, month, year, month, year);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

}