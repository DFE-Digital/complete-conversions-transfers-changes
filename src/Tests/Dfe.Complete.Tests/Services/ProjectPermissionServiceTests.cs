using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using System.Security.Claims;

namespace Dfe.Complete.Tests.Services
{
    public class ProjectPermissionServiceTests
    {
        private readonly ProjectPermissionService _service = new();

        private static ClaimsPrincipal CreateUserClaimPrincipal(UserId userId)
        {
            var claims = new[] { new Claim(CustomClaimTypeConstants.UserId, userId.Value.ToString()) };
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }

        [Theory]
        [InlineData(ProjectState.Active, ProjectTeam.ServiceSupport, false, true)]
        [InlineData(ProjectState.Completed, ProjectTeam.ServiceSupport, false, true)]
        [InlineData(ProjectState.Completed, ProjectTeam.London, false, false)]
        [InlineData(ProjectState.Active, ProjectTeam.London, true, true)]
        [InlineData(ProjectState.Active, ProjectTeam.London, false, false)]
        public void UserCanEdit_WhenMatchesCondition_ShouldReturnCorrectResult(ProjectState projectState, ProjectTeam projectTeam, bool isProjectAssignTouser, bool expectedResult)
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var userClaimPrincipal = CreateUserClaimPrincipal(userId); 
            var project = new ProjectDto 
            { 
                State = projectState,
                AssignedToId = isProjectAssignTouser ? userId : new UserId(Guid.NewGuid())
            };
             
            // Act
            var result = _service.UserCanEdit(project, projectTeam, userClaimPrincipal);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
