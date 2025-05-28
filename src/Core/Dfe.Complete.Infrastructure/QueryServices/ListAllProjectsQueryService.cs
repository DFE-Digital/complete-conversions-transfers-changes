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
    public IQueryable<ListAllProjectsQueryModel> ListAllProjects(List<ProjectState>? projectStatuses,
       ProjectType? projectType,
       AssignedToState? assignedToState = null,
       UserId? assignedToUserId = null,
       UserId? createdByUserId = null,
       string? localAuthorityCode = "",
       Region? region = null,
       ProjectTeam? team = null,
       bool? isFormAMat = null,
       string? newTrustReferenceNumber = "",
       string? search = "",
       OrderProjectQueryBy? orderBy = null)
    {
        var projects = context.Projects
            .Include(project => project.RegionalDeliveryOfficer)
            .Where(project => projectStatuses == null || projectStatuses.Contains(project.State))
            .Where(project => projectType == null || projectType == project.Type);

        projects = SetAssignStateAndUserAssignmentQueryParameters(projects, assignedToState, assignedToUserId, createdByUserId);
        projects = SetRegionAndTeamQueryParameters(projects, region, team);
        projects = SetFormAMatQueryParameters(projects, isFormAMat);

        return SetListAllProjects(projects, localAuthorityCode, newTrustReferenceNumber, search, orderBy);
    }
     
    public IQueryable<ListAllProjectsQueryModel> ListAllProjects(ProjectState? projectStatus,
        ProjectType? projectType,
        AssignedToState? assignedToState = null,
        UserId? assignedToUserId = null,
        UserId? createdByUserId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? team = null,
        bool? isFormAMat = null,
        string? newTrustReferenceNumber = "",
        OrderProjectQueryBy? orderBy = null)
    {
        var projects = context.Projects
            .Include(project => project.RegionalDeliveryOfficer)
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => projectType == null || projectType == project.Type);

        projects = SetAssignStateAndUserAssignmentQueryParameters(projects, assignedToState, assignedToUserId, createdByUserId); 
        projects = SetRegionAndTeamQueryParameters(projects, region, team);
        projects = SetFormAMatQueryParameters(projects, isFormAMat);

        return SetListAllProjects(projects, localAuthorityCode, newTrustReferenceNumber, null, orderBy);
    }

    private static IQueryable<Project> SetAssignStateAndUserAssignmentQueryParameters(IQueryable<Project> projects, AssignedToState? assignedToState = null, UserId? assignedToUserId = null, UserId? createdByUserId = null)
    {
        if (assignedToState == AssignedToState.AssignedOnly)
            projects = projects.Where(project => project.AssignedToId != null);

        if (assignedToState == AssignedToState.UnassignedOnly)
            projects = projects.Where(project => project.AssignedToId == null);

        if (assignedToUserId != null && assignedToUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.AssignedToId != null && project.AssignedToId == assignedToUserId);
        }

        if (createdByUserId != null && createdByUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.RegionalDeliveryOfficerId != null && project.RegionalDeliveryOfficerId == createdByUserId);
        }
        return projects;
    }

    private static IQueryable<Project> SetRegionAndTeamQueryParameters(IQueryable<Project> projects, Region? region = null, ProjectTeam? team = null)
    {
        if (region != null)
        {
            projects = projects.Where(project => project.Region == region);
        }

        if (team != null)
        {
            projects = projects.Where(project => project.Team == team);
        }
        return projects;
    }
    private static IQueryable<Project> SetFormAMatQueryParameters(IQueryable<Project> projects, bool? isFormAMat = null)
    {
        if (isFormAMat == true)
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null &&
                project.NewTrustName != null);
        }
        else if (isFormAMat == false)
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber == null &&
                project.NewTrustName == null);
        }
        return projects;
    }

    private IQueryable<ListAllProjectsQueryModel> SetListAllProjects(IQueryable<Project> projects,
        string? localAuthorityCode = "", 
        string? newTrustReferenceNumber = "",
        string? search = "",
        OrderProjectQueryBy? orderBy = null)
    {
        IQueryable<GiasEstablishment> giasEstablishments = context.GiasEstablishments;
         
        if (!string.IsNullOrEmpty(localAuthorityCode))
        {
            giasEstablishments = giasEstablishments.Where(establishment => establishment.LocalAuthorityCode == localAuthorityCode);
        }
         

        if (!string.IsNullOrWhiteSpace(newTrustReferenceNumber))
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null && project.NewTrustReferenceNumber == newTrustReferenceNumber);
        }

       

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
}