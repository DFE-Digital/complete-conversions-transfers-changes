import { ProjectType, RPAOption, TaskApi, TaskDataId } from "cypress/api/taskApi";

interface UpdateCheckAccuracyOfHigherNeedsTaskRequest {
    taskDataId: TaskDataId;
    confirmNumber?: boolean;
    confirmPublishedNumber?: boolean;
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

interface UpdateConfirmAcademyRiskProtectionArrangementsTaskRequest {
    taskDataId: TaskDataId;
    projectType: ProjectType;
    rpaPolicyConfirm?: boolean;
    rpaOption?: RPAOption;
    rpaReason?: string;
}

interface UpdateConfirmAllConditionsMetTaskRequest {
    projectId: TaskDataId;
    confirm?: boolean;
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

class TaskApiConversions extends TaskApi {
    public updateCheckAccuracyOfHigherNeedsTask(requestBody: UpdateCheckAccuracyOfHigherNeedsTaskRequest) {
        return this.taskDataBaseRequest<void>("CheckAccuracyOfHigherNeeds", requestBody);
    }

    public updateCompleteNotificationOfChangeTask(requestBody: UpdateCompleteNotificationOfChangeTaskRequest) {
        return this.taskDataBaseRequest<void>("CompleteNotificationOfChange", requestBody);
    }

    public updateConfirmAcademyOpenedDateTask(requestBody: UpdateConfirmAcademyOpenedDateTaskRequest) {
        return this.taskDataBaseRequest<void>("ConfirmAcademyOpenedDate", requestBody);
    }

    public updateConfirmAcademyRiskProtectionArrangementsTask(
        requestBody: UpdateConfirmAcademyRiskProtectionArrangementsTaskRequest,
    ) {
        return this.taskDataBaseRequest<void>("ConfirmAcademyRiskProtectionArrangements", requestBody);
    }

    public updateConfirmAllConditionsMetTask(requestBody: UpdateConfirmAllConditionsMetTaskRequest) {
        return this.taskDataBaseRequest<void>("ConfirmAllConditionsMet", requestBody);
    }

    public updateLandQuestionnaireTask(requestBody: UpdateLandQuestionnaireTaskRequest) {
        return this.taskDataBaseRequest<void>("LandQuestionnaire", requestBody);
    }

    public updateLandRegistryTitlePlansTask(requestBody: UpdateLandRegistryTitlePlansTaskRequest) {
        return this.taskDataBaseRequest<void>("LandRegistryTitlePlans", requestBody);
    }

    public updateOneHundredAndTwentyFiveYearLeaseTask(requestBody: UpdateOneHundredAndTwentyFiveYearLeaseTaskRequest) {
        return this.taskDataBaseRequest<void>("OneHundredAndTwentyFiveYearLease", requestBody);
    }

    public updateProcessConversionSupportGrantTask(requestBody: UpdateProcessConversionSupportGrantTaskRequest) {
        return this.taskDataBaseRequest<void>("ProcessConversionSupportGrant", requestBody);
    }

    public updateSubleasesTask(requestBody: UpdateSubleasesTaskRequest) {
        return this.taskDataBaseRequest<void>("Subleases", requestBody);
    }

    public updateTenancyAtWillTask(requestBody: UpdateTenancyAtWillTaskRequest) {
        return this.taskDataBaseRequest<void>("TenancyAtWill", requestBody);
    }

    public updateTrustModificationOrderTask(requestBody: UpdateTrustModificationOrderTaskRequest) {
        return this.taskDataBaseRequest<void>("TrustModificationOrder", requestBody);
    }
}

const taskApiConversions = new TaskApiConversions();

export default taskApiConversions;
