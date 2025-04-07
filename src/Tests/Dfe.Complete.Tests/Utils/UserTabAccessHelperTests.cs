using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using NSubstitute;

namespace Dfe.Complete.Tests.Utils;

public class UserTabAccessHelperTests
{
    [Fact]
    public void UserHasTabAccess_UnprotectedTab_ReturnsTrue()
    {
        var result = UserTabAccessHelper.UserHasTabAccess(Arg.Any<ProjectTeam>(), "your-projects");
        Assert.True(result);
    }

    [Theory]
    [InlineData(ProjectTeam.BusinessSupport, false)]
    [InlineData(ProjectTeam.RegionalCaseWorkerServices, true)]
    [InlineData(ProjectTeam.London, true)]
    public void UserHasTabAccess_ProtectedTab_ReturnsCorrectAccessForTeam(ProjectTeam userTeam, bool expectedPermission)
    {
        var result = UserTabAccessHelper.UserHasTabAccess(userTeam, "your-team-projects");
        Assert.Equal(expectedPermission, result);
    }
}