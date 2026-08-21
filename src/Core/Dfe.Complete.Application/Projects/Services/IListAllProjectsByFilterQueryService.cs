using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Services;

public interface IListAllProjectsByFilterQueryService
{
    IQueryable<ListProjectsByFilterResultsModel> ListAllProjectsByFilter(ProjectState[]? projectStatuses = null);
}