using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForRegionQuery(
    Region Region,
    ProjectState? ProjectStatus,
    ProjectType? Type,
    AssignedToState? AssignedToState = null,
    OrderProjectQueryBy? OrderBy = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForRegionQueryHandler(
    IListAllProjectsQueryService listAllProjectsQueryService,
    ILogger<ListAllProjectsForRegionQueryHandler> logger)
    : IRequestHandler<ListAllProjectsForRegionQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForRegionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsForRegionQuery = listAllProjectsQueryService.ListAllProjects(
                request.ProjectStatus, request.Type, request.AssignedToState, region: request.Region, orderBy: request.OrderBy);

            var count = await projectsForRegionQuery.CountAsync(cancellationToken);

            var paginatedResultModel = await projectsForRegionQuery.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project,
                        proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel, count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForRegionQueryHandler), request);
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}