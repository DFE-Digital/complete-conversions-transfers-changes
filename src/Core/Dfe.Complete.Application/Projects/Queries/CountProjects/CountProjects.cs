using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.CountProjects
{
    public record CountProjectQuery(ProjectState? ProjectStatus, ProjectType? Type, bool? IncludeFormAMat) : IRequest<Result<int>>;

    public class CountProjectsQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService) : IRequestHandler<CountProjectQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountProjectQuery request, CancellationToken cancellationToken)
        {
            // var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(request.Urn.Value.ToString())}";
            try
            {
                var result = await listAllProjectsQueryService.ListAllProjects(request.ProjectStatus, request.Type, request.IncludeFormAMat)
                    .CountAsync(cancellationToken);
                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }
    }
}