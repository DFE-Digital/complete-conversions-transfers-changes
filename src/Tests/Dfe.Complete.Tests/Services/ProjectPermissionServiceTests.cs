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
        public void UserCanEdit_WhenMatchesCondition_ShouldReturnCorrectResult(ProjectState projectState, ProjectTeam projectTeam, bool isProjectAssignToUser, bool expectedResult)
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var userClaimPrincipal = CreateUserClaimPrincipal(userId); 
            var project = new ProjectDto 
            { 
                State = projectState,
                AssignedToId = isProjectAssignToUser ? userId : new UserId(Guid.NewGuid())
            };
             
            // Act
            var result = _service.UserCanEdit(project, projectTeam, userClaimPrincipal);

            // Assert
            Assert.Equal(expectedResult, result);
        }
        [Theory]
        [InlineData(ProjectState.Active, true, ProjectTeam.ServiceSupport, false, true)]
        [InlineData(ProjectState.Completed, true, ProjectTeam.ServiceSupport, false, false)]
        [InlineData(ProjectState.Active, false, ProjectTeam.ServiceSupport, false, false)]
        [InlineData(ProjectState.Active, true, ProjectTeam.London, true, true)]
        [InlineData(ProjectState.Active, false, ProjectTeam.London, true, false)]
        [InlineData(ProjectState.Completed, true, ProjectTeam.London, true, false)]
        [InlineData(ProjectState.Active, true, ProjectTeam.London, false, false)]
        public void UserCanDaoRevocation_ShouldReturnCorrectResult(ProjectState projectState,bool directiveAcademyOrder,
            ProjectTeam projectTeam, bool isProjectAssignToUser, bool expectedResult)
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var userClaimPrincipal = CreateUserClaimPrincipal(userId);
            var project = new ProjectDto
            {
                State = projectState,
                DirectiveAcademyOrder = directiveAcademyOrder,
                AssignedToId = isProjectAssignToUser ? userId : new UserId(Guid.NewGuid())
            };

            // Act
            var result = _service.UserCanDaoRevocation(project, projectTeam, userClaimPrincipal);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
