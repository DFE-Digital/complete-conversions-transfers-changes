using System.Security.Claims;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Tests.Utils;

public class UserTabAccessHelperTests
{
    [Fact]
    public void UserHasTabAccess_UnprotectedTab_ReturnsTrue()
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, "london") };
        var identity = new ClaimsIdentity(claims, "custom");
        var principal = new ClaimsPrincipal(identity);

        var result = UserTabAccessHelper.UserHasTabAccess(principal, "all-projects");
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
        var claims = new List<Claim> { new(ClaimTypes.Role, userTeam.ToDescription()) };
        var identity = new ClaimsIdentity(claims, "custom");
        var principal = new ClaimsPrincipal(identity);

        var result = UserTabAccessHelper.UserHasTabAccess(principal, route);
        Assert.Equal(expectedPermission, result);
    }
}