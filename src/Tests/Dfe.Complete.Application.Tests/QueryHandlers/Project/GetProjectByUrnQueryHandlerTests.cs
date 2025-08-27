using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using NSubstitute.ExceptionExtensions;
using Dfe.Complete.Application.Projects.Interfaces;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectByUrnQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetAProjectByUrn_WhenCommandIsValid(
            [Frozen] IProjectReadRepository mockProjectRepository,
            [Frozen] IMapper mockMapper,
            GetProjectByUrnQueryHandler handler,
            GetProjectByUrnQuery command
        )
        {
            var now = DateTime.UtcNow;

            var project = Domain.Entities.Project.CreateConversionProject(
                new ProjectId(Guid.NewGuid()),
                command.Urn,
                now,
                now,
                TaskType.Conversion,
                ProjectType.Conversion,
                Guid.NewGuid(),
                DateOnly.MinValue,
                true,
                new Ukprn(2),
                Region.London,
                true,
                true,
                DateOnly.MinValue,
                "",
                "",
                "",
                null,
                default,
                new UserId(Guid.NewGuid()),
                null,
                null,
                null,
                Guid.NewGuid());

            // Arrange
            var queryableProjects = new List<Domain.Entities.Project> { project }.AsQueryable().BuildMock();
            mockProjectRepository.Projects.Returns(queryableProjects);

            mockMapper.Map<ProjectDto>(project).Returns(new ProjectDto()
            {
                Urn = command.Urn, Id = project.Id, RegionalDeliveryOfficer = project.RegionalDeliveryOfficer,
                RegionalDeliveryOfficerId = project.RegionalDeliveryOfficerId
            });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value?.Urn == command.Urn);
        }


        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldSucceedAndReturnNullWhenUnfoundProjectByUrn_WhenCommandIsValid(
            [Frozen] IProjectReadRepository mockProjectRepository,
            GetProjectByUrnQueryHandler handler,
            GetProjectByUrnQuery command
        )
        {
            // Arrange
            var noProjects = new List<Domain.Entities.Project> { }.AsQueryable().BuildMock();
            mockProjectRepository.Projects.Returns(noProjects);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnErrorMessage_WhenExceptionIsThrown(
            [Frozen] IProjectReadRepository mockProjectRepository,
            GetProjectByUrnQueryHandler handler,
            GetProjectByUrnQuery command
        )
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            mockProjectRepository.Projects.Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}