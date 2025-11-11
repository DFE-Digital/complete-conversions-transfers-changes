using Dfe.Complete.Utils.Attributes;
using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

public enum ProjectTeam
{
    [Description("regional_casework_services")]
    [DisplayDescription("Regional Casework Services")]
    RegionalCaseWorkerServices = 1,
    [Description("service_support")]
    [DisplayDescription("Service Support")]
    ServiceSupport = 2,
    [Description("london")]
    [DisplayDescription("London")]
    London = 3,
    [Description("south_east")]
    [DisplayDescription("South East")]
    SouthEast = 4,
    [Description("yorkshire_and_the_humber")]
    [DisplayDescription("Yorkshire and the Humber")]
    YorkshireAndTheHumber = 5,
    [Description("north_west")]
    [DisplayDescription("North West")]
    NorthWest = 6,
    [Description("east_of_england")]
    [DisplayDescription("East of England")]
    EastOfEngland = 7,
    [Description("west_midlands")]
    [DisplayDescription("West Midlands")]
    WestMidlands = 8,
    [Description("north_east")]
    [DisplayDescription("North East")]
    NorthEast = 9,
    [Description("south_west")]
    [DisplayDescription("South West")]
    SouthWest = 10,
    [Description("east_midlands")]
    [DisplayDescription("East Midlands")]
    EastMidlands = 11,
    [Description("business_support")]
    [DisplayDescription("Business Support")]
    BusinessSupport = 12,
    [Description("data_consumers")]
    [DisplayDescription("Data Consumers")]
    DataConsumers = 13,
}