using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;

public record ListAllProjectsForRegionQuery(
    ProjectState? ProjectStatus,
    ProjectType? Type)
    : IRequest<Result<List<ListAllProjectsByRegionsResultModel>>>;

public class ListAllProjectsForRegionQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService)
    : IRequestHandler<ListAllProjectsByRegionQuery, Result<List<ListAllProjectsByRegionsResultModel>>>

{
    public async Task<Result<List<ListAllProjectsByRegionsResultModel>>> Handle(ListAllProjectsByRegionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectsList = await listAllProjectsQueryService
                .ListAllProjects(request.ProjectStatus, request.Type).ToListAsync(cancellationToken: cancellationToken);

            var projectsGroupedByRegion = projectsList.GroupBy(p => p.Project.Region);

            var projectsResultModel = projectsGroupedByRegion
                .Select(group =>
                    new ListAllProjectsByRegionsResultModel(
                        Region: (Region)group.Key,
                        ConversionsCount: group.Count(item => item.Project?.Type == ProjectType.Conversion),
                        TransfersCount: group.Count(item => item.Project?.Type == ProjectType.Transfer)
                    )).ToList();

            return Result<List<ListAllProjectsByRegionsResultModel>>.Success(projectsResultModel);
        }
        catch (Exception e)
        {
            return Result<List<ListAllProjectsByRegionsResultModel>>.Failure(e.Message);
        }
    }
}