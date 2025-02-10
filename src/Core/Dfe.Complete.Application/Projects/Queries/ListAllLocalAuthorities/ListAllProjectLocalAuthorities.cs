using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;

public record ListAllProjectLocalAuthoritiesQuery(ProjectState? ProjectStatus, ProjectType? Type)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectLocalAuthoritiesQuery>>>;

public class ListAllProjectLocalAuthorities(IListAllProjectLocalAuthoritiesQueryService queryService)
    : IRequestHandler<ListAllProjectLocalAuthoritiesQuery, PaginatedResult<List<ListAllProjectLocalAuthoritiesQuery>>>
{
    public async Task<PaginatedResult<List<ListAllProjectLocalAuthoritiesQuery>>> Handle(
        ListAllProjectLocalAuthoritiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await queryService.ListAllProjectLocalAuthorities(request.ProjectStatus, request.Type)
                .ToListAsync(cancellationToken);

            var groupedProjectByLa = result.GroupBy(g => g.LocalAuthority);

            var resultModel = groupedProjectByLa
                .Select(group =>
                    new ListAllProjectLocalAuthoritiesResultModel(
                        group.Key,
                        group.Key.Code,
                        Conversions: group.Count(item => item.Project?.Type == ProjectType.Conversion),
                        Transfers: group.Count(item => item.Project?.Type == ProjectType.Transfer)))
                .ToList();

            return PaginatedResult<List<ListAllProjectLocalAuthoritiesQuery>>.Success(resultModel);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectLocalAuthoritiesQuery>>.Failure(e.Message);
        }
    }
}