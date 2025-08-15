using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Utils;

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
                var projectGroupsQuery = projectGroupRepository
                    .Query()
                    .OrderByDescending(pg => pg.GroupIdentifier);

                var totalCount = await projectGroupsQuery.CountAsync(cancellationToken);

                var pageOfProjectGroups = await projectGroupsQuery
                    .Paginate(request.Page, request.Count)
                    .ToListAsync(cancellationToken);
                

                var projectGroupIds = pageOfProjectGroups
                    .Select(p => p.Id)
                    .Distinct()
                    .ToList();

                var projectGroupUkprns = pageOfProjectGroups
                    .Select(p => p.TrustUkprn?.Value.ToString())
                    .Where(ukprn => !string.IsNullOrWhiteSpace(ukprn))
                    .Distinct()
                    .ToList();

                var projectsInGroups = await projectRepository
                    .Query()
                    .Where(p => p.GroupId != null && projectGroupIds.Contains(p.GroupId))
                    .ToListAsync(cancellationToken);

                var projectUrns = projectsInGroups
                    .Select(p => p.Urn.Value)
                    .Distinct();
                var establishments = await establishmentsClient.GetByUrns2Async(projectUrns, cancellationToken);

                var trusts = await trustsClient.GetByUkprnsAllAsync(projectGroupUkprns!, cancellationToken);

                var projectGroupsDto = pageOfProjectGroups.Select(pg =>
                {
                   // Determine group name: use trust name from API if TrustUkprn exists, otherwise use NewTrustName for Form a MAT projects
                   var groupName = string.Empty;
                   var trustUkprn = pg.TrustUkprn?.Value.ToString() ?? string.Empty;
                   
                   if (pg.TrustUkprn != null)
                   {
                       // Regular project group with trust UKPRN - get name from Academies API
                       groupName = trusts.FirstOrDefault(e => e.Ukprn! == pg.TrustUkprn)?.Name ?? string.Empty;
                   }
                   else
                   {
                       // Form a MAT project group - use NewTrustName from projects in this group
                       var projectsInThisGroup = projectsInGroups.Where(p => p.GroupId == pg.Id);
                       var formAMatProject = projectsInThisGroup
                           .Where(p => p.FormAMat)
                           .OrderByDescending(p => p.CreatedAt)
                           .FirstOrDefault();
                       
                       if (formAMatProject != null)
                       {
                           groupName = formAMatProject.NewTrustName ?? string.Empty;
                       }
                   }
                   
                   var groupIdentifier = pg.GroupIdentifier ?? string.Empty;
                   
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

                return PaginatedResult<List<ListProjectsGroupsModel>>.Success(projectGroupsDto, totalCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectGroupsQueryHandler), request);
                return PaginatedResult<List<ListProjectsGroupsModel>>.Failure(ex.Message);
            }
        }
    }
}
