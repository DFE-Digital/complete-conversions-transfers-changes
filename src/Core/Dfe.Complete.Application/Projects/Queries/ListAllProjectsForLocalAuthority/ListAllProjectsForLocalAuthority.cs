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
    IListAllProjectsQueryService listAllProjectsQueryService) :
    IRequestHandler<ListAllProjectsForLocalAuthorityQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(
        ListAllProjectsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var projectsWithEstablishments = await listAllProjectsQueryService.ListAllProjects(request.State, request.Type)
                .ToListAsync(cancellationToken);

            var projectsForLa =
                projectsWithEstablishments.Where(p => p.Establishment.LocalAuthorityCode == request.LocalAuthorityCode);

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
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}