using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.AccuracyOfHigherNeedsTask;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks
{
    public class AccuracyOfHigherNeedsTaskModelTests
    {
        private readonly Mock<ISender> _mockSender = new();
        private readonly Mock<IProjectPermissionService> _mockProjectPermissionService = new();
        private readonly Guid _projectId = Guid.NewGuid();
        private readonly TaskDataId _tasksDataId = new(Guid.NewGuid());

        private AccuracyOfHigherNeedsTaskModel GetModel(ProjectType projectType = ProjectType.Conversion)
        {
            _mockProjectPermissionService
                .Setup(m => m.UserCanView(It.IsAny<ProjectDto>(), It.IsAny<ClaimsPrincipal?>()!))
                .Returns(true);

            _mockSender
                .Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), default))
                .ReturnsAsync(Result<ProjectDto?>.Success(new ProjectDto
                {
                    Id = new ProjectId(_projectId),
                    State = ProjectState.Active,
                    Urn = new Urn(123456),
                    Type = projectType,
                    TasksDataId = _tasksDataId
                }));

            _mockSender
                .Setup(s => s.Send(It.IsAny<GetEstablishmentByUrnRequest>(), default))
                .ReturnsAsync(Result<EstablishmentDto?>.Success(new EstablishmentDto()));

            _mockSender
                .Setup(s => s.Send(It.IsAny<GetUserByOidQuery>(), default))
                .ReturnsAsync(Result<UserDto?>.Success(new UserDto
                {
                    ActiveDirectoryUserId = "MockObjectIdentifier",
                    Team = ProjectTeam.BusinessSupport.ToString()
                }));

            _mockSender
                .Setup(s => s.Send(It.IsAny<GetTaskNotesByProjectIdQuery>(), default))
                .ReturnsAsync(Result<List<NoteDto>>.Success([]));

            _mockSender
                .Setup(s => s.Send(It.IsAny<GetTransferTasksDataByIdQuery>(), default))
                .ReturnsAsync(Result<TransferTaskDataDto>.Success(new TransferTaskDataDto { Id = _tasksDataId }));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
                new Claim("objectidentifier", "MockObjectIdentifier")
            ]));
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };

            return new AccuracyOfHigherNeedsTaskModel(
                _mockSender.Object,
                Mock.Of<IAuthorizationService>(),
                Mock.Of<ILogger<AccuracyOfHigherNeedsTaskModel>>(),
                _mockProjectPermissionService.Object)
            {
                ProjectId = _projectId.ToString(),
                TaskIdentifier = NoteTaskIdentifier.CheckAccuracyOfHigherNeeds,
                PageContext = new PageContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            };
        }

        private void SetupTaskData(ConversionTaskDataDto taskData) =>
            _mockSender
                .Setup(s => s.Send(It.IsAny<GetConversionTasksDataByIdQuery>(), default))
                .ReturnsAsync(Result<ConversionTaskDataDto>.Success(taskData));

        [Fact]
        public async Task OnGetAsync_PopulatesCheckboxes_FromTaskData()
        {
            // Arrange
            SetupTaskData(new ConversionTaskDataDto
            {
                Id = _tasksDataId,
                CheckAccuracyOfHigherNeedsNotApplicable = null,
                CheckAccuracyOfHigherNeedsConfirmPublishedNumber = null,
                CheckAccuracyOfHigherNeedsConfirmPublishedIncorrectNumber = true,
                CheckAccuracyOfHigherNeedsConfirmNumber = true,
                CheckAccuracyOfHigherNeedsCheckReturnedForm = false,
                CheckAccuracyOfHigherNeedsSendForm = true
            });
            var model = GetModel();

            // Act
            var result = await model.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(_tasksDataId.Value, model.TasksDataId);
            Assert.Null(model.NotApplicable);
            Assert.Null(model.ConfirmPublishedNumber);
            Assert.True(model.ConfirmPublishedIncorrectNumber);
            Assert.True(model.ConfirmNumber);
            Assert.False(model.CheckReturnedForm);
            Assert.True(model.SendForm);
            Assert.Equal(["confirm-published-incorrect-number", "confirm-number", "send-form"], model.SelectedOptions);
        }

        [Fact]
        public async Task OnGetAsync_RedirectsToErrorPage_WhenProjectIsNotAConversion()
        {
            // Arrange
            SetupTaskData(new ConversionTaskDataDto { Id = _tasksDataId });
            var model = GetModel(ProjectType.Transfer);

            // Act
            var result = await model.OnGetAsync();

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal(RouteConstants.ErrorPage, redirect.Url);
        }

        [Theory]
        [InlineData(new[] { "not-applicable" }, true, false, false, false, false, false)]
        [InlineData(new[] { "confirm-published-number" }, false, true, false, false, false, false)]
        [InlineData(new[] { "confirm-published-incorrect-number", "confirm-number", "check-returned-form", "send-form" }, false, false, true, true, true, true)]
        [InlineData(new string[0], false, false, false, false, false, false)]
        public async Task OnPost_SendsCommandWithSelections_AndRedirectsToTaskList(
            string[] selectedOptions,
            bool notApplicable,
            bool confirmPublishedNumber,
            bool confirmPublishedIncorrectNumber,
            bool confirmNumber,
            bool checkReturnedForm,
            bool sendForm)
        {
            // Arrange
            var model = GetModel();
            model.TasksDataId = _tasksDataId.Value;
            model.SelectedOptions = [.. selectedOptions];

            // Act
            var result = await model.OnPost();

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal(string.Format(RouteConstants.ProjectTaskList, _projectId), redirect.Url);

            _mockSender.Verify(s => s.Send(
                It.Is<UpdateAccuracyOfHigherNeedsTaskCommand>(c =>
                    c.TaskDataId == _tasksDataId &&
                    c.NotApplicable == notApplicable &&
                    c.ConfirmPublishedNumber == confirmPublishedNumber &&
                    c.ConfirmPublishedIncorrectNumber == confirmPublishedIncorrectNumber &&
                    c.ConfirmNumber == confirmNumber &&
                    c.CheckReturnedForm == checkReturnedForm &&
                    c.SendForm == sendForm),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
