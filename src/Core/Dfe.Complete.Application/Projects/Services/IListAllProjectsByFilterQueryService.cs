using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;

namespace Dfe.Complete.Application.Projects.Services;

public interface IListAllProjectsByFilterQueryService
{
    Task<List<ListProjectsByFilterResultsModel>> ListAllProjectsByFilterAsync(
        ListProjectsByFilterQuery query, CancellationToken cancellationToken);
}