using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dfe.AcademiesApi.Client.Contracts;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectGroupsQuery() : PaginatedRequest<PaginatedResult<List<ListProjectsGroupsModel>>>;

    public class GetProjectGroupsQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        ICompleteRepository<Project> projectRepository,
        ITrustsV4Client trustsClient,
        ILogger<GetProjectGroupsQueryHandler> logger)
        : IRequestHandler<GetProjectGroupsQuery, PaginatedResult<List<ListProjectsGroupsModel>>>
    {
        public async Task<PaginatedResult<List<ListProjectsGroupsModel>>> Handle(GetProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectGroups = await projectGroupRepository.Query()
                    .OrderBy(pg => pg.GroupIdentifier)
                    .ToListAsync(cancellationToken);
                
                var projectGroupsDto = new List<ListProjectsGroupsModel>();
                
                foreach (var group in projectGroups)
                {
                    // Get all projects for this group
                    var projectsInGroup = await projectRepository.Query()
                        .Where(p => p.GroupId == group.Id)
                        .Include(p => p.GiasEstablishment)
                        .ToListAsync(cancellationToken);
                    
                    // Skip groups that have no projects (no schools/academies)
                    if (!projectsInGroup.Any())
                    {
                        continue;
                    }
                    
                    // Get establishment names from the projects in this group
                    var establishmentNames = projectsInGroup
                        .Where(p => p.GiasEstablishment != null)
                        .Select(p => p.GiasEstablishment!.Name)
                        .Distinct()
                        .ToList();
                    
                    // Get trust name from external API if TrustUkprn is available
                    string groupName;
                    if (group.TrustUkprn != null)
                    {
                        var establishments = await trustsClient.GetByUkprnsAllAsync([group.TrustUkprn.Value.ToString()], cancellationToken);
                        groupName = establishments.FirstOrDefault()?.Name ?? group.GroupIdentifier ?? "Unknown Group";
                    }
                    else
                    {
                        groupName = group.GroupIdentifier ?? "Unknown Group";
                    }
                    
                    projectGroupsDto.Add(new ListProjectsGroupsModel(
                        group.Id.Value.ToString(),
                        groupName,
                        group.GroupIdentifier ?? string.Empty,
                        group.TrustUkprn?.Value ?? 0,
                        string.Join("; ", establishmentNames)));
                }
                
                return PaginatedResult<List<ListProjectsGroupsModel>>.Success(projectGroupsDto, projectGroupsDto.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectGroupsQueryHandler), request);
                return PaginatedResult<List<ListProjectsGroupsModel>>.Failure(ex.Message);
            }
        }
    }
}
