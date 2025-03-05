using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForLocalAuthorityQuery(
    string LocalAuthorityCode,
    ProjectState? State = ProjectState.Active,
    ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForLocalAuthority(
    IListAllProjectsForLocalAuthorityQueryService listAllProjectsForLocalAuthorityQueryService)
    : IRequestHandler<ListAllProjectsForLocalAuthorityQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(
        ListAllProjectsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var projectsForLa = await listAllProjectsForLocalAuthorityQueryService
                .ListAllProjectsForLocalAuthority(request.LocalAuthorityCode, request.State, request.Type)
                .ToListAsync(cancellationToken);

            var paginatedResultModel = projectsForLa.Select(proj =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                        proj.Project,
                        proj.Establishment))
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();

            return PaginatedResult<List<ListAllProjectsResultModel>>.Success(paginatedResultModel, projectsForLa.Count);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}