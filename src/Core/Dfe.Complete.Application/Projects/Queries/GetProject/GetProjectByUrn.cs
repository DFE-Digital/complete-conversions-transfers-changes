using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectByUrnQuery(Urn Urn) : IRequest<Result<Project?>>;

    public class GetProjectByUrnQueryHandler(ICompleteRepository<Project> projectRepository,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<GetProjectByUrnQuery, Result<Project?>>
    {
        public async Task<Result<Project?>> Handle(GetProjectByUrnQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(request.Urn.Value.ToString())}";

            var methodName = nameof(GetProjectByUrnQueryHandler);

            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                try
                {
                    var result = await projectRepository.GetAsync(p => p.Urn == request.Urn);

                    return Result<Project?>.Success(result);
                }
                catch (Exception ex)
                {
                    return Result<Project?>.Failure(ex.Message);
                }

            }, methodName);
        }
    }
}