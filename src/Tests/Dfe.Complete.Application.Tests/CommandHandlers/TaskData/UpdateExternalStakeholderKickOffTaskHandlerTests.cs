using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.TaskData;

public class UpdateExternalStakeholderKickOffTaskHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateConversionTaskDataAndSignificantDate_WhenConversionProject(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ITaskDataReadRepository mockTaskDataReadRepository,
        [Frozen] ITaskDataWriteRepository mockTaskDataWriteRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        UpdateExternalStakeholderKickOffTaskCommand command,
        IFixture fixture
    )
    {
        // Arrange
        var project = fixture.Build<Domain.Entities.Project>()
            .With(p => p.Type, ProjectType.Conversion)
            .Create();

        var taskData = fixture.Create<ConversionTasksData>();
        var user = fixture.Create<User>();

        command = command with { ProjectId = project.Id, SignificantDate = DateOnly.FromDateTime(DateTime.UtcNow), UserEmail = user.Email };
        project.TasksDataId = taskData.Id;

        mockProjectRepository.Query().Returns(new[] { project }.AsQueryable().BuildMock());
        mockTaskDataReadRepository.ConversionTaskData.Returns(new[] { taskData }.AsQueryable().BuildMock());
        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>()).Returns(user);

        var handler = new UpdateExternalStakeholderKickOffTaskHandler(
            mockProjectRepository,
            mockTaskDataReadRepository,
            mockTaskDataWriteRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await mockProjectRepository.Received(1).UpdateAsync(project, Arg.Any<CancellationToken>());
        await mockSignificantDateReasonRepository.Received(1).AddAsync(Arg.Any<SignificantDateHistoryReason>(), Arg.Any<CancellationToken>());
        Assert.Equal(command.SignificantDate, project.SignificantDate);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateTransferTaskData_WhenTransferProject(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ITaskDataReadRepository mockTaskDataReadRepository,
        [Frozen] ITaskDataWriteRepository mockTaskDataWriteRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        UpdateExternalStakeholderKickOffTaskCommand command,
        IFixture fixture
    )
    {
        // Arrange
        var project = fixture.Build<Domain.Entities.Project>()
            .With(p => p.Type, ProjectType.Transfer)
            .Create();

        var taskData = fixture.Create<TransferTasksData>();
        var user = fixture.Create<User>();

        command = command with { ProjectId = project.Id, SignificantDate = DateOnly.FromDateTime(DateTime.UtcNow), UserEmail = user.Email };
        project.TasksDataId = taskData.Id;

        mockProjectRepository.Query().Returns(new[] { project }.AsQueryable().BuildMock());
        mockTaskDataReadRepository.TransferTaskData.Returns(new[] { taskData }.AsQueryable().BuildMock());
        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>()).Returns(user);

        var handler = new UpdateExternalStakeholderKickOffTaskHandler(
            mockProjectRepository,
            mockTaskDataReadRepository,
            mockTaskDataWriteRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await mockProjectRepository.Received(1).UpdateAsync(project, Arg.Any<CancellationToken>());
        await mockSignificantDateReasonRepository.Received(1).AddAsync(Arg.Any<SignificantDateHistoryReason>(), Arg.Any<CancellationToken>());
        Assert.Equal(command.SignificantDate, project.SignificantDate);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrow_WhenUserNotFound(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ITaskDataReadRepository mockTaskDataReadRepository,
        [Frozen] ITaskDataWriteRepository mockTaskDataWriteRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        UpdateExternalStakeholderKickOffTaskCommand command,
        IFixture fixture
    )
    {
        // Arrange
        var project = fixture.Create<Domain.Entities.Project>();
        var taskData = fixture.Create<ConversionTasksData>();

        command = command with { ProjectId = project.Id, UserEmail = "someone@dfe.gov.uk" };
        project.TasksDataId = taskData.Id;

        mockProjectRepository.Query().Returns(new[] { project }.AsQueryable().BuildMock());
        mockTaskDataReadRepository.ConversionTaskData.Returns(new[] { taskData }.AsQueryable().BuildMock());
        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())!.Returns((User?)null);

        var handler = new UpdateExternalStakeholderKickOffTaskHandler(
            mockProjectRepository,
            mockTaskDataReadRepository,
            mockTaskDataWriteRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await mockProjectRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>());
    }
}
