using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjectsByRegion;
public record ListAllProjectsByRegionQuery(ProjectState? ProjectStatus, ProjectType? Type) : IRequest<Result<List<ListAllProjectsByRegionResultModel>>>;

public class ListAllProjectsByRegionQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService)
    : IRequestHandler<ListAllProjectsByRegionQuery, Result<List<ListAllProjectsByRegionResultModel>>>

{
    public async Task<Result<List<ListAllProjectsByRegionResultModel>>> Handle(ListAllProjectsByRegionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var projectsList = await listAllProjectsQueryService
                .ListAllProjects(request.ProjectStatus, request.Type)
                .ToListAsync(cancellationToken);

            var projectsGroupedByRegion = projectsList
                .GroupBy(p => p.Project.Region);

            var projectsResultModel = projectsGroupedByRegion
                .Select(group => new ListAllProjectsByRegionResultModel(
                    Region: (Region)group.Key,
                    ConversionsCount: group.Count(item => item.Project?.Type == ProjectType.Conversion),
                    TransfersCount: group.Count(item => item.Project?.Type == ProjectType.Transfer),
                    Projects: group
                        .Select(item => new Model.ListAllProjectsResultModel(
                            item.Establishment.Name,
                            item.Project.Id,
                            item.Project.Urn,
                            item.Project.SignificantDate,
                            item.Project.State,
                            item.Project.Type,
                            item.Project.IncomingTrustUkprn is null,
                            item.Project.AssignedTo is not null
                                ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                                : null
                        ))
                )).ToList();
            
            return Result<List<ListAllProjectsByRegionResultModel>>.Success(projectsResultModel);
        }
        catch (Exception e)
        {
            return Result<List<ListAllProjectsByRegionResultModel>>.Failure(e.Message);

        }
    }
}