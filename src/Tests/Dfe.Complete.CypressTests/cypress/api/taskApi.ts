import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

interface TaskDataId {
    value: string;
}

interface UpdateAcademyAndTrustFinancialInformationTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    academySurplusOrDeficit?: string | null;
    trustSurplusOrDeficit?: string | null;
}

interface UpdateHandoverWithDeliveryOfficerTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    notApplicable?: boolean;
    handoverReview?: boolean;
    handoverNotes?: boolean;
    handoverMeetings?: boolean;
}

interface UpdateIncomingTrustHasCompletedAllActionsTaskRequest {
    taskDataId: TaskDataId;
    emailed?: boolean;
    saved?: boolean;
}

interface UpdateArticleOfAssociationTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    notApplicable?: boolean;
    cleared?: boolean;
    received?: boolean;
    sent?: boolean;
    signed?: boolean;
    saved?: boolean;
}

interface UpdateCheckAccuracyOfHigherNeedsTaskRequest {
    taskDataId: TaskDataId;
    confirmNumber?: boolean;
    confirmPublishedNumber?: boolean;
}

interface UpdateCommercialTransferAgreementTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    agreed?: boolean;
    signed?: boolean;
    questionsReceived?: boolean;
    questionsChecked?: boolean;
    saved?: boolean;
}

interface UpdateCompleteNotificationOfChangeTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    tellLocalAuthority?: boolean;
    checkDocument?: boolean;
    sendDocument?: boolean;
}

interface UpdateConfirmAcademyOpenedDateTaskRequest {
    taskDataId: TaskDataId;
    academyOpenedDate?: string | null;
}

interface UpdateConfirmAllConditionsMetTaskRequest {
    projectId: TaskDataId;
    confirm?: boolean;
}

interface UpdateConfirmDateAcademyTransferredTaskRequest {
    taskDataId: TaskDataId;
    dateAcademyTransferred?: string | null;
}

interface UpdateConfirmTransferHasAuthorityToProceedTaskRequest {
    taskDataId: TaskDataId;
    anyInformationChanged?: boolean;
    baselineSheetApproved?: boolean;
    confirmToProceed?: boolean;
}

interface UpdateChurchSupplementalAgreementTaskRequest {
    taskDataId: TaskDataId;
    projectType?: ProjectType;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    signed?: boolean;
    signedByDiocese?: boolean;
    saved?: boolean;
    signedBySecretaryState?: boolean;
    sentOrSaved?: boolean;
}

interface UpdateDeedOfNovationAndVariationTaskRequest {
    taskDataId: TaskDataId;
    received?: boolean;
    cleared?: boolean;
    signedOutgoingTrust?: boolean;
    signedIncomingTrust?: boolean;
    saved?: boolean;
    signedSecretaryState?: boolean;
    savedAfterSign?: boolean;
}

interface UpdateDeedOfTerminationChurchSupplementalAgreementTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    signed?: boolean;
    signedByDiocese?: boolean;
    saved?: boolean;
    signedBySecretaryState?: boolean;
    savedAfterSigningBySecretaryState?: boolean;
}

interface UpdateDeedOfTerminationMasterFundingAgreementTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    saved?: boolean;
    signed?: boolean;
    contactFinancialReportingTeam?: boolean;
    signedSecretaryState?: boolean;
    savedAcademySharePointHolder?: boolean;
}

interface UpdateDeedOfVariationTaskRequest {
    taskDataId: TaskDataId;
    projectType?: ProjectType;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    sent?: boolean;
    saved?: boolean;
    signed?: boolean;
    signedSecretaryState?: boolean;
}

interface UpdateExternalStakeholderKickOffTaskRequest {
    projectId: TaskDataId;
    stakeholderKickOffIntroductoryEmails?: boolean;
    localAuthorityProforma?: boolean;
    checkProvisionalDate?: boolean;
    stakeholderKickOffSetupMeeting?: boolean;
    stakeholderKickOffMeeting?: boolean;
    significantDate?: string;
    userEmail?: string;
}

interface UpdateLandConsentLetterTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    drafted?: boolean;
    signed?: boolean;
    sent?: boolean;
    saved?: boolean;
}

interface UpdateLandQuestionnaireTaskRequest {
    taskDataId: TaskDataId;
    received?: boolean;
    cleared?: boolean;
    signed?: boolean;
    saved?: boolean;
}

interface UpdateLandRegistryTitlePlansTaskRequest {
    taskDataId: TaskDataId;
    received?: boolean;
    cleared?: boolean;
    saved?: boolean;
}

interface UpdateMasterFundingAgreementTaskRequest {
    taskDataId: TaskDataId;
    projectType?: ProjectType;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    signed?: boolean;
    saved?: boolean;
    sent?: boolean;
    signedSecretaryState?: boolean;
}

interface UpdateOneHundredAndTwentyFiveYearLeaseTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    email?: boolean;
    receive?: boolean;
    save?: boolean;
}

interface UpdateProcessConversionSupportGrantTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    conversionGrantCheckVendorAccount?: boolean;
    conversionGrantPaymentForm?: boolean;
    conversionGrantSendInformation?: boolean;
    conversionGrantSharePaymentDate?: boolean;
}

interface UpdateReceiveDeclarationOfExpenditureCertificateTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    dateReceived?: string | null;
    notApplicable?: boolean;
    checkCertificate?: boolean;
    saved?: boolean;
}

interface UpdateRedactAndSendDocumentsTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    redact?: boolean;
    saved?: boolean;
    sendToEsfa?: boolean;
    send?: boolean;
    sendToSolicitors?: boolean;
}

interface UpdateRequestNewURNAndRecordForAcademyTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    complete?: boolean;
    receive?: boolean;
    give?: boolean;
}

export type RPAOption = "Standard" | "ChurchOrTrust" | "Commercial";

interface UpdateConfirmAcademyRiskProtectionArrangementsTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    rpaPolicyConfirm?: boolean;
    rpaOption?: RPAOption;
    rpaReason?: string;
}

interface UpdateSubleasesTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    signed?: boolean;
    saved?: boolean;
    emailSigned?: boolean;
    saveSigned?: boolean;
    receiveSigned?: boolean;
}

interface UpdateSupplementalFundingAgreementTaskRequest {
    taskDataId: TaskDataId;
    projectType?: ProjectType;
    received?: boolean;
    cleared?: boolean;
    sent?: boolean;
    saved?: boolean;
    signed?: boolean;
    signedSecretaryState?: boolean;
}

interface UpdateTenancyAtWillTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    emailSigned?: boolean;
    saveSigned?: boolean;
    receiveSigned?: boolean;
}

interface UpdateTrustModificationOrderTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    received?: boolean;
    sent?: boolean;
    cleared?: boolean;
    saved?: boolean;
}
export enum ProjectType {
    Conversion = "Conversion",
    Transfer = "Transfer",
}

class TaskApi extends ApiBase {
    private readonly taskDataUrl = `${Cypress.env(EnvApi)}/v1/TasksData/TaskData`;

    public updateAcademyAndTrustFinancialInformationTask(
        requestBody: UpdateAcademyAndTrustFinancialInformationTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("AcademyAndTrustFinancialInformation", requestBody);
    }

    public updateHandoverWithDeliveryOfficerTask(requestBody: UpdateHandoverWithDeliveryOfficerTaskRequest) {
        return this.taskDataBaseRequest<void>("HandoverDeliveryOfficer", requestBody);
    }

    public updateIncomingTrustHasCompletedAllActionsTask(
        requestBody: UpdateIncomingTrustHasCompletedAllActionsTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("IncomingTrustHasCompleteAllActions", requestBody);
    }

    public updateChurchSupplementalAgreementTask(requestBody: UpdateChurchSupplementalAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("ChurchSupplementalAgreement", requestBody);
    }

    public updateDeedOfNovationAndVariationTask(requestBody: UpdateDeedOfNovationAndVariationTaskRequest) {
        return this.taskDataBaseRequest<void>("DeedOfNovationAndVariation", requestBody);
    }

    public updateDeedOfTerminationChurchSupplementalAgreementTask(
        requestBody: UpdateDeedOfTerminationChurchSupplementalAgreementTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("DeedTerminationChurchSupplementalAgreement", requestBody);
    }

    public updateDeedOfTerminationMasterFundingAgreementTask(
        requestBody: UpdateDeedOfTerminationMasterFundingAgreementTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("DeedOfTerminationMasterFundingAgreement", requestBody);
    }

    public updateDeedOfVariationTask(requestBody: UpdateDeedOfVariationTaskRequest) {
        return this.taskDataBaseRequest<void>("DeedOfVariation", requestBody);
    }

    public updateExternalStakeholderKickOffTask(requestBody: UpdateExternalStakeholderKickOffTaskRequest) {
        return this.taskDataBaseRequest<void>("ExternalStakeholderKickOff", requestBody);
    }

    public updateArticleOfAssociationTask(requestBody: UpdateArticleOfAssociationTaskRequest) {
        return this.taskDataBaseRequest<void>("ArticleOfAssociation", requestBody);
    }

    public updateCheckAccuracyOfHigherNeedsTask(requestBody: UpdateCheckAccuracyOfHigherNeedsTaskRequest) {
        return this.taskDataBaseRequest<void>("CheckAccuracyOfHigherNeeds", requestBody);
    }

    public updateCommercialTransferAgreementTask(requestBody: UpdateCommercialTransferAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("CommercialTransferAgreement", requestBody);
    }

    public updateCompleteNotificationOfChangeTask(requestBody: UpdateCompleteNotificationOfChangeTaskRequest) {
        return this.taskDataBaseRequest<void>("CompleteNotificationOfChange", requestBody);
    }

    public updateConfirmAcademyOpenedDateTask(requestBody: UpdateConfirmAcademyOpenedDateTaskRequest) {
        return this.taskDataBaseRequest<void>("ConfirmAcademyOpenedDate", requestBody);
    }

    public updateConfirmAllConditionsMetTask(requestBody: UpdateConfirmAllConditionsMetTaskRequest) {
        return this.taskDataBaseRequest<void>("ConfirmAllConditionsMet", requestBody);
    }

    public updateConfirmDateAcademyTransferredTask(requestBody: UpdateConfirmDateAcademyTransferredTaskRequest) {
        return this.taskDataBaseRequest<void>("ConfirmDateAcademyTransferred", requestBody);
    }

    public updateConfirmTransferHasAuthorityToProceedTask(
        requestBody: UpdateConfirmTransferHasAuthorityToProceedTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("ConfirmTransferHasAuthorityToProceed", requestBody);
    }

    public updateLandConsentLetterTask(requestBody: UpdateLandConsentLetterTaskRequest) {
        return this.taskDataBaseRequest<void>("LandConsentLetter", requestBody);
    }

    public updateLandQuestionnaireTask(requestBody: UpdateLandQuestionnaireTaskRequest) {
        return this.taskDataBaseRequest<void>("LandQuestionnaire", requestBody);
    }

    public updateLandRegistryTitlePlansTask(requestBody: UpdateLandRegistryTitlePlansTaskRequest) {
        return this.taskDataBaseRequest<void>("LandRegistryTitlePlans", requestBody);
    }

    public updateMasterFundingAgreementTask(requestBody: UpdateMasterFundingAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("MasterFundingAgreement", requestBody);
    }

    public updateOneHundredAndTwentyFiveYearLeaseTask(requestBody: UpdateOneHundredAndTwentyFiveYearLeaseTaskRequest) {
        return this.taskDataBaseRequest<void>("OneHundredAndTwentyFiveYearLease", requestBody);
    }

    public updateProcessConversionSupportGrantTask(requestBody: UpdateProcessConversionSupportGrantTaskRequest) {
        return this.taskDataBaseRequest<void>("ProcessConversionSupportGrant", requestBody);
    }

    public updateReceiveDeclarationOfExpenditureCertificateTask(
        requestBody: UpdateReceiveDeclarationOfExpenditureCertificateTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("ReceiveDeclarationOfExpenditureCertificate", requestBody);
    }

    public updateRedactAndSendDocumentsTask(requestBody: UpdateRedactAndSendDocumentsTaskRequest) {
        return this.taskDataBaseRequest<void>("RedactAndSendDocuments", requestBody);
    }

    public updateRequestNewURNAndRecordForAcademyTask(requestBody: UpdateRequestNewURNAndRecordForAcademyTaskRequest) {
        return this.taskDataBaseRequest<void>("RequestNewURNAndRecordForAcademy", requestBody);
    }

    public updateConfirmAcademyRiskProtectionArrangementsTask(
        requestBody: UpdateConfirmAcademyRiskProtectionArrangementsTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("ConfirmAcademyRiskProtectionArrangements", requestBody);
    }

    public updateSubleasesTask(requestBody: UpdateSubleasesTaskRequest) {
        return this.taskDataBaseRequest<void>("Subleases", requestBody);
    }

    public updateSupplementalFundingAgreementTask(requestBody: UpdateSupplementalFundingAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("SupplementalFundingAgreement", requestBody);
    }

    public updateTenancyAtWillTask(requestBody: UpdateTenancyAtWillTaskRequest) {
        return this.taskDataBaseRequest<void>("TenancyAtWill", requestBody);
    }

    public updateTrustModificationOrderTask(requestBody: UpdateTrustModificationOrderTaskRequest) {
        return this.taskDataBaseRequest<void>("TrustModificationOrder", requestBody);
    }

    private taskDataBaseRequest<T>(task: string, body: any) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<T>({
                    method: "PATCH",
                    url: `${this.taskDataUrl}/${task}`,
                    headers,
                    body,
                })
                .then((response) => {
                    expect(response.status, `Expected PATCH request to return 204 but got ${response.status}`).to.eq(
                        204,
                    );
                    return response.body;
                });
        });
    }
}

const taskApi = new TaskApi();

export default taskApi;
