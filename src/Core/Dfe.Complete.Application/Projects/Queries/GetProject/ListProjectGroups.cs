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
    public record ListProjectGroupsQuery() : PaginatedRequest<PaginatedResult<List<ListProjectsGroupsModel>>>;

    public class GetProjectGroupsQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        ICompleteRepository<Project> projectRepository,
        ITrustsV4Client trustsClient,
        IEstablishmentsV4Client establishmentsClient,
        ILogger<GetProjectGroupsQueryHandler> logger)
        : IRequestHandler<ListProjectGroupsQuery, PaginatedResult<List<ListProjectsGroupsModel>>>
    {
        public async Task<PaginatedResult<List<ListProjectsGroupsModel>>> Handle(ListProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectGroups = await projectGroupRepository
                    .Query()
                    .OrderByDescending(pg => pg.GroupIdentifier)
                    .ToListAsync(cancellationToken);
                
                var projectGroupIds = projectGroups.Select(p => p.Id).Distinct();
                var projectGroupUkprns = projectGroups.Select(p => p.TrustUkprn?.Value.ToString()).Distinct();
                
                var projectsInGroups = await projectRepository
                    .Query()
                    .Where(p => projectGroupIds.Contains(p.GroupId))
                    .ToListAsync(cancellationToken);
                
                var projectUrns = projectsInGroups.Select(p => p.Urn.Value).Distinct();
                var establishments = await establishmentsClient.GetByUrns2Async(projectUrns, cancellationToken);
                
                var trusts = await trustsClient.GetByUkprnsAllAsync(projectGroupUkprns!, cancellationToken);

                var projectGroupsDto = projectGroups.Select(pg =>
                {
                   var groupName = trusts.FirstOrDefault(e => e.Ukprn! == pg.TrustUkprn)?.Name ?? string.Empty;
                   var groupIdentifier = pg.GroupIdentifier ?? string.Empty;
                   var trustUkprn = pg.TrustUkprn?.Value.ToString() ?? string.Empty;
                   
                   var urnsForThisGroup = projectsInGroups
                       .Where(p => p.GroupId == pg.Id)
                       .Select(p => p.Urn.Value.ToString());
                   
                   var establishmentInThisGroup = establishments
                       .Where(e => urnsForThisGroup.Contains(e.Urn))
                       .OrderBy(e => e.Name)
                       .Select(e => e.Name)
                       .Distinct();

                   return new ListProjectsGroupsModel(
                       pg.Id.Value.ToString(),
                       groupName,
                       groupIdentifier,
                       trustUkprn,
                       string.Join("; ", establishmentInThisGroup));
                }).ToList();
                
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
