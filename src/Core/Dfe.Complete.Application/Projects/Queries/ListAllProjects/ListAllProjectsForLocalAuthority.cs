using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForLocalAuthorityQuery(
    string LocalAuthorityCode,
    ProjectState? State = ProjectState.Active,
    ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

public class ListAllProjectsForLocalAuthority(IListAllProjectsQueryService listAllProjectsQueryService, ILogger<ListAllProjectsByRegionQueryHandler> logger)
    : IRequestHandler<ListAllProjectsForLocalAuthorityQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(
        ListAllProjectsForLocalAuthorityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orderBy = new OrderProjectQueryBy();
            var projectsForLa = await listAllProjectsQueryService.ListAllProjects(request.State, request.Type, localAuthorityCode: request.LocalAuthorityCode, orderBy: orderBy).ToListAsync(cancellationToken);

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
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForLocalAuthority), request);
            return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(e.Message);
        }
    }
}