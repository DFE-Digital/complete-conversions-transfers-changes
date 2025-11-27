using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Infrastructure.Security.Authorization;
using System.Reflection;
using System.Security.Claims;

namespace Dfe.Complete.Tests.Authorization;

public class ManageTeamAuthorizationTests
{
    private readonly MethodInfo _shouldUserManageTeamMethod;

    public ManageTeamAuthorizationTests()
    {
        // Get the private static method using reflection
        _shouldUserManageTeamMethod = typeof(AuthorizationExtensions)
            .GetMethod("ShouldUserManageTeam", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("ShouldUserManageTeam method not found");
    }

    [Fact]
    public void ShouldUserManageTeam_WithRegionalCaseworkServicesTeamLeadRole_ReturnsTrue()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, UserRolesConstants.RegionalCaseworkServicesTeamLead)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = (bool)_shouldUserManageTeamMethod.Invoke(null, new object[] { principal })!;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldUserManageTeam_WithoutRegionalCaseworkServicesTeamLeadRole_ReturnsFalse()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "SomeOtherRole"),
            new Claim(ClaimTypes.Role, UserRolesConstants.ServiceSupport)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = (bool)_shouldUserManageTeamMethod.Invoke(null, new object[] { principal })!;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldUserManageTeam_WithNullPrincipal_ReturnsFalse()
    {
        // Arrange
        ClaimsPrincipal? principal = null;

        // Act
        var result = (bool)_shouldUserManageTeamMethod.Invoke(null, new object[] { principal! })!;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldUserManageTeam_WithEmptyPrincipal_ReturnsFalse()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var result = (bool)_shouldUserManageTeamMethod.Invoke(null, new object[] { principal })!;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldUserManageTeam_WithMultipleRolesIncludingTeamLead_ReturnsTrue()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "SomeOtherRole"),
            new Claim(ClaimTypes.Role, UserRolesConstants.ServiceSupport),
            new Claim(ClaimTypes.Role, UserRolesConstants.RegionalCaseworkServicesTeamLead),
            new Claim(ClaimTypes.Role, "AnotherRole")
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = (bool)_shouldUserManageTeamMethod.Invoke(null, new object[] { principal })!;

        // Assert
        Assert.True(result);
    }

    [Theory]

    [InlineData(UserRolesConstants.ServiceSupport)]
    [InlineData(UserRolesConstants.DataConsumers)]
    public void ShouldUserManageTeam_WithOtherValidRoles_ReturnsFalse(string roleName)
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, roleName)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = (bool)_shouldUserManageTeamMethod.Invoke(null, [principal])!;

        // Assert
        Assert.False(result);
    }
}