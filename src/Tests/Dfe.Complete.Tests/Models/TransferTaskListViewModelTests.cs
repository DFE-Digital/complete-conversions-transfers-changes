using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;

namespace Dfe.Complete.Tests.Models
{
    public class TransferTaskListViewModelTests
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
            var model = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                HandoverReview = handoverReview,
                HandoverNotes = handoverNotes,
                HandoverMeeting = handoverMeeting,
                HandoverNotApplicable = handoverNotApplicable
            };
            var projectDto = new ProjectDto();
            var result = TransferTaskListViewModel.Create(model, projectDto, null);
            Assert.Equal(taskListStatus, result.HandoverWithRegionalDeliveryOfficer);
        }
        [Theory]
        [InlineData(false, true, false, false, TaskListStatus.InProgress)]
        [InlineData(false, false, true, false, TaskListStatus.InProgress)]
        [InlineData(false, false, false, true, TaskListStatus.InProgress)]
        [InlineData(false, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, TaskListStatus.NotStarted)]
        public void ExternalStakeHolderKickoffTaskStatus_ShouldReturn_CorrectStatus(
            bool? significantDateProvisional,
            bool? introEmails,
            bool? setupMeeting,
            bool? meeting,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                StakeholderKickOffIntroductoryEmails = introEmails,
                StakeholderKickOffSetupMeeting = setupMeeting,
                StakeholderKickOffMeeting = meeting
            };

            var project = new ProjectDto
            {
                SignificantDateProvisional = significantDateProvisional
            };

            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ExternalStakeHolderKickoff);
        }
        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, null, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, "2025-08-01", false, TaskListStatus.Completed)]
        [InlineData(true, false, null, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, true, TaskListStatus.NotApplicable)]
        public void DeclarationOfExpenditureCertificateTaskStatus_ShouldReturn_CorrectStatus(
            bool? correct, bool? saved, string? dateReceivedStr, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DeclarationOfExpenditureCertificateCorrect = correct,
                DeclarationOfExpenditureCertificateSaved = saved,
                DeclarationOfExpenditureCertificateDateReceived = string.IsNullOrWhiteSpace(dateReceivedStr) ? null : DateOnly.Parse(dateReceivedStr),
                DeclarationOfExpenditureCertificateNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DeclarationOfExpenditureCertificate);
        }
        [Theory]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        public void RedactAndSendDocumentsTaskStatus_ShouldReturn_CorrectStatus(
            bool? redact, bool? saved, bool? sendEsfa, bool? sendFundingTeam, bool? sendSolicitors, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                RedactAndSendDocumentsRedact = redact,
                RedactAndSendDocumentsSaved = saved,
                RedactAndSendDocumentsSendToEsfa = sendEsfa,
                RedactAndSendDocumentsSendToFundingTeam = sendFundingTeam,
                RedactAndSendDocumentsSendToSolicitors = sendSolicitors
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.RedactAndSendDocuments);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("2025-09-01", TaskListStatus.Completed)]
        public void ConfirmDateAcademyTransferredTaskStatus_ShouldReturn_CorrectStatus(
         string? dateTransferredStr, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ConfirmDateAcademyTransferredDateTransferred = string.IsNullOrWhiteSpace(dateTransferredStr) ? null : DateOnly.Parse(dateTransferredStr)
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmDateAcademyTransferred);
        }
        [Theory]
        [InlineData(true, false, false, TaskListStatus.InProgress)]
        [InlineData(false, true, false, TaskListStatus.InProgress)]
        [InlineData(false, false, true, TaskListStatus.InProgress)]
        [InlineData(true, true, true, TaskListStatus.Completed)]
        [InlineData(false, false, false, TaskListStatus.NotStarted)]
        [InlineData(null, null, null, TaskListStatus.NotStarted)]
        public void ConfirmThisTransferHasAuthorityToProceedTaskStatus_ShouldReturn_CorrectStatus(
            bool? allConditionsMet, bool? baselineApproved, bool? infoChanged, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ConditionsMetBaselineSheetApproved = baselineApproved,
                ConditionsMetCheckAnyInformationChanged = infoChanged
            };

            var project = new ProjectDto
            {
                AllConditionsMet = allConditionsMet
            };

            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmThisTransferHasAuthorityToProceed);
        }
        [Theory]
        [InlineData(null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, TaskListStatus.NotStarted)]
        [InlineData(true, false, TaskListStatus.InProgress)]
        [InlineData(true, true, TaskListStatus.Completed)]
        public void ConfirmIncomingTrustHasCompletedAllActionsTaskStatus_ShouldReturn_CorrectStatus(
            bool? emailed, bool? saved, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ConfirmIncomingTrustHasCompletedAllActionsEmailed = emailed,
                ConfirmIncomingTrustHasCompletedAllActionsSaved = saved
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmIncomingTrustHasCompletedAllActions);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData(true, TaskListStatus.Completed)]
        [InlineData(false, TaskListStatus.Completed)]
        public void ConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTaskStatus_ShouldReturn_CorrectStatus(
            bool? bankDetailsChanging, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                BankDetailsChangingYesNo = bankDetailsChanging
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmBankDetailsChangingForGeneralAnnualGrantPayment);
        }
        [Theory]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, true, TaskListStatus.NotApplicable)]
        public void ClosureOrTransferDeclarationTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? sent, bool? saved, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ClosureOrTransferDeclarationReceived = received,
                ClosureOrTransferDeclarationCleared = cleared,
                ClosureOrTransferDeclarationSent = sent,
                ClosureOrTransferDeclarationSaved = saved,
                ClosureOrTransferDeclarationNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ClosureOrTransferDeclaration);
        }
        [Theory]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        public void CommercialTransferAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? agreed, bool? signed, bool? questionsChecked, bool? questionsReceived, bool? savedEmails, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                CommercialTransferAgreementConfirmAgreed = agreed,
                CommercialTransferAgreementConfirmSigned = signed,
                CommercialTransferAgreementQuestionsChecked = questionsChecked,
                CommercialTransferAgreementQuestionsReceived = questionsReceived,
                CommercialTransferAgreementSaveConfirmationEmails = savedEmails
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.CommercialTransferAgreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void DeedOfTerminationForChurchSupplementalAreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signedOutgoingTrust, bool? signedDiocese,
            bool? saved, bool? signedSecretaryState, bool? savedAfterSigning, bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DeedTerminationChurchAgreementReceived = received,
                DeedTerminationChurchAgreementCleared = cleared,
                DeedTerminationChurchAgreementSignedOutgoingTrust = signedOutgoingTrust,
                DeedTerminationChurchAgreementSignedDiocese = signedDiocese,
                DeedTerminationChurchAgreementSaved = saved,
                DeedTerminationChurchAgreementSignedSecretaryState = signedSecretaryState,
                DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState = savedAfterSigning,
                DeedTerminationChurchAgreementNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DeedOfTerminationForChurchSupplementalAreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void DeedOfTerminationForTheMasterFundingAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? savedAcademyOutgoingTrust,
            bool? contactFinancialReportingTeam, bool? signedSecretaryState, bool? savedInAcademyFolder,
            bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DeedOfTerminationForTheMasterFundingAgreementReceived = received,
                DeedOfTerminationForTheMasterFundingAgreementCleared = cleared,
                DeedOfTerminationForTheMasterFundingAgreementSigned = signed,
                DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint = savedAcademyOutgoingTrust,
                DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam = contactFinancialReportingTeam,
                DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState = signedSecretaryState,
                DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder = savedInAcademyFolder,
                DeedOfTerminationForTheMasterFundingAgreementNotApplicable = notApplicable
            };
            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DeedOfTerminationForTheMasterFundingAgreement);
        }

        [Theory]
        [InlineData(false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void DeedOfVariationTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? saved,
            bool? sent, bool? signedSecretaryState, bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DeedOfVariationReceived = received,
                DeedOfVariationCleared = cleared,
                DeedOfVariationSigned = signed,
                DeedOfVariationSaved = saved,
                DeedOfVariationSent = sent,
                DeedOfVariationSignedSecretaryState = signedSecretaryState,
                DeedOfVariationNotApplicable = notApplicable
            };
            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DeedoOfVariation);
        }
        [Theory]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        public void ArticlesOfAssociationTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? saved,
            bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ArticlesOfAssociationReceived = received,
                ArticlesOfAssociationCleared = cleared,
                ArticlesOfAssociationSigned = signed,
                ArticlesOfAssociationSaved = saved,
                ArticlesOfAssociationNotApplicable = notApplicable
            };
            var project = new ProjectDto();

            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ArticlesOfAssociation);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void MasterFundingAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signed, bool? saved,
            bool? signedSecretaryState, bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                MasterFundingAgreementReceived = received,
                MasterFundingAgreementCleared = cleared,
                MasterFundingAgreementSigned = signed,
                MasterFundingAgreementSaved = saved,
                MasterFundingAgreementSignedSecretaryState = signedSecretaryState,
                MasterFundingAgreementNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.MasterFundingAgreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void ChurchSupplementalAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signedIncomingTrust, bool? signedDiocese,
            bool? savedByTrustDiocese, bool? signedSecretaryState, bool? savedBySecretaryState,
            bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                ChurchSupplementalAgreementReceived = received,
                ChurchSupplementalAgreementCleared = cleared,
                ChurchSupplementalAgreementSignedIncomingTrust = signedIncomingTrust,
                ChurchSupplementalAgreementSignedDiocese = signedDiocese,
                ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese = savedByTrustDiocese,
                ChurchSupplementalAgreementSignedSecretaryState = signedSecretaryState,
                ChurchSupplementalAgreementSavedAfterSigningBySecretaryState = savedBySecretaryState,
                ChurchSupplementalAgreementNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ChurchSupplementalAgreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void DeedOfNovationAndVariationTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? signedOutgoingTrust, bool? signedIncomingTrust,
            bool? saved, bool? signedSecretaryState, bool? savedAfterSign,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                DeedOfNovationAndVariationReceived = received,
                DeedOfNovationAndVariationCleared = cleared,
                DeedOfNovationAndVariationSignedOutgoingTrust = signedOutgoingTrust,
                DeedOfNovationAndVariationSignedIncomingTrust = signedIncomingTrust,
                DeedOfNovationAndVariationSaved = saved,
                DeedOfNovationAndVariationSignedSecretaryState = signedSecretaryState,
                DeedOfNovationAndVariationSaveAfterSign = savedAfterSign
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.DeedOfNovationAndVariation);
        }
        [Theory]
        [InlineData(false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, TaskListStatus.Completed)]
        [InlineData(true, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, TaskListStatus.NotStarted)]
        public void SupplementalFundingAgreementTaskStatus_ShouldReturn_CorrectStatus(
            bool? received, bool? cleared, bool? saved,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                SupplementalFundingAgreementReceived = received,
                SupplementalFundingAgreementCleared = cleared,
                SupplementalFundingAgreementSaved = saved
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.SupplementalFundingAgreement);
        }
        [Theory]
        [InlineData(false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, TaskListStatus.NotStarted)]
        public void LandConsentLetterTaskStatus_ShouldReturn_CorrectStatus(
            bool? drafted, bool? sent, bool? signed, bool? saved,
            bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                LandConsentLetterDrafted = drafted,
                LandConsentLetterSent = sent,
                LandConsentLetterSigned = signed,
                LandConsentLetterSaved = saved,
                LandConsentLetterNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.LandConsentLetter);
        }
        [Theory]
        [InlineData(false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, true, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, null, null, TaskListStatus.NotStarted)]
        public void FormMTaskStatus_ShouldReturn_CorrectStatus(
            bool? receivedTitlePlans, bool? cleared, bool? signed, bool? saved,
            bool? receivedFormM, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                FormMReceivedTitlePlans = receivedTitlePlans,
                FormMCleared = cleared,
                FormMSigned = signed,
                FormMSaved = saved,
                FormMReceivedFormM = receivedFormM,
                FormMNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.FormM);
        }
        [Theory]
        [InlineData("", "", false, TaskListStatus.NotStarted)]
        [InlineData("Surplus", "Deficit", false, TaskListStatus.Completed)]
        [InlineData("Surplus", "", false, TaskListStatus.InProgress)]
        [InlineData("", "", true, TaskListStatus.NotApplicable)]
        public void CheckAndConfirmAcademyAndTrustFinancialInfoTaskStatus_ShouldReturn_CorrectStatus(
             string academySurplusDeficit, string trustSurplusDeficit,
             bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                CheckAndConfirmFinancialInformationAcademySurplusDeficit = academySurplusDeficit,
                CheckAndConfirmFinancialInformationTrustSurplusDeficit = trustSurplusDeficit,
                CheckAndConfirmFinancialInformationNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.CheckAndConfirmAcademyAndTrustFinancialInfo);
        }
        [Theory]
        [InlineData("", false, TaskListStatus.NotStarted)]
        [InlineData("Type A", false, TaskListStatus.Completed)]
        [InlineData("", true, TaskListStatus.NotApplicable)]
        public void ConfirmTransferGrantFundingLevelTaskStatus_ShouldReturn_CorrectStatus(
             string grantType, bool? notApplicable, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                SponsoredSupportGrantType = grantType,
                SponsoredSupportGrantNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmTransferGrantFundingLevel);
        }
        [Theory]
        [InlineData(false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, true, false, TaskListStatus.Completed)]
        [InlineData(true, false, false, false, TaskListStatus.InProgress)]
        [InlineData(null, null, null, true, TaskListStatus.NotApplicable)]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        public void RequestNewURNAndRecordForTheAcademyTaskStatus_ShouldReturn_CorrectStatus(
            bool? complete, bool? give, bool? receive, bool? notApplicable,
            TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                RequestNewUrnAndRecordComplete = complete,
                RequestNewUrnAndRecordGive = give,
                RequestNewUrnAndRecordReceive = receive,
                RequestNewUrnAndRecordNotApplicable = notApplicable
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.RequestNewURNAndRecordForTheAcademy);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("a2ab584b-b549-443b-9bec-7b40cf066fdb", TaskListStatus.Completed)]
        public void ConfirmOutgoingTrustCeoDetailsTaskStatus_ShouldReturn_CorrectStatus(
            string outgoingTrustCeoId, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var project = new ProjectDto();
            var keyContacts = new KeyContactDto
            {
                OutgoingTrustCeoId = string.IsNullOrWhiteSpace(outgoingTrustCeoId) ? null : new ContactId(Guid.Parse(outgoingTrustCeoId))
            };

            var result = TransferTaskListViewModel.Create(taskData, project, keyContacts);

            Assert.Equal(expectedStatus, result.ConfirmOutgoingTrustCeoDetails);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("a2ab584b-b549-443b-9bec-7b40cf066fdb", TaskListStatus.Completed)]
        public void ConfirmIncomingTrustCeoDetailsTaskStatus_ShouldReturn_CorrectStatus(
            string incomingTrustCeoId, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var project = new ProjectDto();
            var keyContacts = new KeyContactDto
            {
                IncomingTrustCeoId = string.IsNullOrEmpty(incomingTrustCeoId) ? null : new ContactId(Guid.Parse(incomingTrustCeoId))
            };

            var result = TransferTaskListViewModel.Create(taskData, project, keyContacts);

            Assert.Equal(expectedStatus, result.ConfirmIncomingTrustCeoDetails);
        }
        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("a2ab584b-b549-443b-9bec-7b40cf066fdb", TaskListStatus.Completed)]
        public void ConfirmMainContactTaskStatus_ShouldReturn_CorrectStatus(
            string mainContactId, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var project = new ProjectDto
            {
                MainContactId = string.IsNullOrEmpty(mainContactId) ? null : new ContactId(Guid.Parse(mainContactId))
            };

            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmMainContact);
        }

        [Theory]
        [InlineData(null, TaskListStatus.NotStarted)]
        [InlineData("a2ab584b-b549-443b-9bec-7b40cf066fdb", TaskListStatus.Completed)]
        public void ConfirmHeadTeacherDetailsTaskStatus_ShouldReturn_CorrectStatus(
        string headteacherId, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid())
            };

            var project = new ProjectDto();
            var keyContacts = new KeyContactDto
            {
                HeadteacherId = string.IsNullOrEmpty(headteacherId) ? null : new ContactId(Guid.Parse(headteacherId))
            };

            var result = TransferTaskListViewModel.Create(taskData, project, keyContacts);

            Assert.Equal(expectedStatus, result.ConfirmHeadTeacherDetails);
        }
        [Theory]
        [InlineData(false, TaskListStatus.NotStarted)]
        [InlineData(true, TaskListStatus.Completed)]
        public void ConfirmAcademyRiskProtectionArrangementsTaskStatus_ShouldReturn_CorrectStatus(
            bool rpaPolicyConfirm, TaskListStatus expectedStatus)
        {
            var taskData = new TransferTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                RpaPolicyConfirm = rpaPolicyConfirm
            };

            var project = new ProjectDto();
            var result = TransferTaskListViewModel.Create(taskData, project, null);

            Assert.Equal(expectedStatus, result.ConfirmAcademyRiskProtectionArrangements);
        }
    }
}
