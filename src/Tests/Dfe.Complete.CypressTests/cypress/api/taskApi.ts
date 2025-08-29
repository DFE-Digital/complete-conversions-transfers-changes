import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

interface TaskDataIdObject {
    value: string;
}

interface UpdateHandoverWithDeliveryOfficerTaskRequest {
    taskDataId: TaskDataIdObject;
    projectType: ProjectType;
    notApplicable?: boolean;
    handoverReview?: boolean;
    handoverNotes?: boolean;
    handoverMeetings?: boolean;
}

interface UpdateArticleOfAssociationTaskRequest {
    taskDataId: TaskDataIdObject;
    projectType: ProjectType;
    notApplicable?: boolean;
    cleared?: boolean;
    received?: boolean;
    sent?: boolean;
    signed?: boolean;
    saved?: boolean;
}

export enum ProjectType {
    Conversion = "Conversion",
    Transfer = "Transfer",
}

export enum RiskProtectionArrangementOption {
    Standard = "Standard",
    ChurchOrTrust = "ChurchOrTrust",
    Commercial = "Commercial",
}

export interface TaskDataId {
    value: string;
}

export interface TransferTaskDataDto {
    id: TaskDataId;
    createdAt: string;
    updatedAt: string;
    handoverReview?: boolean;
    handoverNotes?: boolean;
    handoverMeeting?: boolean;
    handoverNotApplicable?: boolean;
    stakeholderKickOffIntroductoryEmails?: boolean;
    stakeholderKickOffSetupMeeting?: boolean;
    stakeholderKickOffMeeting?: boolean;
    masterFundingAgreementReceived?: boolean;
    masterFundingAgreementCleared?: boolean;
    masterFundingAgreementSigned?: boolean;
    masterFundingAgreementSaved?: boolean;
    masterFundingAgreementSignedSecretaryState?: boolean;
    masterFundingAgreementNotApplicable?: boolean;
    deedOfNovationAndVariationReceived?: boolean;
    deedOfNovationAndVariationCleared?: boolean;
    deedOfNovationAndVariationSignedOutgoingTrust?: boolean;
    deedOfNovationAndVariationSignedIncomingTrust?: boolean;
    deedOfNovationAndVariationSaved?: boolean;
    deedOfNovationAndVariationSignedSecretaryState?: boolean;
    deedOfNovationAndVariationSaveAfterSign?: boolean;
    articlesOfAssociationReceived?: boolean;
    articlesOfAssociationCleared?: boolean;
    articlesOfAssociationSigned?: boolean;
    articlesOfAssociationSaved?: boolean;
    articlesOfAssociationNotApplicable?: boolean;
    commercialTransferAgreementConfirmAgreed?: boolean;
    commercialTransferAgreementConfirmSigned?: boolean;
    commercialTransferAgreementSaveConfirmationEmails?: boolean;
    supplementalFundingAgreementReceived?: boolean;
    supplementalFundingAgreementCleared?: boolean;
    supplementalFundingAgreementSaved?: boolean;
    deedOfVariationReceived?: boolean;
    deedOfVariationCleared?: boolean;
    deedOfVariationSigned?: boolean;
    deedOfVariationSaved?: boolean;
    deedOfVariationSent?: boolean;
    deedOfVariationSignedSecretaryState?: boolean;
    deedOfVariationNotApplicable?: boolean;
    landConsentLetterDrafted?: boolean;
    landConsentLetterSigned?: boolean;
    landConsentLetterSent?: boolean;
    landConsentLetterSaved?: boolean;
    landConsentLetterNotApplicable?: boolean;
    rpaPolicyConfirm?: boolean;
    formMReceivedFormM?: boolean;
    formMReceivedTitlePlans?: boolean;
    formMCleared?: boolean;
    formMSigned?: boolean;
    formMSaved?: boolean;
    churchSupplementalAgreementReceived?: boolean;
    churchSupplementalAgreementCleared?: boolean;
    churchSupplementalAgreementSignedIncomingTrust?: boolean;
    churchSupplementalAgreementSignedDiocese?: boolean;
    churchSupplementalAgreementSavedAfterSigningByTrustDiocese?: boolean;
    churchSupplementalAgreementSignedSecretaryState?: boolean;
    churchSupplementalAgreementSavedAfterSigningBySecretaryState?: boolean;
    churchSupplementalAgreementNotApplicable?: boolean;
    deedOfTerminationForTheMasterFundingAgreementReceived?: boolean;
    deedOfTerminationForTheMasterFundingAgreementCleared?: boolean;
    deedOfTerminationForTheMasterFundingAgreementSigned?: boolean;
    deedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint?: boolean;
    deedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam?: boolean;
    deedOfTerminationForTheMasterFundingAgreementSignedSecretaryState?: boolean;
    deedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder?: boolean;
    deedOfTerminationForTheMasterFundingAgreementNotApplicable?: boolean;
    deedTerminationChurchAgreementReceived?: boolean;
    deedTerminationChurchAgreementCleared?: boolean;
    deedTerminationChurchAgreementSignedOutgoingTrust?: boolean;
    deedTerminationChurchAgreementSignedDiocese?: boolean;
    deedTerminationChurchAgreementSaved?: boolean;
    deedTerminationChurchAgreementSignedSecretaryState?: boolean;
    deedTerminationChurchAgreementSavedAfterSigningBySecretaryState?: boolean;
    deedTerminationChurchAgreementNotApplicable?: boolean;
    closureOrTransferDeclarationNotApplicable?: boolean;
    closureOrTransferDeclarationReceived?: boolean;
    closureOrTransferDeclarationCleared?: boolean;
    closureOrTransferDeclarationSaved?: boolean;
    closureOrTransferDeclarationSent?: boolean;
    confirmIncomingTrustHasCompletedAllActionsEmailed?: boolean;
    confirmIncomingTrustHasCompletedAllActionsSaved?: boolean;
    redactAndSendDocumentsSendToEsfa?: boolean;
    redactAndSendDocumentsRedact?: boolean;
    redactAndSendDocumentsSaved?: boolean;
    redactAndSendDocumentsSendToFundingTeam?: boolean;
    redactAndSendDocumentsSendToSolicitors?: boolean;
    requestNewUrnAndRecordNotApplicable?: boolean;
    requestNewUrnAndRecordComplete?: boolean;
    requestNewUrnAndRecordReceive?: boolean;
    requestNewUrnAndRecordGive?: boolean;
    inadequateOfsted?: boolean;
    financialSafeguardingGovernanceIssues?: boolean;
    outgoingTrustToClose?: boolean;
    bankDetailsChangingYesNo?: boolean;
    checkAndConfirmFinancialInformationNotApplicable?: boolean;
    checkAndConfirmFinancialInformationAcademySurplusDeficit?: string;
    checkAndConfirmFinancialInformationTrustSurplusDeficit?: string;
    confirmDateAcademyTransferredDateTransferred?: string;
    sponsoredSupportGrantNotApplicable?: boolean;
    sponsoredSupportGrantType?: string;
    declarationOfExpenditureCertificateDateReceived?: string;
    declarationOfExpenditureCertificateCorrect?: boolean;
    declarationOfExpenditureCertificateSaved?: boolean;
    declarationOfExpenditureCertificateNotApplicable?: boolean;
    conditionsMetCheckAnyInformationChanged?: boolean;
    conditionsMetBaselineSheetApproved?: boolean;
    formMNotApplicable?: boolean;
    articlesOfAssociationSent?: boolean;
    commercialTransferAgreementQuestionsReceived?: boolean;
    commercialTransferAgreementQuestionsChecked?: boolean;
}

export interface ConversionTaskDataDto {
    id: TaskDataId;
    handoverReview?: boolean;
    handoverNotes?: boolean;
    handoverMeeting?: boolean;
    createdAt: string;
    updatedAt: string;
    stakeholderKickOffIntroductoryEmails?: boolean;
    stakeholderKickOffLocalAuthorityProforma?: boolean;
    stakeholderKickOffSetupMeeting?: boolean;
    stakeholderKickOffMeeting?: boolean;
    conversionGrantCheckVendorAccount?: boolean;
    conversionGrantPaymentForm?: boolean;
    conversionGrantSendInformation?: boolean;
    conversionGrantSharePaymentDate?: boolean;
    landQuestionnaireReceived?: boolean;
    landQuestionnaireCleared?: boolean;
    landQuestionnaireSigned?: boolean;
    landQuestionnaireSaved?: boolean;
    landRegistryReceived?: boolean;
    landRegistryCleared?: boolean;
    landRegistrySaved?: boolean;
    supplementalFundingAgreementReceived?: boolean;
    supplementalFundingAgreementCleared?: boolean;
    supplementalFundingAgreementSigned?: boolean;
    supplementalFundingAgreementSaved?: boolean;
    supplementalFundingAgreementSent?: boolean;
    supplementalFundingAgreementSignedSecretaryState?: boolean;
    churchSupplementalAgreementReceived?: boolean;
    churchSupplementalAgreementCleared?: boolean;
    churchSupplementalAgreementSigned?: boolean;
    churchSupplementalAgreementSignedDiocese?: boolean;
    churchSupplementalAgreementSaved?: boolean;
    churchSupplementalAgreementSent?: boolean;
    churchSupplementalAgreementSignedSecretaryState?: boolean;
    masterFundingAgreementReceived?: boolean;
    masterFundingAgreementCleared?: boolean;
    masterFundingAgreementSigned?: boolean;
    masterFundingAgreementSaved?: boolean;
    masterFundingAgreementSent?: boolean;
    masterFundingAgreementSignedSecretaryState?: boolean;
    articlesOfAssociationReceived?: boolean;
    articlesOfAssociationCleared?: boolean;
    articlesOfAssociationSigned?: boolean;
    articlesOfAssociationSaved?: boolean;
    deedOfVariationReceived?: boolean;
    deedOfVariationCleared?: boolean;
    deedOfVariationSigned?: boolean;
    deedOfVariationSaved?: boolean;
    deedOfVariationSent?: boolean;
    deedOfVariationSignedSecretaryState?: boolean;
    trustModificationOrderReceived?: boolean;
    trustModificationOrderSentLegal?: boolean;
    trustModificationOrderCleared?: boolean;
    trustModificationOrderSaved?: boolean;
    directionToTransferReceived?: boolean;
    directionToTransferCleared?: boolean;
    directionToTransferSigned?: boolean;
    directionToTransferSaved?: boolean;
    schoolCompletedEmailed?: boolean;
    schoolCompletedSaved?: boolean;
    redactAndSendRedact?: boolean;
    redactAndSendSaveRedaction?: boolean;
    redactAndSendSendRedaction?: boolean;
    updateEsfaUpdate?: boolean;
    receiveGrantPaymentCertificateSaveCertificate?: boolean;
    oneHundredAndTwentyFiveYearLeaseEmail?: boolean;
    oneHundredAndTwentyFiveYearLeaseReceive?: boolean;
    oneHundredAndTwentyFiveYearLeaseSaveLease?: boolean;
    subleasesReceived?: boolean;
    subleasesCleared?: boolean;
    subleasesSigned?: boolean;
    subleasesSaved?: boolean;
    subleasesEmailSigned?: boolean;
    subleasesReceiveSigned?: boolean;
    subleasesSaveSigned?: boolean;
    tenancyAtWillEmailSigned?: boolean;
    tenancyAtWillReceiveSigned?: boolean;
    tenancyAtWillSaveSigned?: boolean;
    shareInformationEmail?: boolean;
    redactAndSendSendSolicitors?: boolean;
    articlesOfAssociationNotApplicable?: boolean;
    churchSupplementalAgreementNotApplicable?: boolean;
    deedOfVariationNotApplicable?: boolean;
    directionToTransferNotApplicable?: boolean;
    masterFundingAgreementNotApplicable?: boolean;
    oneHundredAndTwentyFiveYearLeaseNotApplicable?: boolean;
    subleasesNotApplicable?: boolean;
    tenancyAtWillNotApplicable?: boolean;
    trustModificationOrderNotApplicable?: boolean;
    stakeholderKickOffCheckProvisionalConversionDate?: boolean;
    conversionGrantNotApplicable?: boolean;
    sponsoredSupportGrantPaymentAmount?: boolean;
    sponsoredSupportGrantPaymentForm?: boolean;
    sponsoredSupportGrantSendInformation?: boolean;
    sponsoredSupportGrantInformTrust?: boolean;
    sponsoredSupportGrantNotApplicable?: boolean;
    handoverNotApplicable?: boolean;
    academyDetailsName?: string;
    riskProtectionArrangementOption?: RiskProtectionArrangementOption;
    checkAccuracyOfHigherNeedsConfirmNumber?: boolean;
    checkAccuracyOfHigherNeedsConfirmPublishedNumber?: boolean;
    completeNotificationOfChangeNotApplicable?: boolean;
    completeNotificationOfChangeTellLocalAuthority?: boolean;
    completeNotificationOfChangeCheckDocument?: boolean;
    completeNotificationOfChangeSendDocument?: boolean;
    sponsoredSupportGrantType?: string;
    proposedCapacityOfTheAcademyNotApplicable?: boolean;
    proposedCapacityOfTheAcademyReceptionToSixYears?: string;
    proposedCapacityOfTheAcademySevenToElevenYears?: string;
    proposedCapacityOfTheAcademyTwelveOrAboveYears?: string;
    receiveGrantPaymentCertificateDateReceived?: string;
    receiveGrantPaymentCertificateCheckCertificate?: boolean;
    receiveGrantPaymentCertificateNotApplicable?: boolean;
    confirmDateAcademyOpenedDateOpened?: string;
    riskProtectionArrangementReason?: string;
    articlesOfAssociationSent?: boolean;
    commercialTransferAgreementAgreed?: boolean;
    commercialTransferAgreementSigned?: boolean;
    commercialTransferAgreementQuestionsReceived?: boolean;
    commercialTransferAgreementQuestionsChecked?: boolean;
    commercialTransferAgreementSaved?: boolean;
}

class TaskApi extends ApiBase {
    private readonly taskDataUrl = `${Cypress.env(EnvApi)}/v1/TasksData/TaskData`;

    public getTransferTaskData(taskDataId: string): Cypress.Chainable<TransferTaskDataDto> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<TransferTaskDataDto>({
                    method: "GET",
                    url: `${this.taskDataUrl}/Transfer?Id.Value=${taskDataId}`,
                    headers,
                })
                .then((response) => {
                    expect(response.status, `Expected GET request to return 200 but got ${response.status}`).to.eq(200);
                    return response.body;
                });
        });
    }

    public getConversionTaskData(taskDataId: string): Cypress.Chainable<ConversionTaskDataDto> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<ConversionTaskDataDto>({
                    method: "GET",
                    url: `${this.taskDataUrl}/Conversion?Id.Value=${taskDataId}`,
                    headers,
                })
                .then((response) => {
                    expect(response.status, `Expected GET request to return 200 but got ${response.status}`).to.eq(200);
                    return response.body;
                });
        });
    }

    public updateHandoverWithDeliveryOfficerTask(
        taskDataId: string,
        projectType: ProjectType,
        notApplicable = false,
        handoverReview = false,
        handoverNotes = false,
        handoverMeetings = false,
    ) {
        const requestBody: UpdateHandoverWithDeliveryOfficerTaskRequest = {
            taskDataId: { value: taskDataId },
            notApplicable,
            handoverReview,
            handoverNotes,
            handoverMeetings,
            projectType,
        };

        return this.taskDataBaseRequest<void>("PATCH", `${this.taskDataUrl}/HandoverDeliveryOfficer`, requestBody, 204);
    }

    public updateArticleOfAssociationTask(
        taskDataId: string,
        projectType: ProjectType,
        notApplicable = false,
        cleared = false,
        received = false,
        sent = false,
        signed = false,
        saved = false,
    ) {
        const requestBody: UpdateArticleOfAssociationTaskRequest = {
            taskDataId: { value: taskDataId },
            notApplicable,
            cleared,
            received,
            sent,
            signed,
            saved,
            projectType,
        };

        return this.taskDataBaseRequest<void>("PATCH", `${this.taskDataUrl}/ArticleOfAssociation`, requestBody, 204);
    }

    private taskDataBaseRequest<T>(
        method: string,
        url: string,
        body: UpdateHandoverWithDeliveryOfficerTaskRequest | UpdateArticleOfAssociationTaskRequest,
        expectedStatus: number,
    ) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<T>({
                    method,
                    url,
                    headers,
                    body,
                })
                .then((response) => {
                    expect(
                        response.status,
                        `Expected ${method} request to return ${expectedStatus} but got ${response.status}`,
                    ).to.eq(expectedStatus);
                    return response.body;
                });
        });
    }
}

const taskApi = new TaskApi();

export default taskApi;
