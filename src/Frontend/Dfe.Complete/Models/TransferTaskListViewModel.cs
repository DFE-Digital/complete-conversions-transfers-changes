using Dfe.Complete.Application.Contacts.Models;
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

        public static TransferTaskListViewModel Create(TransferTaskDataDto taskData, ProjectDto project, KeyContactDto? keyContacts)
        {
            return (taskData == null) ? new() : new TransferTaskListViewModel
            {
                HandoverWithRegionalDeliveryOfficer = HandoverWithRegionalDeliveryOfficerTaskStatus(taskData),
                ExternalStakeHolderKickoff = ExternalStakeHolderKickoffTaskStatus(taskData, project),
                ConfirmAcademyRiskProtectionArrangements = ConfirmAcademyRiskProtectionArrangementsTaskStatus(taskData),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(keyContacts),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(keyContacts),
                ConfirmOutgoingTrustCeoDetails = ConfirmOutgoingTrustCeoDetailsTaskStatus(keyContacts),
                ConfirmMainContact = ConfirmMainContactTaskStatus(project),
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
                ConfirmThisTransferHasAuthorityToProceed = ConfirmThisTransferHasAuthorityToProceedTaskStatus(taskData, project),
                ConfirmDateAcademyTransferred = ConfirmDateAcademyTransferredTaskStatus(taskData),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskData),
                DeclarationOfExpenditureCertificate = DeclarationOfExpenditureCertificateTaskStatus(taskData)
            };
        }

        private static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(TransferTaskDataDto taskData)
        {
            if ((!taskData.HandoverReview.HasValue || taskData.HandoverReview == false) &&
                (!taskData.HandoverMeeting.HasValue || taskData.HandoverReview == false) &&
                (!taskData.HandoverNotes.HasValue || taskData.HandoverReview == false) &&
                (!taskData.HandoverNotApplicable.HasValue || taskData.HandoverNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.HandoverNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            return (taskData.HandoverReview.Equals(true) &&
                taskData.HandoverMeeting.Equals(true) &&
                taskData.HandoverNotes.Equals(true))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }
        private static TaskListStatus ExternalStakeHolderKickoffTaskStatus(TransferTaskDataDto taskData, ProjectDto project)
        {
            if (taskData.StakeholderKickOffIntroductoryEmails == true &&
                 taskData.StakeholderKickOffSetupMeeting == true &&
                 taskData.StakeholderKickOffMeeting == true &&
                 project.SignificantDateProvisional == false)
            {
                return TaskListStatus.Completed;
            }

            return (project.SignificantDateProvisional == false ||
                (taskData.StakeholderKickOffIntroductoryEmails == true ||
                   taskData.StakeholderKickOffSetupMeeting == true ||
                   taskData.StakeholderKickOffMeeting == true))
                 ? TaskListStatus.InProgress : TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeclarationOfExpenditureCertificateTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.DeclarationOfExpenditureCertificateCorrect.HasValue || taskData.DeclarationOfExpenditureCertificateCorrect == false) &&
               (!taskData.DeclarationOfExpenditureCertificateSaved.HasValue || taskData.DeclarationOfExpenditureCertificateSaved == false) &&
               (!taskData.DeclarationOfExpenditureCertificateDateReceived.HasValue) &&
               (!taskData.DeclarationOfExpenditureCertificateNotApplicable.HasValue ||
               taskData.DeclarationOfExpenditureCertificateNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.DeclarationOfExpenditureCertificateNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.DeclarationOfExpenditureCertificateCorrect == true &&
               taskData.DeclarationOfExpenditureCertificateSaved == true &&
               taskData.DeclarationOfExpenditureCertificateDateReceived.HasValue)
               ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.RedactAndSendDocumentsRedact.HasValue || taskData.RedactAndSendDocumentsRedact == false) &&
                (!taskData.RedactAndSendDocumentsSaved.HasValue || taskData.RedactAndSendDocumentsSaved == false) &&
                (!taskData.RedactAndSendDocumentsSendToEsfa.HasValue || taskData.RedactAndSendDocumentsSendToEsfa == false) &&
                (!taskData.RedactAndSendDocumentsSendToFundingTeam.HasValue || taskData.RedactAndSendDocumentsSendToFundingTeam == false) &&
                (!taskData.RedactAndSendDocumentsSendToSolicitors.HasValue || taskData.RedactAndSendDocumentsSendToSolicitors == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.RedactAndSendDocumentsRedact == true &&
               taskData.RedactAndSendDocumentsSaved == true &&
               taskData.RedactAndSendDocumentsSendToEsfa == true &&
               taskData.RedactAndSendDocumentsSendToFundingTeam == true &&
               taskData.RedactAndSendDocumentsSendToSolicitors == true)
               ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmDateAcademyTransferredTaskStatus(TransferTaskDataDto taskData)
        {
            return taskData.ConfirmDateAcademyTransferredDateTransferred.HasValue
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmThisTransferHasAuthorityToProceedTaskStatus(TransferTaskDataDto taskData, ProjectDto project)
        {
            if (taskData.ConditionsMetBaselineSheetApproved == true &&
                 taskData.ConditionsMetCheckAnyInformationChanged == true &&
                 project.AllConditionsMet == false)
            {
                return TaskListStatus.Completed;
            }
               
            return (project.AllConditionsMet == true ||
                (taskData.ConditionsMetBaselineSheetApproved == true ||
                taskData.ConditionsMetCheckAnyInformationChanged == true))
                 ? TaskListStatus.InProgress : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustHasCompletedAllActionsTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.ConfirmIncomingTrustHasCompletedAllActionsEmailed.HasValue || taskData.ConfirmIncomingTrustHasCompletedAllActionsEmailed == false) &&
               (!taskData.ConfirmIncomingTrustHasCompletedAllActionsSaved.HasValue || taskData.ConfirmIncomingTrustHasCompletedAllActionsSaved == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.ConfirmIncomingTrustHasCompletedAllActionsEmailed == true &&
                taskData.ConfirmIncomingTrustHasCompletedAllActionsSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTaskStatus(TransferTaskDataDto taskData)
        {
            return taskData.BankDetailsChangingYesNo.HasValue
               ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ClosureOrTransferDeclarationTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.ClosureOrTransferDeclarationReceived.HasValue || taskData.ClosureOrTransferDeclarationReceived == false) &&
               (!taskData.ClosureOrTransferDeclarationCleared.HasValue || taskData.ClosureOrTransferDeclarationCleared == false) &&
               (!taskData.ClosureOrTransferDeclarationSent.HasValue || taskData.ClosureOrTransferDeclarationSent == false) &&
               (!taskData.ClosureOrTransferDeclarationSaved.HasValue || taskData.ClosureOrTransferDeclarationSaved == false) &&
               (!taskData.ClosureOrTransferDeclarationNotApplicable.HasValue ||
               taskData.ClosureOrTransferDeclarationNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.ClosureOrTransferDeclarationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ClosureOrTransferDeclarationReceived == true &&
               taskData.ClosureOrTransferDeclarationCleared == true &&
               taskData.ClosureOrTransferDeclarationSent == true &&
               taskData.ClosureOrTransferDeclarationSaved == true)
               ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.CommercialTransferAgreementConfirmAgreed.HasValue || taskData.CommercialTransferAgreementConfirmAgreed == false) &&
               (!taskData.CommercialTransferAgreementConfirmSigned.HasValue || taskData.CommercialTransferAgreementConfirmSigned == false)  &&
               (!taskData.CommercialTransferAgreementQuestionsChecked.HasValue || taskData.CommercialTransferAgreementQuestionsChecked == false) &&
               (!taskData.CommercialTransferAgreementQuestionsReceived.HasValue || taskData.CommercialTransferAgreementQuestionsReceived == false) &&
               (!taskData.CommercialTransferAgreementSaveConfirmationEmails.HasValue || taskData.CommercialTransferAgreementSaveConfirmationEmails == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.CommercialTransferAgreementConfirmAgreed == true &&
               taskData.CommercialTransferAgreementConfirmSigned == true &&
               taskData.CommercialTransferAgreementQuestionsChecked == true &&
               taskData.CommercialTransferAgreementQuestionsReceived == true &&
               taskData.CommercialTransferAgreementSaveConfirmationEmails == true) 
               ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus DeedOfTerminationForChurchSupplementalAreementTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.DeedTerminationChurchAgreementCleared.HasValue || taskData.DeedTerminationChurchAgreementCleared == false) &&
               (!taskData.DeedTerminationChurchAgreementNotApplicable.HasValue || taskData.DeedTerminationChurchAgreementNotApplicable == false) &&
               (!taskData.DeedTerminationChurchAgreementReceived.HasValue || taskData.DeedTerminationChurchAgreementReceived == false) &&
               (!taskData.DeedTerminationChurchAgreementSaved.HasValue || taskData.DeedTerminationChurchAgreementSaved == false) &&
               (!taskData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState.HasValue || taskData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState == false) &&
               (!taskData.DeedTerminationChurchAgreementSignedDiocese.HasValue || taskData.DeedTerminationChurchAgreementSignedDiocese == false)&&
               (!taskData.DeedTerminationChurchAgreementSignedOutgoingTrust.HasValue || taskData.DeedTerminationChurchAgreementSignedOutgoingTrust == false) &&
               (!taskData.DeedTerminationChurchAgreementSignedSecretaryState.HasValue || taskData.DeedTerminationChurchAgreementSignedSecretaryState == false))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.DeedTerminationChurchAgreementNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.DeedTerminationChurchAgreementReceived == true &&
               taskData.DeedTerminationChurchAgreementCleared == true &&
               taskData.DeedTerminationChurchAgreementSignedOutgoingTrust == true &&
               taskData.DeedTerminationChurchAgreementSignedDiocese == true &&
               taskData.DeedTerminationChurchAgreementSaved == true &&
               taskData.DeedTerminationChurchAgreementSignedSecretaryState == true &&
               taskData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus DeedOfTerminationForTheMasterFundingAgreementTaskStatus(TransferTaskDataDto taskData)
        {
            if ((!taskData.DeedOfTerminationForTheMasterFundingAgreementCleared.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementCleared == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementNotApplicable.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementNotApplicable == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementReceived.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementReceived == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementSigned.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementSigned == false) &&
               (!taskData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState.HasValue || taskData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.DeedOfTerminationForTheMasterFundingAgreementNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.DeedOfTerminationForTheMasterFundingAgreementReceived == true &&
                taskData.DeedOfTerminationForTheMasterFundingAgreementCleared == true &&
                taskData.DeedOfTerminationForTheMasterFundingAgreementSigned == true &&
                taskData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint == true &&
                taskData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam == true &&
                taskData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState == true &&
                taskData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder == true)
                    ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus DeedoOfVariationTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.DeedOfVariationCleared.HasValue || taskData.DeedOfVariationCleared == false) &&
               (!taskData.DeedOfVariationNotApplicable.HasValue || taskData.DeedOfVariationNotApplicable == false) &&
               (!taskData.DeedOfVariationReceived.HasValue || taskData.DeedOfVariationReceived == false) &&
               (!taskData.DeedOfVariationSaved.HasValue || taskData.DeedOfVariationSaved == false) &&
               (!taskData.DeedOfVariationSent.HasValue || taskData.DeedOfVariationSent == false) &&
               (!taskData.DeedOfVariationSigned.HasValue || taskData.DeedOfVariationSigned == false) &&
               (!taskData.DeedOfVariationSignedSecretaryState.HasValue || taskData.DeedOfVariationSignedSecretaryState == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.DeedOfVariationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.DeedOfVariationReceived == true &&
                taskData.DeedOfVariationCleared == true &&
                taskData.DeedOfVariationSigned == true &&
                taskData.DeedOfVariationSaved == true &&
                taskData.DeedOfVariationSent == true &&
                taskData.DeedOfVariationSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.ArticlesOfAssociationReceived.HasValue || taskData.ArticlesOfAssociationReceived == false) &&
                (!taskData.ArticlesOfAssociationCleared.HasValue || taskData.ArticlesOfAssociationCleared == false)&&
                (!taskData.ArticlesOfAssociationSigned.HasValue || taskData.ArticlesOfAssociationSigned == false)&&
                (!taskData.ArticlesOfAssociationSaved.HasValue || taskData.ArticlesOfAssociationSaved == false)&&
                (!taskData.ArticlesOfAssociationNotApplicable.HasValue || taskData.ArticlesOfAssociationNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.ArticlesOfAssociationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ArticlesOfAssociationReceived == true &&
                taskData.ArticlesOfAssociationCleared == true &&
                taskData.ArticlesOfAssociationSigned == true &&
                taskData.ArticlesOfAssociationSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus MasterFundingAgreementTaskStatus(TransferTaskDataDto taskData)
        {
            if ((!taskData.MasterFundingAgreementReceived.HasValue || taskData.MasterFundingAgreementReceived == false) &&
                (!taskData.MasterFundingAgreementCleared.HasValue || taskData.MasterFundingAgreementCleared == false) &&
                (!taskData.MasterFundingAgreementSigned.HasValue || taskData.MasterFundingAgreementSigned == false) &&
                (!taskData.MasterFundingAgreementSaved.HasValue || taskData.MasterFundingAgreementSaved == false) &&
                (!taskData.MasterFundingAgreementSignedSecretaryState.HasValue || taskData.MasterFundingAgreementSignedSecretaryState == false) &&
                (!taskData.MasterFundingAgreementNotApplicable.HasValue || taskData.MasterFundingAgreementNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.MasterFundingAgreementNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.MasterFundingAgreementReceived == true &&
                taskData.MasterFundingAgreementCleared == true &&
                taskData.MasterFundingAgreementSigned == true &&
                taskData.MasterFundingAgreementSaved == true &&
                taskData.MasterFundingAgreementSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(TransferTaskDataDto taskData)
        {
           if((!taskData.ChurchSupplementalAgreementCleared.HasValue || taskData.ChurchSupplementalAgreementCleared == false) &&
                (!taskData.ChurchSupplementalAgreementNotApplicable.HasValue || taskData.ChurchSupplementalAgreementNotApplicable == false) &&
                (!taskData.ChurchSupplementalAgreementReceived.HasValue || taskData.ChurchSupplementalAgreementReceived == false) &&
                (!taskData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState.HasValue || taskData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState == false) &&
                (!taskData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese.HasValue || taskData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese == false)&&
                (!taskData.ChurchSupplementalAgreementSignedDiocese.HasValue || taskData.ChurchSupplementalAgreementSignedDiocese == false) &&
                (!taskData.ChurchSupplementalAgreementSignedIncomingTrust.HasValue || taskData.ChurchSupplementalAgreementSignedIncomingTrust == false) &&
                (!taskData.ChurchSupplementalAgreementSignedSecretaryState.HasValue || taskData.ChurchSupplementalAgreementSignedSecretaryState == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ChurchSupplementalAgreementNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ChurchSupplementalAgreementReceived == true &&
                taskData.ChurchSupplementalAgreementCleared == true &&
                taskData.ChurchSupplementalAgreementSignedIncomingTrust == true &&
                taskData.ChurchSupplementalAgreementSignedDiocese == true &&
                taskData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese == true &&
                taskData.ChurchSupplementalAgreementSignedSecretaryState == true &&
                taskData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus DeedOfNovationAndVariationTaskStatus(TransferTaskDataDto taskData)
        {
           if((!taskData.DeedOfNovationAndVariationReceived.HasValue || taskData.DeedOfNovationAndVariationReceived == false) &&
                (!taskData.DeedOfNovationAndVariationCleared.HasValue || taskData.DeedOfNovationAndVariationCleared == false) &&
                (!taskData.DeedOfNovationAndVariationSignedOutgoingTrust.HasValue || taskData.DeedOfNovationAndVariationSignedOutgoingTrust == false) &&
                (!taskData.DeedOfNovationAndVariationSignedIncomingTrust.HasValue || taskData.DeedOfNovationAndVariationSignedIncomingTrust == false) &&
                (!taskData.DeedOfNovationAndVariationSaved.HasValue || taskData.DeedOfNovationAndVariationSaved == false) &&
                (!taskData.DeedOfNovationAndVariationSignedSecretaryState.HasValue || taskData.DeedOfNovationAndVariationSignedSecretaryState == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.DeedOfNovationAndVariationReceived == true &&
                taskData.DeedOfNovationAndVariationCleared == true &&
                taskData.DeedOfNovationAndVariationSignedOutgoingTrust == true &&
                taskData.DeedOfNovationAndVariationSignedIncomingTrust == true &&
                taskData.DeedOfNovationAndVariationSaved == true &&
                taskData.DeedOfNovationAndVariationSignedSecretaryState == true &&
                taskData.DeedOfNovationAndVariationSaveAfterSign == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(TransferTaskDataDto taskData)
        {
            if ((!taskData.SupplementalFundingAgreementReceived.HasValue || taskData.SupplementalFundingAgreementReceived == false) &&
                (!taskData.SupplementalFundingAgreementCleared.HasValue || taskData.SupplementalFundingAgreementCleared == false) &&
                (!taskData.SupplementalFundingAgreementSaved.HasValue || taskData.SupplementalFundingAgreementSaved == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.SupplementalFundingAgreementReceived == true &&
                taskData.SupplementalFundingAgreementCleared == true &&
                taskData.SupplementalFundingAgreementSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus LandConsentLetterTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.LandConsentLetterDrafted.HasValue || taskData.LandConsentLetterDrafted == false) &&
                (!taskData.LandConsentLetterSent.HasValue || taskData.LandConsentLetterSent == false) &&
                (!taskData.LandConsentLetterSigned.HasValue || taskData.LandConsentLetterSigned == false) &&
                (!taskData.LandConsentLetterSaved.HasValue || taskData.LandConsentLetterSaved == false) &&
                (!taskData.LandConsentLetterNotApplicable.HasValue || taskData.LandConsentLetterNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.LandConsentLetterNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.LandConsentLetterDrafted == true &&
                taskData.LandConsentLetterSent == true &&
                taskData.LandConsentLetterSigned == true &&
                taskData.LandConsentLetterSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus FormMTaskStatus(TransferTaskDataDto taskData)
        {
            if ((!taskData.FormMReceivedTitlePlans.HasValue || taskData.FormMReceivedTitlePlans == false) &&
                (!taskData.FormMCleared.HasValue || taskData.FormMCleared == false) &&
                (!taskData.FormMSigned.HasValue || taskData.FormMSigned == false) &&
                (!taskData.FormMSaved.HasValue || taskData.FormMSaved == false) &&
                (!taskData.FormMNotApplicable.HasValue || taskData.FormMNotApplicable == false) &&
                (!taskData.FormMReceivedFormM.HasValue || taskData.FormMReceivedFormM == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.FormMNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.FormMReceivedTitlePlans == true &&
                taskData.FormMCleared == true &&
                taskData.FormMSigned == true &&
                taskData.FormMSaved == true &&
                taskData.FormMReceivedFormM == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus CheckAndConfirmAcademyAndTrustFinancialInfoTaskStatus(TransferTaskDataDto taskData)
        {
            if(string.IsNullOrWhiteSpace(taskData.CheckAndConfirmFinancialInformationAcademySurplusDeficit) &&
                (!taskData.CheckAndConfirmFinancialInformationNotApplicable.HasValue ||
                taskData.CheckAndConfirmFinancialInformationNotApplicable == false) &&
                string.IsNullOrWhiteSpace(taskData.CheckAndConfirmFinancialInformationTrustSurplusDeficit))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.CheckAndConfirmFinancialInformationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            } 
            return(!string.IsNullOrWhiteSpace(taskData.CheckAndConfirmFinancialInformationAcademySurplusDeficit) &&
                !string.IsNullOrWhiteSpace(taskData.CheckAndConfirmFinancialInformationTrustSurplusDeficit))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmTransferGrantFundingLevelTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.SponsoredSupportGrantNotApplicable.HasValue || taskData.SponsoredSupportGrantNotApplicable == false) &&
                string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.SponsoredSupportGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (!string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType) )
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus RequestNewURNAndRecordForTheAcademyTaskStatus(TransferTaskDataDto taskData)
        {
            if((!taskData.RequestNewUrnAndRecordComplete.HasValue || taskData.RequestNewUrnAndRecordComplete == false) &&
                (!taskData.RequestNewUrnAndRecordGive.HasValue || taskData.RequestNewUrnAndRecordGive == false) &&
                (!taskData.RequestNewUrnAndRecordNotApplicable.HasValue || taskData.RequestNewUrnAndRecordNotApplicable == false) &&
                (!taskData.RequestNewUrnAndRecordReceive.HasValue || taskData.RequestNewUrnAndRecordReceive == false))
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.RequestNewUrnAndRecordNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.RequestNewUrnAndRecordComplete == true &&
                taskData.RequestNewUrnAndRecordGive == true &&
                taskData.RequestNewUrnAndRecordReceive == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmOutgoingTrustCeoDetailsTaskStatus(KeyContactDto? keyContacts)
        {
            return keyContacts?.OutgoingTrustCeoId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }
        private static TaskListStatus ConfirmIncomingTrustCeoDetailsTaskStatus(KeyContactDto? keyContacts)
        {
            return keyContacts?.IncomingTrustCeoId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }
        private static TaskListStatus ConfirmMainContactTaskStatus(ProjectDto project)
        {
            return project.MainContactId != null
               ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmHeadTeacherDetailsTaskStatus(KeyContactDto? keyContacts)
        {
            return keyContacts?.HeadteacherId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(TransferTaskDataDto taskData)
        {
            return (taskData.RpaPolicyConfirm == true) 
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }
    }
}
