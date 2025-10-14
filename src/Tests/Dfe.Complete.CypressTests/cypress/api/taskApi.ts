import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

interface TaskDataId {
    value: string;
}

interface UpdateHandoverWithDeliveryOfficerTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    notApplicable?: boolean;
    handoverReview?: boolean;
    handoverNotes?: boolean;
    handoverMeetings?: boolean;
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

interface UpdateConfirmTransferHasAuthorityToProceedTaskRequest {
    taskDataId: TaskDataId;
    anyInformationChanged?: boolean;
    baselineSheetApproved?: boolean;
    confirmToProceed?: boolean;
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

interface UpdateReceiveDeclarationOfExpenditureCertificateTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    dateReceived?: string;
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

export enum ProjectType {
    Conversion = "Conversion",
    Transfer = "Transfer",
}

class TaskApi extends ApiBase {
    private readonly taskDataUrl = `${Cypress.env(EnvApi)}/v1/TasksData/TaskData`;

    public updateHandoverWithDeliveryOfficerTask(requestBody: UpdateHandoverWithDeliveryOfficerTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "HandoverDeliveryOfficer", requestBody, 204);
    }

    public updateDeedOfNovationAndVariationTask(requestBody: UpdateDeedOfNovationAndVariationTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "DeedOfNovationAndVariation", requestBody, 204);
    }

    public updateDeedOfVariationTask(requestBody: UpdateDeedOfVariationTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "DeedOfVariation", requestBody, 204);
    }

    public updateExternalStakeholderKickOffTask(requestBody: UpdateExternalStakeholderKickOffTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "ExternalStakeholderKickOff", requestBody, 204);
    }

    public updateArticleOfAssociationTask(requestBody: UpdateArticleOfAssociationTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "ArticleOfAssociation", requestBody, 204);
    }

    public updateConfirmTransferHasAuthorityToProceedTask(
        requestBody: UpdateConfirmTransferHasAuthorityToProceedTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("PATCH", "ConfirmTransferHasAuthorityToProceed", requestBody, 204);
    }

    public updateReceiveDeclarationOfExpenditureCertificateTask(
        requestBody: UpdateReceiveDeclarationOfExpenditureCertificateTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("PATCH", "ReceiveDeclarationOfExpenditureCertificate", requestBody, 204);
    }

    public updateRedactAndSendDocumentsTask(requestBody: UpdateRedactAndSendDocumentsTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "RedactAndSendDocuments", requestBody, 204);
    }

    public updateSupplementalFundingAgreementTask(requestBody: UpdateSupplementalFundingAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("PATCH", "SupplementalFundingAgreement", requestBody, 204);
    }

    private taskDataBaseRequest<T>(
        method: string,
        task: string,
        body:
            | UpdateHandoverWithDeliveryOfficerTaskRequest
            | UpdateArticleOfAssociationTaskRequest
            | UpdateDeedOfNovationAndVariationTaskRequest
            | UpdateDeedOfVariationTaskRequest
            | UpdateExternalStakeholderKickOffTaskRequest,
        expectedStatus: number,
    ) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<T>({
                    method,
                    url: `${this.taskDataUrl}/${task}`,
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
