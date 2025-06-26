using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using AutoMapper;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable.NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectHistoryByProjectIdHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task Handle_ShouldReturnSuccess_WhenProjectExists(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] IMapper mockMapper,
            GetProjectHistoryByProjectIdHandler handler,
            Domain.Entities.Project project,
            ProjectDto mappedProject
        )
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            var query = new GetProjectHistoryByProjectIdQuery(validGuid.ToString());

            project.Id = new ProjectId(validGuid);
            project.Notes = new List<Note>
            {
                new Note { NotableType = "SignificantDateHistoryReason" },
                new Note { NotableType = "OtherType" }
            };

            var mockDbSet = new List<Domain.Entities.Project> { project }.AsQueryable().BuildMockDbSet();
            mockProjectRepository.Query().Returns(mockDbSet);
            mockMapper.Map<ProjectDto>(Arg.Any<Domain.Entities.Project>()).Returns(mappedProject);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(mappedProject, result.Value);
        }


        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task Handle_ShouldReturnFailure_WhenExceptionThrown(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ILogger<GetProjectHistoryByProjectIdHandler> logger,
            GetProjectHistoryByProjectIdHandler handler
        )
        {
            // Arrange
            var query = new GetProjectHistoryByProjectIdQuery(Guid.NewGuid().ToString());
            mockProjectRepository.Query().Throws(new Exception("DB Error"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("DB Error", result.Error);
        }
    }
}
