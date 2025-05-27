using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Infrastructure.Extensions; 
using System.Text.RegularExpressions;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsQueryService(CompleteContext context) : IListAllProjectsQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjects(
        ProjectFilters filters,
        string? search = "",
        OrderProjectQueryBy? orderBy = null)
    {
        var projects = context.Projects
            .Include(project => project.RegionalDeliveryOfficer)
            .AsQueryable();

        IQueryable<GiasEstablishment> giasEstablishments = context.GiasEstablishments;

        projects = ApplyProjectFilters(projects, filters);
        giasEstablishments = ApplyGiasEstablishmentFilters(giasEstablishments, filters);

        if (!string.IsNullOrWhiteSpace(search))
        {
            (projects, giasEstablishments) = SearchProjects(projects, giasEstablishments, search);
        }

        return GenerateQuery(projects, giasEstablishments, orderBy);
    }

    public static (IQueryable<Project>, IQueryable<GiasEstablishment> giasEstablishments) SearchProjects(IQueryable<Project> projects, IQueryable<GiasEstablishment> giasEstablishments, string searchTerm)
    {  
        _ = int.TryParse(searchTerm, out int number);
        var timeSpan = TimeSpan.FromMilliseconds(100);

        if (Regex.IsMatch(searchTerm, @"^\d{6}$", RegexOptions.None, timeSpan))
        {
            projects = projects.Where(project => project.Urn == new Urn(number));
        }
        else if (Regex.IsMatch(searchTerm, @"^\d{8}$", RegexOptions.None, timeSpan))
        {
            projects = projects.Where(project => project.IncomingTrustUkprn == new Ukprn(number) || project.OutgoingTrustUkprn == new Ukprn(number));
        }
        else if (Regex.IsMatch(searchTerm, @"^\d{4}$", RegexOptions.None, timeSpan))
        {
            giasEstablishments = giasEstablishments.Where(establishment => establishment.EstablishmentNumber == searchTerm);
        }
        else
        {
            searchTerm = searchTerm.ToLower();

            giasEstablishments = giasEstablishments.Where(establishment => establishment.Name != null && EF.Functions.Like(establishment.Name.ToLower(), $"%{searchTerm}%"));
        }

        return (projects, giasEstablishments);
    }
     
    private static IQueryable<ListAllProjectsQueryModel> GenerateQuery(IQueryable<Project> projects, IQueryable<GiasEstablishment> giasEstablishments, OrderProjectQueryBy? orderBy = null)
    {
        return projects
            .Include(p => p.AssignedTo)
            .Include(p => p.LocalAuthority)
            .Include(p => p.SignificantDateHistories)
            .OrderProjectBy(orderBy)
            .Join(giasEstablishments, project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));
    }

    private static IQueryable<Project> ApplyProjectFilters(IQueryable<Project> projects, ProjectFilters filters)
    {
        projects = FilterByProjectStatus(projects, filters.ProjectStatus);
        projects = FilterByProjectType(projects, filters.ProjectType);
        projects = FilterByAssignedToState(projects, filters.AssignedToState);
        projects = FilterByAssignedToUserId(projects, filters.AssignedToUserId);
        projects = FilterByCreatedByUserId(projects, filters.CreatedByUserId);
        projects = FilterByRegion(projects, filters.Region);
        projects = FilterByTeam(projects, filters.Team);
        projects = FilterByNewTrustReferenceNumber(projects, filters.NewTrustReferenceNumber);
        projects = FilterByIsFormAMat(projects, filters.IsFormAMat);
        projects = FilterBySignificantDateRange(projects, filters.SignificantDateRange);
        return projects;
    }

    private static IQueryable<GiasEstablishment> ApplyGiasEstablishmentFilters(IQueryable<GiasEstablishment> giasEstablishments, ProjectFilters filters)
    {
        if (!string.IsNullOrEmpty(filters.LocalAuthorityCode))
            giasEstablishments = giasEstablishments.Where(establishment => establishment.LocalAuthorityCode == filters.LocalAuthorityCode);

        return giasEstablishments;
    }

    private static IQueryable<Project> FilterByProjectStatus(IQueryable<Project> projects, ProjectState? status)
    {
        if (status != null)
            projects = projects.Where(project => project.State == status);
        return projects;
    }

    private static IQueryable<Project> FilterByProjectType(IQueryable<Project> projects, ProjectType? type)
    {
        if (type != null)
            projects = projects.Where(project => project.Type == type);
        return projects;
    }

    private static IQueryable<Project> FilterByAssignedToState(IQueryable<Project> projects, AssignedToState? assignedToState)
    {
        if (assignedToState == AssignedToState.AssignedOnly)
            projects = projects.Where(project => project.AssignedToId != null);
        else if (assignedToState == AssignedToState.UnassignedOnly)
            projects = projects.Where(project => project.AssignedToId == null);
        return projects;
    }

    private static IQueryable<Project> FilterByAssignedToUserId(IQueryable<Project> projects, UserId? assignedToUserId)
    {
        if (assignedToUserId != null && assignedToUserId.Value != Guid.Empty)
            projects = projects.Where(project => project.AssignedToId != null && project.AssignedToId == assignedToUserId);

        return projects;
    }

    private static IQueryable<Project> FilterByCreatedByUserId(IQueryable<Project> projects, UserId? createdByUserId)
    {
        if (createdByUserId != null && createdByUserId.Value != Guid.Empty)
            projects = projects.Where(project => project.RegionalDeliveryOfficerId != null && project.RegionalDeliveryOfficerId == createdByUserId);

        return projects;
    }

    private static IQueryable<Project> FilterByRegion(IQueryable<Project> projects, Region? region)
    {
        if (region != null)
            projects = projects.Where(project => project.Region == region);
        return projects;
    }

    private static IQueryable<Project> FilterByTeam(IQueryable<Project> projects, ProjectTeam? team)
    {
        if (team != null)
            projects = projects.Where(project => project.Team == team);
        return projects;
    }

    private static IQueryable<Project> FilterByNewTrustReferenceNumber(IQueryable<Project> projects, string? newTrustReferenceNumber)
    {
        if (!string.IsNullOrWhiteSpace(newTrustReferenceNumber))
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null && project.NewTrustReferenceNumber == newTrustReferenceNumber);

        return projects;
    }

    private static IQueryable<Project> FilterByIsFormAMat(IQueryable<Project> projects, bool? isFormAMat)
    {
        if (isFormAMat == true)
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null &&
                project.NewTrustName != null);
        else if (isFormAMat == false)
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber == null &&
                project.NewTrustName == null);

        return projects;
    }

    private static IQueryable<Project> FilterBySignificantDateRange(IQueryable<Project> projects, DateRangeFilter? significantDateRange)
    {
        if (significantDateRange != null)
        {
            var fromDate = significantDateRange.From;
            var toDate = significantDateRange.To ?? fromDate;

            projects = projects.Where(project =>
                project.SignificantDate.HasValue &&
                project.SignificantDate.Value >= fromDate &&
                project.SignificantDate.Value <= toDate);
        }

        return projects;
    }
}