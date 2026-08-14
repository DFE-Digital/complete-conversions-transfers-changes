using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListProjectsByFilterQuery(
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
            var listProjectsByFilterResultsModel = listAllProjectsByFilterQueryService.ListAllProjectsByFilter();

            // TODO I have moved pagination from infra to application, leaving the select there. I think I did select AFTER pagination, something to do with the Concat
            var projects = await listProjectsByFilterResultsModel
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
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