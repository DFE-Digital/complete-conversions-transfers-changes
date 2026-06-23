using Dfe.Complete.Application.KeyContacts.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Models
{
    public class ConversionTaskListViewModel
    {
        public TaskListStatus HandoverWithRegionalDeliveryOfficer { get; set; }
        public TaskListStatus ExternalStakeHolderKickoff { get; set; }
        public TaskListStatus ConfirmAcademyRiskProtectionArrangements { get; set; }
        public TaskListStatus CheckAccuracyOfHigherNeeds { get; set; }
        public TaskListStatus CompleteNotificationOfChange { get; set; }
        public TaskListStatus ProcessConversionSupportGrant { get; set; }
        public TaskListStatus ConfirmAndProcessSponsoredSupportGrant { get; set; }
        public TaskListStatus TupeConsultation { get; set; }
        public TaskListStatus ConfirmAcademyName { get; set; }
        public TaskListStatus ConfirmHeadTeacherDetails { get; set; }
        public TaskListStatus ConfirmChairOfGovernorsDetails { get; set; }
        public TaskListStatus ConfirmIncomingTrustCeoDetails { get; set; }
        public TaskListStatus ConfirmMainContact { get; set; }
        public TaskListStatus ConfirmProposedCapacityOfTheAcademy { get; set; }
        public TaskListStatus LAConfirmsPayrollDeadline { get; set; }
        public TaskListStatus PrivateFinanceInitiative { get; set; }
        public TaskListStatus LandQuestionnaire { get; set; }
        public TaskListStatus LandRegistry { get; set; }
        public TaskListStatus SupplementalFundingAgreement { get; set; }
        public TaskListStatus ChurchSupplementalAgreement { get; set; }
        public TaskListStatus MasterFundingAgreement { get; set; }
        public TaskListStatus ArticlesOfAssociation { get; set; }
        public TaskListStatus DeedOfVariation { get; set; }
        public TaskListStatus TrustModificationOrder { get; set; }
        public TaskListStatus DirectionToTransfer { get; set; }
        public TaskListStatus OneHundredAndTwentyFiveYearLease { get; set; }
        public TaskListStatus Tubleases { get; set; }
        public TaskListStatus TenancyAtWill { get; set; }
        public TaskListStatus ThirdPartyLeases { get; set; }
        public TaskListStatus CommercialTransferAgreement { get; set; }
        public TaskListStatus ConfirmTheSchoolHasCompletedAllActions { get; set; }
        public TaskListStatus ConfirmSchoolBankDetails { get; set; }
        public TaskListStatus ConfirmAllConditionsHaveBeenMet { get; set; }
        public TaskListStatus ShareTheInformationAboutOpening { get; set; }
        public TaskListStatus ConfirmDateAcademyOpened { get; set; }
        public TaskListStatus RedactAndSendDocuments { get; set; }
        public TaskListStatus ProjectReceiveDeclarationOfExpenditureCertificate { get; set; }
        public TaskListStatus ConfirmStatutoryConsultation { get; set; }
        public TaskListStatus ConfirmNurseryArrangement { get; set; }
        public TaskListStatus PostDecisionActions { get; set; }
        public bool ShowProcessConversionSupportGrant { get; set; }
        public TaskListStatus ConfirmDbsChecks { get; set; }

        public static ConversionTaskListViewModel Create(ConversionTaskDataDto taskData, ProjectDto project, KeyContactDto? keyContacts)
        {
            return (taskData == null) ? new() : new ConversionTaskListViewModel
            {
                HandoverWithRegionalDeliveryOfficer = HandoverWithRegionalDeliveryOfficerTaskStatus(taskData),
                ExternalStakeHolderKickoff = ExternalStakeHolderKickoffTaskStatus(taskData, project),
                ConfirmAcademyRiskProtectionArrangements = ConfirmAcademyRiskProtectionArrangementsTaskStatus(taskData),
                CheckAccuracyOfHigherNeeds = CheckAccuracyOfHigherNeedsTaskStatus(taskData),
                CompleteNotificationOfChange = CompleteNotificationOfChangeTaskStatus(taskData),
                ProcessConversionSupportGrant = ProcessConversionSupportGrantTaskStatus(taskData),
                ConfirmAndProcessSponsoredSupportGrant = ConfirmAndProcessSponsoredSupportGrantTaskStatus(taskData),
                TupeConsultation = TupeConsultationTaskStatus(taskData),
                ConfirmAcademyName = ConfirmAcademyNameTaskStatus(taskData),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(keyContacts),
                ConfirmChairOfGovernorsDetails = ConfirmChairOfGovernorsDetailsTaskStatus(keyContacts),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(keyContacts),
                ArticlesOfAssociation = ArticlesOfAssociationTaskStatus(taskData),
                ChurchSupplementalAgreement = ChurchSupplementalAgreementTaskStatus(taskData),
                ConfirmMainContact = ConfirmMainContactTaskStatus(project),
                ConfirmProposedCapacityOfTheAcademy = ConfirmProposedCapacityOfTheAcademyTaskStatus(taskData),
                LAConfirmsPayrollDeadline = LAConfirmsPayrollDeadlineTaskStatus(taskData),
                PrivateFinanceInitiative = PrivateFinanceInitiativeTaskStatus(taskData),
                LandQuestionnaire = LandQuestionnaireTaskStatus(taskData),
                LandRegistry = LandRegistryTaskStatus(taskData),
                SupplementalFundingAgreement = SupplementalFundingAgreementTaskStatus(taskData),
                MasterFundingAgreement = MasterFundingAgreementTaskStatus(taskData),
                DeedOfVariation = DeedOfVariationTaskStatus(taskData),
                TrustModificationOrder = TrustModificationOrderTaskStatus(taskData),
                DirectionToTransfer = DirectionToTransferTaskStatus(taskData),
                OneHundredAndTwentyFiveYearLease = OneHundredAndTwentyFiveYearLeaseTaskStatus(taskData),
                Tubleases = TubleasesTaskStatus(taskData),
                TenancyAtWill = TenancyAtWillTaskStatus(taskData),
                ThirdPartyLeases = ThirdPartyLeasesTaskStatus(taskData),
                CommercialTransferAgreement = CommercialTransferAgreementTaskStatus(taskData),
                ConfirmTheSchoolHasCompletedAllActions = ConfirmTheSchoolHasCompletedAllActionsTaskStatus(taskData),
                ConfirmSchoolBankDetails = ConfirmSchoolBankDetailsTaskStatus(taskData),
                ConfirmAllConditionsHaveBeenMet = ConfirmAllConditionsHaveBeenMetTaskStatus(project),
                ShareTheInformationAboutOpening = ShareTheInformationAboutOpeningTaskStatus(taskData),
                ConfirmDateAcademyOpened = ConfirmDateAcademyOpenedTaskStatus(taskData),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskData),
                ProjectReceiveDeclarationOfExpenditureCertificate = ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus(taskData),
                ShowProcessConversionSupportGrant = ShouldShowProcessConversionSupportGrant(taskData),
                ConfirmNurseryArrangement = ConfirmNurseryArrangementTaskStatus(taskData),
                ConfirmStatutoryConsultation = ConfirmStatutoryConsultationTaskStatus(taskData),
                PostDecisionActions = PostDecisionActionsTaskStatus(taskData),
                ConfirmDbsChecks = ConfirmDbsChecksTaskStatus(taskData)
            };
        }
        private static TaskListStatus ConfirmDbsChecksTaskStatus(ConversionTaskDataDto taskData)
        {
            if (taskData.ConfirmDBSChecks is null)
            {
                return TaskListStatus.NotStarted;
            }

            return taskData.ConfirmDBSChecks == true
                ? TaskListStatus.Completed
                : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmNurseryArrangementTaskStatus(ConversionTaskDataDto taskData)
        {
            if (taskData.NurseryArrangement is null)
            {
                return TaskListStatus.NotStarted;
            }

            if (taskData.NurseryArrangement == NurseryArrangementOption.NotApplicable)
            {
                return TaskListStatus.NotApplicable;
            }

            return TaskListStatus.Completed;
            
        }

        private static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.HandoverReview.HasValue || taskData.HandoverReview == false) &&
                (!taskData.HandoverMeeting.HasValue || taskData.HandoverMeeting == false) &&
                (!taskData.HandoverNotes.HasValue || taskData.HandoverNotes == false) &&
                (!taskData.HandoverConfirmSacreExemption.HasValue || taskData.HandoverConfirmSacreExemption == false) &&
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
                taskData.HandoverNotes.Equals(true) &&
                taskData.HandoverConfirmSacreExemption.Equals(true))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ExternalStakeHolderKickoffTaskStatus(ConversionTaskDataDto taskData, ProjectDto project)
        {
            if (taskData.StakeholderKickOffIntroductoryEmails == true &&
                 taskData.StakeholderKickOffLocalAuthorityProforma == true &&
                 taskData.StakeholderKickOffSetupMeeting == true &&
                 taskData.StakeholderKickOffMeeting == true &&
                 taskData.StakeholderKickOffCheckProvisionalConversionDate == true &&
                 taskData.StakeholderKickOffDeclareBudgetChanges == true &&
                 project.SignificantDateProvisional == false)
            {
                return TaskListStatus.Completed;
            }
            return (taskData.StakeholderKickOffIntroductoryEmails == true ||
                   taskData.StakeholderKickOffLocalAuthorityProforma == true ||
                   taskData.StakeholderKickOffSetupMeeting == true ||
                   taskData.StakeholderKickOffMeeting == true ||
                   taskData.StakeholderKickOffCheckProvisionalConversionDate == true ||
                   taskData.StakeholderKickOffDeclareBudgetChanges == true ||
                    project.SignificantDateProvisional == false)
                    ? TaskListStatus.InProgress : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ReceiveGrantPaymentCertificateCheckCertificate.HasValue || taskData.ReceiveGrantPaymentCertificateCheckCertificate == false) &&
                (!taskData.ReceiveGrantPaymentCertificateSaveCertificate.HasValue || taskData.ReceiveGrantPaymentCertificateSaveCertificate == false) &&
                !taskData.ReceiveGrantPaymentCertificateDateReceived.HasValue &&
                (!taskData.ReceiveGrantPaymentCertificateNotApplicable.HasValue || taskData.ReceiveGrantPaymentCertificateNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ReceiveGrantPaymentCertificateNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ReceiveGrantPaymentCertificateCheckCertificate == true &&
                taskData.ReceiveGrantPaymentCertificateSaveCertificate == true &&
                taskData.ReceiveGrantPaymentCertificateDateReceived.HasValue)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.RedactAndSendRedact.HasValue || taskData.RedactAndSendRedact == false) &&
               (!taskData.RedactAndSendSaveRedaction.HasValue || taskData.RedactAndSendSaveRedaction == false) &&
               (!taskData.RedactAndSendSendRedaction.HasValue || taskData.RedactAndSendSendRedaction == false) &&
               (!taskData.RedactAndSendSendSolicitors.HasValue || taskData.RedactAndSendSendSolicitors == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.RedactAndSendRedact == true &&
                taskData.RedactAndSendSaveRedaction == true &&
                taskData.RedactAndSendSendRedaction == true &&
                taskData.RedactAndSendSendSolicitors == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmDateAcademyOpenedTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.ConfirmDateAcademyOpenedDateOpened.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.ConfirmDateAcademyOpenedDateOpened.HasValue)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ShareTheInformationAboutOpeningTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.ShareInformationEmail.HasValue || taskData.ShareInformationEmail == false)
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.ShareInformationEmail == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmTheSchoolHasCompletedAllActionsTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.SchoolCompletedEmailed.HasValue || taskData.SchoolCompletedEmailed == false) &&
                (!taskData.SchoolCompletedSaved.HasValue || taskData.SchoolCompletedSaved == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.SchoolCompletedEmailed == true &&
                taskData.SchoolCompletedSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmSchoolBankDetailsTaskStatus(ConversionTaskDataDto taskData)
        {

            if (taskData.ConfirmSchoolBankDetailsSent is null or false &&
                (taskData.ConfirmSchoolBankDetailsSubmitted is null or false))
            {
                return TaskListStatus.NotStarted;
            }

            return taskData is { ConfirmSchoolBankDetailsSent: true, ConfirmSchoolBankDetailsSubmitted: true }
                ? TaskListStatus.Completed
                : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAllConditionsHaveBeenMetTaskStatus(ProjectDto project)
        {
            return project.AllConditionsMet == true
                 ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.CommercialTransferAgreementUnamended.HasValue || taskData.CommercialTransferAgreementUnamended == false) &&
                (!taskData.CommercialTransferAgreementAgreed.HasValue || taskData.CommercialTransferAgreementAgreed == false) &&
               (!taskData.CommercialTransferAgreementSigned.HasValue || taskData.CommercialTransferAgreementSigned == false) &&
               (!taskData.CommercialTransferAgreementSaved.HasValue || taskData.CommercialTransferAgreementSaved == false) &&
               (!taskData.CommercialTransferAgreementQuestionsChecked.HasValue || taskData.CommercialTransferAgreementQuestionsChecked == false) &&
               (!taskData.CommercialTransferAgreementQuestionsReceived.HasValue || taskData.CommercialTransferAgreementQuestionsReceived == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.CommercialTransferAgreementUnamended == true &&
                taskData.CommercialTransferAgreementAgreed == true &&
                taskData.CommercialTransferAgreementSigned == true &&
                taskData.CommercialTransferAgreementSaved == true &&
                taskData.CommercialTransferAgreementQuestionsChecked == true &&
                taskData.CommercialTransferAgreementQuestionsReceived == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus TenancyAtWillTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.TenancyAtWillEmailSigned.HasValue || taskData.TenancyAtWillEmailSigned == false) &&
               (!taskData.TenancyAtWillReceiveSigned.HasValue || taskData.TenancyAtWillReceiveSigned == false) &&
               (!taskData.TenancyAtWillSaveSigned.HasValue || taskData.TenancyAtWillSaveSigned == false) &&
               (!taskData.TenancyAtWillNotApplicable.HasValue || taskData.TenancyAtWillNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.TenancyAtWillNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.TenancyAtWillEmailSigned == true &&
               taskData.TenancyAtWillReceiveSigned == true &&
               taskData.TenancyAtWillSaveSigned == true)
            ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus TubleasesTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.SubleasesReceived.HasValue || taskData.SubleasesReceived == false) &&
               (!taskData.SubleasesCleared.HasValue || taskData.SubleasesCleared == false) &&
               (!taskData.SubleasesSigned.HasValue || taskData.SubleasesSigned == false) &&
               (!taskData.SubleasesSaved.HasValue || taskData.SubleasesSaved == false) &&
               (!taskData.SubleasesEmailSigned.HasValue || taskData.SubleasesEmailSigned == false) &&
               (!taskData.SubleasesReceiveSigned.HasValue || taskData.SubleasesReceiveSigned == false) &&
               (!taskData.SubleasesSaveSigned.HasValue || taskData.SubleasesSaveSigned == false) &&
               (!taskData.SubleasesNotApplicable.HasValue || taskData.SubleasesNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.SubleasesNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.SubleasesReceived == true &&
               taskData.SubleasesCleared == true &&
               taskData.SubleasesSigned == true &&
               taskData.SubleasesSaved == true &&
               taskData.SubleasesEmailSigned == true &&
               taskData.SubleasesReceiveSigned == true &&
               taskData.SubleasesSaveSigned == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus OneHundredAndTwentyFiveYearLeaseTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.OneHundredAndTwentyFiveYearLeaseSaveLease.HasValue || taskData.OneHundredAndTwentyFiveYearLeaseSaveLease == false) &&
               (!taskData.OneHundredAndTwentyFiveYearLeaseEmail.HasValue || taskData.OneHundredAndTwentyFiveYearLeaseEmail == false) &&
               (!taskData.OneHundredAndTwentyFiveYearLeaseReceive.HasValue || taskData.OneHundredAndTwentyFiveYearLeaseReceive == false) &&
               (!taskData.OneHundredAndTwentyFiveYearLeaseNotApplicable.HasValue || taskData.OneHundredAndTwentyFiveYearLeaseNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.OneHundredAndTwentyFiveYearLeaseNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.OneHundredAndTwentyFiveYearLeaseSaveLease == true &&
               taskData.OneHundredAndTwentyFiveYearLeaseEmail == true &&
               taskData.OneHundredAndTwentyFiveYearLeaseReceive == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ThirdPartyLeasesTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ThirdPartyLeasesSave.HasValue || taskData.ThirdPartyLeasesSave == false) &&
               (!taskData.ThirdPartyLeasesEmail.HasValue || taskData.ThirdPartyLeasesEmail == false) &&
               (!taskData.ThirdPartyLeasesReceive.HasValue || taskData.ThirdPartyLeasesReceive == false) &&
               (!taskData.ThirdPartyLeasesNotApplicable.HasValue || taskData.ThirdPartyLeasesNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ThirdPartyLeasesNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ThirdPartyLeasesSave == true &&
               taskData.ThirdPartyLeasesEmail == true &&
               taskData.ThirdPartyLeasesReceive == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus DirectionToTransferTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.DirectionToTransferReceived.HasValue || taskData.DirectionToTransferReceived == false) &&
               (!taskData.DirectionToTransferCleared.HasValue || taskData.DirectionToTransferCleared == false) &&
               (!taskData.DirectionToTransferSigned.HasValue || taskData.DirectionToTransferSigned == false) &&
               (!taskData.DirectionToTransferSaved.HasValue || taskData.DirectionToTransferSaved == false) &&
               (!taskData.DirectionToTransferNotApplicable.HasValue || taskData.DirectionToTransferNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.DirectionToTransferNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.DirectionToTransferReceived == true &&
               taskData.DirectionToTransferCleared == true &&
               taskData.DirectionToTransferSigned == true &&
               taskData.DirectionToTransferSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus TrustModificationOrderTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.TrustModificationOrderReceived.HasValue || taskData.TrustModificationOrderReceived == false) &&
               (!taskData.TrustModificationOrderCleared.HasValue || taskData.TrustModificationOrderCleared == false) &&
               (!taskData.TrustModificationOrderSaved.HasValue || taskData.TrustModificationOrderSaved == false) &&
               (!taskData.TrustModificationOrderNotApplicable.HasValue || taskData.TrustModificationOrderNotApplicable == false) &&
               (!taskData.TrustModificationOrderSentLegal.HasValue || taskData.TrustModificationOrderSentLegal == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.TrustModificationOrderNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.TrustModificationOrderReceived == true &&
               taskData.TrustModificationOrderCleared == true &&
               taskData.TrustModificationOrderSaved == true &&
               taskData.TrustModificationOrderSentLegal == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus MasterFundingAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.MasterFundingAgreementReceived.HasValue || taskData.MasterFundingAgreementReceived == false) &&
               (!taskData.MasterFundingAgreementCleared.HasValue || taskData.MasterFundingAgreementCleared == false) &&
               (!taskData.MasterFundingAgreementSaved.HasValue || taskData.MasterFundingAgreementSaved == false) &&
               (!taskData.MasterFundingAgreementSigned.HasValue || taskData.MasterFundingAgreementSigned == false) &&
               (!taskData.MasterFundingAgreementSent.HasValue || taskData.MasterFundingAgreementSent == false) &&
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
               taskData.MasterFundingAgreementSaved == true &&
               taskData.MasterFundingAgreementSigned == true &&
               taskData.MasterFundingAgreementSent == true &&
               taskData.MasterFundingAgreementSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus DeedOfVariationTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.DeedOfVariationReceived.HasValue || taskData.DeedOfVariationReceived == false) &&
               (!taskData.DeedOfVariationCleared.HasValue || taskData.DeedOfVariationCleared == false) &&
               (!taskData.DeedOfVariationSaved.HasValue || taskData.DeedOfVariationSaved == false) &&
               (!taskData.DeedOfVariationDraftSaved.HasValue || taskData.DeedOfVariationDraftSaved == false) &&
               (!taskData.DeedOfVariationSigned.HasValue || taskData.DeedOfVariationSigned == false) &&
               (!taskData.DeedOfVariationSignedSecretaryState.HasValue || taskData.DeedOfVariationSignedSecretaryState == false) &&
               (!taskData.DeedOfVariationNotApplicable.HasValue || taskData.DeedOfVariationNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.DeedOfVariationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.DeedOfVariationReceived == true &&
               taskData.DeedOfVariationCleared == true &&
               taskData.DeedOfVariationSaved == true &&
               taskData.DeedOfVariationDraftSaved == true &&
               taskData.DeedOfVariationSigned == true &&
               taskData.DeedOfVariationSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.SupplementalFundingAgreementReceived.HasValue || taskData.SupplementalFundingAgreementReceived == false) &&
               (!taskData.SupplementalFundingAgreementCleared.HasValue || taskData.SupplementalFundingAgreementCleared == false) &&
               (!taskData.SupplementalFundingAgreementSaved.HasValue || taskData.SupplementalFundingAgreementSaved == false) &&
               (!taskData.SupplementalFundingAgreementSigned.HasValue || taskData.SupplementalFundingAgreementSigned == false) &&
               (!taskData.SupplementalFundingAgreementSignedSecretaryState.HasValue || taskData.SupplementalFundingAgreementSignedSecretaryState == false))
            {
                return !taskData.SupplementalFundingAgreementDraftSaved.HasValue || taskData.SupplementalFundingAgreementDraftSaved == false
                ? TaskListStatus.NotStarted
                : TaskListStatus.InProgress;
            }

            return (taskData.SupplementalFundingAgreementCleared == true &&
               taskData.SupplementalFundingAgreementReceived == true &&
               taskData.SupplementalFundingAgreementSaved == true &&
               taskData.SupplementalFundingAgreementSigned == true &&
               taskData.SupplementalFundingAgreementSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus LandRegistryTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.LandRegistryReceived.HasValue || taskData.LandRegistryReceived == false) &&
               (!taskData.LandRegistryCleared.HasValue || taskData.LandRegistryCleared == false) &&
               (!taskData.LandRegistrySaved.HasValue || taskData.LandRegistrySaved == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.LandRegistryReceived == true &&
                taskData.LandRegistryCleared == true &&
                taskData.LandRegistrySaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus LandQuestionnaireTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.LandQuestionnaireReceived.HasValue || taskData.LandQuestionnaireReceived == false) &&
               (!taskData.LandQuestionnaireCleared.HasValue || taskData.LandQuestionnaireCleared == false) &&
               (!taskData.LandQuestionnaireSigned.HasValue || taskData.LandQuestionnaireSigned == false) &&
               (!taskData.LandQuestionnaireSaved.HasValue || taskData.LandQuestionnaireSaved == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.LandQuestionnaireReceived == true &&
               taskData.LandQuestionnaireCleared == true &&
               taskData.LandQuestionnaireSigned == true &&
               taskData.LandQuestionnaireSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmProposedCapacityOfTheAcademyTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ProposedCapacityOfTheAcademyNotApplicable.HasValue || taskData.ProposedCapacityOfTheAcademyNotApplicable == false) &&
               string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyReceptionToSixYears) &&
               string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademySevenToElevenYears) &&
               string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ProposedCapacityOfTheAcademyNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (!string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyReceptionToSixYears) &&
                 !string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademySevenToElevenYears) &&
                 !string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus LAConfirmsPayrollDeadlineTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.LAPayrollDeadline.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.LAPayrollDeadline.HasValue)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus PrivateFinanceInitiativeTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((taskData.PrivateFinanceInitiativeNotApplicable is null or false) &&
                !taskData.PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted.HasValue &&
                !taskData.PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted.HasValue &&
                (taskData.PrivateFinanceInitiativeReceived is null or false) &&
                (taskData.PrivateFinanceInitiativeCleared is null or false) &&
                (taskData.PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder is null or false) &&
                (taskData.PrivateFinanceInitiativeSignedByAllStakeholders is null or false) &&
                (taskData.PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder is null or false))
            {
                return TaskListStatus.NotStarted;
            }

            if (taskData.PrivateFinanceInitiativeNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            return taskData is
            {
                PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted: true or false,
                PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted: true or false,
                PrivateFinanceInitiativeReceived: true, 
                PrivateFinanceInitiativeCleared: true,
                PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder: true,
                PrivateFinanceInitiativeSignedByAllStakeholders: true,
                PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder: true
            }
                ? TaskListStatus.Completed
                : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmMainContactTaskStatus(ProjectDto project)
        {
            return project.MainContactId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ChurchSupplementalAgreementReceived.HasValue || taskData.ChurchSupplementalAgreementReceived == false) &&
                (!taskData.ChurchSupplementalAgreementCleared.HasValue || taskData.ChurchSupplementalAgreementCleared == false) &&
                (!taskData.ChurchSupplementalAgreementSignedTrust.HasValue || taskData.ChurchSupplementalAgreementSignedTrust == false) &&
                (!taskData.ChurchSupplementalAgreementDraftSaved.HasValue || taskData.ChurchSupplementalAgreementDraftSaved == false) &&
                (!taskData.ChurchSupplementalAgreementNotApplicable.HasValue || taskData.ChurchSupplementalAgreementNotApplicable == false) &&
                (!taskData.ChurchSupplementalAgreementSignedDiocese.HasValue || taskData.ChurchSupplementalAgreementSignedDiocese == false) &&
                (!taskData.ChurchSupplementalAgreementSignedSecretaryState.HasValue || taskData.ChurchSupplementalAgreementSignedSecretaryState == false) &&
                (!taskData.ChurchSupplementalAgreementFinalSaved.HasValue || taskData.ChurchSupplementalAgreementFinalSaved == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ChurchSupplementalAgreementNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ChurchSupplementalAgreementReceived == true &&
                taskData.ChurchSupplementalAgreementCleared == true &&
                taskData.ChurchSupplementalAgreementSignedTrust == true &&
                taskData.ChurchSupplementalAgreementFinalSaved == true &&
                taskData.ChurchSupplementalAgreementSignedDiocese == true &&
                taskData.ChurchSupplementalAgreementSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(ConversionTaskDataDto taskData)
        {
            if (taskData.ArticlesOfAssociationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            if (taskData.ArticlesOfAssociationReceived is not true &&
                taskData.ArticlesOfAssociationCleared is not true &&
                taskData.ArticlesOfAssociationSigned is not true &&
                taskData.ArticlesOfAssociationSaved is not true)
            {
                return TaskListStatus.NotStarted;
            }

            return (taskData.ArticlesOfAssociationReceived is true &&
               taskData.ArticlesOfAssociationCleared is true &&
               taskData.ArticlesOfAssociationSigned is true &&
               taskData.ArticlesOfAssociationSaved is true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmIncomingTrustCeoDetailsTaskStatus(KeyContactDto? keyContacts)
        {
            return keyContacts?.IncomingTrustCeoId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmChairOfGovernorsDetailsTaskStatus(KeyContactDto? keyContacts)
        {
            return keyContacts?.ChairOfGovernorsId != null
                 ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmHeadTeacherDetailsTaskStatus(KeyContactDto? keyContacts)
        {
            return keyContacts?.HeadteacherId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }
        private static TaskListStatus ConfirmAcademyNameTaskStatus(ConversionTaskDataDto taskData)
        {
            return string.IsNullOrWhiteSpace(taskData.AcademyDetailsName)
                ? TaskListStatus.NotStarted : TaskListStatus.Completed;
        }

        private static TaskListStatus ConfirmAndProcessSponsoredSupportGrantTaskStatus(ConversionTaskDataDto taskData)
        {
            if (taskData.SponsoredSupportGrantInformTrust is null or false &&
                taskData.SponsoredSupportGrantPaymentForm is null or false &&
                taskData.SponsoredSupportGrantSendInformation is null or false &&
                taskData.SponsoredSupportGrantPaymentAmount is null or false &&
                string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType) &&
                taskData.SponsoredSupportGrantNotApplicable is null or false &&
                taskData.SponsoredSupportGrantHasVendorAccount is null or false)
            {
                return TaskListStatus.NotStarted;
            }

            if (taskData.SponsoredSupportGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            return (taskData is
                    {
                        SponsoredSupportGrantInformTrust: true, SponsoredSupportGrantPaymentForm: true,
                        SponsoredSupportGrantSendInformation: true, SponsoredSupportGrantPaymentAmount: true,
                        SponsoredSupportGrantHasVendorAccount: true
                    } &&
                    !string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType))
                ? TaskListStatus.Completed
                : TaskListStatus.InProgress;
        }

        private static TaskListStatus TupeConsultationTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.TupeConsultationCompleted.HasValue || taskData.TupeConsultationCompleted == false)
            {
                return TaskListStatus.NotStarted;
            }

            return taskData.TupeConsultationCompleted == true
                ? TaskListStatus.Completed
                : TaskListStatus.InProgress;
        }

        private static TaskListStatus ProcessConversionSupportGrantTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ConversionGrantCheckVendorAccount.HasValue || taskData.ConversionGrantCheckVendorAccount == false) &&
                (!taskData.ConversionGrantPaymentForm.HasValue || taskData.ConversionGrantPaymentForm == false) &&
                (!taskData.ConversionGrantSendInformation.HasValue || taskData.ConversionGrantSendInformation == false) &&
                (!taskData.ConversionGrantSharePaymentDate.HasValue || taskData.ConversionGrantSharePaymentDate == false) &&
                (!taskData.ConversionGrantNotApplicable.HasValue || taskData.ConversionGrantNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ConversionGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ConversionGrantCheckVendorAccount == true &&
                taskData.ConversionGrantPaymentForm == true &&
                taskData.ConversionGrantSendInformation == true &&
                taskData.ConversionGrantSharePaymentDate == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static bool ShouldShowProcessConversionSupportGrant(ConversionTaskDataDto taskData)
        {
            return taskData.ConversionGrantNotApplicable != true;
        }

        private static TaskListStatus CompleteNotificationOfChangeTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.CompleteNotificationOfChangeCheckDocument.HasValue || taskData.CompleteNotificationOfChangeCheckDocument == false) &&
                (!taskData.CompleteNotificationOfChangeNotApplicable.HasValue || taskData.CompleteNotificationOfChangeNotApplicable == false) &&
                (!taskData.CompleteNotificationOfChangeSendDocument.HasValue || taskData.CompleteNotificationOfChangeSendDocument == false) &&
                (!taskData.CompleteNotificationOfChangeTellLocalAuthority.HasValue || taskData.CompleteNotificationOfChangeTellLocalAuthority == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.CompleteNotificationOfChangeNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.CompleteNotificationOfChangeCheckDocument == true &&
                taskData.CompleteNotificationOfChangeSendDocument == true &&
                taskData.CompleteNotificationOfChangeTellLocalAuthority == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus CheckAccuracyOfHigherNeedsTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.CheckAccuracyOfHigherNeedsConfirmNumber.HasValue || taskData.CheckAccuracyOfHigherNeedsConfirmNumber == false) &&
                (!taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber.HasValue || taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.CheckAccuracyOfHigherNeedsConfirmNumber == true &&
                taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.RiskProtectionArrangementOption.HasValue &&
               string.IsNullOrWhiteSpace(taskData.RiskProtectionArrangementReason))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.RiskProtectionArrangementOption == RiskProtectionArrangementOption.Standard ||
                taskData.RiskProtectionArrangementOption == RiskProtectionArrangementOption.ChurchOrTrust ||
                taskData.RiskProtectionArrangementOption == RiskProtectionArrangementOption.Commercial && !string.IsNullOrWhiteSpace(taskData.RiskProtectionArrangementReason))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }


        private static TaskListStatus ConfirmStatutoryConsultationTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.StatutoryConsultationNotApplicable.HasValue || taskData.StatutoryConsultationNotApplicable == false) &&
                (!taskData.StatutoryConsultationComplete.HasValue || taskData.StatutoryConsultationComplete == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.StatutoryConsultationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return taskData.StatutoryConsultationComplete == true 
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus PostDecisionActionsTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.PostDecisionActionsApplicationUploaded.HasValue || taskData.PostDecisionActionsApplicationUploaded == false) &&
                (!taskData.PostDecisionActionsAcademyOrderUploaded.HasValue || taskData.PostDecisionActionsAcademyOrderUploaded == false) &&
                (!taskData.PostDecisionActionsLaProformaUploaded.HasValue || taskData.PostDecisionActionsLaProformaUploaded == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.PostDecisionActionsApplicationUploaded == true &&
                taskData.PostDecisionActionsAcademyOrderUploaded == true &&
                taskData.PostDecisionActionsLaProformaUploaded == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }
    }
}
