using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Domain.Extensions;

public static class ProjectTeamExtensions
{
    private static readonly ProjectTeam[] RdoTeams = [
        ProjectTeam.London,
        ProjectTeam.SouthEast,
        ProjectTeam.YorkshireAndTheHumber,
        ProjectTeam.NorthWest,
        ProjectTeam.EastOfEngland,
        ProjectTeam.WestMidlands,
        ProjectTeam.NorthEast,
        ProjectTeam.SouthWest,
        ProjectTeam.EastMidlands
    ];

    public static bool TeamIsRegionalDeliveryOfficer(this ProjectTeam team) => RdoTeams.ToList().Contains(team);
    public static bool TeamIsRegionalCaseworkServices(this ProjectTeam team) => team is ProjectTeam.RegionalCaseWorkerServices;
}