using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Application.SignificantChange.Queries.QueryFilters;
using Dfe.Complete.Application.SignificantChange.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Utils;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListSignificantChangeProjectsByFilterQueryService(CompleteContext context) : IListSignificantChangeProjectsByFilterQueryService
{
    private enum FilterFacet
    {
        None,
        Status,
        ProjectOwner,
        Region,
        LocalAuthority
    }

    public IQueryable<ListSignificantChangeProjectsByFilterResultsModel> ListSignificantChangeProjectsByFilter(SignificantChangeProjectFilters filters)
    {
        return ApplyFilters(context.SignificantChangeProjects, filters)
            .OrderByDescending(p => p.SignificantDate)
            .ThenBy(p => p.AcademyUrn)
            .Select(p => new ListSignificantChangeProjectsByFilterResultsModel(
                p.Id,
                p.GiasEstablishment!.Name,
                p.AcademyUrn,
                p.AssignedToUser!.FirstName + " " + p.AssignedToUser.LastName,
                p.Region,
                p.LocalAuthority!.Name,
                p.SignificantDate,
                p.State));
    }

    public async Task<SignificantChangeProjectFilterOptions> GetFilterOptions(SignificantChangeProjectFilters filters, CancellationToken cancellationToken)
    {
        var statuses = await ApplyFilters(context.SignificantChangeProjects, filters, FilterFacet.Status)
            .Select(p => p.State)
            .Distinct()
            .ToListAsync(cancellationToken);

        var projectOwners = await ApplyFilters(context.SignificantChangeProjects, filters, FilterFacet.ProjectOwner)
            .Where(p => p.AssignedToUserId != null)
            .Select(p => new { UserId = p.AssignedToUserId!, Name = p.AssignedToUser!.FirstName + " " + p.AssignedToUser.LastName })
            .Distinct()
            .ToListAsync(cancellationToken);

        var regions = await ApplyFilters(context.SignificantChangeProjects, filters, FilterFacet.Region)
            .Where(p => p.Region != null)
            .Select(p => p.Region!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var localAuthorities = await ApplyFilters(context.SignificantChangeProjects, filters, FilterFacet.LocalAuthority)
            .Select(p => new { LocalAuthorityId = p.LocalAuthorityId, Name = p.LocalAuthority!.Name })
            .Distinct()
            .ToListAsync(cancellationToken);

        return new SignificantChangeProjectFilterOptions(
            statuses
                .OrderBy(s => s)
                .Select(s => new SignificantChangeFilterOption(s.ToString(), s.ToString()))
                .ToList(),
            projectOwners
                .Select(u => new SignificantChangeFilterOption(u.UserId.Value.ToString(), u.Name))
                .OrderBy(o => o.Label)
                .ToList(),
            regions
                .Select(r => new SignificantChangeFilterOption(r.ToString(), r.ToDisplayDescription()))
                .OrderBy(o => o.Label)
                .ToList(),
            localAuthorities
                .Select(la => new SignificantChangeFilterOption(la.LocalAuthorityId.Value.ToString(), la.Name))
                .OrderBy(o => o.Label)
                .ToList());
    }

    private static IQueryable<SignificantChangeProject> ApplyFilters(
        IQueryable<SignificantChangeProject> projects,
        SignificantChangeProjectFilters filters,
        FilterFacet facetToExclude = FilterFacet.None)
    {
        if (facetToExclude != FilterFacet.Status)
        {
            projects = new SignificantStateQuery(filters.ProjectStatuses?.ToList()).Apply(projects);
        }

        var keyword = filters.Keyword?.Trim();
        if (!string.IsNullOrEmpty(keyword))
        {
            var isNumber = int.TryParse(keyword, out _);
            var likePattern = $"%{keyword}%";

            projects = projects.Where(p => (p.GiasEstablishment != null && p.GiasEstablishment.Name!.Contains(keyword))
                || p.TrustName.Contains(keyword)
                || (isNumber
                    && (EF.Functions.Like(EF.Property<int>(p, nameof(SignificantChangeProject.AcademyUrn)) + string.Empty, likePattern)
                        || EF.Functions.Like(EF.Property<int>(p, nameof(SignificantChangeProject.TrustUkprn)) + string.Empty, likePattern))));
        }

        if (facetToExclude != FilterFacet.ProjectOwner && filters.ProjectOwnerIds is { Length: > 0 })
        {
            var userIds = filters.ProjectOwnerIds.ToList();
            projects = projects.Where(p => p.AssignedToUserId != null && userIds.Contains(p.AssignedToUserId));
        }

        if (facetToExclude != FilterFacet.Region && filters.Regions is { Length: > 0 })
        {
            var regions = filters.Regions.ToList();
            projects = projects.Where(p => p.Region != null && regions.Contains(p.Region.Value));
        }

        if (facetToExclude != FilterFacet.LocalAuthority && filters.LocalAuthorityIds is { Length: > 0 })
        {
            var localAuthorityIds = filters.LocalAuthorityIds.ToList();
            projects = projects.Where(p => localAuthorityIds.Contains(p.LocalAuthorityId));
        }

        return projects;
    }
}