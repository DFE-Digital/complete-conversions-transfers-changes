using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForRegionQuery(
    Region Region,
    ProjectState? ProjectStatus,
    ProjectType? Type,
    bool ExcludeUnassigned = false)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForRegionQueryHandler(
    IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService)
    : IRequestHandler<ListAllProjectsForRegionQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForRegionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsForRegion = await listAllProjectsByFilterQueryService.ListAllProjectsByFilter(
                request.ProjectStatus, request.Type, request.ExcludeUnassigned, region: request.Region)
                .ToListAsync(cancellationToken);

            var paginatedResultModel = projectsForRegion.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project,
                        proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel,
                projectsForRegion.Count);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}