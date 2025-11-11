using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectGroupByGroupReferenceNumberHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetAProjectGroupByGroupReference_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> mockProjectGroupRepository,
            [Frozen] IMapper mockMapper,
            GetProjectGroupByGroupReferenceNumberQueryHandler handler,
            GetProjectGroupByGroupReferenceNumberQuery command
            )
        {
            var projectGroup = new ProjectGroup();

            // Arrange
            mockProjectGroupRepository.GetAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>())
                .Returns(projectGroup);

            mockMapper.Map<ProjectGroupDto>(projectGroup).Returns(new ProjectGroupDto());

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockProjectGroupRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>());
            Assert.True(result.IsSuccess == true);
        }


        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldSucceedAndReturnNullWhenUnfoundProjectByUrn_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> mockProjectGroupRepository,
            GetProjectGroupByGroupReferenceNumberQueryHandler handler,
            GetProjectGroupByGroupReferenceNumberQuery command
            )
        {
            // Arrange
            mockProjectGroupRepository.GetAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup?, bool>>>())
                .Returns((Domain.Entities.ProjectGroup?)null);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockProjectGroupRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>());
            Assert.True(result.IsSuccess == true);
            Assert.True(result.Value == null);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnError_WhenRepoCallFails(
        [Frozen] ICompleteRepository<Domain.Entities.ProjectGroup> mockProjectGroupRepository,
        GetProjectGroupByGroupReferenceNumberQueryHandler handler,
        GetProjectGroupByGroupReferenceNumberQuery command
        )
        {
            // Arrange
            mockProjectGroupRepository.GetAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup?, bool>>>())
                .Throws(new Exception());

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockProjectGroupRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Domain.Entities.ProjectGroup, bool>>>());
            Assert.True(result.IsSuccess == false);
        }
    }
}
