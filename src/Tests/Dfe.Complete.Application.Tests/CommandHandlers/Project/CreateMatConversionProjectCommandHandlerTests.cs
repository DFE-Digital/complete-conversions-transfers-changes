using System.Linq.Expressions;
using AutoFixture;
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
using MediatR;
using Moq;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Tests.Common.Customizations.Models;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class CreateMatConversionProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command
    )
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        const ProjectTeam userTeam = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = userTeam.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);
            
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
            
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        Domain.Entities.Project capturedProject = null!;
        mockProjectRepository
            .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(capturedProject));

        mockConversionTaskRepository
            .AddAsync(Arg.Any<ConversionTasksData>(), Arg.Any<CancellationToken>())
            .Returns(conversionTask);
        
        GiasEstablishment giasEstablishment =
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
        await mockConversionTaskRepository.Received(1)
            .AddAsync(Arg.Any<ConversionTasksData>(), Arg.Any<CancellationToken>());

        // Verify key fields are mapped correctly
        Assert.Equal(command.Urn, capturedProject.Urn);
        Assert.Equal(command.NewTrustName, capturedProject.NewTrustName);
        Assert.Equal(command.NewTrustReferenceNumber, capturedProject.NewTrustReferenceNumber);
        Assert.Equal(command.SignificantDate, capturedProject.SignificantDate);
        Assert.Equal(command.IsSignificantDateProvisional, capturedProject.SignificantDateProvisional);
        Assert.Equal(command.IsDueTo2Ri, capturedProject.TwoRequiresImprovement);
        Assert.Equal(command.HasAcademyOrderBeenIssued, capturedProject.DirectiveAcademyOrder);
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
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command
    )
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        command = command with { HandingOverToRegionalCaseworkService = true };

        var userTeam = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = userTeam.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);
            
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
            
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        Domain.Entities.Project capturedProject = null!;
        mockProjectRepository
            .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(capturedProject));

        mockConversionTaskRepository
            .AddAsync(Arg.Any<ConversionTasksData>(), Arg.Any<CancellationToken>())
            .Returns(conversionTask);
        
        GiasEstablishment giasEstablishment =
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
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command
    )
    {
        // Arrange
        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        command = command with { HandingOverToRegionalCaseworkService = false };

        const ProjectTeam userTeam = ProjectTeam.WestMidlands;
        var userDto = new UserDto
        {
            Id = new UserId(Guid.NewGuid()),
            Team = userTeam.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);
            
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));
            
        mockSender
            .Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), default))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));

        Domain.Entities.Project capturedProject = null!;
        mockProjectRepository
            .AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(capturedProject));

        mockConversionTaskRepository
            .AddAsync(Arg.Any<ConversionTasksData>(), Arg.Any<CancellationToken>())
            .Returns(conversionTask);
        
        GiasEstablishment giasEstablishment =
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
    public async Task Handle_ShouldThrowNotFoundException_WhenUserRequestFails(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command)
    {
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(
                new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));

        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto?>.Failure("DB ERROR"));

        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        GiasEstablishment giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));
        
        command = command with {HandingOverToRegionalCaseworkService = false};

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        Assert.Equal("No user found.", exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Equal("DB ERROR", exception.InnerException.Message);
            
        await mockProjectRepository.Received(0)
            .AddAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>());
        await mockConversionTaskRepository.Received(0)
            .AddAsync(It.IsAny<ConversionTasksData>(), It.IsAny<CancellationToken>());
    }
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserRequestCantFindMatchingUser(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateMatConversionProjectCommand command,
        IFixture fixture)
    {
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(
                new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));

        mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto?>.Success(null));
        
        GiasEstablishment giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));

        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        Assert.Equal("No user found.", exception.Message);
        Assert.NotNull(exception.InnerException);
            
        await mockProjectRepository.Received(0)
            .AddAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>());
        await mockConversionTaskRepository.Received(0)
            .AddAsync(It.IsAny<ConversionTasksData>(), It.IsAny<CancellationToken>());
    }

    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowNotFoundException_WhenLocalAuthorityRequestFails(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command)
    {
        var expectedError = "Local authority not found";
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Failure(expectedError));

        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        GiasEstablishment giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        var expectedMessage = $"No Local authority could be found via Establishments for School Urn: {command.Urn.Value}.";
        Assert.Equal(expectedMessage, exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Equal(expectedError, exception.InnerException.Message);
            
        await mockProjectRepository.Received(0)
            .AddAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>());
        await mockConversionTaskRepository.Received(0)
            .AddAsync(It.IsAny<ConversionTasksData>(), It.IsAny<CancellationToken>());
    }
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowNotFoundException_WhenLocalAuthorityIdIsNull(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command)
    {
        var responseDto = new GetLocalAuthorityBySchoolUrnResponseDto(null);
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(responseDto));

        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        GiasEstablishment giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        var expectedMessage = $"No Local authority could be found via Establishments for School Urn: {command.Urn.Value}.";
        Assert.Equal(expectedMessage, exception.Message);
            
        await mockProjectRepository.Received(0)
            .AddAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>());
        await mockConversionTaskRepository.Received(0)
            .AddAsync(It.IsAny<ConversionTasksData>(), It.IsAny<CancellationToken>());
    }
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowNotFoundException_WhenLocalAuthorityRequestSuccess_WithNullResponse(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command)
    {
        mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(null));

        var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
        var handler =
            new CreateMatConversionProjectCommandHandler(mockProjectRepository,
                mockConversionTaskRepository,
                commonProject);
        
        var giasEstablishment =
            fixture.Customize(new GiasEstablishmentsCustomization() { Urn = command.Urn }).Create<GiasEstablishment>();

        mockEstablishmentRepository.FindAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(giasEstablishment));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        var expectedMessage = $"No Local authority could be found via Establishments for School Urn: {command.Urn.Value}.";
        Assert.Equal(expectedMessage, exception.Message);
            
        await mockProjectRepository.Received(0)
            .AddAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>());
        await mockConversionTaskRepository.Received(0)
            .AddAsync(It.IsAny<ConversionTasksData>(), It.IsAny<CancellationToken>());
    }
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldThrowException_WhenEstablishmentIsNotFound(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepository,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture,
        CreateMatConversionProjectCommand command)
    {
        // Arrange
       //Setup the handler
       var commonProject = new CreateProjectCommon(mockEstablishmentRepository, mockSender.Object);
       var handler =
           new CreateMatConversionProjectCommandHandler(mockProjectRepository,
               mockConversionTaskRepository,
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
        await mockConversionTaskRepository.DidNotReceive()
            .AddAsync(Arg.Any<ConversionTasksData>(), Arg.Any<CancellationToken>());
    }
}