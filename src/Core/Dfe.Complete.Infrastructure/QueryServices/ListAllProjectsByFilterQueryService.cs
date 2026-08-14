using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsByFilterQueryService(CompleteContext context) : IListAllProjectsByFilterQueryService
{
    public IQueryable<ListProjectsByFilterResultsModel> ListAllProjectsByFilter()
    {
        return context.SignificantChangeProjects
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.AcademyUrn)
            .Select(p => new ListProjectsByFilterResultsModel(
                p.Id,
                "School Name",
                p.AcademyUrn,
                "Significant Change",
                null,
                p.AssignedToUser != null ? p.AssignedToUser.FullName : null,
                p.Region,
                "N/A",
                p.SignificantDate,
                p.State));
    }
}