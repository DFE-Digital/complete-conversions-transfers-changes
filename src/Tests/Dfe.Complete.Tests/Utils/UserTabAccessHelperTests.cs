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
    [InlineData(ProjectTeam.ServiceSupport, "groups", true)]
    [InlineData(ProjectTeam.ServiceSupport, "service-support", true)]
    [InlineData(ProjectTeam.ServiceSupport, "all-projects-exports", true)]
    [InlineData(ProjectTeam.ServiceSupport, "all-projects-handover", true)]
    public void UserHasTabAccess_ProtectedTab_ReturnsCorrectAccessForTeam(ProjectTeam userTeam, string route, bool expectedPermission)
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, userTeam.ToDescription()) };
        var identity = new ClaimsIdentity(claims, "custom");
        var principal = new ClaimsPrincipal(identity);

        var result = UserTabAccessHelper.UserHasTabAccess(principal, route);
        Assert.Equal(expectedPermission, result);
    }

    [Theory]
    [InlineData("your-team-projects", true)]
    [InlineData("your-projects", true)]
    [InlineData("all-projects-handover", true)]
    [InlineData("groups", true)]
    [InlineData("team-projects-handed-over", true)]
    public void UserHasTabAccess_ProtectedTab_ReturnsCorrectAccessForRdo(string route, bool expectedPermission)
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, "regional_delivery_officer") };
        var identity = new ClaimsIdentity(claims, "custom");
        var principal = new ClaimsPrincipal(identity);

        var result = UserTabAccessHelper.UserHasTabAccess(principal, route);
        Assert.Equal(expectedPermission, result);
    }

    public static readonly object[][] TeamLeadTabAccessTestData = new object[][]
    {
        [new[] { "service_support" }, false],
        [new[] { "service_support", "manage_team" }, false],
        [new[] { "regional_casework_services" }, false],
        [new[] { "regional_casework_services", "manage_team" }, true],
        [new[] { "regional_delivery_officer" }, false],
        [new[] { "regional_delivery_officer", "manage_team" }, true],
    };

    [Theory]
    [MemberData(nameof(TeamLeadTabAccessTestData))]
    public void UserHasTabAccess_ProtectedTab_ReturnsCorrectAccessForTeamLead(string[] userClaims, bool expectedPermission)
    {
        var claims = userClaims.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        var identity = new ClaimsIdentity(claims, "custom");
        var principal = new ClaimsPrincipal(identity);

        var result = UserTabAccessHelper.UserHasTabAccess(principal, "team-projects-unassigned");
        Assert.Equal(expectedPermission, result);
    }
}