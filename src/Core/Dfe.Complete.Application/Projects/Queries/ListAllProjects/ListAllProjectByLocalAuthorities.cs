using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsByLocalAuthoritiesQuery(ProjectState? State = ProjectState.Active, ProjectType? Type = null)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>>;

public class ListAllProjectByLocalAuthorities(ICompleteRepository<LocalAuthority> localAuthoritiesRepo, IListAllProjectsWithLAsQueryService ListAllProjectsWithLAsQueryService, ILogger<ListAllProjectByLocalAuthorities> logger)
    : IRequestHandler<ListAllProjectsByLocalAuthoritiesQuery, PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>> Handle(
    ListAllProjectsByLocalAuthoritiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var grouped = ListAllProjectsWithLAsQueryService.ListAllProjects(request.State, request.Type)
                .GroupBy(p => p.LocalAuthority.Code)
                .Select(g => new
                {
                    LocalAuthorityCode = g.Key,
                    LocalAuthorityName = g.First().LocalAuthority.Name,
                    ConversionCount = g.Count(p => p.Type == ProjectType.Conversion),
                    TransferCount = g.Count(p => p.Type == ProjectType.Transfer)
                });

            var count = await grouped.CountAsync(cancellationToken);

            var paginated = await grouped
                .OrderBy(p => p.LocalAuthorityName)
                .Paginate(request.Page, request.Count)
                .ToListAsync(cancellationToken);

            var resultModel = paginated.Select(item => new ListAllProjectLocalAuthoritiesResultModel(
                item.LocalAuthorityName,
                item.LocalAuthorityCode,
                item.ConversionCount,
                item.TransferCount)).ToList();

            return PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>.Success(resultModel, count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectByLocalAuthorities), request);
            return PaginatedResult<List<ListAllProjectLocalAuthoritiesResultModel>>.Failure(e.Message);
        }
    }
}