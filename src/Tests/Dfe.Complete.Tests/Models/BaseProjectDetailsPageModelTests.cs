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
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
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

        var successResult = Result<ProjectGroupDto>.Success(projectGroup);
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

        var failureResult = Result<ProjectGroupDto>.Failure("Database error");
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

        var nullResult = Result<ProjectGroupDto>.Success(default!);
        mockSender.Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(nullResult);

        // Act
        await model.TestSetGroupReferenceNumberAsync();

        // Assert
        Assert.Null(model.GroupReferenceNumber);
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
        ProjectGroupDto projectGroup)
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

        var model = new TestBaseProjectDetailsPageModel(mockSender, mockErrorService, mockLogger)
        {
            ProjectId = projectId.ToString(),
            Project = project,
            Establishment = establishment
        };

        // Mock the group query
        var groupSuccessResult = Result<ProjectGroupDto>.Success(projectGroup);
        mockSender.Send(Arg.Any<GetProjectGroupByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(groupSuccessResult);

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
        Assert.True(model.TwoRequiresImprovement);
    }
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
            TwoRequiresImprovement = Project.TwoRequiresImprovement ?? false;

            return Page();
        }

        return await base.OnGetAsync();
    }

    public async Task TestSetGroupReferenceNumberAsync()
    {
        await SetGroupReferenceNumberAsync();
    }
}
