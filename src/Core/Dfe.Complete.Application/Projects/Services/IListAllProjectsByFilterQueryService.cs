using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Projects.Services;

public interface IListAllProjectsByFilterQueryService
{
    IQueryable<ListProjectsByFilterResultsModel> ListAllProjectsByFilter();
}