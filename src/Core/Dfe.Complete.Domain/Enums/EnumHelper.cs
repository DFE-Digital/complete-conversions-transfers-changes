namespace Dfe.Complete.Domain.Enums;

public static class EnumHelper
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

    public static bool TeamIsRdo(ProjectTeam? projectTeam)
    {
        if (projectTeam == null) return false;
        return RdoTeams.ToList().Contains((ProjectTeam)projectTeam);
    }
}