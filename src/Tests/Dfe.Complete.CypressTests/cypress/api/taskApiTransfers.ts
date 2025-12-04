import { TaskApi, TaskDataId } from "cypress/api/taskApi";

interface UpdateAcademyAndTrustFinancialInformationTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    academySurplusOrDeficit?: string | null;
    trustSurplusOrDeficit?: string | null;
}

interface UpdateClosureOrTransferDeclarationTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    received?: boolean;
    cleared?: boolean;
    saved?: boolean;
    sent?: boolean;
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

interface UpdateFormMTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    received: boolean;
    receivedTitlePlans: boolean;
    cleared: boolean;
    signed: boolean;
    saved: boolean;
}

interface UpdateIncomingTrustHasCompletedAllActionsTaskRequest {
    taskDataId: TaskDataId;
    emailed?: boolean;
    saved?: boolean;
}

interface UpdateLandConsentLetterTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    drafted?: boolean;
    signed?: boolean;
    sent?: boolean;
    saved?: boolean;
}

interface UpdateRequestNewURNAndRecordForAcademyTaskRequest {
    taskDataId: TaskDataId;
    notApplicable?: boolean;
    complete?: boolean;
    receive?: boolean;
    give?: boolean;
}

class TaskApiTransfers extends TaskApi {
    public updateAcademyAndTrustFinancialInformationTask(
        requestBody: UpdateAcademyAndTrustFinancialInformationTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("AcademyAndTrustFinancialInformation", requestBody);
    }

    public updateClosureOrTransferDeclarationTask(requestBody: UpdateClosureOrTransferDeclarationTaskRequest) {
        return this.taskDataBaseRequest<void>("ClosureOrTransferDeclaration", requestBody);
    }

    public updateConfirmDateAcademyTransferredTask(requestBody: UpdateConfirmDateAcademyTransferredTaskRequest) {
        return this.taskDataBaseRequest<void>("ConfirmDateAcademyTransferred", requestBody);
    }

    public updateConfirmTransferHasAuthorityToProceedTask(
        requestBody: UpdateConfirmTransferHasAuthorityToProceedTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("ConfirmTransferHasAuthorityToProceed", requestBody);
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

    public updateFormMTask(requestBody: UpdateFormMTaskRequest) {
        return this.taskDataBaseRequest<void>("FormM", requestBody);
    }

    public updateIncomingTrustHasCompletedAllActionsTask(
        requestBody: UpdateIncomingTrustHasCompletedAllActionsTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("IncomingTrustHasCompleteAllActions", requestBody);
    }

    public updateLandConsentLetterTask(requestBody: UpdateLandConsentLetterTaskRequest) {
        return this.taskDataBaseRequest<void>("LandConsentLetter", requestBody);
    }

    public updateRequestNewURNAndRecordForAcademyTask(requestBody: UpdateRequestNewURNAndRecordForAcademyTaskRequest) {
        return this.taskDataBaseRequest<void>("RequestNewURNAndRecordForAcademy", requestBody);
    }
}

const taskApiTransfers = new TaskApiTransfers();

export default taskApiTransfers;
