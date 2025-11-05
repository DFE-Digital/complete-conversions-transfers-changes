using System.Linq.Expressions;
using AutoFixture.Xunit2;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class UpdateConversionProjectCommandHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ExistingGroupIdentifier_IsHandlingRSC_True_ExistingLastComment_WithHandoverComment_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<ProjectGroup> projectGroupRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var project = new Domain.Entities.Project { Id = projectId, CreatedAt = DateTime.UtcNow };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => project = proj),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(project));

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

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

            var notesQueryable = new[] { note }.AsQueryable().BuildMock();

            var command = new UpdateConversionProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               IsHandingToRCS: true,
               DirectiveAcademyOrder: true,
               TwoRequiresImprovement: true,
               User: userDto
            );

            var handler = new UpdateConversionProjectCommandHandler(mockProjectRepository, projectGroupRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                ), default);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_Dettaches_GroupIdentifier_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var project = new Domain.Entities.Project { Id = projectId, CreatedAt = DateTime.UtcNow };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => project = proj),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(project));

            var command = new UpdateConversionProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: null, // No goup, so project will be dettached
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               IsHandingToRCS: true,
               DirectiveAcademyOrder: true,
               TwoRequiresImprovement: true,
               User: userDto
            );

            var handler = new UpdateConversionProjectCommandHandler(mockProjectRepository, projectGroupRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == null // Group should be null
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                ), default);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_IsHandlingRCS_False_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());
            var project = new Domain.Entities.Project { Id = projectId, CreatedAt = DateTime.UtcNow };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => project = proj),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(project));

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

            var command = new UpdateConversionProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               IsHandingToRCS: false, // Not handling to RCS
               DirectiveAcademyOrder: true,
               TwoRequiresImprovement: true,
               User: userDto
            );

            var handler = new UpdateConversionProjectCommandHandler(mockProjectRepository, projectGroupRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.AssignedToId == userDto.Id // assigned to user id
                        && x.AssignedAt != null // assigned at should be set
                        && x.Team == (userDto!.Team).FromDescription<ProjectTeam>()
                ), default);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_LastComment_Without_HandoverNotes_Removes_ExistingNote_UpdatesProject(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> projectGroupRepository)
        {
            // Setup the user dto
            const ProjectTeam team = ProjectTeam.WestMidlands;
            var userDto = new UserDto
            {
                Id = new UserId(Guid.NewGuid()),
                Team = team.ToDescription()
            };

            var projectId = new ProjectId(Guid.NewGuid());

            var project = new Domain.Entities.Project 
            { 
                Id = projectId, 
                CreatedAt = DateTime.UtcNow,
            };

            var projectsQueryable = new[] { project }.AsQueryable().BuildMock();
            mockProjectRepository.Query().Returns(projectsQueryable);

            mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => project = proj),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(project));

            var groupIdentifier = "GR123";

            var group = new ProjectGroup
            {
                Id = new ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupIdentifier
            };

            projectGroupRepository
                .FindAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(group));

            var command = new UpdateConversionProjectCommand(
               projectId,
               IncomingTrustUkprn: new Ukprn(21),
               NewTrustReferenceNumber: "TR123",
               GroupReferenceNumber: groupIdentifier,
               AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
               AdvisoryBoardConditions: "Conditions",
               EstablishmentSharepointLink: "https://example.com/establishment",
               IncomingTrustSharepointLink: "https://example.com/incoming-trust",
               IsHandingToRCS: true,
               DirectiveAcademyOrder: true,
               TwoRequiresImprovement: true,
               User: userDto
            );

            var handler = new UpdateConversionProjectCommandHandler(mockProjectRepository, projectGroupRepository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await mockProjectRepository
                .Received(1)
                .UpdateAsync(Arg.Is<Domain.Entities.Project>(
                    x => x.Id == projectId
                        && x.IncomingTrustUkprn == command.IncomingTrustUkprn
                        && x.NewTrustReferenceNumber == command.NewTrustReferenceNumber
                        && x.GroupId == group.Id
                        && x.AdvisoryBoardDate == command.AdvisoryBoardDate
                        && x.AdvisoryBoardConditions == command.AdvisoryBoardConditions
                        && x.EstablishmentSharepointLink == command.EstablishmentSharepointLink
                        && x.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                        && x.Team == ProjectTeam.RegionalCaseWorkerServices
                ), default);
        }
    }
}
