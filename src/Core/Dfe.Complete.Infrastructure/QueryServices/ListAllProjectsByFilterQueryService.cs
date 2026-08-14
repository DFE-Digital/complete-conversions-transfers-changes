using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsByFilterQueryService(CompleteContext context) : IListAllProjectsByFilterQueryService
{
    public async Task<List<ListProjectsByFilterResultsModel>> ListAllProjectsByFilterAsync(
        ListProjectsByFilterQuery query, CancellationToken cancellationToken)
    {
        var significantChangeProjects = context.SignificantChangeProjects
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.AcademyUrn)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
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

        var results = await significantChangeProjects.ToListAsync(cancellationToken);

        return results;
    }
}