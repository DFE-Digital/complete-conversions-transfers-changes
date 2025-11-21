using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;

namespace Dfe.Complete.Domain.Tests.Extensions;

public class ProjectTeamExtensionsTests
{
    [Theory]
    [InlineData(ProjectTeam.London, true)]
    [InlineData(ProjectTeam.RegionalCaseWorkerServices, false)]
    [InlineData(ProjectTeam.ServiceSupport, false)]
    public void TeamIsRegionalDeliveryOfficer_ReturnsCorrectResult(ProjectTeam team, bool expectedResult)
    {
        var result = team.TeamIsRegionalDeliveryOfficer();
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectTeam.London, false)]
    [InlineData(ProjectTeam.RegionalCaseWorkerServices, true)]
    public void TeamIsRegionalCaseworkServices_ReturnsCorrectResult(ProjectTeam team, bool expectedResult)
    {
        var result = team.TeamIsRegionalCaseworkServices();
        Assert.Equal(expectedResult, result);
    }
}