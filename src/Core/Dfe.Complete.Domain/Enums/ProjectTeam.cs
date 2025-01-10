using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum ProjectTeam
{
    [Description("regional_casework_services")]
    RegionalCaseWorkerServices,
    [Description("service_support")]
    ServiceSupport,
    [Description("london")]
    London,
    [Description("south_east")]
    SouthEast,
    [Description("yorkshire_and_the_humber")]
    YorkshireAndTheHumber,
    [Description("north_west")]
    NorthWest,
    [Description("east_of_england")]
    EastOfEngland,
    [Description("west_midlands")]
    WestMidlands,
    [Description("north_east")]
    NorthEast,
    [Description("south_west")]
    SouthWest,
    [Description("east_midlands")]
    EastMidlands,
}