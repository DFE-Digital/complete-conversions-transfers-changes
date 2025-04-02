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

    public static bool TeamIsRdo(this ProjectTeam team) => RdoTeams.ToList().Contains(team);
}