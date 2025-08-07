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
        IEstablishmentsV4Client establishmentsClient,
        ILogger<GetProjectGroupsQueryHandler> logger)
        : IRequestHandler<GetProjectGroupsQuery, PaginatedResult<List<ListProjectsGroupsModel>>>
    {
        public async Task<PaginatedResult<List<ListProjectsGroupsModel>>> Handle(GetProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectGroups = await projectGroupRepository
                    .Query()
                    .OrderByDescending(pg => pg.GroupIdentifier)
                    .ToListAsync(cancellationToken);
                
                var projectGroupIds = projectGroups.Select(p => p.Id).Distinct();
                var projectGroupUkprns = projectGroups.Select(p => p.TrustUkprn.Value.ToString()).Distinct();
                
                var projectsInGroups = await projectRepository
                    .Query()
                    .Where(p => projectGroupIds.Contains(p.GroupId))
                    .ToListAsync(cancellationToken);
                
                var projectUrns = projectsInGroups.Select(p => p.Urn.Value).Distinct();
                var establishments = await establishmentsClient.GetByUrns2Async(projectUrns, cancellationToken);
                
                var trusts = await trustsClient.GetByUkprnsAllAsync(projectGroupUkprns, cancellationToken);

                var projectGroupsDto = projectGroups.Select(pg =>
                {
                   var groupName = trusts.FirstOrDefault(e => e.Ukprn == pg.TrustUkprn)?.Name;
                   
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
                       pg.GroupIdentifier,
                       pg.TrustUkprn.Value.ToString(),
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
