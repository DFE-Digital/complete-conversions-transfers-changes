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

        public static TransferTaskListViewModel Create(TransferTaskDataModel taskDataModel)
        {
            return new TransferTaskListViewModel
            {
                HandoverWithRegionalDeliveryOfficer = HandoverWithRegionalDeliveryOfficerTaskStatus(taskDataModel),
                ExternalStakeHolderKickoff = ExternalStakeHolderKickoffTaskStatus(taskDataModel),
                ConfirmAcademyRiskProtectionArrangements = ConfirmAcademyRiskProtectionArrangementsTaskStatus(taskDataModel),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(taskDataModel),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(taskDataModel),
                ConfirmOutgoingTrustCeoDetails = ConfirmOutgoingTrustCeoDetailsTaskStatus(taskDataModel),
                ConfirmMainContact = ConfirmMainContactTaskStatus(taskDataModel),
                RequestNewURNAndRecordForTheAcademy = RequestNewURNAndRecordForTheAcademyTaskStatus(taskDataModel),
                ConfirmTransferGrantFundingLevel = ConfirmTransferGrantFundingLevelTaskStatus(taskDataModel),
                CheckAndConfirmAcademyAndTrustFinancialInfo = CheckAndConfirmAcademyAndTrustFinancialInfoTaskStatus(taskDataModel),
                FormM = FormMTaskStatus(taskDataModel),
                LandConsentLetter = LandConsentLetterTaskStatus(taskDataModel),
                SupplementalFundingAgreement = SupplementalFundingAgreementTaskStatus(taskDataModel),
                DeedOfNovationAndVariation = DeedOfNovationAndVariationTaskStatus(taskDataModel),
                ChurchSupplementalAgreement = ChurchSupplementalAgreementTaskStatus(taskDataModel),
                MasterFundingAgreement = MasterFundingAgreementTaskStatus(taskDataModel),
                ArticlesOfAssociation = ArticlesOfAssociationTaskStatus(taskDataModel),
                DeedoOfVariation = DeedoOfVariationTaskStatus(taskDataModel),
                DeedOfTerminationForTheMasterFundingAgreement = DeedOfTerminationForTheMasterFundingAgreementTaskStatus(taskDataModel),
                DeedOfTerminationForChurchSupplementalAreement = DeedOfTerminationForChurchSupplementalAreementTaskStatus(taskDataModel),
                CommercialTransferAgreement = CommercialTransferAgreementTaskStatus(taskDataModel),
                ClosureOrTransferDeclaration = ClosureOrTransferDeclarationTaskStatus(taskDataModel),
                ConfirmBankDetailsChangingForGeneralAnnualGrantPayment = ConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTaskStatus(taskDataModel),
                ConfirmIncomingTrustHasCompletedAllActions = ConfirmIncomingTrustHasCompletedAllActionsTaskStatus(taskDataModel),
                ConfirmThisTransferHasAuthorityToProceed = ConfirmThisTransferHasAuthorityToProceedTaskStatus(taskDataModel),
                ConfirmDateAcademyTransferred = ConfirmDateAcademyTransferredTaskStatus(taskDataModel),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskDataModel),
                DeclarationOfExpenditureCertificate = DeclarationOfExpenditureCertificateTaskStatus(taskDataModel)
            };
        }

        public static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(TransferTaskDataModel taskDataModel)
        {
            if (!taskDataModel.HandoverReview.HasValue &&
                !taskDataModel.HandoverMeeting.HasValue &&
                !taskDataModel.HandoverNotes.HasValue &&
                !taskDataModel.HandoverNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if (taskDataModel.HandoverNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            if (taskDataModel.HandoverReview.Equals(true) &&
                taskDataModel.HandoverMeeting.Equals(true) &&
                taskDataModel.HandoverNotes.Equals(true))
            {
                return TaskListStatus.Completed;
            }

            return TaskListStatus.InProgress;
        }
        public static TaskListStatus ExternalStakeHolderKickoffTaskStatus(TransferTaskDataModel taskDataModel)
        {
            // Assuming ExternalStakeHolderKickoff is not part of the taskDataModel, returning NotStarted for now.
            // This can be updated based on actual requirements.
            return TaskListStatus.NotStarted;
        }
        private static TaskListStatus ConfirmOutgoingTrustCeoDetailsTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeclarationOfExpenditureCertificateTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmDateAcademyTransferredTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmThisTransferHasAuthorityToProceedTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustHasCompletedAllActionsTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ClosureOrTransferDeclarationTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfTerminationForChurchSupplementalAreementTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfTerminationForTheMasterFundingAgreementTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedoOfVariationTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus MasterFundingAgreementTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfNovationAndVariationTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus LandConsentLetterTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus FormMTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CheckAndConfirmAcademyAndTrustFinancialInfoTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmTransferGrantFundingLevelTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus RequestNewURNAndRecordForTheAcademyTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmMainContactTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustCeoDetailsTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmHeadTeacherDetailsTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(TransferTaskDataModel taskDataModel)
        {
           return TaskListStatus.NotStarted;
        }
    }
}
