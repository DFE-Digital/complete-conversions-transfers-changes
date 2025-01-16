using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum ProjectTeam
{
    [Description("regional_casework_services")]
    RegionalCaseWorkerServices = 1,
    [Description("service_support")]
    ServiceSupport = 2,
    [Description("london")]
    London = 3,
    [Description("south_east")]
    SouthEast = 4,
    [Description("yorkshire_and_the_humber")]
    YorkshireAndTheHumber = 5,
    [Description("north_west")]
    NorthWest = 6,
    [Description("east_of_england")]
    EastOfEngland = 7,
    [Description("west_midlands")]
    WestMidlands = 8,
    [Description("north_east")]
    NorthEast = 9,
    [Description("south_west")]
    SouthWest = 10,
    [Description("east_midlands")]
    EastMidlands = 11,
}