using System.Linq.Expressions;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using Moq;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class CreateTransferProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateTransferProjectCommand command)
    {
        // Arrange
        var handler = new CreateTransferProjectCommandHandler(mockProjectRepository, mockTransferTaskRepository, mockSender.Object);

        const ProjectTeam team = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        var groupId = new ProjectGroupId(Guid.NewGuid());
           
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
        
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto { Id = groupId }));
        
        Domain.Entities.Project capturedProject = null!;
        
        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockProjectRepository.AddAsync(capturedProject, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));
        
        // Act
        var projectId = await handler.Handle(command, default);

        Assert.NotNull(projectId);
        Assert.IsType<ProjectId>(projectId);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(capturedProject);

        await mockTransferTaskRepository.Received(1).AddAsync(Arg.Any<TransferTasksData>());

        Assert.Equal(command.Urn, capturedProject.Urn);
        Assert.Equal(command.OutgoingTrustUkprn, capturedProject.OutgoingTrustUkprn);
        Assert.Equal(command.IncomingTrustUkprn, capturedProject.IncomingTrustUkprn);
        Assert.Equal(command.SignificantDate, capturedProject.SignificantDate);
        Assert.Equal(command.IsSignificantDateProvisional, capturedProject.SignificantDateProvisional);
        Assert.Equal(command.IsDueTo2Ri, capturedProject.TwoRequiresImprovement);
        Assert.Equal(command.AdvisoryBoardDate, capturedProject.AdvisoryBoardDate);
        Assert.Equal(command.AdvisoryBoardConditions, capturedProject.AdvisoryBoardConditions);
        Assert.Equal(command.EstablishmentSharepointLink, capturedProject.EstablishmentSharepointLink);
        Assert.Equal(command.IncomingTrustSharepointLink, capturedProject.IncomingTrustSharepointLink);
        Assert.Equal(command.OutgoingTrustSharepointLink, capturedProject.OutgoingTrustSharepointLink);
        Assert.Equal(groupId, capturedProject.GroupId);

        var capturedNote = capturedProject.Notes.FirstOrDefault();
        Assert.Equal(command.HandoverComments, capturedNote?.Body);
        Assert.Equal("handover", capturedNote?.TaskIdentifier);

    }


    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldAddNotes_WhenHandoverComments(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateTransferProjectCommand command)
    {
        var handler = new CreateTransferProjectCommandHandler(mockProjectRepository, mockTransferTaskRepository, mockSender.Object);

        command = command with { HandingOverToRegionalCaseworkService = false, HandoverComments = "this is a test note"};
        
        const ProjectTeam team = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        var groupId = new ProjectGroupId(Guid.NewGuid());
        
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
        
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto { Id = groupId }));

        mockProjectGroupRepository.FindAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ProjectGroup { Id = groupId }));
        
        Domain.Entities.Project capturedProject = null!;
        
        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));
    
        // Act
        await handler.Handle(command, default);
    
        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Notes.FirstOrDefault().Body == command.HandoverComments), default);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldSetTeamToRcs_WhenHandoverToRcsTrue(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateTransferProjectCommand command
        )
    {
        // Arrange
        var handler = new CreateTransferProjectCommandHandler(mockProjectRepository, mockTransferTaskRepository, mockSender.Object);

        command = command with { HandingOverToRegionalCaseworkService = true };

        const ProjectTeam team = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var transferTaskId = Guid.NewGuid();
        var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);
        var groupId = new ProjectGroupId(Guid.NewGuid());

   
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
        
        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<UserDto?>.Success(userDto));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto { Id = groupId }));

        Domain.Entities.Project capturedProject = null!;

        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockTransferTaskRepository.AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(transferTask));

        // Act
        var projectId = await handler.Handle(command, default);

        // Assert
        Assert.NotNull(projectId);
        Assert.Equal(ProjectTeam.RegionalCaseWorkerServices, capturedProject.Team);
        Assert.Null(capturedProject.AssignedAt);
        Assert.Null(capturedProject.AssignedToId);
    }


    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldSetTeam_AssignedAt_AssignedTo_WhenNOTHandingOverToRcs(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateTransferProjectCommand command
    )
    {
        // Arrange
        var handler = new CreateTransferProjectCommandHandler(mockProjectRepository, mockTransferTaskRepository, mockSender.Object);

        command = command with { HandingOverToRegionalCaseworkService = false };

        const ProjectTeam team = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var transferTaskId = Guid.NewGuid();
        var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);
        var groupId = new ProjectGroupId(Guid.NewGuid());

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
        
        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<UserDto?>.Success(userDto));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto { Id = groupId }));

        Domain.Entities.Project capturedProject = null!;

        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockProjectRepository.AddAsync(capturedProject, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockTransferTaskRepository.AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(transferTask));

        // Act
        var projectId = await handler.Handle(command, default);

        // Assert
        Assert.NotNull(projectId);
        Assert.Equal(team, capturedProject.Team);
        Assert.NotNull(capturedProject.AssignedAt);
        Assert.NotNull(capturedProject.AssignedToId);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowExceptionWhenUserRequestFails(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateTransferProjectCommand command)
    {
        // Arrange
        var handler = new CreateTransferProjectCommandHandler(mockProjectRepository, mockTransferTaskRepository, mockSender.Object);
        var expectedErrorMessage = "User retrieval failed: DB ERROR";

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
        
        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<UserDto>.Failure("DB ERROR"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, default));


        await mockProjectRepository.Received(0).AddAsync(Arg.Any<Domain.Entities.Project>());
        await mockTransferTaskRepository.Received(0).AddAsync(Arg.Any<TransferTasksData>());

        Assert.Equal(exception.Message, expectedErrorMessage);
    }
    
            [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldThrowException_WhenLocalAuthorityRequestFails(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
            [Frozen] Mock<ISender> mockSender,
            CreateTransferProjectCommand command)
        {
            mockSender
                .Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Failure("Local authority not found"));

            var handler = new CreateTransferProjectCommandHandler(
                mockProjectRepository, 
                mockTransferTaskRepository, 
                mockSender.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Failed to retrieve Local authority for School URN: {command.Urn}", exception.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldThrowException_WhenLocalAuthorityIdIsNull(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
            [Frozen] Mock<ISender> mockSender,
            CreateTransferProjectCommand command)
        {
            var responseDto = new GetLocalAuthorityBySchoolUrnResponseDto(null);
            mockSender
                .Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(responseDto));

            var handler = new CreateTransferProjectCommandHandler(
                mockProjectRepository, 
                mockTransferTaskRepository, 
                mockSender.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Failed to retrieve Local authority for School URN: {command.Urn}", exception.Message);
        }

}