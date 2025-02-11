using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjectsForLocalAuthority;

public record ListAllProjectsForLocalAuthorityQuery(
    string LocalAuthorityCode,
    ProjectState? State = ProjectState.Active,
    ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForLocalAuthority(
    IListAllProjectLocalAuthoritiesQueryService listAllProjectLocalAuthoritiesQueryService) :
    IRequestHandler<ListAllProjectsForLocalAuthorityQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(
        ListAllProjectsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        var allLocalAuthoritiesWithProjects = await listAllProjectLocalAuthoritiesQueryService
            .ListAllProjectLocalAuthorities(request.State, null)
            .ToListAsync(cancellationToken);

        var count = allLocalAuthoritiesWithProjects.Count;

        var projectsForSpecificLa =
            allLocalAuthoritiesWithProjects
                .Where(la => la.LocalAuthority.Code == request.LocalAuthorityCode)
                .Skip(request.Page * request.Count)
                .Take(request.Count);
        
        var result = projectsForSpecificLa.Select(proj => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(proj.Project))
    }
}