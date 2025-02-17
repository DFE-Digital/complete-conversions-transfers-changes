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
using MediatR;
using Moq;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Tests.Common.Customizations.Models;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class CreateMatConversionProjectCommandHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
            [Frozen] Mock<ISender> mockSender,
            CreateMatConversionProjectCommand command
        )
        {
            // Arrange
            var handler = new CreateMatConversionProjectCommandHandler(mockProjectRepository, mockConversionTaskRepository, mockSender.Object);
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
            [Frozen] Mock<ISender> mockSender,
            CreateMatConversionProjectCommand command
        )
        {
            // Arrange
            var handler = new CreateMatConversionProjectCommandHandler(mockProjectRepository, mockConversionTaskRepository, mockSender.Object);

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
            [Frozen] Mock<ISender> mockSender,
            CreateMatConversionProjectCommand command
        )
        {
            // Arrange
            var handler = new CreateMatConversionProjectCommandHandler(mockProjectRepository, mockConversionTaskRepository, mockSender.Object);
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
        public async Task Handle_ShouldThrowExceptionWhenUserRequestFails(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] Mock<ISender> mockSender,
        CreateMatConversionProjectCommand command)
        {
            // Arrange
            var handler = new CreateMatConversionProjectCommandHandler(mockProjectRepository, mockConversionTaskRepository, mockSender.Object);
            var expectedErrorMessage = "User retrieval failed: DB ERROR";
               
            mockSender.Setup(s => s.Send(It.IsAny<GetLocalAuthorityBySchoolUrnQuery>(), default))
                .ReturnsAsync(Result<GetLocalAuthorityBySchoolUrnResponseDto?>.Success(new GetLocalAuthorityBySchoolUrnResponseDto(Guid.NewGuid())));

            mockSender.Setup(s => s.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result<UserDto>.Failure("DB ERROR"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, default));
            
            await mockProjectRepository.Received(0).AddAsync(Arg.Any<Domain.Entities.Project>());
            await mockConversionTaskRepository.Received(0).AddAsync(Arg.Any<ConversionTasksData>());

            Assert.Equal(exception.Message, expectedErrorMessage);
        }
    }
}
