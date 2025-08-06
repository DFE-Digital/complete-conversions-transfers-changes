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

        public static ConversionTaskListViewModel Create(ConversionTaskDataDto taskData, ProjectDto project)
        {
            return new ConversionTaskListViewModel
            {
                HandoverWithRegionalDeliveryOfficer = HandoverWithRegionalDeliveryOfficerTaskStatus(taskData),
                ExternalStakeHolderKickoff = ExternalStakeHolderKickoffTaskStatus(taskData, project),
                ConfirmAcademyRiskProtectionArrangements = ConfirmAcademyRiskProtectionArrangementsTaskStatus(taskData),
                CheckAccuracyOfHigherNeeds = CheckAccuracyOfHigherNeedsTaskStatus(taskData),
                CompleteNotificationOfChange = CompleteNotificationOfChangeTaskStatus(taskData),
                ProcessConversionSupportGrant = ProcessConversionSupportGrantTaskStatus(taskData),
                ConfirmAndProcessSponsoredSupportGrant = ConfirmAndProcessSponsoredSupportGrantTaskStatus(taskData),
                ConfirmAcademyName = ConfirmAcademyNameTaskStatus(taskData),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(taskData),
                ConfirmChairOfGovernorsDetails = ConfirmChairOfGovernorsDetailsTaskStatus(taskData),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(taskData),
                ArticlesOfAssociation = ArticlesOfAssociationTaskStatus(taskData),
                ChurchSupplementalAgreement = ChurchSupplementalAgreementTaskStatus(taskData),
                ConfirmMainContact = ConfirmMainContactTaskStatus(taskData),
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
                ConfirmAllConditionsHaveBeenMet = ConfirmAllConditionsHaveBeenMetTaskStatus(taskData),
                ShareTheInformationAboutOpening = ShareTheInformationAboutOpeningTaskStatus(taskData),
                ConfirmDateAcademyOpened = ConfirmDateAcademyOpenedTaskStatus(taskData),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskData),
                ProjectReceiveDeclarationOfExpenditureCertificate = ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus(taskData)
            };
        }

        public static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(ConversionTaskDataDto taskData)
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

        public static TaskListStatus ExternalStakeHolderKickoffTaskStatus(ConversionTaskDataDto taskData, ProjectDto project)
        {
            if ((taskData.StakeholderKickOffIntroductoryEmails == false ||
                   taskData.StakeholderKickOffLocalAuthorityProforma == false ||
                   taskData.StakeholderKickOffSetupMeeting == false ||
                   taskData.StakeholderKickOffMeeting == false ||
                   taskData.StakeholderKickOffCheckProvisionalConversionDate == false) ||
                   project.SignificantDateProvisional == true)
            {
                return TaskListStatus.InProgress;
            }
            if ((taskData.StakeholderKickOffIntroductoryEmails == true &&
                 taskData.StakeholderKickOffLocalAuthorityProforma == true &&
                 taskData.StakeholderKickOffSetupMeeting == true &&
                 taskData.StakeholderKickOffMeeting == true &&
                 taskData.StakeholderKickOffCheckProvisionalConversionDate == true) &&
                 project.SignificantDateProvisional == false)
            {
                return TaskListStatus.Completed;
            }
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.ConversionGrantCheckVendorAccount.HasValue &&
                !taskData.ConversionGrantPaymentForm.HasValue &&
                !taskData.ConversionGrantSendInformation.HasValue &&
                !taskData.ConversionGrantSharePaymentDate.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.ConversionGrantCheckVendorAccount == true &&
                taskData.ConversionGrantPaymentForm == true &&
                taskData.ConversionGrantSendInformation == true &&
                taskData.ConversionGrantSharePaymentDate == true)
            {
                return TaskListStatus.Completed;
            }
            if(taskData.ConversionGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.RedactAndSendRedact.HasValue &&
               !taskData.RedactAndSendSaveRedaction.HasValue &&
               !taskData.RedactAndSendSendRedaction.HasValue &&
               !taskData.RedactAndSendSendSolicitors.HasValue)
            {
                return TaskListStatus.NotStarted; 
            }
            if (taskData.RedactAndSendRedact == true &&
                taskData.RedactAndSendSaveRedaction == true &&
                taskData.RedactAndSendSendRedaction == true &&
                taskData.RedactAndSendSendSolicitors == true)
            {
                return TaskListStatus.Completed;
            }

            return TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmDateAcademyOpenedTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.ConfirmDateAcademyOpenedDateOpened.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ConfirmDateAcademyOpenedDateOpened.HasValue)
            {
                return TaskListStatus.Completed;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus ShareTheInformationAboutOpeningTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmTheSchoolHasCompletedAllActionsTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAllConditionsHaveBeenMetTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus TenancyAtWillTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus TubleasesTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus OneHundredAndTwentyFiveYearLeaseTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DirectionToTransferTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus TrustModificationOrderTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus MasterFundingAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfVariationTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus LandRegistryTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus LandQuestionnaireTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmProposedCapacityOfTheAcademyTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.ProposedCapacityOfTheAcademyNotApplicable.HasValue &&
               string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyReceptionToSixYears) &&
               string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademySevenToElevenYears) &&
               string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.ProposedCapacityOfTheAcademyNotApplicable == false ||
                (!string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyReceptionToSixYears) &&
                 !string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademySevenToElevenYears) &&
                 !string.IsNullOrWhiteSpace(taskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears)))
            {
                return TaskListStatus.Completed;
            }
            if (taskData.ProposedCapacityOfTheAcademyNotApplicable == true)
            { 
                return TaskListStatus.NotApplicable;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmMainContactTaskStatus(ConversionTaskDataDto taskData)
        {
            //Get main from coontact table by project id
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustCeoDetailsTaskStatus(ConversionTaskDataDto taskData)
        {
            //Get CEO details from coontact table by project id
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmChairOfGovernorsDetailsTaskStatus(ConversionTaskDataDto taskData)
        {
            //Get chair of governers details from coontact table by project id
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmHeadTeacherDetailsTaskStatus(ConversionTaskDataDto taskData)
        {
            //Get head teacher details from coontact table by project id
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAcademyNameTaskStatus(ConversionTaskDataDto taskData)
        {
            if(string.IsNullOrWhiteSpace(taskData.AcademyDetailsName))
            {
                return TaskListStatus.NotStarted;
            }
            if(!string.IsNullOrWhiteSpace(taskData.AcademyDetailsName))
            {
                return TaskListStatus.Completed;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAndProcessSponsoredSupportGrantTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.SponsoredSupportGrantInformTrust.HasValue &&
                !taskData.SponsoredSupportGrantPaymentForm.HasValue &&
                !taskData.SponsoredSupportGrantSendInformation.HasValue &&
                !taskData.SponsoredSupportGrantPaymentAmount.HasValue &&
                string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.SponsoredSupportGrantInformTrust == true &&
                taskData.SponsoredSupportGrantPaymentForm == true &&
                taskData.SponsoredSupportGrantSendInformation == true &&
                taskData.SponsoredSupportGrantPaymentAmount == true &&
                !string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType))
            {
                return TaskListStatus.Completed;
            }
            if (taskData.SponsoredSupportGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus ProcessConversionSupportGrantTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.ConversionGrantCheckVendorAccount.HasValue &&
                !taskData.ConversionGrantPaymentForm.HasValue &&
                !taskData.ConversionGrantSendInformation.HasValue &&
                !taskData.ConversionGrantSharePaymentDate.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.ConversionGrantCheckVendorAccount == true &&
                taskData.ConversionGrantPaymentForm == true &&
                taskData.ConversionGrantSendInformation == true &&
                taskData.ConversionGrantSharePaymentDate == true)
            {
                return TaskListStatus.Completed;
            }
            if (taskData.ConversionGrantNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus CompleteNotificationOfChangeTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.CompleteNotificationOfChangeCheckDocument.HasValue &&
                !taskData.CompleteNotificationOfChangeNotApplicable.HasValue &&
                !taskData.CompleteNotificationOfChangeSendDocument.HasValue &&
                !taskData.CompleteNotificationOfChangeTellLocalAuthority.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.CompleteNotificationOfChangeNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }
            if (taskData.CompleteNotificationOfChangeCheckDocument == true &&
                taskData.CompleteNotificationOfChangeNotApplicable == false &&
                taskData.CompleteNotificationOfChangeSendDocument == true &&
                taskData.CompleteNotificationOfChangeTellLocalAuthority == true)
            {
                return TaskListStatus.Completed;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus CheckAccuracyOfHigherNeedsTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.CheckAccuracyOfHigherNeedsConfirmNumber.HasValue &&
                !taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.CheckAccuracyOfHigherNeedsConfirmNumber == true &&
                taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber == true)
            {
                return TaskListStatus.Completed;
            }
            return TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.RiskProtectionArrangementOption.HasValue &&
               string.IsNullOrWhiteSpace(taskData.RiskProtectionArrangementReason))
            {
                return TaskListStatus.NotStarted;
            }
            if (taskData.RiskProtectionArrangementOption == RiskProtectionArrangementOption.Standard ||
                taskData.RiskProtectionArrangementOption == RiskProtectionArrangementOption.ChurchOrTrust ||
                taskData.RiskProtectionArrangementOption == RiskProtectionArrangementOption.Commercial && !string.IsNullOrWhiteSpace(taskData.RiskProtectionArrangementReason))
            {
                return TaskListStatus.Completed;
            }
            return TaskListStatus.InProgress;
        }
    }
}
