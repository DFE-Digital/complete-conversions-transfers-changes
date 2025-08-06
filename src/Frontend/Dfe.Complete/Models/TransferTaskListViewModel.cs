using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Models
{
    public class TransferTaskListViewModel
    {
        public TaskListStatus HandoverWithRegionalDeliveryOfficer { get; set; }
        public TaskListStatus ExternalStakeHolderKickoff { get; set; }
        public TaskListStatus ConfirmAcademyRiskProtectionArrangements { get; set; }
        public TaskListStatus ConfirmHeadTeacherDetails { get; set; }
        public TaskListStatus ConfirmIncomingTrustCeoDetails { get; set; }
        public TaskListStatus ConfirmOutgoingTrustCeoDetails { get; set; }
        public TaskListStatus ConfirmMainContact { get; set; }
        public TaskListStatus RequestNewURNAndRecordForTheAcademy { get; set; }
        public TaskListStatus ConfirmTransferGrantFundingLevel { get; set; }
        public TaskListStatus CheckAndConfirmAcademyAndTrustFinancialInfo { get; set; }
        public TaskListStatus FormM { get; set; }
        public TaskListStatus LandConsentLetter { get; set; }
        public TaskListStatus SupplementalFundingAgreement { get; set; }
        public TaskListStatus DeedOfNovationAndVariation { get; set; }
        public TaskListStatus ChurchSupplementalAgreement { get; set; }
        public TaskListStatus MasterFundingAgreement { get; set; }
        public TaskListStatus ArticlesOfAssociation { get; set; }
        public TaskListStatus DeedoOfVariation { get; set; }
        public TaskListStatus DeedOfTerminationForTheMasterFundingAgreement { get; set; }
        public TaskListStatus DeedOfTerminationForChurchSupplementalAreement { get; set; }
        public TaskListStatus CommercialTransferAgreement { get; set; }
        public TaskListStatus ClosureOrTransferDeclaration { get; set; }
        public TaskListStatus ConfirmBankDetailsChangingForGeneralAnnualGrantPayment { get; set; }
        public TaskListStatus ConfirmIncomingTrustHasCompletedAllActions { get; set; }
        public TaskListStatus ConfirmThisTransferHasAuthorityToProceed { get; set; }
        public TaskListStatus ConfirmDateAcademyTransferred { get; set; }
        public TaskListStatus RedactAndSendDocuments { get; set; }
        public TaskListStatus DeclarationOfExpenditureCertificate { get; set; }  

        public static TransferTaskListViewModel Create(TransferTaskDataDto taskData, ProjectDto project)
        {
            return new TransferTaskListViewModel
            {
                HandoverWithRegionalDeliveryOfficer = HandoverWithRegionalDeliveryOfficerTaskStatus(taskData),
                ExternalStakeHolderKickoff = ExternalStakeHolderKickoffTaskStatus(taskData),
                ConfirmAcademyRiskProtectionArrangements = ConfirmAcademyRiskProtectionArrangementsTaskStatus(taskData),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(taskData),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(taskData),
                ConfirmOutgoingTrustCeoDetails = ConfirmOutgoingTrustCeoDetailsTaskStatus(taskData),
                ConfirmMainContact = ConfirmMainContactTaskStatus(taskData),
                RequestNewURNAndRecordForTheAcademy = RequestNewURNAndRecordForTheAcademyTaskStatus(taskData),
                ConfirmTransferGrantFundingLevel = ConfirmTransferGrantFundingLevelTaskStatus(taskData),
                CheckAndConfirmAcademyAndTrustFinancialInfo = CheckAndConfirmAcademyAndTrustFinancialInfoTaskStatus(taskData),
                FormM = FormMTaskStatus(taskData),
                LandConsentLetter = LandConsentLetterTaskStatus(taskData),
                SupplementalFundingAgreement = SupplementalFundingAgreementTaskStatus(taskData),
                DeedOfNovationAndVariation = DeedOfNovationAndVariationTaskStatus(taskData),
                ChurchSupplementalAgreement = ChurchSupplementalAgreementTaskStatus(taskData),
                MasterFundingAgreement = MasterFundingAgreementTaskStatus(taskData),
                ArticlesOfAssociation = ArticlesOfAssociationTaskStatus(taskData),
                DeedoOfVariation = DeedoOfVariationTaskStatus(taskData),
                DeedOfTerminationForTheMasterFundingAgreement = DeedOfTerminationForTheMasterFundingAgreementTaskStatus(taskData),
                DeedOfTerminationForChurchSupplementalAreement = DeedOfTerminationForChurchSupplementalAreementTaskStatus(taskData),
                CommercialTransferAgreement = CommercialTransferAgreementTaskStatus(taskData),
                ClosureOrTransferDeclaration = ClosureOrTransferDeclarationTaskStatus(taskData),
                ConfirmBankDetailsChangingForGeneralAnnualGrantPayment = ConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTaskStatus(taskData),
                ConfirmIncomingTrustHasCompletedAllActions = ConfirmIncomingTrustHasCompletedAllActionsTaskStatus(taskData),
                ConfirmThisTransferHasAuthorityToProceed = ConfirmThisTransferHasAuthorityToProceedTaskStatus(taskData),
                ConfirmDateAcademyTransferred = ConfirmDateAcademyTransferredTaskStatus(taskData),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskData),
                DeclarationOfExpenditureCertificate = DeclarationOfExpenditureCertificateTaskStatus(taskData)
            };
        }

        public static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(TransferTaskDataDto taskData)
        {
            if (!taskData.HandoverReview.HasValue &&
                !taskData.HandoverMeeting.HasValue &&
                !taskData.HandoverNotes.HasValue &&
                !taskData.HandoverNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.HandoverNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            if (taskData.HandoverReview.Equals(true) &&
                taskData.HandoverMeeting.Equals(true) &&
                taskData.HandoverNotes.Equals(true))
            {
                return TaskListStatus.Completed;
            }

            return TaskListStatus.InProgress;
        }
        public static TaskListStatus ExternalStakeHolderKickoffTaskStatus(TransferTaskDataDto taskData)
        {
            // Assuming ExternalStakeHolderKickoff is not part of the taskData, returning NotStarted for now.
            // This can be updated based on actual requirements.
            return TaskListStatus.NotStarted;
        }
        private static TaskListStatus ConfirmOutgoingTrustCeoDetailsTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeclarationOfExpenditureCertificateTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmDateAcademyTransferredTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmThisTransferHasAuthorityToProceedTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustHasCompletedAllActionsTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ClosureOrTransferDeclarationTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfTerminationForChurchSupplementalAreementTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfTerminationForTheMasterFundingAgreementTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedoOfVariationTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus MasterFundingAgreementTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfNovationAndVariationTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus LandConsentLetterTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus FormMTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CheckAndConfirmAcademyAndTrustFinancialInfoTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmTransferGrantFundingLevelTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus RequestNewURNAndRecordForTheAcademyTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmMainContactTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustCeoDetailsTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmHeadTeacherDetailsTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(TransferTaskDataDto taskData)
        {
           return TaskListStatus.NotStarted;
        }
    }
}
