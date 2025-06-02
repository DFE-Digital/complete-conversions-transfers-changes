using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

public class ProjectsQueryBuilder(CompleteContext context) : IProjectsQueryBuilder
{
    private IQueryable<Project> _projects = context.Projects.AsQueryable();
    private IQueryable<GiasEstablishment> _giasEstablishments = context.GiasEstablishments.AsQueryable();

    public IProjectsQueryBuilder ApplyProjectFilters(ProjectFilters filters)
    {
        _projects = FilterByProjectStatus(_projects, filters.ProjectStatus);
        _projects = FilterByProjectStatuses(_projects, filters.ProjectStatuses);
        _projects = FilterByProjectType(_projects, filters.ProjectType);
        _projects = FilterByAssignedToState(_projects, filters.AssignedToState);
        _projects = FilterByAssignedToUserId(_projects, filters.AssignedToUserId);
        _projects = FilterByCreatedByUserId(_projects, filters.CreatedByUserId);
        _projects = FilterByRegion(_projects, filters.Region);
        _projects = FilterByTeam(_projects, filters.Team);
        _projects = FilterByNewTrustReferenceNumber(_projects, filters.NewTrustReferenceNumber);
        _projects = FilterByIsFormAMat(_projects, filters.IsFormAMat);
        _projects = FilterByIncomingTrustUkprn(_projects, filters.IncomingTrustUkprn);
        _projects = FilterBySignificantDateRange(_projects, filters.SignificantDateRange);
        return this;
    }

    public IProjectsQueryBuilder ApplyGiasEstablishmentFilters(ProjectFilters filters)
    {
        if (!string.IsNullOrEmpty(filters.LocalAuthorityCode))
            _giasEstablishments = _giasEstablishments.Where(est => est.LocalAuthorityCode == filters.LocalAuthorityCode);
        return this;
    }

    public IProjectsQueryBuilder Search(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return this;

        _ = int.TryParse(searchTerm, out int number);
        var timeSpan = TimeSpan.FromMilliseconds(100);

        if (Regex.IsMatch(searchTerm, @"^\d{6}$", RegexOptions.None, timeSpan))
        {
            _projects = _projects.Where(project => project.Urn == new Urn(number));
        }
        else if (Regex.IsMatch(searchTerm, @"^\d{8}$", RegexOptions.None, timeSpan))
        {
            _projects = _projects.Where(project =>
                project.IncomingTrustUkprn == new Ukprn(number) || project.OutgoingTrustUkprn == new Ukprn(number));
        }
        else if (Regex.IsMatch(searchTerm, @"^\d{4}$", RegexOptions.None, timeSpan))
        {
            _giasEstablishments = _giasEstablishments.Where(est => est.EstablishmentNumber == searchTerm);
        }
        else
        {
            searchTerm = searchTerm.ToLower();
            _giasEstablishments = _giasEstablishments.Where(est =>
                est.Name != null && EF.Functions.Like(est.Name.ToLower(), $"%{searchTerm}%"));
        }

        return this;
    }

    public IProjectsQueryBuilder Where(Expression<Func<Project, bool>> predicate)
    {
        _projects = _projects.Where(predicate);
        return this;
    }

    public IQueryable<Project> GetProjects()
    {
        return _projects;
    }

    public IQueryable<ListAllProjectsQueryModel> GenerateQuery(OrderProjectQueryBy? orderBy = null)
    {
        return _projects
            .Include(p => p.AssignedTo)
            .Include(p => p.LocalAuthority)
            .Include(p => p.SignificantDateHistories)
            .Include(p => p.RegionalDeliveryOfficer)
            .OrderProjectBy(orderBy)
            .Join(_giasEstablishments,
                  project => project.Urn,
                  establishment => establishment.Urn,
                  (project, establishment) => new ListAllProjectsQueryModel(project, establishment));
    }

    private static IQueryable<Project> FilterByProjectStatus(IQueryable<Project> projects, ProjectState? status)
    {
        if (status != null)
            projects = projects.Where(project => project.State == status);
        return projects;
    }
    private static IQueryable<Project> FilterByProjectStatuses(IQueryable<Project> projects, List<ProjectState>? statuses)
    {
        if (statuses != null)
            projects = projects.Where(project => statuses.Contains(project.State));
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
            projects = projects.Where(project => project.NewTrustReferenceNumber != null && project.NewTrustReferenceNumber == newTrustReferenceNumber);
        return projects;
    }

    private static IQueryable<Project> FilterByIsFormAMat(IQueryable<Project> projects, bool? isFormAMat)
    {
        if (isFormAMat == true)
            projects = projects.Where(project => project.NewTrustReferenceNumber != null && project.NewTrustName != null);
        else if (isFormAMat == false)
            projects = projects.Where(project => project.NewTrustReferenceNumber == null && project.NewTrustName == null);
        return projects;
    }

    private static IQueryable<Project> FilterByIncomingTrustUkprn(IQueryable<Project> projects, string? incomingTrustUkprn)
    {
        if (!string.IsNullOrWhiteSpace(incomingTrustUkprn))
            projects = projects.Where(project =>
                project.IncomingTrustUkprn != null && project.IncomingTrustUkprn == incomingTrustUkprn);
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
