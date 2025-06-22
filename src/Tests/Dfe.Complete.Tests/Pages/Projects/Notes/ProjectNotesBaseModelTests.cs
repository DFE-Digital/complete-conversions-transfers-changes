using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.Notes;
using MediatR;
using Moq;

namespace Dfe.Complete.Tests.Pages.Projects.Notes;

public class ProjectNotesBaseModelTests
{
    private readonly Mock<ISender> sender = new();

    [Fact]
    public async Task GetNoteById_NoNoteId_ReturnsNull()
    {
        // Arrange
        var model = new ProjectNotesBaseModel(sender.Object, "");

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

        sender.Setup(x => x.Send(It.Is<GetNoteByIdQuery>(q => q.NoteId == new NoteId(noteId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<NoteDto>.Failure(""));
        var model = new ProjectNotesBaseModel(sender.Object, "");

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
        sender.Setup(x => x.Send(It.Is<GetNoteByIdQuery>(q => q.NoteId == new NoteId(noteId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<NoteDto>.Success(expectedNote));


        var model = new ProjectNotesBaseModel(sender.Object, "");

        // Act
        var result = await model.GetNoteById(noteId);

        // Assert
        Assert.Equivalent(expectedNote, result);
    }
}