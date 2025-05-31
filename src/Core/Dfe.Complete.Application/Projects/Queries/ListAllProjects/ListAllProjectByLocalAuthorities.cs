using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsByLocalAuthoritiesQuery(ProjectState? State = ProjectState.Active, ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>>;

public class ListAllProjectByLocalAuthorities(ICompleteRepository<LocalAuthority> localAuthoritiesRepo, IListAllProjectsQueryService listAllProjectsQueryService, ILogger<ListAllProjectByLocalAuthorities> logger)
    : IRequestHandler<ListAllProjectsByLocalAuthoritiesQuery, PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>> Handle(
        ListAllProjectsByLocalAuthoritiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var localAuthorities = await localAuthoritiesRepo.FetchAsync(la => !string.IsNullOrEmpty(la.Code), cancellationToken);

            var projectsWithEstablishments = await listAllProjectsQueryService.ListAllProjects(new ProjectFilters(request.State, request.Type)).ToListAsync(cancellationToken);

            var localAuthoritiesWithProjectsDict = localAuthorities.OrderBy(la => la.Name).ToDictionary(
                localAuthority => localAuthority,
                localAuthority => projectsWithEstablishments
                    .Where(p => p.Establishment.LocalAuthorityCode == localAuthority.Code).ToList());

            var filteredLocalAuthoritiesWithProjects = localAuthoritiesWithProjectsDict
                .Where(entry => entry.Value.Count > 0);

            var paginatedLocalAuthoritiesWithProjects = filteredLocalAuthoritiesWithProjects
                .Skip(request.Page * request.Count)
                .Take(request.Count);

            var resultModel = paginatedLocalAuthoritiesWithProjects.Select(item =>
                new ListAllProjectLocalAuthoritiesResultModel(
                    item.Key,
                    item.Key.Code,
                    item.Value.Count(p => p.Project.Type == ProjectType.Conversion),
                    item.Value.Count(p => p.Project.Type == ProjectType.Transfer)))
                .ToList();

            return PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>.Success(resultModel, filteredLocalAuthoritiesWithProjects.Count());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectByLocalAuthorities), request);
            return PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>.Failure(e.Message);
        }
    }
}