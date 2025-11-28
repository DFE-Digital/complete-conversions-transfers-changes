import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

export interface TaskDataId {
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

interface UpdateCommercialTransferAgreementTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    agreed?: boolean;
    signed?: boolean;
    questionsReceived?: boolean;
    questionsChecked?: boolean;
    saved?: boolean;
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
export type SponsoredSupportGrantType = "StandardTransferGrant" | "FastTrack" | "Intermediate" | "FullSponsored";

interface UpdateSponsoredSupportGrantTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    notApplicable?: boolean;
    sponsoredSupportGrantType?: SponsoredSupportGrantType;
    paymentAmount: boolean;
    paymentForm: boolean;
    sendInformation: boolean;
    informTrust: boolean;
}

export type RPAOption = "Standard" | "ChurchOrTrust" | "Commercial";

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

export class TaskApi extends ApiBase {
    private readonly taskDataUrl = `${Cypress.env(EnvApi)}/v1/TasksData/TaskData`;

    public updateHandoverWithDeliveryOfficerTask(requestBody: UpdateHandoverWithDeliveryOfficerTaskRequest) {
        return this.taskDataBaseRequest<void>("HandoverDeliveryOfficer", requestBody);
    }

    public updateChurchSupplementalAgreementTask(requestBody: UpdateChurchSupplementalAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("ChurchSupplementalAgreement", requestBody);
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

    public updateCommercialTransferAgreementTask(requestBody: UpdateCommercialTransferAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("CommercialTransferAgreement", requestBody);
    }

    public updateMasterFundingAgreementTask(requestBody: UpdateMasterFundingAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("MasterFundingAgreement", requestBody);
    }

    public updateReceiveDeclarationOfExpenditureCertificateTask(
        requestBody: UpdateReceiveDeclarationOfExpenditureCertificateTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("ReceiveDeclarationOfExpenditureCertificate", requestBody);
    }

    public updateRedactAndSendDocumentsTask(requestBody: UpdateRedactAndSendDocumentsTaskRequest) {
        return this.taskDataBaseRequest<void>("RedactAndSendDocuments", requestBody);
    }

    public updateSponsoredSupportGrantTask(requestBody: UpdateSponsoredSupportGrantTaskRequest) {
        return this.taskDataBaseRequest<void>("SponsoredSupportGrant", requestBody);
    }

    public updateSupplementalFundingAgreementTask(requestBody: UpdateSupplementalFundingAgreementTaskRequest) {
        return this.taskDataBaseRequest<void>("SupplementalFundingAgreement", requestBody);
    }

    protected taskDataBaseRequest<T>(task: string, body: any) {
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
