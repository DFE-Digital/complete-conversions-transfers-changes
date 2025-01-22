using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Interfaces.Repositories;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetUkprnByGroupReferenceNumberQuery(string GroupReferenceNumber) : IRequest<Result<Ukprn>>;

    public class GetUkprnByGroupReferenceNumberQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<GetUkprnByGroupReferenceNumberQuery, Result<Ukprn>>
    {
        public async Task<Result<Ukprn>> Handle(GetUkprnByGroupReferenceNumberQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"GroupReferenceNumber_{CacheKeyHelper.GenerateHashedCacheKey(request.GroupReferenceNumber)}";

            var methodName = nameof(GetProjectByUrnQueryHandler);

            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                try
                {
                    var result = await projectGroupRepository.GetAsync(p => p.GroupIdentifier == request.GroupReferenceNumber);

                    return Result<Ukprn>.Success(result?.TrustUkprn);
                }
                catch (Exception ex)
                {
                    return Result<Ukprn>.Failure(ex.Message);
                }

            }, methodName);
        }
    }
}