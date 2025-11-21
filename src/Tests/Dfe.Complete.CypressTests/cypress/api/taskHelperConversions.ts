import { TaskHelper, TaskStatus } from "cypress/api/taskHelper";
import taskApiConversions from "cypress/api/taskApiConversions";
import { ProjectType, RPAOption } from "cypress/api/taskApi";

class TaskHelperConversions extends TaskHelper {
    public updateCheckAccuracyOfHigherNeeds(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            confirmNumber: false,
            confirmPublishedNumber: false,
        };

        switch (status) {
            case "inProgress":
                return taskApiConversions.updateCheckAccuracyOfHigherNeedsTask({
                    ...defaultBody,
                    confirmNumber: true,
                });

            case "completed":
                return taskApiConversions.updateCheckAccuracyOfHigherNeedsTask({
                    taskDataId: { value: taskDataId },
                    confirmNumber: true,
                    confirmPublishedNumber: true,
                });

            default:
                return taskApiConversions.updateCheckAccuracyOfHigherNeedsTask(defaultBody);
        }
    }

    public updateCompleteNotificationOfChange(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            tellLocalAuthority: false,
            checkDocument: false,
            sendDocument: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApiConversions.updateCompleteNotificationOfChangeTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiConversions.updateCompleteNotificationOfChangeTask({
                    ...defaultBody,
                    tellLocalAuthority: true,
                });

            case "completed":
                return taskApiConversions.updateCompleteNotificationOfChangeTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    tellLocalAuthority: true,
                    checkDocument: true,
                    sendDocument: true,
                });

            default:
                return taskApiConversions.updateCompleteNotificationOfChangeTask(defaultBody);
        }
    }

    public updateConfirmAcademyOpenedDate(taskDataId: string, academyOpenedDate?: string) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            academyOpenedDate: academyOpenedDate || null,
        };

        return taskApiConversions.updateConfirmAcademyOpenedDateTask(defaultBody);
    }

    public updateConfirmAllConditionsMet(projectId: string, status: TaskStatus) {
        if (status === "completed") {
            return taskApiConversions.updateConfirmAllConditionsMetTask({
                projectId: { value: projectId },
                confirm: true,
            });
        }

        return taskApiConversions.updateConfirmAllConditionsMetTask({
            projectId: { value: projectId },
            confirm: false,
        });
    }

    updateConfirmSchoolHasCompletedAllActions(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            emailed: false,
            saved: false,
        };

        switch (status) {
            case "inProgress":
                return taskApiConversions.updateConfirmSchoolHasCompletedAllActionsTask({
                    ...defaultBody,
                    emailed: true,
                });

            case "completed":
                return taskApiConversions.updateConfirmSchoolHasCompletedAllActionsTask({
                    taskDataId: { value: taskDataId },
                    emailed: true,
                    saved: true,
                });

            default:
                return taskApiConversions.updateConfirmSchoolHasCompletedAllActionsTask(defaultBody);
        }
    }

    updateLandQuestionnaire(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            received: false,
            cleared: false,
            signed: false,
            saved: false,
        };
        switch (status) {
            case "inProgress":
                return taskApiConversions.updateLandQuestionnaireTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiConversions.updateLandQuestionnaireTask({
                    taskDataId: { value: taskDataId },
                    received: true,
                    cleared: true,
                    signed: true,
                    saved: true,
                });

            default:
                return taskApiConversions.updateLandQuestionnaireTask(defaultBody);
        }
    }

    updateLandRegistryTitlePlans(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            received: false,
            cleared: false,
            saved: false,
        };
        switch (status) {
            case "inProgress":
                return taskApiConversions.updateLandRegistryTitlePlansTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiConversions.updateLandRegistryTitlePlansTask({
                    taskDataId: { value: taskDataId },
                    received: true,
                    cleared: true,
                    saved: true,
                });

            default:
                return taskApiConversions.updateLandRegistryTitlePlansTask(defaultBody);
        }
    }

    updateOneHundredAndTwentyFiveYearLease(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            email: false,
            receive: false,
            save: false,
        };
        switch (status) {
            case "notApplicable":
                return taskApiConversions.updateOneHundredAndTwentyFiveYearLeaseTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiConversions.updateOneHundredAndTwentyFiveYearLeaseTask({
                    ...defaultBody,
                    email: true,
                });

            case "completed":
                return taskApiConversions.updateOneHundredAndTwentyFiveYearLeaseTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    email: true,
                    receive: true,
                    save: true,
                });

            default:
                return taskApiConversions.updateOneHundredAndTwentyFiveYearLeaseTask(defaultBody);
        }
    }

    updateProcessConversionSupportGrant(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            conversionGrantCheckVendorAccount: false,
            conversionGrantPaymentForm: false,
            conversionGrantSendInformation: false,
            conversionGrantSharePaymentDate: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApiConversions.updateProcessConversionSupportGrantTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiConversions.updateProcessConversionSupportGrantTask({
                    ...defaultBody,
                    conversionGrantCheckVendorAccount: true,
                });

            case "completed":
                return taskApiConversions.updateProcessConversionSupportGrantTask({
                    ...defaultBody,
                    conversionGrantCheckVendorAccount: true,
                    conversionGrantPaymentForm: true,
                    conversionGrantSendInformation: true,
                    conversionGrantSharePaymentDate: true,
                });

            default:
                return taskApiConversions.updateProcessConversionSupportGrantTask(defaultBody);
        }
    }

    updateConfirmAcademyRiskProtectionArrangements(
        taskDataId: string,
        projectType: ProjectType,
        rpaPolicyConfirm?: boolean,
        rpaOption?: RPAOption,
    ) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
        };

        if (rpaPolicyConfirm) {
            return taskApiConversions.updateConfirmAcademyRiskProtectionArrangementsTask({
                ...defaultBody,
                rpaPolicyConfirm: true,
            });
        }
        if (rpaOption === "Commercial") {
            return taskApiConversions.updateConfirmAcademyRiskProtectionArrangementsTask({
                ...defaultBody,
                rpaOption: "Commercial",
                rpaReason: "Some reason for commercial RPA",
            });
        } else if (rpaOption) {
            return taskApiConversions.updateConfirmAcademyRiskProtectionArrangementsTask({
                ...defaultBody,
                rpaOption: rpaOption,
            });
        } else {
            return taskApiConversions.updateConfirmAcademyRiskProtectionArrangementsTask(defaultBody);
        }
    }

    updateSubleases(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            received: false,
            cleared: false,
            signed: false,
            saved: false,
            emailSigned: false,
            saveSigned: false,
            receiveSigned: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApiConversions.updateSubleasesTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiConversions.updateSubleasesTask({
                    ...defaultBody,
                    cleared: true,
                });

            case "completed":
                return taskApiConversions.updateSubleasesTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    received: true,
                    cleared: true,
                    signed: true,
                    saved: true,
                    emailSigned: true,
                    saveSigned: true,
                    receiveSigned: true,
                });

            default:
                return taskApiConversions.updateSubleasesTask(defaultBody);
        }
    }

    updateTenancyAtWill(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            emailSigned: false,
            saveSigned: false,
            receiveSigned: false,
        };
        switch (status) {
            case "notApplicable":
                return taskApiConversions.updateTenancyAtWillTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiConversions.updateTenancyAtWillTask({
                    ...defaultBody,
                    emailSigned: true,
                });

            case "completed":
                return taskApiConversions.updateTenancyAtWillTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    emailSigned: true,
                    saveSigned: true,
                    receiveSigned: true,
                });
        }
    }

    updateTrustModificationOrder(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            received: false,
            sent: false,
            cleared: false,
            saved: false,
        };
        switch (status) {
            case "notApplicable":
                return taskApiConversions.updateTrustModificationOrderTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApiConversions.updateTrustModificationOrderTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApiConversions.updateTrustModificationOrderTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    received: true,
                    sent: true,
                    cleared: true,
                    saved: true,
                });

            default:
                return taskApiConversions.updateTrustModificationOrderTask(defaultBody);
        }
    }
}

const taskHelperConversions = new TaskHelperConversions();

export default taskHelperConversions;
