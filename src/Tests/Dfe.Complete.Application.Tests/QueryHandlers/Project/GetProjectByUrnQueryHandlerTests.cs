using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Caching.Interfaces;
using Dfe.Complete.Application.Common.Models;
using DfE.CoreLibs.Caching.Helpers;
using Dfe.Complete.Application.Projects.Queries.GetProject;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectByUrnQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetAProjectByUrn_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] ICacheService<IMemoryCacheType> mockCacheService,
            GetProjectByUrnQueryHandler handler,
            GetProjectByUrnQuery command
            )
        {
            var now = DateTime.UtcNow;

            var urn = 123456;

            var project = Domain.Entities.Project.CreateConversionProject(new Domain.ValueObjects.Urn(urn),
                now,
                now,
                Domain.Enums.TaskType.Conversion,
                Domain.Enums.ProjectType.Conversion,
                Guid.NewGuid(),
                DateOnly.MinValue,
                true,
                new Domain.ValueObjects.Ukprn(2),
                Domain.Enums.Region.YorkshireAndTheHumber,
                true,
                true,
                DateOnly.MinValue,
                "",
                "",
                "",
                "",
                DateOnly.MinValue,
                false,
                "");

            var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(urn.ToString())}";

            // Arrange
            mockProjectRepository.GetAsync()
                .Returns(project);

            mockCacheService.GetOrAddAsync(
                cacheKey,
                Arg.Any<Func<Task<Result<Domain.Entities.Project?>>>>(),
                Arg.Any<string>())
            .Returns(callInfo =>
            {
                var callback = callInfo.ArgAt<Func<Task<Result<Domain.Entities.Project?>>>>(1);
                return callback();
            });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            //await mockProjectRepository.Received(1).GetAsync();
        }
    }
}
