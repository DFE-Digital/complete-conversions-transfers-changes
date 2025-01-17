using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(ProjectState? ProjectStatus, ProjectType? Type, bool? IncludeFormAMat, int Page, int Count) : IRequest<Result<List<ListAllProjectsResultModel>>>;

    public class ListAllProjectsQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService) : IRequestHandler<ListAllProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request, CancellationToken cancellationToken)
        {
            // var cacheKey = $"Project_{CacheKeyHelper.GenerateHashedCacheKey(request.Urn.Value.ToString())}";
            try
            {
                var result = await listAllProjectsQueryService.ListAllProjects(request.ProjectStatus, request.Type, request.IncludeFormAMat)
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
        }
    }
}