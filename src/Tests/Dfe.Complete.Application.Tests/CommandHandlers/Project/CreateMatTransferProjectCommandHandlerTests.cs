using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using NSubstitute;
using Moq;
using MediatR;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class CreateMatTransferProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command
    )
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        const ProjectTeam userTeam = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = userTeam.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var transferTaskId = Guid.NewGuid();
        var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);
            
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
            
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        Domain.Entities.Project capturedProject = null!;
        mockProjectRepository
            .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(capturedProject));

        mockTransferTaskRepository
            .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
            .Returns(transferTask);

        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
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
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command
    )
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        command = command with { HandingOverToRegionalCaseworkService = true };

        var userTeam = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = userTeam.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var transferTaskId = Guid.NewGuid();
        var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);
            
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
            
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        Domain.Entities.Project capturedProject = null!;
        mockProjectRepository
            .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(capturedProject));

        mockTransferTaskRepository
            .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
            .Returns(transferTask);

        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
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
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command
    )
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        command = command with { HandingOverToRegionalCaseworkService = false };

        const ProjectTeam userTeam = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = userTeam.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var transferTaskId = Guid.NewGuid();
        var transferTask = new TransferTasksData(new TaskDataId(transferTaskId), createdAt, createdAt, command.IsDueToInedaquateOfstedRating, command.IsDueToIssues, command.OutGoingTrustWillClose);
            
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
            
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        Domain.Entities.Project capturedProject = null!;
        mockProjectRepository
            .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(capturedProject));

        mockTransferTaskRepository
            .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>())
            .Returns(transferTask);

        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
        // Act
        var projectId = await handler.Handle(command, default);

        // Assert
        Assert.NotNull(projectId);
        Assert.Equal(userTeam, capturedProject.Team);
        Assert.NotNull(capturedProject.AssignedAt);
        Assert.NotNull(capturedProject.AssignedToId);
    }
        
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowException_WhenLocalAuthorityRequestFails(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command)
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        var expectedErrorMessage = $"No Local authority could be found via Establishments for School Urn: {command.Urn.Value}.";

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Failure("Local Authority DB error"));

        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

        await mockProjectRepository.Received(0)
            .AddAsync(Arg.Any<Domain.Entities.Project>());
        await mockTransferTaskRepository.Received(0)
            .AddAsync(Arg.Any<TransferTasksData>());

        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Equal("Local Authority DB error", exception.InnerException.Message);
    }
        
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowException_WhenUserRequestFails(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command)
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        var expectedErrorMessage = "No user found.";

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));

        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Failure("User DB error"));

        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
        command = command with {HandingOverToRegionalCaseworkService = false};
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

        await mockProjectRepository.Received(0)
            .AddAsync(Arg.Any<Domain.Entities.Project>());
        await mockTransferTaskRepository.Received(0)
            .AddAsync(Arg.Any<TransferTasksData>());

        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Equal("User DB error", exception.InnerException.Message);
    }
        
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowException_WhenUserRequestCantFindMatchingUser(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateMatTransferProjectCommand command,
        IFixture fixture)
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        var expectedErrorMessage = "No user found.";

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));

        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(null));
        
        var groupId = new ProjectGroupId(Guid.NewGuid());
        
        mockSender.Setup(s =>
                s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto { Id = groupId }));
        
        GiasEstablishment giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));

        command = command with { HandingOverToRegionalCaseworkService = false };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

        await mockProjectRepository.Received(0)
            .AddAsync(Arg.Any<Domain.Entities.Project>());
        await mockTransferTaskRepository.Received(0)
            .AddAsync(Arg.Any<TransferTasksData>());

        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowException_WhenLocalAuthorityRequestReturnsSuccess_WithNullResponse(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command)
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatTransferProjectCommandHandler(mockProjectRepository,
                mockTransferTaskRepository,
                commonProject);

        var expectedErrorMessage = $"No Local authority could be found via Establishments for School Urn: {command.Urn.Value}.";

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(null));

        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

        await mockProjectRepository.Received(0)
            .AddAsync(Arg.Any<Domain.Entities.Project>());
        await mockTransferTaskRepository.Received(0)
            .AddAsync(Arg.Any<TransferTasksData>());

        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.NotNull(exception.InnerException);
    }
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldThrowException_WhenEstablishmentIsNotFound(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatTransferProjectCommand command)
    {
        // Arrange
       //Setup the handler
       var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
       var handler =
           new CreateMatTransferProjectCommandHandler(mockProjectRepository,
               mockTransferTaskRepository,
               commonProject);

        // Setup the user dto
        const ProjectTeam team = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        // Setup the Project group
        var groupId = new ProjectGroupId(Guid.NewGuid());
        
        mockSender.Setup(s =>
                s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto { Id = groupId }));
        
        // Setup the Local Authority

        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(
                Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(
                    new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
        
        // Setup the User Ad query

        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        
        // Setup the mock project repository calls 
        Domain.Entities.Project capturedProject = null!;

        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockProjectRepository.AddAsync(capturedProject, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));
        
        // Setup the GiasEstablishment and mock call

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((GiasEstablishment) null));
        
        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal($"No establishment could be found for Urn: {command.Urn.Value}.", exception.Message);
        Assert.Null(exception.InnerException);

        await mockProjectRepository.DidNotReceive()
            .AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>());
        await mockTransferTaskRepository.DidNotReceive()
            .AddAsync(Arg.Any<TransferTasksData>(), Arg.Any<CancellationToken>());
    }
}