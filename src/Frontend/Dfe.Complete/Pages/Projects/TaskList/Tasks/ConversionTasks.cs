using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks
{
    public static class ConversionTasks
    {
        public static (List<TaskListItemViewModel> ProjectKickoffTasks, 
            List<TaskListItemViewModel> LegalDocumentsTasks, 
            List<TaskListItemViewModel> ReadyForOpeningTasks, 
            List<TaskListItemViewModel> AfterOpeningTasks) BuildTaskList(ConversionTaskListViewModel conversionTaskList, string projectId)
        {
            TaskLinkBuilder taskLinkBuilder = new TaskLinkBuilder(RouteConstants.ProjectTask, projectId);

            return (GetProjectKickoffTasks(conversionTaskList, taskLinkBuilder),
                GetLegalDocumentsTasks(conversionTaskList, taskLinkBuilder),
                GetReadyForOpeningTasks(conversionTaskList, taskLinkBuilder),
                GetAfterOpeningTasks(conversionTaskList, taskLinkBuilder));
        }

        private static List<TaskListItemViewModel> GetProjectKickoffTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var projectKickoffTasks = new List<TaskListItemViewModel>
            {
                new("Post decision actions", taskLinkBuilder.Build("post_decision_actions"), conversionTaskList.PostDecisionActions, 1),
                new("Handover with regional delivery officer", taskLinkBuilder.Build("handover"), conversionTaskList.HandoverWithRegionalDeliveryOfficer, 2),
                new("External stakeholder kick-off", taskLinkBuilder.Build("stakeholder_kick_off"), conversionTaskList.ExternalStakeHolderKickoff, 3),
                new("Confirm the academy's risk protection arrangements", taskLinkBuilder.Build("risk_protection_arrangement"), conversionTaskList.ConfirmAcademyRiskProtectionArrangements, 4),
                new("Check accuracy of high needs places information", taskLinkBuilder.Build("check_accuracy_of_higher_needs"), conversionTaskList.CheckAccuracyOfHigherNeeds, 5),
                new("Complete a notification of changes to funded high needs places form", taskLinkBuilder.Build("complete_notification_of_change"), conversionTaskList.CompleteNotificationOfChange, 7),
                new("Confirm statutory consultation is complete and any issues are being managed", taskLinkBuilder.Build("confirm_statutory_consultation"), conversionTaskList.ConfirmStatutoryConsultation, 8),
                new("Confirm and process the sponsored support grant", taskLinkBuilder.Build("sponsored_support_grant"), conversionTaskList.ConfirmAndProcessSponsoredSupportGrant, 9),
                new("Confirm academy nursery arrangement", taskLinkBuilder.Build("confirm_nursery_arrangement"), conversionTaskList.ConfirmNurseryArrangement, 10),
                new("TUPE Consultation", taskLinkBuilder.Build("tupe_consultation"), conversionTaskList.TupeConsultation, 11),
                new("Confirm the academy name", taskLinkBuilder.Build("academy_details"), conversionTaskList.ConfirmAcademyName, 12),
                new("Confirm the headteacher's details", taskLinkBuilder.Build("confirm_headteacher_contact"), conversionTaskList.ConfirmHeadTeacherDetails, 13),
                new("DBS checks", taskLinkBuilder.Build("confirm_dbs_checks"), conversionTaskList.ConfirmDbsChecks, 14),
                new("Confirm the chair of governors' details", taskLinkBuilder.Build("confirm_chair_of_governors_contact"), conversionTaskList.ConfirmChairOfGovernorsDetails, 15),
                new("Confirm the incoming trust CEO's details", taskLinkBuilder.Build("confirm_incoming_trust_ceo_contact"), conversionTaskList.ConfirmIncomingTrustCeoDetails, 16),
                new("Confirm the main contact", taskLinkBuilder.Build("main_contact"), conversionTaskList.ConfirmMainContact, 17),
                new("Confirm the proposed capacity of the academy", taskLinkBuilder.Build("proposed_capacity_of_the_academy"), conversionTaskList.ConfirmProposedCapacityOfTheAcademy, 18)
            };


            if (conversionTaskList.ShowProcessConversionSupportGrant)
            {
                projectKickoffTasks.Add(new TaskListItemViewModel("Process conversion support grant", taskLinkBuilder.Build("conversion_grant"), conversionTaskList.ProcessConversionSupportGrant, 6));
            }

            return projectKickoffTasks.OrderBy(x => x.DisplayOrder).ToList();

        }

        private static List<TaskListItemViewModel> GetLegalDocumentsTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var legalDocumentsTasks = new List<TaskListItemViewModel>
            {
                new("Land questionnaire", taskLinkBuilder.Build("land_questionnaire"), conversionTaskList.LandQuestionnaire, 1),
                new("Land registry title plans", taskLinkBuilder.Build("land_registry"), conversionTaskList.LandRegistry, 2),
                new("Supplemental funding agreement", taskLinkBuilder.Build("supplemental_funding_agreement"), conversionTaskList.SupplementalFundingAgreement, 3),
                new("Church supplemental agreement", taskLinkBuilder.Build("church_supplemental_agreement"), conversionTaskList.ChurchSupplementalAgreement, 4),
                new("Master funding agreement", taskLinkBuilder.Build("master_funding_agreement"), conversionTaskList.MasterFundingAgreement, 5),
                new("Articles of association", taskLinkBuilder.Build("articles_of_association"), conversionTaskList.ArticlesOfAssociation, 6),
                new("Deed of variation", taskLinkBuilder.Build("deed_of_variation"), conversionTaskList.DeedOfVariation, 7),
                new("Trust modification order", taskLinkBuilder.Build("trust_modification_order"), conversionTaskList.TrustModificationOrder, 8),
                new("Direction to transfer", taskLinkBuilder.Build("direction_to_transfer"), conversionTaskList.DirectionToTransfer, 9),
                new("125 year lease", taskLinkBuilder.Build("one_hundred_and_twenty_five_year_lease"), conversionTaskList.OneHundredAndTwentyFiveYearLease, 10),
                new("Subleases", taskLinkBuilder.Build("subleases"), conversionTaskList.Tubleases, 11),
                new("Tenancy at will", taskLinkBuilder.Build("tenancy_at_will"), conversionTaskList.TenancyAtWill, 12),
                new("Commercial transfer agreement", taskLinkBuilder.Build("commercial_transfer_agreement"), conversionTaskList.CommercialTransferAgreement, 13)
            };

            return legalDocumentsTasks;
        }

        private static List<TaskListItemViewModel> GetReadyForOpeningTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var readyForOpeningTasks = new List<TaskListItemViewModel>
            {
                new("Confirm the school has completed all actions", taskLinkBuilder.Build("school_completed"), conversionTaskList.ConfirmTheSchoolHasCompletedAllActions, 1),
                new("Confirm all conditions have been met", taskLinkBuilder.Build("conditions_met"), conversionTaskList.ConfirmAllConditionsHaveBeenMet, 2),
                new("Share the information about opening", taskLinkBuilder.Build("share_information"), conversionTaskList.ShareTheInformationAboutOpening, 3)
            };

            return readyForOpeningTasks.OrderBy(x => x.DisplayOrder).ToList();
        }

        private static List<TaskListItemViewModel> GetAfterOpeningTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var confirmDateAcademyOpenedTitle = NoteTaskIdentifier.ConfirmAcademyOpenedDate.ToDisplayDescription();

            var afterOpeningTasks = new List<TaskListItemViewModel>
            {
                new (confirmDateAcademyOpenedTitle, taskLinkBuilder.Build("confirm_date_academy_opened"), conversionTaskList.ConfirmDateAcademyOpened, 1),
                new ("Redact and send documents", taskLinkBuilder.Build("redact_and_send"), conversionTaskList.RedactAndSendDocuments, 2),
                new ("Receive declaration of expenditure certificate", taskLinkBuilder.Build("receive_grant_payment_certificate"), conversionTaskList.ProjectReceiveDeclarationOfExpenditureCertificate, 3)
            };

            return afterOpeningTasks.OrderBy(x => x.DisplayOrder).ToList();
        }
    }
}