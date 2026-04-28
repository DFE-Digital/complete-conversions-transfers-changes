namespace Dfe.Complete.Pages.Projects.TaskList
{
    using Dfe.Complete.Constants;
    using Dfe.Complete.Domain.Enums;
    using Dfe.Complete.Models;
    using Dfe.Complete.Utils;

    public static class TransferTasks
    {
        public static (List<TaskListItemViewModel> ProjectKickoffTasks, 
            List<TaskListItemViewModel> ReadyToTransferTasks,
            List<TaskListItemViewModel> LegalDocumentsTasks, 
            List<TaskListItemViewModel> AfterTransferTasks) BuildTaskList(TransferTaskListViewModel transferTaskList, string projectId)
        {
            TaskLinkBuilder taskLinkBuilder = new TaskLinkBuilder(RouteConstants.ProjectTask, projectId);

            return (GetProjectKickoffTasks(transferTaskList, taskLinkBuilder),
                GetReadyToTransferTasks(transferTaskList, taskLinkBuilder),
                GetLegalDocumentsTasks(transferTaskList, taskLinkBuilder),
                GetAfterTransferTasks(transferTaskList, taskLinkBuilder));
        }

        private static List<TaskListItemViewModel> GetProjectKickoffTasks(TransferTaskListViewModel transferTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var projectKickoffTasks = new List<TaskListItemViewModel>
            {
                new("Handover with regional delivery officer", taskLinkBuilder.Build("handover"), transferTaskList.HandoverWithRegionalDeliveryOfficer, 1),
                new("External stakeholder kick-off", taskLinkBuilder.Build("stakeholder_kick_off"), transferTaskList.ExternalStakeHolderKickoff, 2),
                new("Confirm the academy's risk protection arrangements", taskLinkBuilder.Build("rpa_policy"), transferTaskList.ConfirmAcademyRiskProtectionArrangements, 3),
                new("Confirm the headteacher's details", taskLinkBuilder.Build("confirm_headteacher_contact"), transferTaskList.ConfirmHeadTeacherDetails, 4),
                new("Confirm the incoming trust CEO's details", taskLinkBuilder.Build("confirm_incoming_trust_ceo_contact"), transferTaskList.ConfirmIncomingTrustCeoDetails, 5),
                new("Confirm the outgoing trust CEO's details", taskLinkBuilder.Build("confirm_outgoing_trust_ceo_contact"), transferTaskList.ConfirmOutgoingTrustCeoDetails, 6),
                new("Confirm the main contact", taskLinkBuilder.Build("main_contact"), transferTaskList.ConfirmMainContact, 7),
                new("Request a new URN and record for the academy", taskLinkBuilder.Build("request_new_urn_and_record"), transferTaskList.RequestNewURNAndRecordForTheAcademy, 8),
                new("Confirm transfer grant funding level", taskLinkBuilder.Build("sponsored_support_grant"), transferTaskList.ConfirmTransferGrantFundingLevel, 9),
                new("Check and confirm academy and trust financial information", taskLinkBuilder.Build("check_and_confirm_financial_information"), transferTaskList.CheckAndConfirmAcademyAndTrustFinancialInfo, 10)
            };

            return projectKickoffTasks.OrderBy(x => x.DisplayOrder).ToList();
        }

        private static List<TaskListItemViewModel> GetReadyToTransferTasks(TransferTaskListViewModel transferTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var getReadyToTransferTasks = new List<TaskListItemViewModel>
            {
                new("Confirm if the bank details for the general annual grant payment need to change", taskLinkBuilder.Build("bank_details_changing"), transferTaskList.ConfirmBankDetailsChangingForGeneralAnnualGrantPayment, 1),
                new("Confirm the incoming trust has completed all actions", taskLinkBuilder.Build("confirm_incoming_trust_has_completed_all_actions"), transferTaskList.ConfirmIncomingTrustHasCompletedAllActions, 2),
                new("Confirm this transfer has authority to proceed", taskLinkBuilder.Build("conditions_met"), transferTaskList.ConfirmThisTransferHasAuthorityToProceed, 3)
            };

            return getReadyToTransferTasks.OrderBy(x => x.DisplayOrder).ToList();
        }

        private static List<TaskListItemViewModel> GetLegalDocumentsTasks(TransferTaskListViewModel transferTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var legalDocumentsTasks = new List<TaskListItemViewModel>
            {
                new("Form M", taskLinkBuilder.Build("form_m"), transferTaskList.FormM, 1),
                new("Land consent letter", taskLinkBuilder.Build("land_consent_letter"), transferTaskList.LandConsentLetter, 2),
                new("Supplemental funding agreement", taskLinkBuilder.Build("supplemental_funding_agreement"), transferTaskList.SupplementalFundingAgreement, 3),
                new("Deed of novation and variation", taskLinkBuilder.Build("deed_of_novation_and_variation"), transferTaskList.DeedOfNovationAndVariation, 4),
                new("Church supplemental agreement", taskLinkBuilder.Build("church_supplemental_agreement"), transferTaskList.ChurchSupplementalAgreement, 5),
                new("Master funding agreement", taskLinkBuilder.Build("master_funding_agreement"), transferTaskList.MasterFundingAgreement, 6),
                new("Articles of association", taskLinkBuilder.Build("articles_of_association"), transferTaskList.ArticlesOfAssociation, 7),
                new("Deed of variation", taskLinkBuilder.Build("deed_of_variation"), transferTaskList.DeedoOfVariation, 8),
                new("Deed of termination for the master funding agreement", taskLinkBuilder.Build("deed_of_termination_for_the_master_funding_agreement"), transferTaskList.DeedOfTerminationForTheMasterFundingAgreement, 9),
                new("Deed of termination for the church supplemental agreement", taskLinkBuilder.Build("deed_termination_church_agreement"), transferTaskList.DeedOfTerminationForChurchSupplementalAreement, 10),
                new("Commercial transfer agreement", taskLinkBuilder.Build("commercial_transfer_agreement"), transferTaskList.CommercialTransferAgreement, 11),
                new("Closure or transfer declaration", taskLinkBuilder.Build("closure_or_transfer_declaration"), transferTaskList.ClosureOrTransferDeclaration, 12)
            };

            return legalDocumentsTasks.OrderBy(x => x.DisplayOrder).ToList();
        }

        private static List<TaskListItemViewModel> GetAfterTransferTasks(TransferTaskListViewModel transferTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var confirmDateAcademyTransferredTitle = NoteTaskIdentifier.ConfirmDateAcademyTransferred.ToDisplayDescription();

            var afterTransferTasks = new List<TaskListItemViewModel>
            {
                new(confirmDateAcademyTransferredTitle, taskLinkBuilder.Build("confirm_date_academy_transferred"), transferTaskList.ConfirmDateAcademyTransferred, 1),
                new("Redact and send documents", taskLinkBuilder.Build("redact_and_send_documents"), transferTaskList.RedactAndSendDocuments, 2),
                new("Receive declaration of expenditure certificate", taskLinkBuilder.Build("declaration_of_expenditure_certificate"), transferTaskList.DeclarationOfExpenditureCertificate, 3)
            };

            return afterTransferTasks.OrderBy(x => x.DisplayOrder).ToList();
        }
    }
}
