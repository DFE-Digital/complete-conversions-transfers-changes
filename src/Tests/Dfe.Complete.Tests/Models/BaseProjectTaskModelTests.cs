using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.Tests;

public class BaseProjectTaskModelTests
{
    private readonly Mock<ISender> _mockSender = new();
    private readonly Mock<IAuthorizationService> _mockAuthService = new();
    private readonly Mock<ILogger<BaseProjectTaskModel>> _mockLogger = new();

    [Theory]
    [InlineData(ProjectState.Deleted, false)]
    [InlineData(ProjectState.Completed, false)]
    [InlineData(ProjectState.DaoRevoked, false)]
    [InlineData(ProjectState.Active, true)]
    [InlineData(ProjectState.Inactive, true)]
    public void CanAddNotes_ReturnsExpectedResult(ProjectState projectState, bool expected)
    {
        // Arrange
        var model = new BaseProjectTaskModel(_mockSender.Object, _mockAuthService.Object, _mockLogger.Object, NoteTaskIdentifier.Handover)
        {
            Project = new ProjectDto { State = projectState },
            TaskIdentifier = NoteTaskIdentifier.Handover
        };

        // Act
        var result = model.CanAddNotes;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ProjectState.Completed, "00000000-0000-0000-0000-000000000001", false)]
    [InlineData(ProjectState.Active, "00000000-0000-0000-0000-000000000001", true)]
    [InlineData(ProjectState.Active, "00000000-0000-0000-0000-000000000002", false)]
    public void CanEditNote_ReturnsExpectedResult(ProjectState projectState, string currentUserGuidString, bool expected)
    {
        // Arrange
        var assignedUserGuid = new Guid("00000000-0000-0000-0000-000000000001");
        var assignedUserId = new UserId(assignedUserGuid);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim("objectidentifier", currentUserGuidString),
            new Claim(CustomClaimTypeConstants.UserId, currentUserGuidString)
        ]));

        var model = new BaseProjectTaskModel(_mockSender.Object, _mockAuthService.Object, _mockLogger.Object, NoteTaskIdentifier.Handover)
        {
            Project = new ProjectDto { State = projectState },
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            },
            TaskIdentifier = NoteTaskIdentifier.Handover
        };

        // Act
        var result = model.CanEditNote(assignedUserId);

        // Assert
        Assert.Equal(expected, result);
    }

}
