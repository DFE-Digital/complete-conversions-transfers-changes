using AutoFixture.Xunit2;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using System.Linq.Expressions;
using Dfe.Complete.Domain.ValueObjects;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using NSubstitute.ExceptionExtensions;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Projects.Queries.GetProject;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectGroupByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetAProjectGroupByIdd_WhenCommandIsValid(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupByIdRepository,
            [Frozen] IMapper mockMapper,
            GetProjectGroupByIdQueryHandler handler,
            GetProjectGroupByIdQuery command
        )
        {
            var now = DateTime.UtcNow;

            var projectGroup = new ProjectGroup
            {
                Id = new ProjectGroupId(command.Id!.Value),
                GroupIdentifier = "GRP_12345670",
                TrustUkprn = "10058689",
                CreatedAt = now,
                UpdatedAt = now
            };

            // Arrange
            mockProjectGroupByIdRepository.GetAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>())
                .Returns(projectGroup);

            mockMapper.Map<ProjectGroupDto>(projectGroup).Returns(new ProjectGroupDto()
            {
                Id = projectGroup.Id,
                GroupIdentifier = projectGroup.GroupIdentifier,
                TrustUkprn = projectGroup.TrustUkprn,
                CreatedAt = projectGroup.CreatedAt,
                UpdatedAt = projectGroup.UpdatedAt
            });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockProjectGroupByIdRepository.Received(1)
                .GetAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>());
            Assert.True(result.IsSuccess);
            Assert.True(result.Value?.Id == command.Id);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldSucceedAndReturnNullWhenUnfoundProjectGroupById_WhenCommandIsValid(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupByIdRepository,
            GetProjectGroupByIdQueryHandler handler,
            GetProjectGroupByIdQuery command
        )
        {
            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockProjectGroupByIdRepository.Received(1)
                .GetAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>());
            Assert.True(result.IsSuccess);
            Assert.True(result.Value == null);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnErrorMessage_WhenExceptionIsThrown(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupByIdRepository,
            GetProjectGroupByIdQueryHandler handler,
            GetProjectGroupByIdQuery command
        )
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            mockProjectGroupByIdRepository.GetAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>())
                .Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockProjectGroupByIdRepository.Received(1)
                .GetAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>());
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}
