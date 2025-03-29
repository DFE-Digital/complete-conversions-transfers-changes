
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using NSubstitute;

namespace Dfe.Complete.Tests.Models;

public class TabNavigationModelTests
{
    [Fact]
    public void Constructor_WhenReceivesTabName_ShouldHaveCorrectCurrentTabName()
    {
        var tabNavigationModel = new TabNavigationModel("my-current-tab");

        Assert.Equal("my-current-tab", tabNavigationModel.CurrentTab);
    }

    [Fact]
    public void UserHasTabAccess_UnprotectedTab_ReturnsTrue()
    {
        var result = TabNavigationModel.UserHasTabAccess(Arg.Any<ProjectTeam>(), "your-projects");
        Assert.True(result);
    }

    [Theory]
    [InlineData(ProjectTeam.BusinessSupport, false)]
    [InlineData(ProjectTeam.RegionalCaseWorkerServices, true)]
    [InlineData(ProjectTeam.London, true)]
    public void UserHasTabAccess_ProtectedTab_ReturnsCorrectAccessForTeam(ProjectTeam userTeam, bool expectedPermission)
    {
        var result = TabNavigationModel.UserHasTabAccess(userTeam, "your-team-projects");
        Assert.Equal(expectedPermission, result);
    }
}
