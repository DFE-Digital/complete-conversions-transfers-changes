using AutoMapper;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectByUrnQueryHandlerTests
    {
        private readonly IProjectReadRepository _mockProjectRepository;
        private readonly IMapper _mockMapper;
        private readonly GetProjectByUrnQueryHandler _handler;

        public GetProjectByUrnQueryHandlerTests()
        {
            _mockProjectRepository = Substitute.For<IProjectReadRepository>();
            _mockMapper = Substitute.For<IMapper>();
            var mockLogger = Substitute.For<Microsoft.Extensions.Logging.ILogger<GetProjectByUrnQueryHandler>>();
            _handler = new GetProjectByUrnQueryHandler(_mockProjectRepository, _mockMapper, mockLogger);
        }
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetAProjectByUrn_WhenCommandIsValid(GetProjectByUrnQuery command)
        {
            // Arrange
            var parameters = new CreateConversionProjectParams(
                new ProjectId(Guid.NewGuid()),
                command.Urn,
                Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
                new Ukprn(2),
                Region.London,
                true,
                DateOnly.FromDateTime(DateTime.Today.AddDays(15)),
                "Advisory board conditions",
                null,
                new UserId(Guid.NewGuid()),
                Guid.NewGuid()
            );

            var project = Domain.Entities.Project.CreateConversionProject(parameters);

            var queryableProjects = new List<Domain.Entities.Project> { project }.AsQueryable().BuildMock();
            _mockProjectRepository.Projects.Returns(queryableProjects);

            _mockMapper.Map<ProjectDto>(project).Returns(new ProjectDto()
            {
                Urn = command.Urn,
                Id = project.Id,
                RegionalDeliveryOfficer = project.RegionalDeliveryOfficer!,
                RegionalDeliveryOfficerId = project.RegionalDeliveryOfficerId
            });

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value?.Urn == command.Urn);
        }


        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldSucceedAndReturnNullWhenUnfoundProjectByUrn_WhenCommandIsValid(GetProjectByUrnQuery command)
        {
            // Arrange
            var noProjects = new List<Domain.Entities.Project> { }.AsQueryable().BuildMock();
            _mockProjectRepository.Projects.Returns(noProjects);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnErrorMessage_WhenExceptionIsThrown(GetProjectByUrnQuery command)
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            _mockProjectRepository.Projects.Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}