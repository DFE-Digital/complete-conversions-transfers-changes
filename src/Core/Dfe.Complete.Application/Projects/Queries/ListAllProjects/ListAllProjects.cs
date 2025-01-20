using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        bool? IncludeFormAMat,
        int Page,
        int Count) : IRequest<Result<List<ListAllProjectsResultModel>>>
    {
        public override string ToString()
        {
            return $"{ProjectStatus.ToString()}{Type.ToString()}{IncludeFormAMat.ToString()}{Page}{Count}";
        }
    }

    public class ListAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<ListAllProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"ListAllProjects_{CacheKeyHelper.GenerateHashedCacheKey(request.ToString())}";
            var methodName = nameof(ListAllProjectsQueryHandler);
            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                try
                {
                    var result = await listAllProjectsQueryService
                        .ListAllProjects(request.ProjectStatus, request.Type, request.IncludeFormAMat)
                        .Skip(request.Page * request.Count).Take(request.Count)
                        .Select(item => new ListAllProjectsResultModel(
                            item.Establishment.Name,
                            item.Project.Id,
                            item.Project.Urn,
                            item.Project.SignificantDate,
                            item.Project.State,
                            item.Project.Type,
                            item.Project.IncomingTrustUkprn == null,
                            item.Project.AssignedTo
                        ))
                        .ToListAsync(cancellationToken);
                    return Result<List<ListAllProjectsResultModel>>.Success(result);
                }
                catch (Exception ex)
                {
                    return Result<List<ListAllProjectsResultModel>>.Failure(ex.Message);
                }
            }, methodName);
        }
    }
}