using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.SignificantChange.Services;

public interface IListSignificantChangeProjectsByFilterQueryService
{
    IQueryable<ListSignificantChangeProjectsByFilterResultsModel> ListSignificantChangeProjectsByFilter(ProjectState[]? projectStatuses = null);
}