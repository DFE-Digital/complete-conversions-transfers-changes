using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForTeamHandoverQuery(
    Region Region,
    ProjectState? ProjectStatus,
    ProjectType? Type,
    AssignedToState? ProjectAssignedToState = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForTeamHandoverQueryHandler(
    IListAllProjectsQueryService listAllProjectsQueryService,
    ILogger<ListAllProjectsForTeamHandoverQueryHandler> logger)
    : IRequestHandler<ListAllProjectsForTeamHandoverQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForTeamHandoverQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsHandedOverForTeamQuery = listAllProjectsQueryService.ListAllProjects(
                new ProjectFilters(request.ProjectStatus, request.Type, Region: request.Region, Team: ProjectTeam.RegionalCaseWorkerServices));

            var count = await projectsHandedOverForTeamQuery.CountAsync(cancellationToken);

            var paginatedResultModel = await projectsHandedOverForTeamQuery.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project,
                        proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel, count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForTeamHandoverQueryHandler), request);
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}