using System.Linq.Expressions;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations; 
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours; 
using Dfe.Complete.Utils;
using NSubstitute;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class CreateMatTransferProjectCommandHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
            [Frozen] ICompleteRepository<User> mockUserRepository,
            CreateMatTransferProjectCommandHandler handler,
            CreateMatTransferProjectCommand command
        )
        {
            // Arrange
            const ProjectTeam userTeam = ProjectTeam.WestMidlands;
            var user = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Team = userTeam.ToDescription()
            };

            var createdAt = DateTime.UtcNow;
            var transferTaskId = Guid.NewGuid();
            var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);

            mockUserRepository
                .FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(user);

            Domain.Entities.Project capturedProject = null!;
            mockProjectRepository
                .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(capturedProject));

            mockTransferTaskRepository
                .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
                .Returns(transferTask);

            // Act
            var projectId = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(projectId);
            Assert.IsType<ProjectId>(projectId);

            await mockProjectRepository.Received(1)
                .AddAsync(capturedProject, Arg.Any<CancellationToken>());
            await mockTransferTaskRepository.Received(1)
                .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>());

            // Verify key fields are mapped correctly
            Assert.Equal(command.Urn, capturedProject.Urn);
            Assert.Equal(command.NewTrustName, capturedProject.NewTrustName);
            Assert.Equal(command.NewTrustReferenceNumber, capturedProject.NewTrustReferenceNumber);
            Assert.Equal(command.SignificantDate, capturedProject.SignificantDate);
            Assert.Equal(command.IsSignificantDateProvisional, capturedProject.SignificantDateProvisional);
            Assert.Equal(command.IsDueTo2Ri, capturedProject.TwoRequiresImprovement);
            Assert.Equal(command.AdvisoryBoardDate, capturedProject.AdvisoryBoardDate);
            Assert.Equal(command.AdvisoryBoardConditions, capturedProject.AdvisoryBoardConditions);
            Assert.Equal(command.EstablishmentSharepointLink, capturedProject.EstablishmentSharepointLink);
            Assert.Equal(command.IncomingTrustSharepointLink, capturedProject.IncomingTrustSharepointLink);
            Assert.Equal(command.HandoverComments, capturedProject.Notes.FirstOrDefault()?.Body);
            
            Assert.Equal(command.NewTrustName, capturedProject.NewTrustName);
            Assert.Equal(command.NewTrustReferenceNumber, capturedProject.NewTrustReferenceNumber);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldSetTeamToRcs_WhenHandoverToRcsTrue(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
            [Frozen] ICompleteRepository<User> mockUserRepository,
            CreateMatTransferProjectCommandHandler handler,
            CreateMatTransferProjectCommand command
        )
        {
            // Arrange
            command = command with { HandingOverToRegionalCaseworkService = true };

            var userTeam = ProjectTeam.WestMidlands;
            var user = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Team = userTeam.ToDescription()
            };

            var createdAt = DateTime.UtcNow;
            var transferTaskId = Guid.NewGuid();
            var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);

            mockUserRepository
                .FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(user);

            Domain.Entities.Project capturedProject = null!;
            mockProjectRepository
                .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(capturedProject));

            mockTransferTaskRepository
                .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
                .Returns(transferTask);

            // Act
            var projectId = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(projectId);
            Assert.Equal(ProjectTeam.RegionalCaseWorkerServices, capturedProject.Team);
            Assert.Null(capturedProject.AssignedAt);
            Assert.Null(capturedProject.AssignedToId);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldSetTeam_AssignedAt_AssignedTo_WhenNotHandingOverToRcs(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
            [Frozen] ICompleteRepository<User> mockUserRepository,
            CreateMatTransferProjectCommandHandler handler,
            CreateMatTransferProjectCommand command
        )
        {
            // Arrange
            command = command with { HandingOverToRegionalCaseworkService = false };

            const ProjectTeam userTeam = ProjectTeam.WestMidlands;
            var user = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Team = userTeam.ToDescription()
            };

            var createdAt = DateTime.UtcNow;
            var transferTaskId = Guid.NewGuid();
            var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);

            mockUserRepository
                .FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(user);

            Domain.Entities.Project capturedProject = null!;
            mockProjectRepository
                .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(capturedProject));

            mockTransferTaskRepository
                .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
                .Returns(transferTask);

            // Act
            var projectId = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(projectId);
            Assert.Equal(userTeam, capturedProject.Team);
            Assert.NotNull(capturedProject.AssignedAt);
            Assert.NotNull(capturedProject.AssignedToId);
        }
    }
}
