using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Application.SignificantChange.Queries.QueryFilters;
using Dfe.Complete.Application.SignificantChange.Services;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListSignificantChangeProjectsByFilterQueryService(CompleteContext context) : IListSignificantChangeProjectsByFilterQueryService
{
    public IQueryable<ListSignificantChangeProjectsByFilterResultsModel> ListSignificantChangeProjectsByFilter(ProjectState[]? projectStatuses = null)
    {
        var statusList = projectStatuses?.ToList();

        return new SignificantStateQuery(statusList)
            .Apply(context.SignificantChangeProjects)
            .OrderByDescending(p => p.SignificantDate)
            .ThenBy(p => p.AcademyUrn)
            .Select(p => new ListSignificantChangeProjectsByFilterResultsModel(
                p.Id,
                p.GiasEstablishment!.Name,
                p.AcademyUrn,
                p.AssignedToUser!.FullName,
                p.Region,
                p.LocalAuthority!.Name,
                p.SignificantDate,
                p.State));
    }
}