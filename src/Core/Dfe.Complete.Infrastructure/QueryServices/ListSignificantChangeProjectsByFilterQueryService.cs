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

        var significantChangeProjects = new SignificantStateQuery(statusList)
            .Apply(context.SignificantChangeProjects)
            .Include(p => p.LocalAuthority)
            .Include(p => p.GiasEstablishment)
            .Select(p => new
            {
                p.Id,
                EstablishmentName = p.GiasEstablishment!.Name,
                Urn = p.AcademyUrn,
                p.AssignedToUser,
                p.Region,
                LocalAuthorityName = p.LocalAuthority!.Name,
                p.SignificantDate,
                p.State 
            });

        return significantChangeProjects
            .OrderByDescending(x => x.SignificantDate)
            .ThenBy(x => x.Urn)
            .Select(p => new ListSignificantChangeProjectsByFilterResultsModel(
                p.Id,
                p.EstablishmentName,
                p.Urn,
                p.AssignedToUser!.FullName,
                p.Region,
                p.LocalAuthorityName,
                p.SignificantDate,
                p.State));
    }
}