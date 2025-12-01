using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using System.Security.Claims;

namespace Dfe.Complete.Tests.Services;

public class ProjectPermissionServiceTests
{
    private readonly ProjectPermissionService _service = new();

    private static ClaimsPrincipal CreateUserClaimPrincipal(UserId userId, string role)
    {
        var claims = new[] {
            new Claim(CustomClaimTypeConstants.UserId, userId.Value.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }

    [Theory]
    [InlineData(ProjectState.Active, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Completed, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Deleted, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Inactive, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Active, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.RegionalDeliveryOfficer, false)]
    public void UserCanComplete_WhenMatchesCondition_ShouldReturnCorrectResult(ProjectState projectState, string projectTeam, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, projectTeam);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = null
        };

        // Act
        var result = _service.UserCanComplete(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active, true, true)]
    [InlineData(ProjectState.DaoRevoked, true, false)]
    [InlineData(ProjectState.Completed, true, false)]
    [InlineData(ProjectState.Deleted, true, false)]
    [InlineData(ProjectState.Inactive, true, false)]
    public void UserCanComplete_WhenProjectAssignedToUser_ShouldReturnCorrectResult(ProjectState projectState, bool isProjectAssignedToUser, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, UserRolesConstants.RegionalDeliveryOfficer);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = isProjectAssignedToUser ? userId : new UserId(Guid.NewGuid())
        };

        // Act
        var result = _service.UserCanComplete(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Completed, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Deleted, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Inactive, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Active, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Completed, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Deleted, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Inactive, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Active, UserRolesConstants.RegionalCaseworkServices, false)]
    public void UserCanEdit_WhenMatchesCondition_ShouldReturnCorrectResult(ProjectState projectState, string projectTeam, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, projectTeam);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = null
        };

        // Act
        var result = _service.UserCanEdit(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active, true)]
    [InlineData(ProjectState.Completed, false)]
    [InlineData(ProjectState.DaoRevoked, false)]
    [InlineData(ProjectState.Deleted, false)]
    [InlineData(ProjectState.Inactive, false)]
    public void UserCanEdit_WhenProjectAssignedToUser_ShouldReturnCorrectResult(ProjectState projectState, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, UserRolesConstants.RegionalDeliveryOfficer);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = userId
        };

        // Act
        var result = _service.UserCanEdit(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Completed, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Deleted, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Inactive, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Active, UserRolesConstants.RegionalDeliveryOfficer, true)]
    [InlineData(ProjectState.Completed, UserRolesConstants.RegionalDeliveryOfficer, true)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.RegionalDeliveryOfficer, true)]
    [InlineData(ProjectState.Deleted, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Inactive, UserRolesConstants.RegionalDeliveryOfficer, true)]
    [InlineData(ProjectState.Active, UserRolesConstants.RegionalCaseworkServices, true)]
    [InlineData(ProjectState.Completed, UserRolesConstants.RegionalCaseworkServices, true)]
    [InlineData(ProjectState.DaoRevoked, UserRolesConstants.RegionalCaseworkServices, true)]
    [InlineData(ProjectState.Deleted, UserRolesConstants.RegionalCaseworkServices, false)]
    [InlineData(ProjectState.Inactive, UserRolesConstants.RegionalCaseworkServices, true)]
    public void UserCanView_WhenMatchesCondition_ShouldReturnCorrectResult(ProjectState projectState, string projectTeam, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, projectTeam);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = null
        };

        // Act
        var result = _service.UserCanView(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active, true, UserRolesConstants.ServiceSupport, true)]
    [InlineData(ProjectState.Completed, true, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.DaoRevoked, true, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Deleted, true, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Inactive, true, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Active, false, UserRolesConstants.ServiceSupport, false)]
    [InlineData(ProjectState.Active, true, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Active, false, UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(ProjectState.Active, true, UserRolesConstants.RegionalCaseworkServices, false)]
    public void UserCanDaoRevocation_WhenMatchesCondition_ShouldReturnCorrectResult(ProjectState projectState, bool directiveAcademyOrder, string projectTeam, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, projectTeam);
        var project = new ProjectDto
        {
            State = projectState,
            DirectiveAcademyOrder = directiveAcademyOrder,
            AssignedToId = null
        };

        // Act
        var result = _service.UserCanDaoRevocation(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active, true)]
    [InlineData(ProjectState.Completed, false)]
    [InlineData(ProjectState.DaoRevoked, false)]
    [InlineData(ProjectState.Deleted, false)]
    [InlineData(ProjectState.Inactive, false)]
    public void UserCanDaoRevocation_WhenProjectAssignedToUser_ShouldReturnCorrectResult(ProjectState projectState, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, UserRolesConstants.RegionalDeliveryOfficer);
        var project = new ProjectDto
        {
            State = projectState,
            DirectiveAcademyOrder = true,
            AssignedToId = userId
        };

        // Act
        var result = _service.UserCanDaoRevocation(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(UserRolesConstants.ServiceSupport, true)]
    [InlineData(UserRolesConstants.RegionalDeliveryOfficer, false)]
    [InlineData(UserRolesConstants.RegionalCaseworkServices, false)]
    [InlineData("SomeOtherRole", false)]
    public void UserIsAdmin_WhenUserHasRole_ShouldReturnCorrectResult(string userRole, bool expectedResult)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, userRole);
        var project = new ProjectDto
        {
            State = ProjectState.Active,
            AssignedToId = userId
        };

        // Act
        var result = _service.UserIsAdmin(project, userClaimPrincipal);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(ProjectState.Active)]
    [InlineData(ProjectState.Completed)]
    [InlineData(ProjectState.DaoRevoked)]
    [InlineData(ProjectState.Deleted)]
    [InlineData(ProjectState.Inactive)]
    public void UserIsAdmin_WhenServiceSupportUser_ShouldReturnTrueRegardlessOfProjectState(ProjectState projectState)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, UserRolesConstants.ServiceSupport);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = new UserId(Guid.NewGuid()) // Different user assigned
        };

        // Act
        var result = _service.UserIsAdmin(project, userClaimPrincipal);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(ProjectState.Active)]
    [InlineData(ProjectState.Completed)]
    [InlineData(ProjectState.DaoRevoked)]
    [InlineData(ProjectState.Deleted)]
    [InlineData(ProjectState.Inactive)]
    public void UserIsAdmin_WhenNonServiceSupportUser_ShouldReturnFalseRegardlessOfProjectState(ProjectState projectState)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var userClaimPrincipal = CreateUserClaimPrincipal(userId, UserRolesConstants.RegionalDeliveryOfficer);
        var project = new ProjectDto
        {
            State = projectState,
            AssignedToId = userId // User is assigned to project
        };

        // Act
        var result = _service.UserIsAdmin(project, userClaimPrincipal);

        // Assert
        Assert.False(result);
    }
}

