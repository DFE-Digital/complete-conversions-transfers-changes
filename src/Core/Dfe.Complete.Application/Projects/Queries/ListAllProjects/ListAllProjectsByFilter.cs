using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListProjectsByFilterQuery(
    ProjectState[]? ProjectStatuses = null,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<PaginatedResult<List<ListProjectsByFilterResultsModel>>>;

internal class ListProjectsByFilterQueryHandler(
    IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService
) : IRequestHandler<ListProjectsByFilterQuery, PaginatedResult<List<ListProjectsByFilterResultsModel>>>
{
    public async Task<PaginatedResult<List<ListProjectsByFilterResultsModel>>> Handle(ListProjectsByFilterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var listProjectsByFilterResultsModel = listAllProjectsByFilterQueryService.ListAllProjectsByFilter(request.ProjectStatuses);

            var projects = await listProjectsByFilterResultsModel
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync(cancellationToken);

            var projectCount = await listProjectsByFilterResultsModel.CountAsync(cancellationToken);

            return PaginatedResult<List<ListProjectsByFilterResultsModel>>.Success(projects, projectCount);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListProjectsByFilterResultsModel>>.Failure(e.Message);
        }
    }
}