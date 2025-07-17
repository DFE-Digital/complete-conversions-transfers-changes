using System.Security.Claims;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.Notes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dfe.Complete.Tests.Pages.Projects.Notes;

public class ProjectNotesBaseModelTests
{
    private readonly Mock<ISender> _mockSender = new();
    private readonly Mock<ILogger<ProjectNotesBaseModel>> _mockLogger = new();

    [Fact]
    public async Task GetNoteById_NoNoteId_ReturnsNull()
    {
        // Arrange
        var model = new ProjectNotesBaseModel(_mockSender.Object, _mockLogger.Object, "");

        // Act
        var result = await model.GetNoteById(Guid.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNoteById_NoteFetchThrowsError_ReturnsNull()
    {
        // Arrange
        var noteId = Guid.NewGuid();

        _mockSender.Setup(x => x.Send(It.Is<GetNoteByIdQuery>(q => q.NoteId == new NoteId(noteId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<NoteDto>.Failure(""));
        var model = new ProjectNotesBaseModel(_mockSender.Object, _mockLogger.Object, "");

        // Act
        var result = await model.GetNoteById(noteId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNoteById_OnSuccess_ReturnsNoteId()
    {
        // Arrange
        var noteId = Guid.NewGuid();

        var expectedNote = new NoteDto(
            new NoteId(noteId),
            "Test note",
            new ProjectId(Guid.NewGuid()),
            new UserId(Guid.NewGuid()),
            "Test User",
            DateTime.UtcNow
        );
        _mockSender.Setup(x => x.Send(It.Is<GetNoteByIdQuery>(q => q.NoteId == new NoteId(noteId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<NoteDto>.Success(expectedNote));


        var model = new ProjectNotesBaseModel(_mockSender.Object, _mockLogger.Object, "");

        // Act
        var result = await model.GetNoteById(noteId);

        // Assert
        Assert.Equivalent(expectedNote, result);
    }

    [Theory]
    [InlineData(ProjectState.Deleted, false)]
    [InlineData(ProjectState.Completed, false)]
    [InlineData(ProjectState.DaoRevoked, false)]
    [InlineData(ProjectState.Active, true)]
    [InlineData(ProjectState.Inactive, true)]
    public void CanAddNotes_ReturnsExpectedResult(ProjectState projectState, bool expected)
    {
        // Arrange
        var model = new ProjectNotesBaseModel(_mockSender.Object, _mockLogger.Object, "")
        {
            Project = new ProjectDto { State = projectState }
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

        var model = new ProjectNotesBaseModel(_mockSender.Object, _mockLogger.Object, "")
        {
            Project = new ProjectDto { State = projectState },
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            }
        };

        // Act
        var result = model.CanEditNote(assignedUserId);

        // Assert
        Assert.Equal(expected, result);
    }
}