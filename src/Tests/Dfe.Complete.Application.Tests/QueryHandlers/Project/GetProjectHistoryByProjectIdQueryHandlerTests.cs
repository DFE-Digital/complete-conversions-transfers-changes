using AutoFixture.Xunit2;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using NSubstitute;
using AutoMapper;
using Dfe.Complete.Application.Projects.Interfaces;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectHistoryByProjectIdHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task Handle_ShouldReturnSuccess_WhenProjectExists(
            [Frozen] IProjectReadRepository mockProjectRepository,
            [Frozen] IMapper mockMapper,
            GetProjectHistoryByProjectIdHandler handler,
            ProjectDto mappedProject)
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            var query = new GetProjectHistoryByProjectIdQuery(validGuid.ToString());

            var projects = new List<Domain.Entities.Project>
            {
                new ()
                {
                    Id = new ProjectId(validGuid),
                    Notes = new List<Note>
                    {
                        new Note { NotableType = "SignificantDateHistoryReason" }
                    },
                }
            }.AsQueryable().BuildMock();

            mockProjectRepository.Projects.Returns(projects);

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
        public async Task Handle_ShouldReturnFailure_WhenSignificantDateHasNoUser(
            [Frozen] IProjectReadRepository mockProjectRepository,
            GetProjectHistoryByProjectIdHandler handler
        )
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            var query = new GetProjectHistoryByProjectIdQuery(validGuid.ToString());
            
            var projects = new List<Domain.Entities.Project>
            {
                new ()
                {
                    Id = new ProjectId(validGuid),
                    Notes = new List<Note>
                    {
                        new Note { NotableType = "SignificantDateHistoryReason" }
                    },
                    SignificantDateHistories = new List<SignificantDateHistory>
                    {
                        new()
                        {
                            User = null,
                            Reasons = new List<SignificantDateHistoryReason>(){ new SignificantDateHistoryReason() }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            mockProjectRepository.Projects.Returns(projects);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("One or more significant dates do not have an associated user", result.Error);
        }
    }
}
