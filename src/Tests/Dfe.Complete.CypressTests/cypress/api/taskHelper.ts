import taskApi, { ProjectType, RPAOption } from "./taskApi";
import { cypressUser } from "cypress/constants/cypressConstants";

export type TaskStatus = "notStarted" | "notApplicable" | "inProgress" | "completed";

class TaskHelper {
    public updateCheckAndConfirmAcademyAndTrustFinancialInformation(
        taskDataId: string,
        projectType: ProjectType,
        status: TaskStatus,
    ) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateAcademyAndTrustFinancialInformationTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateAcademyAndTrustFinancialInformationTask({
                    ...defaultBody,
                    academySurplusOrDeficit: "Surplus",
                });

            case "completed":
                return taskApi.updateAcademyAndTrustFinancialInformationTask({
                    taskDataId: { value: taskDataId },
                    academySurplusOrDeficit: "Surplus",
                    trustSurplusOrDeficit: "Deficit",
                });

            default:
                return taskApi.updateAcademyAndTrustFinancialInformationTask(defaultBody);
        }
    }

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

    public updateCheckAccuracyOfHigherNeeds(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            confirmNumber: false,
            confirmPublishedNumber: false,
        };

        switch (status) {
            case "inProgress":
                return taskApi.updateCheckAccuracyOfHigherNeedsTask({
                    ...defaultBody,
                    confirmNumber: true,
                });

            case "completed":
                return taskApi.updateCheckAccuracyOfHigherNeedsTask({
                    taskDataId: { value: taskDataId },
                    confirmNumber: true,
                    confirmPublishedNumber: true,
                });

            default:
                return taskApi.updateCheckAccuracyOfHigherNeedsTask(defaultBody);
        }
    }

    public updateCommercialTransferAgreement(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            agreed: false,
            signed: false,
            questionsReceived: false,
            questionsChecked: false,
            saved: false,
        };

        switch (status) {
            case "inProgress":
                return taskApi.updateCommercialTransferAgreementTask({
                    ...defaultBody,
                    agreed: true,
                });

            case "completed":
                return taskApi.updateCommercialTransferAgreementTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    agreed: true,
                    signed: true,
                    questionsReceived: true,
                    questionsChecked: true,
                    saved: true,
                });

            default:
                return taskApi.updateCommercialTransferAgreementTask(defaultBody);
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
                return taskApi.updateCompleteNotificationOfChangeTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateCompleteNotificationOfChangeTask({
                    ...defaultBody,
                    tellLocalAuthority: true,
                });

            case "completed":
                return taskApi.updateCompleteNotificationOfChangeTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    tellLocalAuthority: true,
                    checkDocument: true,
                    sendDocument: true,
                });

            default:
                return taskApi.updateCompleteNotificationOfChangeTask(defaultBody);
        }
    }

    public updateConfirmAcademyOpenedDate(taskDataId: string, academyOpenedDate?: string) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            academyOpenedDate: academyOpenedDate || null,
        };

        return taskApi.updateConfirmAcademyOpenedDateTask(defaultBody);
    }

    public updateConfirmDateAcademyTransferred(taskDataId: string, dateAcademyTransferred?: string) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            dateAcademyTransferred: dateAcademyTransferred || null,
        };

        return taskApi.updateConfirmDateAcademyTransferredTask(defaultBody);
    }

    public updateConfirmAllConditionsMet(projectId: string, status: TaskStatus) {
        if (status === "completed") {
            return taskApi.updateConfirmAllConditionsMetTask({
                projectId: { value: projectId },
                confirm: true,
            });
        }

        return taskApi.updateConfirmAllConditionsMetTask({
            projectId: { value: projectId },
            confirm: false,
        });
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
                return taskApi.updateConfirmTransferHasAuthorityToProceedTask({
                    ...defaultBody,
                    anyInformationChanged: true,
                });

            case "completed":
                return taskApi.updateConfirmTransferHasAuthorityToProceedTask({
                    taskDataId: { value: taskDataId },
                    anyInformationChanged: true,
                    baselineSheetApproved: true,
                    confirmToProceed: true,
                });

            default:
                return taskApi.updateConfirmTransferHasAuthorityToProceedTask(defaultBody);
        }
    }

    public updateChurchSupplementalAgreement(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            notApplicable: false,
            received: false,
            cleared: false,
            signed: false,
            signedByDiocese: false,
            saved: false,
            signedBySecretaryState: false,
            sentOrSaved: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateChurchSupplementalAgreementTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateChurchSupplementalAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateChurchSupplementalAgreementTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    notApplicable: false,
                    received: true,
                    cleared: true,
                    signed: true,
                    signedByDiocese: true,
                    saved: true,
                    signedBySecretaryState: true,
                    sentOrSaved: true,
                });

            default:
                return taskApi.updateChurchSupplementalAgreementTask(defaultBody);
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
                return taskApi.updateDeedOfTerminationChurchSupplementalAgreementTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateDeedOfTerminationChurchSupplementalAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateDeedOfTerminationChurchSupplementalAgreementTask({
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
                return taskApi.updateDeedOfTerminationChurchSupplementalAgreementTask(defaultBody);
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
                return taskApi.updateDeedOfTerminationMasterFundingAgreementTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateDeedOfTerminationMasterFundingAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateDeedOfTerminationMasterFundingAgreementTask({
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
                return taskApi.updateDeedOfTerminationMasterFundingAgreementTask(defaultBody);
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
                return taskApi.updateLandConsentLetterTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateLandConsentLetterTask({
                    ...defaultBody,
                    drafted: true,
                });
            case "completed":
                return taskApi.updateLandConsentLetterTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    drafted: true,
                    signed: true,
                    sent: true,
                    saved: true,
                });
            default:
                return taskApi.updateLandConsentLetterTask(defaultBody);
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
                return taskApi.updateLandQuestionnaireTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateLandQuestionnaireTask({
                    taskDataId: { value: taskDataId },
                    received: true,
                    cleared: true,
                    signed: true,
                    saved: true,
                });

            default:
                return taskApi.updateLandQuestionnaireTask(defaultBody);
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
                return taskApi.updateLandRegistryTitlePlansTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateLandRegistryTitlePlansTask({
                    taskDataId: { value: taskDataId },
                    received: true,
                    cleared: true,
                    saved: true,
                });

            default:
                return taskApi.updateLandRegistryTitlePlansTask(defaultBody);
        }
    }
    updateMasterFundingAgreement(taskDataId: string, projectType: ProjectType, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            notApplicable: false,
            received: false,
            cleared: false,
            signed: false,
            saved: false,
            sent: false,
            signedSecretaryState: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateMasterFundingAgreementTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateMasterFundingAgreementTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateMasterFundingAgreementTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    notApplicable: false,
                    received: true,
                    cleared: true,
                    signed: true,
                    saved: true,
                    sent: true,
                    signedSecretaryState: true,
                });

            default:
                return taskApi.updateMasterFundingAgreementTask(defaultBody);
        }
    }

    updateReceiveDeclarationOfExpenditureCertificate(
        taskDataId: string,
        projectType: ProjectType,
        status: TaskStatus,
        significantDate?: string,
    ) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            projectType: projectType,
            dateReceived: significantDate || null,
            notApplicable: false,
            checkCertificate: false,
            saved: false,
        };

        switch (status) {
            case "notApplicable":
                return taskApi.updateReceiveDeclarationOfExpenditureCertificateTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateReceiveDeclarationOfExpenditureCertificateTask({
                    ...defaultBody,
                    checkCertificate: true,
                });

            case "completed":
                return taskApi.updateReceiveDeclarationOfExpenditureCertificateTask({
                    taskDataId: { value: taskDataId },
                    projectType: projectType,
                    dateReceived: significantDate || "2025-09-09",
                    notApplicable: false,
                    checkCertificate: true,
                    saved: true,
                });

            default:
                return taskApi.updateReceiveDeclarationOfExpenditureCertificateTask(defaultBody);
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
                return taskApi.updateRequestNewURNAndRecordForAcademyTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateRequestNewURNAndRecordForAcademyTask({
                    ...defaultBody,
                    complete: true,
                });

            case "completed":
                return taskApi.updateRequestNewURNAndRecordForAcademyTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    complete: true,
                    receive: true,
                    give: true,
                });

            default:
                return taskApi.updateRequestNewURNAndRecordForAcademyTask(defaultBody);
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
            return taskApi.updateConfirmAcademyRiskProtectionArrangementsTask({
                ...defaultBody,
                rpaPolicyConfirm: true,
            });
        }
        if (rpaOption === "Commercial") {
            return taskApi.updateConfirmAcademyRiskProtectionArrangementsTask({
                ...defaultBody,
                rpaOption: "Commercial",
                rpaReason: "Some reason for commercial RPA",
            });
        } else if (rpaOption) {
            return taskApi.updateConfirmAcademyRiskProtectionArrangementsTask({
                ...defaultBody,
                rpaOption: rpaOption,
            });
        } else {
            return taskApi.updateConfirmAcademyRiskProtectionArrangementsTask(defaultBody);
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

    updateTrustModificationOrder(taskDataId: string, status: TaskStatus) {
        const defaultBody = {
            taskDataId: { value: taskDataId },
            notApplicable: false,
            received: false,
            sent: false,
            cleared: false,
            saved: false
        }
        switch (status) {
            case "notApplicable":
                return taskApi.updateTrustModificationOrderTask({
                    ...defaultBody,
                    notApplicable: true,
                });

            case "inProgress":
                return taskApi.updateTrustModificationOrderTask({
                    ...defaultBody,
                    received: true,
                });

            case "completed":
                return taskApi.updateTrustModificationOrderTask({
                    taskDataId: { value: taskDataId },
                    notApplicable: false,
                    received: true,
                    sent: true,
                    cleared: true,
                    saved: true
                });

            default:
                return taskApi.updateTrustModificationOrderTask(defaultBody);
        }
    }

    // task that also updates significant date on project
    updateExternalStakeholderKickOff(
        projectId: string,
        status: TaskStatus,
        significantDate?: string,
        userEmail?: string,
    ) {
        const defaultBody = {
            projectId: { value: projectId },
            stakeholderKickOffIntroductoryEmails: false,
            localAuthorityProforma: false,
            checkProvisionalDate: false,
            stakeholderKickOffSetupMeeting: false,
            stakeholderKickOffMeeting: false,
            significantDate: significantDate || "2027-09-01",
            userEmail: userEmail || cypressUser.email,
        };

        switch (status) {
            case "inProgress":
                return taskApi.updateExternalStakeholderKickOffTask({
                    ...defaultBody,
                    stakeholderKickOffIntroductoryEmails: true,
                });

            case "completed":
                return taskApi.updateExternalStakeholderKickOffTask({
                    projectId: { value: projectId },
                    stakeholderKickOffIntroductoryEmails: true,
                    localAuthorityProforma: true,
                    checkProvisionalDate: true,
                    stakeholderKickOffSetupMeeting: true,
                    stakeholderKickOffMeeting: true,
                    significantDate: significantDate || "2027-09-01",
                    userEmail: userEmail || cypressUser.email,
                });

            default:
                return taskApi.updateExternalStakeholderKickOffTask(defaultBody);
        }
    }
}

const taskHelper = new TaskHelper();
export default taskHelper;
