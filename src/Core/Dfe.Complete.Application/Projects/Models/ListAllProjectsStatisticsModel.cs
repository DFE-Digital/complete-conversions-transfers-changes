using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models
{
    public class ListAllProjectsStatisticsModel
    {
        public ProjectStatisticsModel OverAllProjects { get; set; } = null!;
        public ProjectStatisticsModel RegionalCaseworkServicesProjects { get; set; } = null!;
        public ProjectStatisticsModel NotRegionalCaseworkServicesProjects { get; set; } = null!;
        public List<RegionalProjectsStatisticsModel> ConversionsPerRegion { get; set; } = [];
        public List<RegionalProjectsStatisticsModel> TransfersPerRegion { get; set; } = [];
        public List<AllOpenersProjectsStatisticsModel> SixMonthViewOfAllProjectOpeners { get; set; } = null!;
        public ThisMonthNewProjectsStatisticsModel NewProjects { get; set; } = null!;
        public Dictionary<string, int> UsersPerTeam { get; set; } = [];

    }

    public record ProjectStatisticsModel(ProjectDetailsStatisticsModel Conversions, ProjectDetailsStatisticsModel Transfers);
    public record RegionalProjectsStatisticsModel(string RegionName, ProjectDetailsStatisticsModel Details);
    public record AllOpenersProjectsStatisticsModel(string Date, int Conversions, int Tranfers);
    public record ProjectDetailsStatisticsModel(int InProgressProjects, int CompletedProjects, int UnassignedProjects, int TotalProjects, int? DaoRevokedProjects = null);
    public record ThisMonthNewProjectsStatisticsModel(string Date, int TotalProjects, int TotalConversions, int TotalTransfers);
    public record ProjectModel(ProjectType? Type, ProjectState? State, UserId? AssignedToId, ProjectTeam? Team, Region? Region, bool? SignificantDateProvisional, DateOnly? SignificantDate, DateTime? CreatedAt);
}