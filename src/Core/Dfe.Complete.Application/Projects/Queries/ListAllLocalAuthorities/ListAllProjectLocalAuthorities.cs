using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;

public record ListAllProjectLocalAuthoritiesQuery(ProjectState? State = ProjectState.Active, ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>>;

public class ListAllProjectLocalAuthorities(IListAllProjectLocalAuthoritiesQueryService queryService)
    : IRequestHandler<ListAllProjectLocalAuthoritiesQuery, PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>> Handle(
        ListAllProjectLocalAuthoritiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await queryService.ListAllProjectLocalAuthorities(request.State, null)
                .ToListAsync(cancellationToken);

            var count = result.Count;

            var groupedProjectByLa = result.GroupBy(g => g.LocalAuthority)
                .Skip(request.Page * request.Count)
                .Take(request.Count);

            var resultModel = groupedProjectByLa
                .Select(group =>
                    new ListAllProjectLocalAuthoritiesResultModel(
                        group.Key,
                        group.Key.Code,
                        Conversions: group.Count(item => item.Project.Type == ProjectType.Conversion),
                        Transfers: group.Count(item => item.Project.Type == ProjectType.Transfer)))
                .ToList();

            return PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>.Success(resultModel, count);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>.Failure(e.Message);
        }
    }
}