using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;

public record ListAllProjectsForRegionQuery(
    Region? Region,
    ProjectState? ProjectStatus,
    ProjectType? Type,
    int Page = 0,
    int Count = 20)
    : IRequest<Result<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForRegionQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService)
    : IRequestHandler<ListAllProjectsForRegionQuery, Result<List<ListAllProjectsResultModel>>>

{
    public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsForRegionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsList = await listAllProjectsQueryService
                .ListAllProjects(request.ProjectStatus, request.Type)
                .ToListAsync(cancellationToken: cancellationToken);

            var projectsGroupedByRegion = projectsList
                .Where(projectList => projectList.Project.Region == request.Region)
                .Skip(request.Page * request.Count).Take(request.Count);

            var projectsResultModel = projectsGroupedByRegion
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

            return Result<List<ListAllProjectsResultModel>>.Success(projectsResultModel);
        }
        catch (Exception e)
        {
            return Result<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}