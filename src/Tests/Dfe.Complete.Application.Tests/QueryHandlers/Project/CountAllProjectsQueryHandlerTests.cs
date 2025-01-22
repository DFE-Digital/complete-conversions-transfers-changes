using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Caching.Interfaces;
using Dfe.Complete.Application.Common.Models;
using DfE.CoreLibs.Caching.Helpers;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class CountAllProjectsQueryHandlerTests
    {
        
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnCorrectCount(
            [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
            [Frozen] ICacheService<IMemoryCacheType> mockCacheService,
            CountAllProjectsQueryHandler handler,
            CountAllProjectsQuery query,
            List<ListAllProjectsQueryModel> listAllProjectsQueryModels)
        {
            // Arrange
            var expected = listAllProjectsQueryModels.Count;

            var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(query.ToString())}";

            var mock = listAllProjectsQueryModels.BuildMock();

            mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
                .Returns(mock);

            mockCacheService.GetOrAddAsync(cacheKey, Arg.Any<Func<Task<Result<int>>>>(), Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var callback = callInfo.ArgAt<Func<Task<Result<int>>>>(1);
                    return callback();
                });

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.Equal(expected, result.Value);

            await mockCacheService.Received(1).GetOrAddAsync(cacheKey, Arg.Any<Func<Task<Result<int>>>>(), nameof(CountAllProjectsQueryHandler));
        }
    }
}
