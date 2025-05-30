using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForTeamQuery(
    ProjectTeam Team,
    ProjectState? ProjectStatus,
    ProjectType? Type,
    AssignedToState? AssignedToState = null,
    OrderProjectQueryBy? OrderBy = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForTeamQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ILogger<ListAllProjectsForTeamQueryHandler> logger)
    : IRequestHandler<ListAllProjectsForTeamQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForTeamQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsForTeamQuery = listAllProjectsQueryService.ListAllProjects(
                new ProjectFilters(request.ProjectStatus, request.Type, request.AssignedToState, Team: request.Team), orderBy: request.OrderBy);

            var count = await projectsForTeamQuery.CountAsync(cancellationToken);

            var paginatedResultModel = await projectsForTeamQuery.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project, proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel, count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForTeamQueryHandler), request);
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}