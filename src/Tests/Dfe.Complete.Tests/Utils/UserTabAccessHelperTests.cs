using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using NSubstitute;

namespace Dfe.Complete.Tests.Utils;

public class UserTabAccessHelperTests
{
    [Fact]
    public void UserHasTabAccess_UnprotectedTab_ReturnsTrue()
    {
        var result = UserTabAccessHelper.UserHasTabAccess(ProjectTeam.London, "all-projects");
        Assert.True(result);
    }

    [Theory]
    [InlineData(ProjectTeam.BusinessSupport, "your-team-projects", false)]
    [InlineData(ProjectTeam.RegionalCaseWorkerServices, "your-team-projects", true)]
    [InlineData(ProjectTeam.London, "your-team-projects", true)]
    [InlineData(ProjectTeam.London, "your-projects", true)]
    [InlineData(ProjectTeam.London, "all-projects-handover", true)]
    [InlineData(ProjectTeam.ServiceSupport, "groups", true)]
    [InlineData(ProjectTeam.ServiceSupport, "service-support", true)]
    [InlineData(ProjectTeam.ServiceSupport, "all-projects-exports", true)]
    public void UserHasTabAccess_ProtectedTab_ReturnsCorrectAccessForTeam(ProjectTeam userTeam, string route, bool expectedPermission)
    {
        var result = UserTabAccessHelper.UserHasTabAccess(userTeam, route);
        Assert.Equal(expectedPermission, result);
    }
}