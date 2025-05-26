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
            .Where(project => filters.ProjectStatus == null || project.State == filters.ProjectStatus)
            .Where(project => filters.ProjectType == null || filters.ProjectType == project.Type);

        IQueryable<GiasEstablishment> giasEstablishments = context.GiasEstablishments;

        if (filters.AssignedToState == AssignedToState.AssignedOnly)
            projects = projects.Where(project => project.AssignedToId != null);

        if (filters.AssignedToState == AssignedToState.UnassignedOnly)
            projects = projects.Where(project => project.AssignedToId == null);

        if (filters.AssignedToUserId != null && filters.AssignedToUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.AssignedToId != null && project.AssignedToId == filters.AssignedToUserId);
        }
        
        if (filters.CreatedByUserId != null && filters.CreatedByUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.RegionalDeliveryOfficerId != null && project.RegionalDeliveryOfficerId == filters.CreatedByUserId);
        }

        if (!string.IsNullOrEmpty(filters.LocalAuthorityCode))
        {
            giasEstablishments = giasEstablishments.Where(establishment => establishment.LocalAuthorityCode == filters.LocalAuthorityCode);
        }

        if (filters.Region != null)
        {
            projects = projects.Where(project => project.Region == filters.Region);
        }

        if (filters.Team != null)
        {
            projects = projects.Where(project => project.Team == filters.Team);
        }

        if (!string.IsNullOrWhiteSpace(filters.NewTrustReferenceNumber))
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null && project.NewTrustReferenceNumber == filters.NewTrustReferenceNumber);
        }
        
        if (filters.IsFormAMat == true)
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber != null &&
                project.NewTrustName != null);
        }
        else if (filters.IsFormAMat == false)
        {
            projects = projects.Where(project =>
                project.NewTrustReferenceNumber == null &&
                project.NewTrustName == null);
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