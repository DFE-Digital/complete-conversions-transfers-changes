using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.CountAllProjects
{
    public record CountAllProjectsQuery(ProjectState? ProjectStatus, ProjectType? Type)
        : IRequest<Result<int>>
    {
        public override string ToString()
        {
            return $"{ProjectStatus.ToString()}{Type.ToString()}";
        }
    }

    public class CountAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<CountAllProjectsQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(request.ToString())}";
            var methodName = nameof(CountAllProjectsQueryHandler);
            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                try
                {
                    var result = await listAllProjectsQueryService
                        .ListAllProjects(request.ProjectStatus, request.Type)
                        .CountAsync(cancellationToken);
                    return Result<int>.Success(result);
                }
                catch (Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }, methodName);
        }
    }
}