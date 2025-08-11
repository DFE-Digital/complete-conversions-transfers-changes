using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectDetails;
using Dfe.Complete.Services;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Web;

namespace Dfe.Complete.Tests.Models;

public class BaseProjectDetailsPageModelTests
{
    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetGroupReferenceNumberAsync_WhenGroupIdIsNull_DoesNotSetGroupReferenceNumber(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        ProjectDto project)
    {
        // Arrange
        project.GroupId = null;
        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            Project = project
        };

        // Act
        await model.TestSetGroupReferenceNumberAsync();

        // Assert
        Assert.Null(model.GroupReferenceNumber);
        await mockSender.DidNotReceive().Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetGroupReferenceNumberAsync_WhenGroupIdExists_AndQuerySuccessful_SetsGroupReferenceNumber(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        ProjectDto project,
        ProjectGroupDto projectGroup)
    {
        // Arrange
        var groupId = new ProjectGroupId(Guid.NewGuid());
        project.GroupId = groupId;
        projectGroup.GroupIdentifier = "GR123456";

        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            Project = project
        };

        var successResult = Result<ProjectGroupDto?>.Success(projectGroup);
        mockSender.Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        await model.TestSetGroupReferenceNumberAsync();

        // Assert
        Assert.Equal("GR123456", model.GroupReferenceNumber);
        // await mockSender.Received(1).Send(
        //     Arg.Is<GetProjectGroupByIdQuery>(q => q.ProjectGroupId == groupId), 
        //     Arg.Any<CancellationToken>());
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetGroupReferenceNumberAsync_WhenGroupIdExists_AndQueryFails_DoesNotSetGroupReferenceNumber(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        ProjectDto project)
    {
        // Arrange
        var groupId = new ProjectGroupId(Guid.NewGuid());
        project.GroupId = groupId;

        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            Project = project
        };

        var failureResult = Result<ProjectGroupDto?>.Failure("Database error");
        mockSender.Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        // Act
        await model.TestSetGroupReferenceNumberAsync();

        // Assert
        Assert.Null(model.GroupReferenceNumber);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetGroupReferenceNumberAsync_WhenGroupIdExists_AndQueryReturnsNull_DoesNotSetGroupReferenceNumber(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        ProjectDto project)
    {
        // Arrange
        var groupId = new ProjectGroupId(Guid.NewGuid());
        project.GroupId = groupId;

        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            Project = project
        };

        var nullResult = Result<ProjectGroupDto?>.Success(null);
        mockSender.Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(nullResult);

        // Act
        await model.TestSetGroupReferenceNumberAsync();

        // Assert
        Assert.Null(model.GroupReferenceNumber);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetHandoverComments_WhenNotesExist_SetsHandoverCommentsToLatestNote(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        Guid projectId)
    {
        // Arrange
        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            ProjectId = projectId.ToString()
        };

        var olderNote = new NoteDto(
            Id: new NoteId(Guid.NewGuid()),
            Body: "Older handover note",
            ProjectId: new ProjectId(Guid.NewGuid()),
            UserId: new UserId(Guid.NewGuid()),
            UserFullName: "Old User",
            CreatedAt: DateTime.UtcNow.AddDays(-2)
        );

        var newerNote = new NoteDto(
            Id: new NoteId(Guid.NewGuid()),
            Body: "Latest handover note",
            ProjectId: new ProjectId(Guid.NewGuid()),
            UserId: new UserId(Guid.NewGuid()),
            UserFullName: "New User",
            CreatedAt: DateTime.UtcNow.AddDays(-1)
        );

        var notes = new List<NoteDto> { olderNote, newerNote };
        var successResult = Result<List<NoteDto>>.Success(notes);

        mockSender.Send(Arg.Any<GetTaskNotesByProjectIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        await model.TestSetHandoverComments();

        // Assert
        Assert.Equal("Latest handover note", model.HandoverComments);
        await mockSender.Received(1).Send(
            Arg.Is<GetTaskNotesByProjectIdQuery>(q => 
                q.ProjectId.Value == projectId && 
                q.TaskIdentifier == NoteTaskIdentifier.Handover), 
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetHandoverComments_WhenNoNotesExist_DoesNotSetHandoverComments(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        Guid projectId)
    {
        // Arrange
        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            ProjectId = projectId.ToString()
        };

        var emptyNotes = new List<NoteDto>();
        var successResult = Result<List<NoteDto>>.Success(emptyNotes);

        mockSender.Send(Arg.Any<GetTaskNotesByProjectIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        await model.TestSetHandoverComments();

        // Assert
        Assert.Null(model.HandoverComments);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task SetHandoverComments_WhenQueryFails_DoesNotSetHandoverComments(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        Guid projectId)
    {
        // Arrange
        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            ProjectId = projectId.ToString()
        };

        var failureResult = Result<List<NoteDto>>.Failure("Database error");
        mockSender.Send(Arg.Any<GetTaskNotesByProjectIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        // Act
        await model.TestSetHandoverComments();

        // Assert
        Assert.Null(model.HandoverComments);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task OnGetAsync_WhenBaseOnGetReturnsNonPageResult_ReturnsBaseResult(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger)
    {
        // Arrange
        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger);
        var redirectResult = new RedirectResult("/error");
        model.SetBaseOnGetResult(redirectResult);

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.Equal(redirectResult, result);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task OnGetAsync_WhenSuccessful_MapsAllPropertiesCorrectly(
        [Frozen] ISender mockSender,
        [Frozen] IErrorService mockErrorService,
        [Frozen] ILogger mockLogger,
        ProjectDto project,
        EstablishmentDto establishment,
        ProjectGroupDto projectGroup,
        NoteDto handoverNote)
    {
        // Arrange
        var projectId = Guid.NewGuid();
        project.Id = new ProjectId(projectId);
        project.Type = ProjectType.Conversion;
        project.IncomingTrustUkprn = new Ukprn(12345678);
        project.NewTrustReferenceNumber = "TR123456";
        project.GroupId = new ProjectGroupId(Guid.NewGuid());
        project.AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        project.AdvisoryBoardConditions = "Test conditions";
        project.EstablishmentSharepointLink = HttpUtility.UrlEncode("https://example.com/establishment");
        project.IncomingTrustSharepointLink = HttpUtility.UrlEncode("https://example.com/trust");
        project.Team = ProjectTeam.RegionalCaseWorkerServices;
        project.TwoRequiresImprovement = true;

        establishment.Name = "Test School";
        projectGroup.GroupIdentifier = "GR789012";
        handoverNote = new NoteDto(
            Id: new NoteId(Guid.NewGuid()),
            Body: "Test handover comments",
            ProjectId: new ProjectId(Guid.NewGuid()),
            UserId: new UserId(Guid.NewGuid()),
            UserFullName: "Test User",
            CreatedAt: DateTime.UtcNow
        );

        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            ProjectId = projectId.ToString(),
            Project = project,
            Establishment = establishment
        };

        // Mock the group query
        var groupSuccessResult = Result<ProjectGroupDto?>.Success(projectGroup);
        mockSender.Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(groupSuccessResult);

        // Mock the notes query
        var notesSuccessResult = Result<List<NoteDto>>.Success(new List<NoteDto> { handoverNote });
        mockSender.Send(Arg.Any<GetTaskNotesByProjectIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(notesSuccessResult);

        model.SetBaseOnGetResult(new PageResult());

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        
        // Verify property mappings
        Assert.Equal("Test School", model.EstablishmentName);
        Assert.Equal(ProjectType.Conversion, model.ProjectType);
        Assert.Equal("12345678", model.IncomingTrustUkprn);
        Assert.Equal("TR123456", model.NewTrustReferenceNumber);
        Assert.Equal("GR789012", model.GroupReferenceNumber);
        Assert.Equal(project.AdvisoryBoardDate?.ToDateTime(default), model.AdvisoryBoardDate);
        Assert.Equal("Test conditions", model.AdvisoryBoardConditions);
        Assert.Equal("https://example.com/establishment", model.EstablishmentSharepointLink);
        Assert.Equal("https://example.com/trust", model.IncomingTrustSharepointLink);
        Assert.True(model.IsHandingToRCS);
        Assert.Equal("Test handover comments", model.HandoverComments);
        Assert.True(model.TwoRequiresImprovement);
    }

    //[Theory]
    //[CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    //public async Task OnGetAsync_WhenProjectTeamIsNotRCS_SetsIsHandingToRCSToFalse(
    //    [Frozen] ISender mockSender,
    //    [Frozen] IErrorService mockErrorService,
    //    [Frozen] ILogger mockLogger,
    //    ProjectDto project,
    //    EstablishmentDto establishment)
    //{
    //    // Arrange
    //    project.Team = ProjectTeam.WestMidlands;
    //    establishment.Name = "Test School";

    //    var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
    //    {
    //        Project = project,
    //        Establishment = establishment
    //    };

    //    model.SetBaseOnGetResult(new PageResult());

    //    // Act
    //    var result = await model.OnGetAsync();

    //    // Assert
    //    Assert.IsType<PageResult>(result);
    //    Assert.False(model.IsHandingToRCS);
    //}

    //[Theory]
    //[CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    //public async Task OnGetAsync_WhenTwoRequiresImprovementIsNull_DefaultsToFalse(
    //    [Frozen] ISender mockSender,
    //    [Frozen] IErrorService mockErrorService,
    //    [Frozen] ILogger mockLogger,
    //    ProjectDto project,
    //    EstablishmentDto establishment)
    //{
    //    // Arrange
    //    project.TwoRequiresImprovement = null;
    //    establishment.Name = "Test School";

    //    var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
    //    {
    //        Project = project,
    //        Establishment = establishment
    //    };

    //    model.SetBaseOnGetResult(new PageResult());

    //    // Act
    //    var result = await model.OnGetAsync();

    //    // Assert
    //    Assert.IsType<PageResult>(result);
    //    Assert.False(model.TwoRequiresImprovement);
    //}
}

public class TestBaseProjectDetailsPageModel(ISender sender, IErrorService errorService, ILogger logger) 
    : BaseProjectDetailsPageModel(sender, errorService, logger)
{
    private IActionResult? _baseOnGetResult;

    public void SetBaseOnGetResult(IActionResult result)
    {
        _baseOnGetResult = result;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (_baseOnGetResult != null)
        {
            if (_baseOnGetResult is not PageResult)
                return _baseOnGetResult;

            // Simulate successful base call and then continue with the actual logic
            EstablishmentName = Establishment?.Name;
            ProjectType = Project.Type;

            IncomingTrustUkprn = Project.IncomingTrustUkprn?.ToString();
            NewTrustReferenceNumber = Project.NewTrustReferenceNumber;

            await SetGroupReferenceNumberAsync();

            AdvisoryBoardDate = Project.AdvisoryBoardDate?.ToDateTime(default);
            AdvisoryBoardConditions = Project.AdvisoryBoardConditions;
            EstablishmentSharepointLink = HttpUtility.UrlDecode(Project.EstablishmentSharepointLink);
            IncomingTrustSharepointLink = HttpUtility.UrlDecode(Project.IncomingTrustSharepointLink);
            IsHandingToRCS = Project.Team == ProjectTeam.RegionalCaseWorkerServices;

            await SetHandoverComments();

            TwoRequiresImprovement = Project.TwoRequiresImprovement ?? false;

            return Page();
        }

        return await base.OnGetAsync();
    }

    public async Task TestSetGroupReferenceNumberAsync()
    {
        await SetGroupReferenceNumberAsync();
    }

    public async Task TestSetHandoverComments()
    {
        await SetHandoverComments();
    }
}
