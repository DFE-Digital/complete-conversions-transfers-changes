using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Services;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListProjectsByFilterQuery(
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<Result<List<ListProjectsByFilterResultsModel>>>;

internal class ListProjectsByFilterQueryHandler(
    IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService
) : IRequestHandler<ListProjectsByFilterQuery, Result<List<ListProjectsByFilterResultsModel>>>
{
    public async Task<Result<List<ListProjectsByFilterResultsModel>>> Handle(ListProjectsByFilterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var listProjectsByFilterResultsModel = await listAllProjectsByFilterQueryService.ListAllProjectsByFilterAsync(request, cancellationToken);
            return Result<List<ListProjectsByFilterResultsModel>>.Success(listProjectsByFilterResultsModel);
        }
        catch (Exception e)
        {
            return Result<List<ListProjectsByFilterResultsModel>>.Failure(e.Message);
        }
    }
}