using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsByRegionQuery(
    ProjectState? ProjectStatus,
    ProjectType? Type)
    : IRequest<Result<List<ListAllProjectsByRegionsResultModel>>>;

public class ListAllProjectsByRegionQueryHandler(IProjectsQueryBuilder projectsQueryBuilder, ILogger<ListAllProjectsByRegionQueryHandler> logger)
    : IRequestHandler<ListAllProjectsByRegionQuery, Result<List<ListAllProjectsByRegionsResultModel>>>

{
    public async Task<Result<List<ListAllProjectsByRegionsResultModel>>> Handle(ListAllProjectsByRegionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var filters = new ProjectFilters(request.ProjectStatus, request.Type);

            var projectsQuery = projectsQueryBuilder
                .ApplyProjectFilters(filters)
                .Where(p => p.Region != null)
                .GetProjects();

            var projectsList = await projectsQuery
                .GroupBy(p => p.Region)
                .Select(group =>
                    new ListAllProjectsByRegionsResultModel(
                        group.Key.GetValueOrDefault(),
                        group.Count(item => item != null && item.Type == ProjectType.Conversion),
                        group.Count(item => item != null && item.Type == ProjectType.Transfer)
                    ))
                .ToListAsync(cancellationToken);

            var orderedRegions = projectsList.OrderBy(region => region.Region.ToString()).ToList();

            return Result<List<ListAllProjectsByRegionsResultModel>>.Success(orderedRegions);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsByRegionQueryHandler), request);
            return Result<List<ListAllProjectsByRegionsResultModel>>.Failure(e.Message);
        }
    }
}