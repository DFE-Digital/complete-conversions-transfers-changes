using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Application.SignificantChange.Services;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.SignificantChange.Queries;

public record ListSignificantChangeProjectsByFilterQuery(
    ProjectState[]? ProjectStatuses = null,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<PaginatedResult<List<ListSignificantChangeProjectsByFilterResultsModel>>>;

internal class ListSignificantChangeProjectsByFilterQueryHandler(
    IListSignificantChangeProjectsByFilterQueryService listSignificantChangeProjectsByFilterQueryService
) : IRequestHandler<ListSignificantChangeProjectsByFilterQuery, PaginatedResult<List<ListSignificantChangeProjectsByFilterResultsModel>>>
{
    public async Task<PaginatedResult<List<ListSignificantChangeProjectsByFilterResultsModel>>> Handle(ListSignificantChangeProjectsByFilterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var projects = listSignificantChangeProjectsByFilterQueryService.ListSignificantChangeProjectsByFilter(request.ProjectStatuses);

            var projectsResult = await projects
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync(cancellationToken);

            var projectCount = await projects.CountAsync(cancellationToken);

            return PaginatedResult<List<ListSignificantChangeProjectsByFilterResultsModel>>.Success(projectsResult, projectCount);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListSignificantChangeProjectsByFilterResultsModel>>.Failure(e.Message);
        }
    }
}