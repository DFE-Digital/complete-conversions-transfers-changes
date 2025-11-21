import { TaskHelper, TaskStatus } from "cypress/api/taskHelper";
import { ProjectType } from "cypress/api/taskApi";
import taskApiTransfers from "cypress/api/taskApiTransfers";

class TaskHelperTransfers extends TaskHelper {
    public updateCheckAndConfirmAcademyAndTrustFinancialInformation(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApiTransfers.updateAcademyAndTrustFinancialInformationTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiTransfers.updateAcademyAndTrustFinancialInformationTask({
                    ...defaultBody,
                    academySurplusOrDeficit: "Surplus",
                });

            case "completed":
                return taskApiTransfers.updateAcademyAndTrustFinancialInformationTask({
                    taskDataId: { value: taskDataId },
                    academySurplusOrDeficit: "Surplus",
                    trustSurplusOrDeficit: "Deficit",
                });

            default:
                return taskApiTransfers.updateAcademyAndTrustFinancialInformationTask(defaultBody);
        }
    }

    public updateConfirmDateAcademyTransferred(taskDataId: string, dateAcademyTransferred?: string) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            dateAcademyTransferred: dateAcademyTransferred || null,
        };

        return taskApiTransfers.updateConfirmDateAcademyTransferredTask(defaultBody);
    }

    public updateConfirmTransferHasAuthorityToProceed(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            anyInformationChanged: false,
            baselineSheetApproved: false,
            confirmToProceed: false,
        };

        switch (status) {
            case "inProgress":
                return taskApiTransfers.updateConfirmTransferHasAuthorityToProceedTask({
                    ...defaultBody,
                    anyInformationChanged: true,
                });

            case "completed":
                return taskApiTransfers.updateConfirmTransferHasAuthorityToProceedTask({
                    taskDataId: { value: taskDataId },
                    anyInformationChanged: true,
                    baselineSheetApproved: true,
                    confirmToProceed: true,
                });

            default:
                return taskApiTransfers.updateConfirmTransferHasAuthorityToProceedTask(defaultBody);
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
                return taskApiTransfers.updateDeedOfNovationAndVariationTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiTransfers.updateDeedOfNovationAndVariationTask({
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
                return taskApiTransfers.updateDeedOfNovationAndVariationTask(defaultBody);
        }
    }

    public updateDeedOfTerminationChurchSupplementalAgreement(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            received: false,
            cleared: false,
            signed: false,
            signedByDiocese: false,
            saved: false,
            signedBySecretaryState: false,
            savedAfterSigningBySecretaryState: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApiTransfers.updateDeedOfTerminationChurchSupplementalAgreementTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiTransfers.updateDeedOfTerminationChurchSupplementalAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiTransfers.updateDeedOfTerminationChurchSupplementalAgreementTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    received: true,
                    cleared: true,
                    signed: true,
                    signedByDiocese: true,
                    saved: true,
                    signedBySecretaryState: true,
                    savedAfterSigningBySecretaryState: true,
                });
            default:
                return taskApiTransfers.updateDeedOfTerminationChurchSupplementalAgreementTask(defaultBody);
        }
    }

    public updateDeedOfTerminationMasterFundingAgreement(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            received: false,
            cleared: false,
            saved: false,
            signed: false,
            contactFinancialReportingTeam: false,
            signedSecretaryState: false,
            savedAcademySharePointHolder: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApiTransfers.updateDeedOfTerminationMasterFundingAgreementTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiTransfers.updateDeedOfTerminationMasterFundingAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiTransfers.updateDeedOfTerminationMasterFundingAgreementTask({
                    taskDataId: { value: taskDataId },
                    received: true,
                    cleared: true,
                    saved: true,
                    signed: true,
                    contactFinancialReportingTeam: true,
                    signedSecretaryState: true,
                    savedAcademySharePointHolder: true,
                });
            default:
                return taskApiTransfers.updateDeedOfTerminationMasterFundingAgreementTask(defaultBody);
        }
    }

    updateFormM(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            received: false,
            receivedTitlePlans: false,
            cleared: false,
            signed: false,
            saved: false,
        };
        switch (status) {
            case "notApplicable":
                return taskApiTransfers.updateFormMTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiTransfers.updateFormMTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiTransfers.updateFormMTask({
                    ...defaultBody,
                    received: true,
                    receivedTitlePlans: true,
                    cleared: true,
                    signed: true,
                    saved: true,
                });

            default:
                return taskApiTransfers.updateFormMTask(defaultBody);
        }
    }

    updateIncomingTrustHasCompletedAllActions(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            emailed: false,
            saved: false,
        };
        switch (status) {
            case "inProgress":
                return taskApiTransfers.updateIncomingTrustHasCompletedAllActionsTask({
                    ...defaultBody,
                    emailed: true,
                });

            case "completed":
                return taskApiTransfers.updateIncomingTrustHasCompletedAllActionsTask({
                    taskDataId: { value: taskDataId },
                    emailed: true,
                    saved: true,
                });

            default:
                return taskApiTransfers.updateIncomingTrustHasCompletedAllActionsTask(defaultBody);
        }
    }

    updateLandConsentLetter(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            drafted: false,
            signed: false,
            sent: false,
            saved: false,
        };
        switch (status) {
            case "notApplicable":
                return taskApiTransfers.updateLandConsentLetterTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiTransfers.updateLandConsentLetterTask({
                    ...defaultBody,
                    drafted: true,
                });
            case "completed":
                return taskApiTransfers.updateLandConsentLetterTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    drafted: true,
                    signed: true,
                    sent: true,
                    saved: true,
                });
            default:
                return taskApiTransfers.updateLandConsentLetterTask(defaultBody);
        }
    }

    updateRequestNewURNAndRecordForAcademy(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            complete: false,
            receive: false,
            give: false,
        };
        switch (status) {
            case "notApplicable":
                return taskApiTransfers.updateRequestNewURNAndRecordForAcademyTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiTransfers.updateRequestNewURNAndRecordForAcademyTask({
                    ...defaultBody,
                    complete: true,
                });

            case "completed":
                return taskApiTransfers.updateRequestNewURNAndRecordForAcademyTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    complete: true,
                    receive: true,
                    give: true,
                });

            default:
                return taskApiTransfers.updateRequestNewURNAndRecordForAcademyTask(defaultBody);
        }
    }
}

const taskHelperTransfers = new TaskHelperTransfers();

export default taskHelperTransfers;
