using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Microsoft.EntityFrameworkCore;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class UpdateTransferProjectCommandHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ExistingGroupIdentifier_IsHandlingRSC_True_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());
            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

            var transferTasksData = new TransferTasksData
            (
                tasksDataId,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false,
                false
            );

            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult(transferTasksData));

            var handOverText = "Initial handover note";

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: true,
               HandoverComments: handOverText,
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.OutgoingTrustUkprn == command.OutgoingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.OutgoingTrustSharepointLink == command.OutgoingTrustSharepointLink
                        && x.TwoRequiresImprovement == command.TwoRequiresImprovement
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                ), Arg.Any<CancellationToken>());

            await transferTaskDataRepository
                .Received(1)
                .UpdateAsync(Arg.Is<TransferTasksData>(
                    x => x.Id == tasksDataId
                        && x.InadequateOfsted == command.InadequateOfsted
                        && x.FinancialSafeguardingGovernanceIssues == command.FinancialSafeguardingGovernanceIssues
                        && x.OutgoingTrustToClose == command.OutgoingTrustToClose
                ), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_DetachesGroupIdentifier_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());
            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            var transferTasksData = new TransferTasksData
            (
                tasksDataId,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false,
                false
            );

            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult(transferTasksData));

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: null, // No group, so project will be detached
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: true,
               HandoverComments: "Test handover",
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.OutgoingTrustUkprn == command.OutgoingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == null // Group should be null
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.OutgoingTrustSharepointLink == command.OutgoingTrustSharepointLink
                        && x.TwoRequiresImprovement == command.TwoRequiresImprovement
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                ), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_IsHandingToRCS_False_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());
            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

          var transferTasksData = new TransferTasksData
            (
                tasksDataId,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false,
                false
            );

            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult(transferTasksData));

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: false, // Not handling to RCS
               HandoverComments: "Test handover",
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.OutgoingTrustUkprn == command.OutgoingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.OutgoingTrustSharepointLink == command.OutgoingTrustSharepointLink
                        && x.TwoRequiresImprovement == command.TwoRequiresImprovement
                        && x.AssignedToId == userDto.Id // assigned to user id
                        && x.AssignedAt != null // assigned at should be set
                        && x.Team == (userDto!.Team).FromDescription<ProjectTeam>()
                ), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_LastComment_Without_HandoverNotes_Removes_ExistingNote_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());

            var handOverText = "Initial handover note";
            Note note = new Note
            {
                Id = new NoteId(Guid.NewGuid()),
                ProjectId = projectId,
                UserId = userDto.Id,
                TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(),
                Body = handOverText,
                CreatedAt = DateTime.UtcNow
            };

            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId,
                Notes = new List<Domain.Entities.Note> { note } // Existing note
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

          var transferTasksData = new TransferTasksData
            (
                tasksDataId,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false,
                false
            );

            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult(transferTasksData));

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: true,
               HandoverComments: null, // no handover comments
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.OutgoingTrustUkprn == command.OutgoingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.OutgoingTrustSharepointLink == command.OutgoingTrustSharepointLink
                        && x.TwoRequiresImprovement == command.TwoRequiresImprovement
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                ), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_NO_LastComment_With_HandoverNotes_AddsNewNote_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());
            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

          var transferTasksData = new TransferTasksData
            (
                tasksDataId,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false,
                false
            );

            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult(transferTasksData));

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: true,
               HandoverComments: "Initial handover note",
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.OutgoingTrustUkprn == command.OutgoingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.OutgoingTrustSharepointLink == command.OutgoingTrustSharepointLink
                        && x.TwoRequiresImprovement == command.TwoRequiresImprovement
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                        && x.Notes.Count == 1 // One note should be present
                ), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_LastComment_With_HandoverNotes_Updates_ExistingNote_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());

            var originalHandOverText = "Original handover note";
            var updatedHandOverText = "Updated handover note";

            Note note = new Note
            {
                Id = new NoteId(Guid.NewGuid()),
                ProjectId = projectId,
                UserId = userDto.Id,
                TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription(),
                Body = originalHandOverText,
                CreatedAt = DateTime.UtcNow
            };

            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId,
                Notes = new List<Domain.Entities.Note> { note } // Existing note
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

            var transferTasksData = new TransferTasksData
                (
                    tasksDataId,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    false,
                    false,
                    false
                );

            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult(transferTasksData));

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: true,
               HandoverComments: updatedHandOverText, // Updated handover comments
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.OutgoingTrustUkprn == command.OutgoingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.OutgoingTrustSharepointLink == command.OutgoingTrustSharepointLink
                        && x.TwoRequiresImprovement == command.TwoRequiresImprovement
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                        && x.Notes.Count == 1 // One note should still be present
                        && x.Notes.First().Body == updatedHandOverText // Note body should be updated
                ), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_WhenTransferTasksDataIsNull_DoesNotUpdateTasksData(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository,
            [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> transferTaskDataRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var tasksDataId = new TaskDataId(Guid.NewGuid());
            var project = new Domain.Entities.Project
            {
                Id = projectId,
                CreatedAt = DateTime.UtcNow,
                TasksDataId = tasksDataId
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            // Return null for transfer tasks data
            transferTaskDataRepository
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(Task.FromResult<TransferTasksData>(default!));

            var command = new UpdateTransferProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               OutgoingTrustUkprn: new Ukprn(22),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: null,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               OutgoingTrustSharepointLink: "https://example.com/outgoing-trust",
               TwoRequiresImprovement: true,
               InadequateOfsted: true,
               FinancialSafeguardingGovernanceIssues: true,
               OutgoingTrustToClose: true,
               IsHandingToRCS: true,
               HandoverComments: null,
               User: userDto
            );

            var handler = new UpdateTransferProjectCommandHandler(mockProjectRepository, projectGroupRepository, transferTaskDataRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await transferTaskDataRepository
                .DidNotReceive()
                .UpdateAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>());

            // But project should still be updated
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>());
        }
    }
}
