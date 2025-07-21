using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum TaskIdentifier
    {
        [Description("handover")]
        Handover = 0,
        [Description("stakeholder_kick_off")]
        StakeholderKickOff = 1,
        [Description("risk_protection_arrangement")]
        RiskProtectionArrangement = 2,
        [Description("check_accuracy_of_higher_needs")]
        CheckAccuracyOfHigherNeeds = 3,
        [Description("complete_notification_of_change")]
        CompleteNotificationOfChange = 4,
        [Description("conversion_grant")]
        ConversionGrant = 5,
        [Description("sponsored_support_grant")]
        SponsoredSupportGrant = 6,
        [Description("academy_details")]
        AcademyDetails = 7,
        [Description("confirm_headteacher_contact")]
        ConfirmHeadteacherContact = 8,
        [Description("confirm_chair_of_governors_contact")]
        ConfirmChairOfGovernorsContact = 9,
        [Description("main_contact")]
        MainContact = 10,
        [Description("proposed_capacity_of_the_academy")]
        ProposedCapacityOfTheAcademy = 11,
    }
}
