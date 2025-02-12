using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjectsForLocalAuthority;

public record ListAllProjectsForLocalAuthorityQuery(
    string LocalAuthorityCode,
    ProjectState? State = ProjectState.Active,
    ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForLocalAuthority(
    IListAllProjectsQueryService listAllProjectsQueryService,
    ICompleteRepository<LocalAuthority> localAuthorityRepo) :
    IRequestHandler<ListAllProjectsForLocalAuthorityQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(
        ListAllProjectsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        var localAuthorities = await localAuthorityRepo.FetchAsync(la => !string.IsNullOrEmpty(la.Code) && la.Code == request.LocalAuthorityCode, cancellationToken);
        var specificLocalAuthority = localAuthorities.SingleOrDefault();
        
        var projectsWithEstablishments = await listAllProjectsQueryService.ListAllProjects(request.State, request.Type).ToListAsync(cancellationToken);
        
        var projectsForLa = projectsWithEstablishments.Where(p => p.Establishment.LocalAuthorityCode == specificLocalAuthority.Code);
        
        var projectsForLaWithEstablishments = projectsForLa.Select(proj =>
                ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    proj.Project,
                    proj.Establishment))
            .ToList();

        var count = projectsForLaWithEstablishments.Count;

        var paginatedResult = projectsForLaWithEstablishments.Skip(request.Page * request.Count).Take(request.Count)
            .ToList();

        return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResult, count);
    }
}