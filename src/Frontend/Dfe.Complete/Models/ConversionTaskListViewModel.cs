using Dfe.Complete.Application.Projects.Models;

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
        public TaskListStatus ReceiveGrantPaymentCertificate { get; set; } 

        public static ConversionTaskListViewModel Create(ConversionTaskDataModel taskDataModel)
        {
            return new ConversionTaskListViewModel
            {
                HandoverWithRegionalDeliveryOfficer = HandoverWithRegionalDeliveryOfficerTaskStatus(taskDataModel),
                ExternalStakeHolderKickoff = ExternalStakeHolderKickoffTaskStatus(taskDataModel),
                ConfirmAcademyRiskProtectionArrangements = ConfirmAcademyRiskProtectionArrangementsTaskStatus(taskDataModel),
                CheckAccuracyOfHigherNeeds = CheckAccuracyOfHigherNeedsTaskStatus(taskDataModel),
                CompleteNotificationOfChange = CompleteNotificationOfChangeTaskStatus(taskDataModel),
                ProcessConversionSupportGrant = ProcessConversionSupportGrantTaskStatus(taskDataModel),
                ConfirmAndProcessSponsoredSupportGrant = ConfirmAndProcessSponsoredSupportGrantTaskStatus(taskDataModel),
                ConfirmAcademyName = ConfirmAcademyNameTaskStatus(taskDataModel),
                ConfirmHeadTeacherDetails = ConfirmHeadTeacherDetailsTaskStatus(taskDataModel),
                ConfirmChairOfGovernorsDetails = ConfirmChairOfGovernorsDetailsTaskStatus(taskDataModel),
                ConfirmIncomingTrustCeoDetails = ConfirmIncomingTrustCeoDetailsTaskStatus(taskDataModel),
                ArticlesOfAssociation = ArticlesOfAssociationTaskStatus(taskDataModel),
                ChurchSupplementalAgreement = ChurchSupplementalAgreementTaskStatus(taskDataModel),
                ConfirmMainContact = ConfirmMainContactTaskStatus(taskDataModel),
                ConfirmProposedCapacityOfTheAcademy = ConfirmProposedCapacityOfTheAcademyTaskStatus(taskDataModel),
                LandQuestionnaire = LandQuestionnaireTaskStatus(taskDataModel),
                LandRegistry = LandRegistryTaskStatus(taskDataModel),
                SupplementalFundingAgreement = SupplementalFundingAgreementTaskStatus(taskDataModel),
                MasterFundingAgreement = MasterFundingAgreementTaskStatus(taskDataModel),
                DeedOfVariation = DeedOfVariationTaskStatus(taskDataModel),
                TrustModificationOrder = TrustModificationOrderTaskStatus(taskDataModel),
                DirectionToTransfer = DirectionToTransferTaskStatus(taskDataModel),
                OneHundredAndTwentyFiveYearLease = OneHundredAndTwentyFiveYearLeaseTaskStatus(taskDataModel),
                Tubleases = TubleasesTaskStatus(taskDataModel),
                TenancyAtWill = TenancyAtWillTaskStatus(taskDataModel),
                CommercialTransferAgreement = CommercialTransferAgreementTaskStatus(taskDataModel),
                ConfirmTheSchoolHasCompletedAllActions = ConfirmTheSchoolHasCompletedAllActionsTaskStatus(taskDataModel),
                ConfirmAllConditionsHaveBeenMet = ConfirmAllConditionsHaveBeenMetTaskStatus(taskDataModel),
                ShareTheInformationAboutOpening = ShareTheInformationAboutOpeningTaskStatus(taskDataModel),
                ConfirmDateAcademyOpened = ConfirmDateAcademyOpenedTaskStatus(taskDataModel),
                RedactAndSendDocuments = RedactAndSendDocumentsTaskStatus(taskDataModel),
                ReceiveGrantPaymentCertificate = ReceiveGrantPaymentCertificateTaskStatus(taskDataModel)
            };
        }

        public static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(ConversionTaskDataModel taskDataModel)
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

        public static TaskListStatus ExternalStakeHolderKickoffTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            // Assuming ExternalStakeHolderKickoff is not part of the taskDataModel, returning NotStarted for now.
            // This can be updated based on actual requirements.
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ReceiveGrantPaymentCertificateTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmDateAcademyOpenedTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ShareTheInformationAboutOpeningTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmTheSchoolHasCompletedAllActionsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAllConditionsHaveBeenMetTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus TenancyAtWillTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus TubleasesTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus OneHundredAndTwentyFiveYearLeaseTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DirectionToTransferTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus TrustModificationOrderTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus MasterFundingAgreementTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus DeedOfVariationTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus SupplementalFundingAgreementTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus LandRegistryTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus LandQuestionnaireTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmProposedCapacityOfTheAcademyTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmMainContactTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ArticlesOfAssociationTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmIncomingTrustCeoDetailsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmChairOfGovernorsDetailsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmHeadTeacherDetailsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAcademyNameTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAndProcessSponsoredSupportGrantTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ProcessConversionSupportGrantTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CompleteNotificationOfChangeTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CheckAccuracyOfHigherNeedsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(ConversionTaskDataModel taskDataModel)
        {
            return TaskListStatus.NotStarted;
        }
    }
}
