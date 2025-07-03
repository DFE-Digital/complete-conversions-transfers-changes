using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsStatisticsQuery() : IRequest<Result<ListAllProjectsStatisticsModel>>;

    public class ListAllProjectsStatisticsQueryHandler(IProjectReadRepository projectReadRepository, IReadUserRepository readUserRepository, ILogger<ListAllProjectsStatisticsQueryHandler> logger) : IRequestHandler<ListAllProjectsStatisticsQuery, Result<ListAllProjectsStatisticsModel>>
    {
        public async Task<Result<ListAllProjectsStatisticsModel>> Handle(ListAllProjectsStatisticsQuery request, CancellationToken cancellationToken)
        {  
            try
            {
                var projectsQuery = new StateQuery([ProjectState.Active, ProjectState.DaoRevoked, ProjectState.Completed, ProjectState.Inactive ])
                    .Apply(projectReadRepository.Projects.AsNoTracking());

                var conversions = await projectsQuery
                    .Where(p => p.Type == ProjectType.Conversion)
                    .Select(p => new ProjectModel(p.Type, p.State, p.AssignedToId, p.Team,  p.Region, p.SignificantDateProvisional, p.SignificantDate, p.CreatedAt))
                    .ToListAsync(cancellationToken);

                var transfers = await projectsQuery
                    .Where(p => p.Type == ProjectType.Transfer)
                    .Select(p => new ProjectModel(p.Type, p.State, p.AssignedToId, p.Team, p.Region, p.SignificantDateProvisional, p.SignificantDate, p.CreatedAt))
                    .ToListAsync(cancellationToken);

                var (conversionsWithRegionalCasework, conversionsNotWithRegionalCasework) = SplitProjectsByRegionalCasework(conversions);
                var (transfersWithRegionalCasework, transfersNotWithRegionalCasework) = SplitProjectsByRegionalCasework(transfers);

                var regionalTeams = GetRegions();

                var result = new ListAllProjectsStatisticsModel
                {
                    OverAllProjects = new ProjectStatisticsModel(GetProjectsStats(conversions), GetProjectsStats(transfers)),
                    RegionalCaseworkServicesProjects = new ProjectStatisticsModel(GetProjectsStats(conversionsWithRegionalCasework, false), GetProjectsStats(transfersWithRegionalCasework, false)),
                    NotRegionalCaseworkServicesProjects = new ProjectStatisticsModel(GetProjectsStats(conversionsNotWithRegionalCasework, false), GetProjectsStats(transfersNotWithRegionalCasework, false)),
                    ConversionsPerRegion = ProjectsPerRegion(regionalTeams, conversions),
                    TransfersPerRegion = ProjectsPerRegion(regionalTeams, transfers),
                    SixMonthViewOfAllProjectOpeners = GetSixMonthViewOfAllProjectOpeners(conversions, transfers),
                    NewProjects = GetNewProjectsThisMonth(conversions, transfers),
                    UsersPerTeam = await GetUsersPerTeamAsync(regionalTeams, readUserRepository, cancellationToken)
                };

                return Result<ListAllProjectsStatisticsModel>.Success(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsStatisticsQueryHandler), request);
                return Result<ListAllProjectsStatisticsModel>.Failure(e.Message);
            }
        }

        private static (List<ProjectModel> regionalCaseworkProjects, List<ProjectModel> notRegionalCaseworkProjects) SplitProjectsByRegionalCasework(List<ProjectModel> projects)
        {
            var regionalCaseworkProjects = new List<ProjectModel>();
            var notRegionalCaseworkProjects = new List<ProjectModel>();

            foreach (var project in projects)
            {
                (IsAssignedToRegionalCasework(project) ? regionalCaseworkProjects : notRegionalCaseworkProjects).Add(project);
            }

            return (regionalCaseworkProjects, notRegionalCaseworkProjects);
        }

        private static ProjectDetailsStatisticsModel GetProjectsStats(List<ProjectModel> projects, bool includeDaoRevokedCount = true)
        {
            var inProgressCount = 0;
            var completedCount = 0;
            var unassignedCount = 0;
            var daoRevokedCount = 0;

            foreach (var project in projects)
            {
                if (IsInProgress(project)) inProgressCount++;
                if (IsCompleted(project)) completedCount++;
                if (IsUnassigned(project)) unassignedCount++;
                if (includeDaoRevokedCount && IsDaoRevoked(project)) daoRevokedCount++;
            }

            return new ProjectDetailsStatisticsModel(inProgressCount, completedCount, unassignedCount, projects.Count, daoRevokedCount);
        }

        private static List<RegionalProjectsStatisticsModel> ProjectsPerRegion(List<Region> regions, List<ProjectModel> projects)
        {
            return regions.Select(region =>
            {
                var regionProjects = projects.Where(p => p.Region == region).ToList();
                var projectDetails = GetProjectsStats(regionProjects, false);
                return new RegionalProjectsStatisticsModel(FormatDescription(region.ToDescription()), projectDetails);
            }).ToList();
        }

        private static List<AllOpenersProjectsStatisticsModel> GetSixMonthViewOfAllProjectOpeners(List<ProjectModel> conversions, List<ProjectModel> transfers)
        {
            var now = DateTime.Now;
            return Enumerable.Range(1, 6).Select(i =>
            {
                var targetMonth = now.AddMonths(i);
                int OpenerProjectCount(List<ProjectModel> project) => project.Count(p =>
                    p.SignificantDateProvisional is false &&
                    p.SignificantDate.HasValue &&
                    p.SignificantDate.Value.Year == targetMonth.Year &&
                    p.SignificantDate.Value.Month == targetMonth.Month);

                return new AllOpenersProjectsStatisticsModel(
                    $"{targetMonth:MMMM} {targetMonth.Year}",
                    OpenerProjectCount(conversions),
                    OpenerProjectCount(transfers)
                );
            }).ToList();
        }

        private static ThisMonthNewProjectsStatisticsModel GetNewProjectsThisMonth(List<ProjectModel> conversions, List<ProjectModel> transfers)
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var monthYear = $"{now:MMMM} {now.Year}";

            int CountNewProjects(List<ProjectModel> projects) => projects.Count(p => p.CreatedAt >= monthStart && p.CreatedAt <= monthEnd);

            var newConversionsThisMonth = CountNewProjects(conversions);
            var newTransfersThisMonth = CountNewProjects(transfers);

            return new ThisMonthNewProjectsStatisticsModel(monthYear, newConversionsThisMonth + newTransfersThisMonth, newConversionsThisMonth, newTransfersThisMonth);
        }

        public static async Task<Dictionary<string, int>> GetUsersPerTeamAsync(List<Region> regions, IReadUserRepository readUserRepository, CancellationToken cancellationToken)
        {
            var usersGroupedByTeam = await readUserRepository
                .Users
                .Where(x => !string.IsNullOrWhiteSpace(x.Team) && x.Id != null)
                .GroupBy(u => u.Team ?? string.Empty)
                .ToDictionaryAsync(group => group.Key, group => group.Count(), cancellationToken);

            var allTeams = regions.Select(x => x.ToDescription()).Concat(
            [
               ProjectTeam.RegionalCaseWorkerServices.ToDescription(),
               ProjectTeam.ServiceSupport.ToDescription(),
               ProjectTeam.BusinessSupport.ToDescription(),
               ProjectTeam.DataConsumers.ToDescription()
           ]);

            return allTeams.ToDictionary(FormatDescription, team => usersGroupedByTeam.TryGetValue(team, out var count) ? count : 0);
        }

        private static string FormatDescription(string description) => string.Join(" ", description.Replace("_", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));

        private static bool IsInProgress(ProjectModel p) => p.State == ProjectState.Active && p.AssignedToId != null;
        private static bool IsCompleted(ProjectModel p) => p.State == ProjectState.Completed;
        private static bool IsDaoRevoked(ProjectModel p) => p.State == ProjectState.DaoRevoked;
        private static bool IsUnassigned(ProjectModel p) => p.AssignedToId == null;
        private static bool IsAssignedToRegionalCasework(ProjectModel p) => p.Team == ProjectTeam.RegionalCaseWorkerServices;

        private static List<Region> GetRegions() =>
       [
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
