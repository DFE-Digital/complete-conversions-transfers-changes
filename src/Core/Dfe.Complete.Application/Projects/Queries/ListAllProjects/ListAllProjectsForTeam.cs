using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForTeamQuery(
    ProjectTeam Team,
    ProjectState? ProjectStatus,
    ProjectType? Type)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForTeamQueryHandler(IListAllProjectsForTeamQueryService listAllProjectsForTeamQueryServiceQueryService)
    : IRequestHandler<ListAllProjectsForTeamQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForTeamQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsForTeam = await listAllProjectsForTeamQueryServiceQueryService.ListAllProjectsForTeam(request.Team,
                request.ProjectStatus, request.Type).ToListAsync(cancellationToken);

            var paginatedResultModel = projectsForTeam.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project, proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();
            
            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel, projectsForTeam.Count);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}