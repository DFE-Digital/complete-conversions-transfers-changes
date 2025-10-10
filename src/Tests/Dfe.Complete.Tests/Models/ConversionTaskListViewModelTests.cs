using Dfe.Complete.Application.KeyContacts.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;

namespace Dfe.Complete.Tests.Models
{
    public class ConversionTaskListViewModelTests
    {
        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(true, null, null, null, TaskListStatus.NotApplicable)]
        [InlineData(true, true, true, true, TaskListStatus.NotApplicable)]
        [InlineData(false, null, true, null, TaskListStatus.InProgress)]
        [InlineData(null, null, true, null, TaskListStatus.InProgress)]
        [InlineData(null, true, false, true, TaskListStatus.InProgress)]
        [InlineData(false, true, false, true, TaskListStatus.InProgress)]
        [InlineData(false, true, true, false, TaskListStatus.InProgress)]
        [InlineData(false, true, true, true, TaskListStatus.Completed)]
        [InlineData(null, true, true, true, TaskListStatus.Completed)]
        public void HandoverWithRegionalDeliveryOfficerTaskStatus_ShouldReturns_CorrectResult(bool? handoverNotApplicable, bool? handoverReview, bool? handoverNotes, bool? handoverMeeting, TaskListStatus taskListStatus)
        {
            var model = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                HandoverReview = handoverReview,
                HandoverNotes = handoverNotes,
                HandoverMeeting = handoverMeeting,
                HandoverNotApplicable = handoverNotApplicable
            };
            var project = new ProjectDto(); 
            var result = ConversionTaskListViewModel.Create(model, project, null);
            Assert.Equal(taskListStatus, result.HandoverWithRegionalDeliveryOfficer);
        }

        [Theory]
        [InlineData(false, null, null, null, null, null, TaskListStatus.InProgress)]
        [InlineData(true, false, null, false, null, false, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, TaskListStatus.InProgress)]
        [InlineData(false, true, true, true, true, true, TaskListStatus.Completed)]
        public void ExternalStakeHolderKickoffTaskStatus_ShouldReturn_CorrectStatus(
            bool? significantDateProvisional,
            bool? introEmails,
            bool? proforma,
            bool? setupMeeting,
            bool? meeting,
            bool? checkProvisionalDate,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                StakeholderKickOffIntroductoryEmails = introEmails,
                StakeholderKickOffLocalAuthorityProforma = proforma,
                StakeholderKickOffSetupMeeting = setupMeeting,
                StakeholderKickOffMeeting = meeting,
                StakeholderKickOffCheckProvisionalConversionDate = checkProvisionalDate
            };

            var project = new ProjectDto
            {
                SignificantDateProvisional = significantDateProvisional
            };

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ExternalStakeHolderKickoff);
        }

        [Theory]
        [InlineData(null, null, TaskListStatus.NotStarted)]
        [InlineData(RiskProtectionArrangementOption.Standard, null, TaskListStatus.Completed)]
        [InlineData(RiskProtectionArrangementOption.Commercial, "reason", TaskListStatus.Completed)]
        [InlineData(RiskProtectionArrangementOption.ChurchOrTrust, null, TaskListStatus.Completed)]
        [InlineData(RiskProtectionArrangementOption.Commercial, null, TaskListStatus.InProgress)]
        public void ConfirmAcademyRiskProtectionArrangementsTaskStatus_ShouldReturn_CorrectStatus(
            RiskProtectionArrangementOption? riskProtectionArrangementOption,
            string? riskProtectionArrangementReason,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                RiskProtectionArrangementOption = riskProtectionArrangementOption,
                RiskProtectionArrangementReason = riskProtectionArrangementReason
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmAcademyRiskProtectionArrangements);
        }


        [Theory]
        [InlineData(null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, TaskListStatus.NotStarted)]
        [InlineData(true, false, TaskListStatus.InProgress)]
        [InlineData(false, true, TaskListStatus.InProgress)]
        [InlineData(true, true, TaskListStatus.Completed)]
        public void CheckAccuracyOfHigherNeedsTaskStatus_ShouldReturn_CorrectStatus(
                bool? confirmNumber,
                bool? confirmPublishedNumber,
                TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                CheckAccuracyOfHigherNeedsConfirmNumber = confirmNumber,
                CheckAccuracyOfHigherNeedsConfirmPublishedNumber = confirmPublishedNumber
            };
            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.CheckAccuracyOfHigherNeeds);
        }

        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, true, false, TaskListStatus.Completed)]
        [InlineData(false, false, false, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, true, TaskListStatus.NotApplicable)]
        public void CompleteNotificationOfChangeTaskStatus_ShouldReturn_CorrectStatus(
            bool? checkDocument,
            bool? sendDocument,
            bool? tellLocalAuthority,
            bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                CompleteNotificationOfChangeCheckDocument = checkDocument,
                CompleteNotificationOfChangeSendDocument = sendDocument,
                CompleteNotificationOfChangeTellLocalAuthority = tellLocalAuthority,
                CompleteNotificationOfChangeNotApplicable = notApplicable
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.CompleteNotificationOfChange);
        }
        [Theory]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, true, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(false, false, false, false, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, true, TaskListStatus.NotApplicable)]
        public void ProcessConversionSupportGrantTaskStatus_ShouldReturn_CorrectStatus(
            bool? checkVendorAccount,
            bool? paymentForm,
            bool? sendInformation,
            bool? sharePaymentDate,
            bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ConversionGrantCheckVendorAccount = checkVendorAccount,
                ConversionGrantPaymentForm = paymentForm,
                ConversionGrantSendInformation = sendInformation,
                ConversionGrantSharePaymentDate = sharePaymentDate,
                ConversionGrantNotApplicable = notApplicable
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ProcessConversionSupportGrant);
        }
        [Theory]
        [InlineData(null, null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, "", false, TaskListStatus.NotStarted)]
        [InlineData(true, false, false, false, "", false, TaskListStatus.InProgress)] 
        [InlineData(true, true, true, true, "", false, TaskListStatus.InProgress)]
        [InlineData(true, true, true, true, "GrantType", false, TaskListStatus.Completed)]
        [InlineData(false, false, false, false, "", true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        public void ConfirmAndProcessSponsoredSupportGrantTaskStatus_ShouldReturn_CorrectStatus(
            bool? informTrust,
            bool? paymentForm,
            bool? sendInformation,
            bool? paymentAmount,
            string grantType,
            bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                SponsoredSupportGrantInformTrust = informTrust,
                SponsoredSupportGrantPaymentForm = paymentForm,
                SponsoredSupportGrantSendInformation = sendInformation,
                SponsoredSupportGrantPaymentAmount = paymentAmount,
                SponsoredSupportGrantType = grantType,
                SponsoredSupportGrantNotApplicable = notApplicable
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmAndProcessSponsoredSupportGrant);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("", TaskListStatus.NotStarted)]
        [InlineData("Greenwood Academy", TaskListStatus.Completed)]
        public void ConfirmAcademyNameTaskStatus_ShouldReturn_CorrectStatus(
             string academyName,
             TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                AcademyDetailsName = academyName
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmAcademyName);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("", TaskListStatus.NotStarted)]
        [InlineData("07431436-6e65-4be1-81f5-93c3e1c1a453", TaskListStatus.Completed)]
        public void ConfirmIncomingTrustCeoDetailsTaskStatus_ShouldReturn_CorrectStatus(
            string? incomingTrustCeoId,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var keyContacts = new KeyContactDto
            {
                IncomingTrustCeoId = string.IsNullOrWhiteSpace(incomingTrustCeoId) ? null : new ContactId(Guid.Parse(incomingTrustCeoId))
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, keyContacts);

            Assert.Equal(expectedStatus, result.ConfirmIncomingTrustCeoDetails);
        }

        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("", TaskListStatus.NotStarted)]
        [InlineData("07431436-6e65-4be1-81f5-93c3e1c1a453", TaskListStatus.Completed)]
        public void ConfirmChairOfGovernorsDetailsTaskStatus_ShouldReturn_CorrectStatus(
            string? chairOfGovernorsId,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var keyContacts = new KeyContactDto
            {
                ChairOfGovernorsId = string.IsNullOrWhiteSpace(chairOfGovernorsId) ? null : new ContactId(Guid.Parse(chairOfGovernorsId))
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, keyContacts);

            Assert.Equal(expectedStatus, result.ConfirmChairOfGovernorsDetails);
        }

        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("", TaskListStatus.NotStarted)]
        [InlineData("07431436-6e65-4be1-81f5-93c3e1c1a453", TaskListStatus.Completed)]
        public void ConfirmHeadTeacherDetailsTaskStatus_ShouldReturn_CorrectStatus(
            string? headteacherId,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var keyContacts = new KeyContactDto
            {
                Id = new KeyContactId(Guid.NewGuid()),
                HeadteacherId = string.IsNullOrWhiteSpace(headteacherId) ? null : new ContactId(Guid.Parse(headteacherId))
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, keyContacts);

            Assert.Equal(expectedStatus, result.ConfirmHeadTeacherDetails);
        }

        [Theory]
        [InlineData(false, false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, null, true, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, true, true, true, true, true, true, true, TaskListStatus.NotApplicable)]
        public void ChurchSupplementalAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received,
            bool? cleared,
            bool? signed,
            bool? saved,
            bool? signedDiocese,
            bool? signedSecretaryState,
            bool? sent,
            bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ChurchSupplementalAgreementReceived = received,
                ChurchSupplementalAgreementCleared = cleared,
                ChurchSupplementalAgreementSigned = signed,
                ChurchSupplementalAgreementSaved = saved,
                ChurchSupplementalAgreementNotApplicable = notApplicable,
                ChurchSupplementalAgreementSignedDiocese = signedDiocese,
                ChurchSupplementalAgreementSignedSecretaryState = signedSecretaryState,
                ChurchSupplementalAgreementSent = sent
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ChurchSupplementalAgreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, true, true, true, true, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, true, null, TaskListStatus.InProgress)]
        [InlineData(true, false, false, false, false, false, TaskListStatus.InProgress)]
        public void ArticlesOfAssociationTaskStatus_ShouldReturn_CorrectStatus(
            bool? received,
            bool? cleared,
            bool? signed,
            bool? saved,
            bool? sent,
            bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ArticlesOfAssociationReceived = received,
                ArticlesOfAssociationCleared = cleared,
                ArticlesOfAssociationSigned = signed,
                ArticlesOfAssociationSaved = saved,
                ArticlesOfAssociationNotApplicable = notApplicable,
                ArticlesOfAssociationSent = sent
            };

            var project = new ProjectDto();

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ArticlesOfAssociation);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, false, false, false, TaskListStatus.InProgress)]
        public void MasterFundingAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? saved, bool? signed, bool? sent,
            bool? signedSecretaryState, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                MasterFundingAgreementReceived = received,
                MasterFundingAgreementCleared = cleared,
                MasterFundingAgreementSaved = saved,
                MasterFundingAgreementSigned = signed,
                MasterFundingAgreementSent = sent,
                MasterFundingAgreementSignedSecretaryState = signedSecretaryState,
                MasterFundingAgreementNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.MasterFundingAgreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, false, false, false, TaskListStatus.InProgress)]
        public void DeedOfVariationTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? saved, bool? signed, bool? sent,
            bool? signedSecretaryState, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DeedOfVariationReceived = received,
                DeedOfVariationCleared = cleared,
                DeedOfVariationSaved = saved,
                DeedOfVariationSigned = signed,
                DeedOfVariationSent = sent,
                DeedOfVariationSignedSecretaryState = signedSecretaryState,
                DeedOfVariationNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DeedOfVariation);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, TaskListStatus.InProgress)]
        public void SupplementalFundingAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? saved, bool? signed, bool? sent,
            bool? signedSecretaryState, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                SupplementalFundingAgreementReceived = received,
                SupplementalFundingAgreementCleared = cleared,
                SupplementalFundingAgreementSaved = saved,
                SupplementalFundingAgreementSigned = signed,
                SupplementalFundingAgreementSent = sent,
                SupplementalFundingAgreementSignedSecretaryState = signedSecretaryState
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.SupplementalFundingAgreement);
        }
        [Theory]
        [InlineData(false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, TaskListStatus.InProgress)]
        public void LandRegistryTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? saved, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                LandRegistryReceived = received,
                LandRegistryCleared = cleared,
                LandRegistrySaved = saved
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.LandRegistry);
        }

        [Theory]
        [InlineData(false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, TaskListStatus.InProgress)]
        public void LandQuestionnaireTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? saved, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                LandQuestionnaireReceived = received,
                LandQuestionnaireCleared = cleared,
                LandQuestionnaireSigned = signed,
                LandQuestionnaireSaved = saved
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.LandQuestionnaire);
        }
        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(true, "", "", "", TaskListStatus.NotApplicable)]
        [InlineData(false, "30", "60", "90", TaskListStatus.Completed)]
        [InlineData(false, "30", "", "90", TaskListStatus.InProgress)]
        public void ConfirmProposedCapacityOfTheAcademyTaskStatus_ShouldReturn_CorrectStatus(
            bool? notApplicable, string rTo6, string sevenTo11, string twelvePlus, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ProposedCapacityOfTheAcademyNotApplicable = notApplicable,
                ProposedCapacityOfTheAcademyReceptionToSixYears = rTo6,
                ProposedCapacityOfTheAcademySevenToElevenYears = sevenTo11,
                ProposedCapacityOfTheAcademyTwelveOrAboveYears = twelvePlus
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmProposedCapacityOfTheAcademy);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("21f3a2ef-6266-4dcb-bec3-2e77d61702bb", TaskListStatus.Completed)]
        public void ConfirmMainContactTaskStatus_ShouldReturn_CorrectStatus(
            string mainContactId, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var project = new ProjectDto
            {
                Id = new ProjectId(Guid.NewGuid()), 
                MainContactId = string.IsNullOrWhiteSpace(mainContactId) ? null : new ContactId(Guid.Parse(mainContactId))
            };

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmMainContact);
        }
        [Theory]
        [InlineData(null, null, null, null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, false, false, false, false, TaskListStatus.InProgress)]
        public void TubleasesTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? saved,
            bool? emailSigned, bool? receiveSigned, bool? saveSigned, bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                SubleasesReceived = received,
                SubleasesCleared = cleared,
                SubleasesSigned = signed,
                SubleasesSaved = saved,
                SubleasesEmailSigned = emailSigned,
                SubleasesReceiveSigned = receiveSigned,
                SubleasesSaveSigned = saveSigned,
                SubleasesNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.Tubleases);
        }
        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, TaskListStatus.InProgress)]
        public void OneHundredAndTwentyFiveYearLeaseTaskStatus_ShouldReturn_CorrectStatus(
            bool? saveLease, bool? email, bool? receive, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                OneHundredAndTwentyFiveYearLeaseSaveLease = saveLease,
                OneHundredAndTwentyFiveYearLeaseEmail = email,
                OneHundredAndTwentyFiveYearLeaseReceive = receive,
                OneHundredAndTwentyFiveYearLeaseNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.OneHundredAndTwentyFiveYearLease);
        }
        [Theory]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        public void DirectionToTransferTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? saved, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DirectionToTransferReceived = received,
                DirectionToTransferCleared = cleared,
                DirectionToTransferSigned = signed,
                DirectionToTransferSaved = saved,
                DirectionToTransferNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DirectionToTransfer);
        }
        [Theory]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, true, null, TaskListStatus.InProgress)]
        [InlineData(null, null, null, true, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        public void TrustModificationOrderTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? saved, bool? sentLegal, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                TrustModificationOrderReceived = received,
                TrustModificationOrderCleared = cleared,
                TrustModificationOrderSaved = saved,
                TrustModificationOrderNotApplicable = notApplicable,
                TrustModificationOrderSentLegal = sentLegal
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.TrustModificationOrder);
        }

        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData(false, TaskListStatus.NotStarted)]
        [InlineData(true, TaskListStatus.Completed)]
        public void ShareTheInformationAboutOpeningTaskStatus_ShouldReturn_CorrectStatus(
            bool? email, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ShareInformationEmail = email
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ShareTheInformationAboutOpening);
        }
        [Theory]
        [InlineData(null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, TaskListStatus.NotStarted)]
        [InlineData(true, false, TaskListStatus.InProgress)]
        [InlineData(true, true, TaskListStatus.Completed)]
        public void ConfirmTheSchoolHasCompletedAllActionsTaskStatus_ShouldReturn_CorrectStatus(
            bool? emailed, bool? saved, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                SchoolCompletedEmailed = emailed,
                SchoolCompletedSaved = saved
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmTheSchoolHasCompletedAllActions);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData(false, TaskListStatus.NotStarted)]
        [InlineData(true, TaskListStatus.Completed)]
        public void ConfirmAllConditionsHaveBeenMetTaskStatus_ShouldReturn_CorrectStatus(
         bool? allConditionsMet, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var project = new ProjectDto
            {
                AllConditionsMet = allConditionsMet
            };

            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmAllConditionsHaveBeenMet);
        }
        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, false, TaskListStatus.Completed)]
        [InlineData(null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(true, false, false, false, TaskListStatus.InProgress)]
        public void TenancyAtWillTaskStatus_ShouldReturn_CorrectStatus(
            bool? emailSigned, bool? receiveSigned, bool? saveSigned, bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                TenancyAtWillEmailSigned = emailSigned,
                TenancyAtWillReceiveSigned = receiveSigned,
                TenancyAtWillSaveSigned = saveSigned,
                TenancyAtWillNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.TenancyAtWill);
        }
        [Theory]
        [InlineData(false, false, null, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, "2025-08-01", false, TaskListStatus.Completed)]
        [InlineData(true, false, null, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, true, TaskListStatus.NotApplicable)]
        public void ProjectReceiveDeclarationOfExpenditureCertificateTaskStatus_ShouldReturn_CorrectStatus(
            bool? checkCertificate, bool? saveCertificate, string? dateReceivedStr, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ReceiveGrantPaymentCertificateCheckCertificate = checkCertificate,
                ReceiveGrantPaymentCertificateSaveCertificate = saveCertificate,
                ReceiveGrantPaymentCertificateDateReceived = string.IsNullOrWhiteSpace(dateReceivedStr) ? null : DateOnly.Parse(dateReceivedStr),
                ReceiveGrantPaymentCertificateNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ProjectReceiveDeclarationOfExpenditureCertificate);
        }
        [Theory]
        [InlineData(false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, TaskListStatus.InProgress)]
        public void RedactAndSendDocumentsTaskStatus_ShouldReturn_CorrectStatus(
            bool? redact, bool? saveRedaction, bool? sendRedaction, bool? sendSolicitors, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                RedactAndSendRedact = redact,
                RedactAndSendSaveRedaction = saveRedaction,
                RedactAndSendSendRedaction = sendRedaction,
                RedactAndSendSendSolicitors = sendSolicitors
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.RedactAndSendDocuments);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("2025-09-01", TaskListStatus.Completed)]
        public void ConfirmDateAcademyOpenedTaskStatus_ShouldReturn_CorrectStatus(
            string? dateOpenedStr, TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ConfirmDateAcademyOpenedDateOpened = string.IsNullOrWhiteSpace(dateOpenedStr) ? null : DateOnly.Parse(dateOpenedStr)
            };

            var project = new ProjectDto();
            var result = ConversionTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmDateAcademyOpened);
        }

    }
}
