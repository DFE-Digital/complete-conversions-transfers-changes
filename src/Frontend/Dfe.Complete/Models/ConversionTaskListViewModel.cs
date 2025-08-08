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

            return (taskData.HandoverReview.Equals(true) &&
                taskData.HandoverMeeting.Equals(true) &&
                taskData.HandoverNotes.Equals(true))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        public static TaskListStatus ExternalStakeHolderKickoffTaskStatus(ConversionTaskDataDto taskData, ProjectDto project)
        { 
            if((project.SignificantDateProvisional == false) || 
                (taskData.StakeholderKickOffIntroductoryEmails == true ||
                   taskData.StakeholderKickOffLocalAuthorityProforma == true ||
                   taskData.StakeholderKickOffSetupMeeting == true ||
                   taskData.StakeholderKickOffMeeting == true))
            {
                return TaskListStatus.InProgress;
            }
            return (taskData.StakeholderKickOffIntroductoryEmails == true &&
                 taskData.StakeholderKickOffLocalAuthorityProforma == true &&
                 taskData.StakeholderKickOffSetupMeeting == true &&
                 taskData.StakeholderKickOffMeeting == true &&
                 taskData.StakeholderKickOffCheckProvisionalConversionDate == true &&
                 project.SignificantDateProvisional == false)
                    ? TaskListStatus.Completed : TaskListStatus.NotStarted;
        }

        private static TaskListStatus ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.ConversionGrantCheckVendorAccount.HasValue &&
                !taskData.ConversionGrantPaymentForm.HasValue &&
                !taskData.ConversionGrantSendInformation.HasValue &&
                !taskData.ConversionGrantSharePaymentDate.HasValue &&
                !taskData.ConversionGrantNotApplicable.HasValue)
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

        private static TaskListStatus RedactAndSendDocumentsTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.RedactAndSendRedact.HasValue &&
               !taskData.RedactAndSendSaveRedaction.HasValue &&
               !taskData.RedactAndSendSendRedaction.HasValue &&
               !taskData.RedactAndSendSendSolicitors.HasValue)
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
            if(!taskData.ShareInformationEmail.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.ShareInformationEmail == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmTheSchoolHasCompletedAllActionsTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.SchoolCompletedEmailed.HasValue &&
                !taskData.SchoolCompletedSaved.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.SchoolCompletedEmailed == true &&
                taskData.SchoolCompletedSaved == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAllConditionsHaveBeenMetTaskStatus(ConversionTaskDataDto taskData)
        {
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus CommercialTransferAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.CommercialTransferAgreementAgreed.HasValue &&
               !taskData.CommercialTransferAgreementSigned.HasValue &&
               !taskData.CommercialTransferAgreementSaved.HasValue &&
               !taskData.CommercialTransferAgreementQuestionsChecked.HasValue &&
               !taskData.CommercialTransferAgreementQuestionsReceived.HasValue)
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
            if(!taskData.TenancyAtWillEmailSigned.HasValue &&
               !taskData.TenancyAtWillReceiveSigned.HasValue &&
               !taskData.TenancyAtWillSaveSigned.HasValue &&
               !taskData.TenancyAtWillNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.TenancyAtWillNotApplicable == true)
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
            if(!taskData.SubleasesReceived.HasValue &&
               !taskData.SubleasesCleared.HasValue &&
               !taskData.SubleasesSigned.HasValue &&
               !taskData.SubleasesSaved.HasValue &&
               !taskData.SubleasesEmailSigned.HasValue &&
               !taskData.SubleasesReceiveSigned.HasValue &&
               !taskData.SubleasesSaveSigned.HasValue &&
               !taskData.SubleasesNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.SubleasesNotApplicable == true)
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
            if(!taskData.OneHundredAndTwentyFiveYearLeaseSaveLease.HasValue &&
               !taskData.OneHundredAndTwentyFiveYearLeaseEmail.HasValue &&
               !taskData.OneHundredAndTwentyFiveYearLeaseReceive.HasValue &&
               !taskData.OneHundredAndTwentyFiveYearLeaseNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.OneHundredAndTwentyFiveYearLeaseNotApplicable == true)
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
            if(!taskData.DirectionToTransferReceived.HasValue &&
               !taskData.DirectionToTransferCleared.HasValue &&
               !taskData.DirectionToTransferSigned.HasValue &&
               !taskData.DirectionToTransferSaved.HasValue &&
               !taskData.DirectionToTransferNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.DirectionToTransferNotApplicable == true)
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
            if(!taskData.TrustModificationOrderReceived.HasValue &&
               !taskData.TrustModificationOrderCleared.HasValue &&
               !taskData.TrustModificationOrderSaved.HasValue && 
               !taskData.TrustModificationOrderNotApplicable.HasValue &&
               !taskData.TrustModificationOrderSentLegal.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.TrustModificationOrderNotApplicable == true)
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
            if(!taskData.MasterFundingAgreementReceived.HasValue &&
               !taskData.MasterFundingAgreementCleared.HasValue &&
               !taskData.MasterFundingAgreementSaved.HasValue &&
               !taskData.MasterFundingAgreementSigned.HasValue &&
               !taskData.MasterFundingAgreementSent.HasValue &&
               !taskData.MasterFundingAgreementSignedSecretaryState.HasValue &&
               !taskData.MasterFundingAgreementNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.MasterFundingAgreementNotApplicable == true)
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
            if (!taskData.DeedOfVariationReceived.HasValue &&
               !taskData.DeedOfVariationCleared.HasValue &&
               !taskData.DeedOfVariationSaved.HasValue &&
               !taskData.DeedOfVariationSigned.HasValue &&
               !taskData.DeedOfVariationSent.HasValue &&
               !taskData.DeedOfVariationSignedSecretaryState.HasValue &&
               !taskData.DeedOfVariationNotApplicable.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            if(taskData.DeedOfVariationNotApplicable == true)
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
            if(!taskData.SupplementalFundingAgreementReceived.HasValue &&
               !taskData.SupplementalFundingAgreementCleared.HasValue &&
               !taskData.SupplementalFundingAgreementSaved.HasValue &&
               !taskData.SupplementalFundingAgreementSigned.HasValue &&
               !taskData.SupplementalFundingAgreementSent.HasValue &&
               !taskData.SupplementalFundingAgreementSignedSecretaryState.HasValue)
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
            if(!taskData.LandRegistryReceived.HasValue &&
               !taskData.LandRegistryCleared.HasValue &&
               !taskData.LandRegistrySaved.HasValue)
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
            if(!taskData.LandQuestionnaireReceived.HasValue &&
               !taskData.LandQuestionnaireCleared.HasValue &&
               !taskData.LandQuestionnaireSigned.HasValue &&
               !taskData.LandQuestionnaireSaved.HasValue)
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
            if(!taskData.ProposedCapacityOfTheAcademyNotApplicable.HasValue &&
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

        private static TaskListStatus ConfirmMainContactTaskStatus(ConversionTaskDataDto taskData)
        {
            //Get main from coontact table by project id
            return TaskListStatus.NotStarted;
        }

        private static TaskListStatus ChurchSupplementalAgreementTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.ChurchSupplementalAgreementReceived.HasValue &&
                !taskData.ChurchSupplementalAgreementCleared.HasValue &&
                !taskData.ChurchSupplementalAgreementSigned.HasValue &&
                !taskData.ChurchSupplementalAgreementSaved.HasValue &&
                !taskData.ChurchSupplementalAgreementNotApplicable.HasValue &&
                !taskData.ChurchSupplementalAgreementSignedDiocese.HasValue &&
                !taskData.ChurchSupplementalAgreementSignedSecretaryState.HasValue &&
                !taskData.ChurchSupplementalAgreementSent.HasValue)
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
            if(!taskData.ArticlesOfAssociationReceived.HasValue &&
               !taskData.ArticlesOfAssociationCleared.HasValue &&
               !taskData.ArticlesOfAssociationSigned.HasValue &&
               !taskData.ArticlesOfAssociationSaved.HasValue &&
               !taskData.ArticlesOfAssociationNotApplicable.HasValue &&
               !taskData.ArticlesOfAssociationSent.HasValue)
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
               taskData.ArticlesOfAssociationSaved == true &&
               taskData.ArticlesOfAssociationSent == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
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
            return (!string.IsNullOrWhiteSpace(taskData.AcademyDetailsName))
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAndProcessSponsoredSupportGrantTaskStatus(ConversionTaskDataDto taskData)
        {
            if (!taskData.SponsoredSupportGrantInformTrust.HasValue &&
                !taskData.SponsoredSupportGrantPaymentForm.HasValue &&
                !taskData.SponsoredSupportGrantSendInformation.HasValue &&
                !taskData.SponsoredSupportGrantPaymentAmount.HasValue &&
                string.IsNullOrWhiteSpace(taskData.SponsoredSupportGrantType) &&
                !taskData.SponsoredSupportGrantNotApplicable.HasValue )
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
            if(!taskData.ConversionGrantCheckVendorAccount.HasValue &&
                !taskData.ConversionGrantPaymentForm.HasValue &&
                !taskData.ConversionGrantSendInformation.HasValue &&
                !taskData.ConversionGrantSharePaymentDate.HasValue &&
                !taskData.ConversionGrantNotApplicable.HasValue)
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
            return (taskData.CompleteNotificationOfChangeCheckDocument == true && 
                taskData.CompleteNotificationOfChangeSendDocument == true &&
                taskData.CompleteNotificationOfChangeTellLocalAuthority == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus CheckAccuracyOfHigherNeedsTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.CheckAccuracyOfHigherNeedsConfirmNumber.HasValue &&
                !taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber.HasValue)
            {
                return TaskListStatus.NotStarted;
            }
            return (taskData.CheckAccuracyOfHigherNeedsConfirmNumber == true &&
                taskData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber == true)
                ? TaskListStatus.Completed : TaskListStatus.InProgress;
        }

        private static TaskListStatus ConfirmAcademyRiskProtectionArrangementsTaskStatus(ConversionTaskDataDto taskData)
        {
            if(!taskData.RiskProtectionArrangementOption.HasValue &&
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
