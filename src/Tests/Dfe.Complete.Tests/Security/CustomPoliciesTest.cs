using System.Security.Claims;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Tests.Security;

public class CustomPoliciesTest
{
    private bool EvaluateAccess(ClaimsPrincipal user)
    {
        if (user?.Identity is not { IsAuthenticated: true })
            return false;

        return user.IsInRole(UserRolesConstants.RegionalDeliveryOfficer)
                || (user.IsInRole(UserRolesConstants.RegionalCaseworkServices) && !user.IsInRole(UserRolesConstants.ManageTeam));
    }

    [Theory]
    [InlineData(new string[] { "regional_delivery_officer" }, true)]
    [InlineData(new string[] { "regional_casework_services" }, true)]
    [InlineData(new string[] { "regional_casework_services", "manage_team" }, false)]
    [InlineData(new string[] { "manage_team" }, false)]
    [InlineData(new string[] { }, false)]
    public void CanViewYourProjectsPolicy_EvaluatesCorrectly(string[] roles, bool expectedAccess)
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            roles.Select(role => new Claim(ClaimTypes.Role, role)),
            authenticationType: "TestAuthentication"));

        // Act
        var actualAccess = EvaluateAccess(user);

        // Assert
        Assert.Equal(expectedAccess, actualAccess);
    }

    [Fact]
    public void CanViewYourProjectsPolicy_UnauthenticatedUser_ShouldBeDenied()
    {
        // Arrange: Create an unauthenticated user.
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        // Act
        var actualAccess = EvaluateAccess(user);

        // Assert
        Assert.False(actualAccess);
    }
}
