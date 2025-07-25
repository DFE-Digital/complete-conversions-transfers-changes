using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.TaskData;

public class UpdateExternalStakeholderKickOffTaskHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateTaskDataAndSignificantDate_WhenValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockTaskDataRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        UpdateExternalStakeholderKickOffTaskCommand command,
        Domain.Entities.Project project,
        IFixture fixture
    )
    {
        // Arrange
        var user = fixture.Create<User>();
        var taskData = fixture.Create<ConversionTasksData>();
        command = command with { ProjectId = project.Id, SignificantDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        project.TasksDataId = taskData.Id;
        user.Email = command.UserEmail;

        mockProjectRepository.Query().Returns(new[] { project }.AsQueryable().BuildMock());
        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>()).Returns(user);
        mockTaskDataRepository.Query().Returns(new[] { taskData }.AsQueryable().BuildMock());
        
        var handler = new UpdateExternalStakeholderKickOffTaskHandler(
            mockProjectRepository,
            mockTaskDataRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        await mockProjectRepository.Received(1).UpdateAsync(project, Arg.Any<CancellationToken>());
        await mockSignificantDateReasonRepository.Received(1).AddAsync(Arg.Any<SignificantDateHistoryReason>(), Arg.Any<CancellationToken>());
        Assert.Equal(project.SignificantDate, command.SignificantDate);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrow_WhenUserNotFound(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockTaskDataRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        UpdateExternalStakeholderKickOffTaskCommand command,
        Domain.Entities.Project project,
        ConversionTasksData taskData
    )
    {
        // Arrange
        command = command with { ProjectId = project.Id, SignificantDate = DateOnly.FromDateTime(DateTime.UtcNow) };

        project.TasksDataId = taskData.Id;

        mockProjectRepository.Query().Returns(new[] { project }.AsQueryable().BuildMock());
        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())!.Returns((User?)null);
        mockTaskDataRepository.Query().Returns(new[] { taskData }.AsQueryable().BuildMock());
        
        var handler = new UpdateExternalStakeholderKickOffTaskHandler(
            mockProjectRepository,
            mockTaskDataRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await mockProjectRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>());
    }
}
