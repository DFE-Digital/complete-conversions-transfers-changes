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
        public TaskListStatus ConfirmAcademyName { get; set; }
        public TaskListStatus ConfirmHeadTeacherDetails { get; set; }
        public TaskListStatus ConfirmChairOfGovernorsDetails { get; set; }
        public TaskListStatus ConfirmIncomingTrustCeoDetails { get; set; }
        public TaskListStatus ConfirmMainContact { get; set; }
        public TaskListStatus ConfirmProposedCapacityOfTheAcademy { get; set; }
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
        public TaskListStatus CommercialTransferAgreement { get; set; }
        public TaskListStatus ConfirmTheSchoolHasCompletedAllActions { get; set; }
        public TaskListStatus ConfirmAllConditionsHaveBeenMet { get; set; }
        public TaskListStatus ShareTheInformationAboutOpening { get; set; }
        public TaskListStatus ConfirmDateAcademyOpened { get; set; }
        public TaskListStatus RedactAndSendDocuments { get; set; }
        public TaskListStatus ProjectReceiveDeclarationOfExpenditureCertificate { get; set; }

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
                ConfirmAcademyName = ConfirmAcademyNameTaskStatus(taskData),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(keyContacts),
                ConfirmChairOfGovernorsDetails = ConfirmChairOfGovernorsDetailsTaskStatus(keyContacts),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(keyContacts),
                ArticlesOfAssociation = ArticlesOfAssociationTaskStatus(taskData),
                ChurchSupplementalAgreement = ChurchSupplementalAgreementTaskStatus(taskData),
                ConfirmMainContact = ConfirmMainContactTaskStatus(project),
                ConfirmProposedCapacityOfTheAcademy = ConfirmProposedCapacityOfTheAcademyTaskStatus(taskData),
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
                CommercialTransferAgreement = CommercialTransferAgreementTaskStatus(taskData),
                ConfirmTheSchoolHasCompletedAllActions = ConfirmTheSchoolHasCompletedAllActionsTaskStatus(taskData),
                ConfirmAllConditionsHaveBeenMet = ConfirmAllConditionsHaveBeenMetTaskStatus(project),
                ShareTheInformationAboutOpening = ShareTheInformationAboutOpeningTaskStatus(taskData),
                ConfirmDateAcademyOpened = ConfirmDateAcademyOpenedTaskStatus(taskData),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskData),
                ProjectReceiveDeclarationOfExpenditureCertificate = ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus(taskData)
            };
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
                 project.SignificantDateProvisional == false)
            {
                return TaskListStatus.Completed;
            }
            return (taskData.StakeholderKickOffIntroductoryEmails == true ||
                   taskData.StakeholderKickOffLocalAuthorityProforma == true ||
                   taskData.StakeholderKickOffSetupMeeting == true ||
                   taskData.StakeholderKickOffMeeting == true ||
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

        private static TaskListStatus ConfirmAllConditionsHaveBeenMetTaskStatus(ProjectDto project)
        {
            return project.AllConditionsMet == true
                 ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.CommercialTransferAgreementAgreed.HasValue || taskData.CommercialTransferAgreementAgreed == false) &&
               (!taskData.CommercialTransferAgreementSigned.HasValue || taskData.CommercialTransferAgreementSigned == false) &&
               (!taskData.CommercialTransferAgreementSaved.HasValue || taskData.CommercialTransferAgreementSaved == false) &&
               (!taskData.CommercialTransferAgreementQuestionsChecked.HasValue || taskData.CommercialTransferAgreementQuestionsChecked == false) &&
               (!taskData.CommercialTransferAgreementQuestionsReceived.HasValue || taskData.CommercialTransferAgreementQuestionsReceived == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.CommercialTransferAgreementAgreed == true &&
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
               (!taskData.DeedOfVariationSigned.HasValue || taskData.DeedOfVariationSigned == false) &&
               (!taskData.DeedOfVariationSent.HasValue || taskData.DeedOfVariationSent == false) &&
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
               taskData.DeedOfVariationSigned == true &&
               taskData.DeedOfVariationSent == true &&
               taskData.DeedOfVariationSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.SupplementalFundingAgreementReceived.HasValue || taskData.SupplementalFundingAgreementReceived == false) &&
               (!taskData.SupplementalFundingAgreementCleared.HasValue || taskData.SupplementalFundingAgreementCleared == false) &&
               (!taskData.SupplementalFundingAgreementSaved.HasValue || taskData.SupplementalFundingAgreementSaved == false) &&
               (!taskData.SupplementalFundingAgreementSigned.HasValue || taskData.SupplementalFundingAgreementSigned == false) &&
               (!taskData.SupplementalFundingAgreementSent.HasValue || taskData.SupplementalFundingAgreementSent == false) &&
               (!taskData.SupplementalFundingAgreementSignedSecretaryState.HasValue || taskData.SupplementalFundingAgreementSignedSecretaryState == false))
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.SupplementalFundingAgreementCleared == true &&
               taskData.SupplementalFundingAgreementReceived == true &&
               taskData.SupplementalFundingAgreementSaved == true &&
               taskData.SupplementalFundingAgreementSigned == true &&
               taskData.SupplementalFundingAgreementSent == true &&
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

        private static TaskListStatus ConfirmMainContactTaskStatus(ProjectDto project)
        {
            return project.MainContactId != null
                ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ChurchSupplementalAgreementReceived.HasValue || taskData.ChurchSupplementalAgreementReceived == false) &&
                (!taskData.ChurchSupplementalAgreementCleared.HasValue || taskData.ChurchSupplementalAgreementCleared == false) &&
                (!taskData.ChurchSupplementalAgreementSigned.HasValue || taskData.ChurchSupplementalAgreementSigned == false) &&
                (!taskData.ChurchSupplementalAgreementSaved.HasValue || taskData.ChurchSupplementalAgreementSaved == false) &&
                (!taskData.ChurchSupplementalAgreementNotApplicable.HasValue || taskData.ChurchSupplementalAgreementNotApplicable == false) &&
                (!taskData.ChurchSupplementalAgreementSignedDiocese.HasValue || taskData.ChurchSupplementalAgreementSignedDiocese == false) &&
                (!taskData.ChurchSupplementalAgreementSignedSecretaryState.HasValue || taskData.ChurchSupplementalAgreementSignedSecretaryState == false) &&
                (!taskData.ChurchSupplementalAgreementSent.HasValue || taskData.ChurchSupplementalAgreementSent == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ChurchSupplementalAgreementNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ChurchSupplementalAgreementReceived == true &&
                taskData.ChurchSupplementalAgreementCleared == true &&
                taskData.ChurchSupplementalAgreementSigned == true &&
                taskData.ChurchSupplementalAgreementSaved == true &&
                taskData.ChurchSupplementalAgreementSent == true &&
                taskData.ChurchSupplementalAgreementSignedDiocese == true &&
                taskData.ChurchSupplementalAgreementSignedSecretaryState == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.ArticlesOfAssociationReceived.HasValue || taskData.ArticlesOfAssociationReceived == false) &&
               (!taskData.ArticlesOfAssociationCleared.HasValue || taskData.ArticlesOfAssociationCleared == false) &&
               (!taskData.ArticlesOfAssociationSigned.HasValue || taskData.ArticlesOfAssociationSigned == false) &&
               (!taskData.ArticlesOfAssociationSaved.HasValue || taskData.ArticlesOfAssociationSaved == false) &&
               (!taskData.ArticlesOfAssociationNotApplicable.HasValue || taskData.ArticlesOfAssociationNotApplicable == false) &&
               (!taskData.ArticlesOfAssociationSent.HasValue || taskData.ArticlesOfAssociationSent == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ArticlesOfAssociationNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.ArticlesOfAssociationReceived == true &&
               taskData.ArticlesOfAssociationCleared == true &&
               taskData.ArticlesOfAssociationSigned == true &&
               taskData.ArticlesOfAssociationSaved == true &&
               taskData.ArticlesOfAssociationSent == true)
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
            return (string.IsNullOrWhiteSpace(taskData.AcademyDetailsName))
                ? TaskListStatus.NotStarted : TaskListStatus.Completed;
        }

        private static TaskListStatus ConfirmAndProcessSponsoredSupportGrantTaskStatus(ConversionTaskDataDto taskData)
        {
            if ((!taskData.SponsoredSupportGrantInformTrust.HasValue || taskData.SponsoredSupportGrantInformTrust == false) &&
                (!taskData.SponsoredSupportGrantPaymentForm.HasValue || taskData.SponsoredSupportGrantPaymentForm == false) &&
                (!taskData.SponsoredSupportGrantSendInformation.HasValue || taskData.SponsoredSupportGrantSendInformation == false) &&
                (!taskData.SponsoredSupportGrantPaymentAmount.HasValue || taskData.SponsoredSupportGrantPaymentAmount == false) &&
                string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType) &&
                (!taskData.SponsoredSupportGrantNotApplicable.HasValue || taskData.SponsoredSupportGrantNotApplicable == false))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.SponsoredSupportGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return (taskData.SponsoredSupportGrantInformTrust == true &&
                taskData.SponsoredSupportGrantPaymentForm == true &&
                taskData.SponsoredSupportGrantSendInformation == true &&
                taskData.SponsoredSupportGrantPaymentAmount == true &&
                !string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
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
    }
}
