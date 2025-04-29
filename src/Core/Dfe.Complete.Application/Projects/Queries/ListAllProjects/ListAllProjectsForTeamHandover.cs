using System.Net.NetworkInformation;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForTeamHandoverQuery(
    Region Region,
    ProjectState? ProjectStatus,
    ProjectType? Type)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForTeamHandoverQueryHandler(
    IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService)
    : IRequestHandler<ListAllProjectsForTeamHandoverQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForTeamHandoverQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsHandedOverForTeam = await listAllProjectsByFilterQueryService.ListAllProjectsByFilter(
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
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}