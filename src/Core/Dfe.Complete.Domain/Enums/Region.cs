using System.ComponentModel;
using Dfe.Complete.Utils.Attributes;

namespace Dfe.Complete.Domain.Enums
{
    public enum Region
    {
        [Description("london")]
        [DisplayDescription("London")]
        London = 'H',
        [Description("south_east")]
        [DisplayDescription("South East")]
        SouthEast = 'J',
        [Description("yorkshire_and_the_humber")]
        [DisplayDescription("Yorkshire and the Humber")]
        YorkshireAndTheHumber = 'D',
        [Description("north_west")]
        [DisplayDescription("North West")]
        NorthWest = 'B',
        [Description("east_of_england")]
        [DisplayDescription("East of England")]
        EastOfEngland = 'G',
        [Description("west_midlands")]
        [DisplayDescription("West Midlands")]
        WestMidlands = 'F',
        [Description("north_east")]
        [DisplayDescription("North East")]
        NorthEast = 'A',
        [Description("south_west")]
        [DisplayDescription("South West")]
        SouthWest = 'K',
        [Description("east_midlands")]
        [DisplayDescription("East Midlands")]
        EastMidlands = 'E'
    }
}
