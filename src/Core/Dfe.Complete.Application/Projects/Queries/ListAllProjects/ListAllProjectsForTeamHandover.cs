using System.Net.NetworkInformation;
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
    ProjectType? Type)
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
            var projectsHandedOverForTeam = await listAllProjectsQueryService.ListAllProjects(
                request.ProjectStatus, request.Type, region: request.Region, team: ProjectTeam.RegionalCaseWorkerServices)
                .ToListAsync(cancellationToken);

            var paginatedResultModel = projectsHandedOverForTeam.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project,
                        proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel,
                projectsHandedOverForTeam.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForTeamHandoverQueryHandler), request);
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}