using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsByFilterQueryService(CompleteContext context) : IListAllProjectsByFilterQueryService
{
    public IQueryable<ListProjectsByFilterResultsModel> ListAllProjectsByFilter()
    {
        var significantChangeProjects = context.SignificantChangeProjects
            .Include(p => p.LocalAuthority)
            .Include(p => p.GiasEstablishment)
            .Select(p => new
            {
                p.Id,
                EstablishmentName = p.GiasEstablishment!.Name,
                Urn = p.AcademyUrn,
                ProjectType = "Significant Change",
                IsFormAMat = (bool?)null,
                AssignedToUser = p.AssignedToUser,
                p.Region,
                LocalAuthorityName = p.LocalAuthority!.Name,
                p.SignificantDate,
                p.State 
            });

        var projects = context.Projects
            .Include(p => p.LocalAuthority)
            .Include(p => p.GiasEstablishment)
            .Select(p => new
            {
                p.Id,
                EstablishmentName = p.GiasEstablishment!.Name,
                Urn = p.Urn,
                ProjectType = p.Type == ProjectType.Conversion
                    ? "Conversion"
                    : p.Type == ProjectType.Transfer
                        ? "Transfer"
                        : string.Empty,
                IsFormAMat = (bool?)(p.NewTrustReferenceNumber != null && p.NewTrustName != null),
                AssignedToUser = p.AssignedTo,
                p.Region,
                LocalAuthorityName = p.LocalAuthority!.Name,
                p.SignificantDate,
                p.State
            });

        return significantChangeProjects
            .Concat(projects)
            .OrderByDescending(x => x.SignificantDate)
            .ThenBy(x => x.Urn)
            .Select(p => new ListProjectsByFilterResultsModel(
                p.Id,
                p.EstablishmentName,
                p.Urn,
                p.ProjectType,
                p.IsFormAMat,
                p.AssignedToUser!.FullName,
                p.Region,
                p.LocalAuthorityName,
                p.SignificantDate,
                p.State));
    }
}