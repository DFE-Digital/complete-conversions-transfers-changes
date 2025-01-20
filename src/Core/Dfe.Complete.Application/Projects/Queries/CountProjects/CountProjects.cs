using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.CountProjects
{
    public record CountProjectQuery(ProjectState? ProjectStatus, ProjectType? Type, bool? IncludeFormAMat)
        : IRequest<Result<int>>
    {
        public override string ToString()
        {
            return $"{ProjectStatus.ToString()}{Type.ToString()}{IncludeFormAMat.ToString()}";
        }
    }

    public class CountProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<CountProjectQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountProjectQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(request.ToString())}";
            var methodName = nameof(CountProjectsQueryHandler);
            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                try
                {
                    var result = await listAllProjectsQueryService
                        .ListAllProjects(request.ProjectStatus, request.Type, request.IncludeFormAMat)
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