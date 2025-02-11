using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjectsForLocalAuthority;

public record ListAllProjectsForLocalAuthorityQuery(
    string LocalAuthorityCode,
    ProjectState? State = ProjectState.Active,
    ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForLocalAuthority(
    IListAllProjectLocalAuthoritiesQueryService listAllProjectLocalAuthoritiesQueryService,
    IListAllProjectsQueryService listAllProjectsQueryService) :
    IRequestHandler<ListAllProjectsForLocalAuthorityQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(
        ListAllProjectsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        var allLocalAuthoritiesWithProjects = await listAllProjectLocalAuthoritiesQueryService
            .ListAllProjectLocalAuthorities(request.State, null)
            .ToListAsync(cancellationToken);

        var projectsForSpecificLa =
            allLocalAuthoritiesWithProjects
                .Where(la => la.LocalAuthority.Code == request.LocalAuthorityCode);

        var projectsWithEstablishments = await listAllProjectsQueryService.ListAllProjects(request.State, request.Type).ToListAsync(cancellationToken);

        var projectsForLaWithEstablishmentName = projectsForSpecificLa.Select(proj =>
            ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                proj.Project, 
                projectsWithEstablishments
                    .First(p => p.Project.Urn == proj.Project.Urn).Establishment))
            .ToList();
        
        var count = projectsForLaWithEstablishmentName.Count;

        var paginatedResult = projectsForLaWithEstablishmentName.Skip(request.Page * request.Count).Take(request.Count).ToList();

        return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResult, count);
    }
}