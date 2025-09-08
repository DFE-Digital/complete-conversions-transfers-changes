import taskApi, { ProjectType } from "./taskApi";

export type TaskStatus = "notStarted" | "notApplicable" | "inProgress" | "completed";

class TaskHelper {
    public updateArticleOfAssociation(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            notApplicable: false,
            cleared: false,
            received: false,
            sent: false,
            signed: false,
            saved: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateArticleOfAssociationTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateArticleOfAssociationTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateArticleOfAssociationTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    notApplicable: false,
                    cleared: true,
                    received: true,
                    sent: true,
                    signed: true,
                    saved: true,
                });

            default:
                return taskApi.updateArticleOfAssociationTask(defaultBody);
        }
    }
    public updateDeedOfNovationAndVariation(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            received: false,
            cleared: false,
            signedOutgoingTrust: false,
            signedIncomingTrust: false,
            saved: false,
            signedSecretaryState: false,
            savedAfterSign: false,
        };

        switch (status) {
            case "inProgress":
                return taskApi.updateDeedOfNovationAndVariationTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateDeedOfNovationAndVariationTask({
                    taskDataId: { value: taskDataId },
                    received: true,
                    cleared: true,
                    signedOutgoingTrust: true,
                    signedIncomingTrust: true,
                    saved: true,
                    signedSecretaryState: true,
                    savedAfterSign: true,
                });

            default:
                return taskApi.updateDeedOfNovationAndVariationTask(defaultBody);
        }
    }

    public updateDeedOfVariation(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            notApplicable: false,
            received: false,
            cleared: false,
            sent: false,
            saved: false,
            signed: false,
            signedSecretaryState: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateDeedOfVariationTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateDeedOfVariationTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateDeedOfVariationTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    notApplicable: false,
                    received: true,
                    cleared: true,
                    sent: true,
                    saved: true,
                    signed: true,
                    signedSecretaryState: true,
                });

            default:
                return taskApi.updateDeedOfVariationTask(defaultBody);
        }
    }

    updateHandoverWithDeliveryOfficer(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            notApplicable: false,
            handoverReview: false,
            handoverNotes: false,
            handoverMeetings: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateHandoverWithDeliveryOfficerTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateHandoverWithDeliveryOfficerTask({
                    ...defaultBody,
                    handoverReview: true,
                });

            case "completed":
                return taskApi.updateHandoverWithDeliveryOfficerTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    notApplicable: false,
                    handoverReview: true,
                    handoverNotes: true,
                    handoverMeetings: true,
                });

            default:
                return taskApi.updateHandoverWithDeliveryOfficerTask(defaultBody);
        }
    }

    updateRedactAndSendDocuments(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            redact: false,
            saved: false,
            sendToEsfa: false,
            send: false,
            sendToSolicitors: false,
        };

        switch (status) {
            case "inProgress":
                return taskApi.updateRedactAndSendDocumentsTask({
                    ...defaultBody,
                    redact: true,
                });

            case "completed":
                return taskApi.updateRedactAndSendDocumentsTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    redact: true,
                    saved: true,
                    sendToEsfa: true,
                    send: true,
                    sendToSolicitors: true,
                });

            default:
                return taskApi.updateRedactAndSendDocumentsTask(defaultBody);
        }
    }

    updateSupplementalFundingAgreement(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            received: false,
            cleared: false,
            sent: false,
            signed: false,
            saved: false,
            signedSecretaryState: false,
        };
        switch (status) {
            case "inProgress":
                return taskApi.updateSupplementalFundingAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateSupplementalFundingAgreementTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    received: true,
                    cleared: true,
                    sent: true,
                    signed: true,
                    saved: true,
                    signedSecretaryState: true,
                });

            default:
                return taskApi.updateSupplementalFundingAgreementTask(defaultBody);
        }
    }
}

const taskHelper = new TaskHelper();
export default taskHelper;
