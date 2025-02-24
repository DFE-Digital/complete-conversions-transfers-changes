using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForRegionQuery(
    Region? Region,
    ProjectState? ProjectStatus,
    ProjectType? Type)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForRegionQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService)
    : IRequestHandler<ListAllProjectsForRegionQuery, PaginatedResult<List<ListAllProjectsResultModel>>>

{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForRegionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsList = await listAllProjectsQueryService
                .ListAllProjects(request.ProjectStatus, request.Type)
                .ToListAsync(cancellationToken: cancellationToken);

            // Materialise the filtered list to avoid enumerating multiple times.
            var filteredProjects = projectsList
                .Where(project => project.Project.Region == request.Region)
                .ToList();

            var totalCount = filteredProjects.Count;

            var paginatedProjects = filteredProjects
                .Skip(request.Page * request.Count)
                .Take(request.Count);

            var projectsResultModel = paginatedProjects
                .Select(item => new ListAllProjectsResultModel(
                    item.Establishment.Name,
                    item.Project.Id,
                    item.Project.Urn,
                    item.Project.SignificantDate,
                    item.Project.State,
                    item.Project.Type,
                    item.Project.IncomingTrustUkprn == null,
                    item.Project.AssignedTo != null
                        ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                        : null))
                .ToList();

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(projectsResultModel,
                totalCount);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}