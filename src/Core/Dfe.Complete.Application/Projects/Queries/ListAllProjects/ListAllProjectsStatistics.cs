using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsStatisticsQuery() : IRequest<Result<ListAllProjectsStatisticsModel>>
    {
    }
    public class ListAllProjectsStatisticsQueryHandler(IProjectsQueryBuilder projectsQueryBuilder, IUsersQueryBuilder usersQueryBuilder,  ILogger<ListAllProjectsStatisticsQueryHandler> logger) : IRequestHandler<ListAllProjectsStatisticsQuery, Result<ListAllProjectsStatisticsModel>>
    { 
        public async Task<Result<ListAllProjectsStatisticsModel>> Handle(ListAllProjectsStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectStates = new List<ProjectState> { ProjectState.Active, ProjectState.DaoRevoked, ProjectState.Completed };
                var filters = new ProjectFilters(null, null, ProjectStatuses: projectStates);
                var allProjects = await projectsQueryBuilder.ApplyProjectFilters(filters) 
                    .GetProjects()
                    .ToListAsync(cancellationToken); 
                 
                var conversions = allProjects.Where(p => p.Type == ProjectType.Conversion).ToList();
                var transfers = allProjects.Where(p => p.Type == ProjectType.Transfer).ToList();
                  
                var conversionsWithRegionalCasework = conversions.Where(IsAssignedToRegionalCasework).ToList();
                var conversionsNotWithRegionalCasework = conversions.Where(IsNotAssignedToRegionalCasework).ToList();
                var transfersWithRegionalCasework = transfers.Where(IsAssignedToRegionalCasework).ToList();
                var transfersNotWithRegionalCasework = transfers.Where(IsNotAssignedToRegionalCasework).ToList();

                var regionalTeams = GetRegions();

                var result = new ListAllProjectsStatisticsModel
                {
                    OveraAllProjects = new ProjectsModel(GetProjectsStats(conversions), GetProjectsStats(transfers)),
                    RegionalCaseworkServicesProjects = new ProjectsModel(GetProjectsStats(conversionsWithRegionalCasework, false), GetProjectsStats(transfersWithRegionalCasework, false)),
                    NotRegionalCaseworkServicesProjects = new ProjectsModel(GetProjectsStats(conversionsNotWithRegionalCasework, false), GetProjectsStats(transfersNotWithRegionalCasework, false)),
                    ConversionsPerRegion = ProjectsPerRegion(regionalTeams, conversions),
                    TransfersPerRegion = ProjectsPerRegion(regionalTeams, transfers),
                    SixMonthViewOfAllProjectOpeners = GetSixMonthViewOfAllProjectOpeners(conversions, transfers),
                    NewProjects = GetNewProjectsThisMonth(conversions, transfers),
                    UsersPerTeam = await GetUsersPerTeamAsync(regionalTeams, usersQueryBuilder, cancellationToken)
                };

                return Result<ListAllProjectsStatisticsModel>.Success(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForTeamQueryHandler), request);
                return Result<ListAllProjectsStatisticsModel>.Failure(e.Message);
            }
        }
        private static List<ProjectTeam> GetRegionalTeams(List<ProjectTeam> usersTeams) => Enum.GetValues(typeof(ProjectTeam))
                .Cast<ProjectTeam>()
                .Where(t => !usersTeams.Contains(t))
                .ToList();

        private static ProjectDetailsModel GetProjectsStats(List<Project> projects, bool includeDaoRevokedCount = true) => new(
                projects.Count(IsInProgress),
                projects.Count(IsCompleted),
                projects.Count(IsUnassigned),
                projects.Count,
                includeDaoRevokedCount ? projects.Count(IsDaoRevoked) : null);

        private static List<RegionProjectDetailsModel> ProjectsPerRegion(List<Region> regions, List<Project> projects)
            => regions.Select(region =>
            {
                var regionProjects = projects.Where(p => p.Region == region).ToList();
                var projectDetails = new ProjectDetailsModel(
                    InProgressProjects: regionProjects.Count(IsInProgress),
                    CompletedProjects: regionProjects.Count(IsCompleted),
                    UnassignedProjects: regionProjects.Count(IsUnassigned),
                    TotalProjects: regionProjects.Count
                );
                return new RegionProjectDetailsModel(FormatDescription(region.ToDescription()), projectDetails);
            }).ToList();

        private static List<AllOpenersProjectsModel> GetSixMonthViewOfAllProjectOpeners(List<Project> conversions, List<Project> transfers) 
        {
            var openersPerMonth = new List<AllOpenersProjectsModel>();

            for (int i = 1; i <= 6; i++)
            {
                var targetMonth = DateTime.Now.AddMonths(i);  
                int OpenerProjectCount(List<Project> project)
                    => project.Count(p =>
                    p.SignificantDateProvisional is false &&
                    p.SignificantDate.HasValue &&
                    p.SignificantDate.Value.Year == targetMonth.Year &&
                    p.SignificantDate.Value.Month == targetMonth.Month);

                openersPerMonth.Add(new AllOpenersProjectsModel(
                    $"{targetMonth:MMMM} {targetMonth.Year}",
                    OpenerProjectCount(conversions),
                    OpenerProjectCount(transfers)
                ));
            }
            return openersPerMonth;
        }

        private static NewProjectsInThisMonth GetNewProjectsThisMonth(List<Project> conversions, List<Project> transfers)
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var monthYear = $"{now:MMMM} {now.Year}";

            int CountNewProjects(List<Project> projects) => projects.Count(p => p.CreatedAt >= monthStart && p.CreatedAt <= monthEnd);

            var newConversionsThisMonth = CountNewProjects(conversions);
            var newTransfersThisMonth = CountNewProjects(transfers);

            return new NewProjectsInThisMonth(monthYear, newConversionsThisMonth + newTransfersThisMonth, newConversionsThisMonth, newTransfersThisMonth);
        }

        public static async Task<Dictionary<string, int>> GetUsersPerTeamAsync(List<Region> regions, IUsersQueryBuilder usersQueryBuilder, CancellationToken cancellationToken)
        {
            var usersGroupedByTeam = await usersQueryBuilder
                .ApplyUsersFilters(new UsersFilters())
                .Where(x => !string.IsNullOrWhiteSpace(x.Team))
                .GetUsers()
                .GroupBy(u => u.Team ?? string.Empty)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => group.Count(user => user.Id != null),
                    cancellationToken);

            return regions.Select(x => x.ToDescription()).Concat(
            [
                ProjectTeam.RegionalCaseWorkerServices.ToDescription(),
                ProjectTeam.ServiceSupport.ToDescription(),
                ProjectTeam.BusinessSupport.ToDescription(),
                ProjectTeam.DataConsumers.ToDescription()
            ]).ToDictionary(FormatDescription, team => usersGroupedByTeam.TryGetValue(team, out var count) ? count : 0);
        }

        private static string FormatDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;

            return string.Join(" ", description.Replace("_", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
        }
        private static bool IsInProgress(Project p) => p.State == ProjectState.Active && p.AssignedToId != null;
        private static bool IsCompleted(Project p) => p.State == ProjectState.Completed;
        private static bool IsDaoRevoked(Project p) => p.State == ProjectState.DaoRevoked;
        private static bool IsUnassigned(Project p) => p.State == ProjectState.Active && p.AssignedToId == null;
        private static bool IsAssignedToRegionalCasework(Project p) => p.Team == ProjectTeam.RegionalCaseWorkerServices;
        private static bool IsNotAssignedToRegionalCasework(Project p) => p.Team != ProjectTeam.RegionalCaseWorkerServices && !string.IsNullOrEmpty(p.Team?.ToString());

        private static List<Region> GetRegions() => [
               Region.London,
               Region.SouthEast,
               Region.YorkshireAndTheHumber,
               Region.NorthWest,
               Region.EastOfEngland,
               Region.WestMidlands,
               Region.NorthEast,
               Region.SouthWest,
               Region.EastMidlands
           ];
    }
}
