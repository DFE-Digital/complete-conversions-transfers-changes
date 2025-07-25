using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Mappers;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq.Expressions; 

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnSuccess_WhenProjectIsFound(
            [Frozen] IProjectReadRepository mockProjectRepository,
            [Frozen] IMapper mockMapper,
            GetProjectByIdQueryHandler handler,
            Domain.Entities.Project project,
             ProjectDto mappedProject)
        {
            // Arrange
            var queryableProjects = new List<Domain.Entities.Project> { project }.AsQueryable().BuildMock();
            mockProjectRepository.Projects.Returns(queryableProjects);
            mockMapper.Map<ProjectDto>(Arg.Any<Domain.Entities.Project>()).Returns(mappedProject);

            var query = new GetProjectByIdQuery(project.Id); 

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(mappedProject, result.Value);
        }
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException(
            [Frozen] IProjectReadRepository mockProjectRepository,
           GetProjectByIdQueryHandler handler,
           Domain.Entities.Project project)
        {
            // Arrange
            var expectedMessage = "Repository error";
            mockProjectRepository.Projects
                .Throws(new Exception(expectedMessage));

            var query = new GetProjectByIdQuery(project.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Error);
        }
    }
}
