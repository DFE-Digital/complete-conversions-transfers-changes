namespace Dfe.Complete.Domain.Enums
{
    using Utils.Attributes;
    using System.ComponentModel;

    public enum NurseryArrangementOption
    {
        [Description("notApplicable")]
        [DisplayDescription("Not applicable")]
        NotApplicable = 0,

        [Description("directProvision")]
        [DisplayDescription("A direct provision")]
        DirectProvision = 1,

        [Description("subsidiaryCompanyOfTheTrust")]
        [DisplayDescription("Subsidiary Company of the Trust")]
        SubsidiaryCompanyOfTheTrust = 2,

        [Description("independentProvider")]
        [DisplayDescription("An independent provider")]
        AnIndependentProvider = 3,

        [Description("childrensCentre")]
        [DisplayDescription("A children's centre")]
        AChildrensCentre = 4
    }
}
