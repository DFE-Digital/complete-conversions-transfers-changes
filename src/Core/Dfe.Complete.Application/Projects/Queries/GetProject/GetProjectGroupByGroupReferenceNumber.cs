using MediatR;
using Dfe.Complete.Domain.Interfaces.Repositories;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using AutoMapper;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectGroupByGroupReferenceNumberQuery(string GroupReferenceNumber) : IRequest<Result<ProjectGroupDto>>;

    public class GetProjectGroupByGroupReferenceNumberQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        IMapper mapper,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<GetProjectGroupByGroupReferenceNumberQuery, Result<ProjectGroupDto>>
    {
        public async Task<Result<ProjectGroupDto?>> Handle(GetProjectGroupByGroupReferenceNumberQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"GroupReferenceNumber_{CacheKeyHelper.GenerateHashedCacheKey(request.GroupReferenceNumber)}";

            var methodName = nameof(GetProjectByUrnQueryHandler);

            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                try
                {
                    var result = await projectGroupRepository.GetAsync(p => p.GroupIdentifier == request.GroupReferenceNumber);

                    var projectGroupDto = mapper.Map<ProjectGroupDto?>(result);

                    return Result<ProjectGroupDto?>.Success(projectGroupDto);
                }
                catch (Exception ex)
                {
                    return Result<ProjectGroupDto?>.Failure(ex.Message);
                }

            }, methodName);
        }
    }
}