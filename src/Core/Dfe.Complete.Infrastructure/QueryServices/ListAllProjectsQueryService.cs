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
        string? search = "",
        OrderProjectQueryBy? orderBy = null)
    {
        var projects = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus);
        if (string.IsNullOrWhiteSpace(search))
        {
            projects = projects
                .Where(project => projectStatus != ProjectState.Active || project.AssignedToId != null)
                .Where(project => projectType == null || projectType == project.Type);
        }

        IQueryable<GiasEstablishment> giasEstablishments = context.GiasEstablishments;

        if (assignedToState == AssignedToState.AssignedOnly)
            projects = projects.Where(project => project.AssignedToId != null);

        if (assignedToUserId != null && assignedToUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.AssignedToId != null && project.AssignedToId == assignedToUserId);
        }
        
        if (createdByUserId != null && createdByUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.RegionalDeliveryOfficerId != null && project.RegionalDeliveryOfficerId == createdByUserId);
        }

        if (!string.IsNullOrEmpty(localAuthorityCode))
        {
            giasEstablishments = giasEstablishments.Where(establishment => establishment.LocalAuthorityCode == localAuthorityCode);
        }

        if (region != null)
        {
            projects = projects.Where(project => project.Region == region);
        }

        if (team != null)
        {
            projects = projects.Where(project => project.Team == team);
        }

        if (!string.IsNullOrWhiteSpace(newTrustReferenceNumber))
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null && project.NewTrustReferenceNumber == newTrustReferenceNumber);
        }
        
        if (isFormAMat == true)
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null &&
                project.NewTrustName != null &&
                project.IncomingTrustUkprn == null);
        }
        else if (isFormAMat == false)
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber == null &&
                project.NewTrustName == null &&
                project.IncomingTrustUkprn != null);
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