import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { beforeEach } from "mocha";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import yourProjects from "cypress/pages/projects/yourProjects";
import { ProjectType } from "cypress/api/taskApi";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";

const completableProject = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transfer.abbey,
});
let completableProjectId: string;
const completableSchoolName = "Abbey College Manchester";

const allTasksCompleteNoUKPRNProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transfer.priory,
});
let allTasksCompleteNoUKPRNProjectId: string;

const noTasksCompletedProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transfer.prees,
});
let noTasksCompletedProjectId: string;

describe("Complete transfer projects tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(completableProject.urn);
        projectRemover.removeProjectIfItExists(allTasksCompleteNoUKPRNProject.urn);
        projectRemover.removeProjectIfItExists(noTasksCompletedProject.urn);
        projectApi.createAndUpdateTransferProject(completableProject).then((response) => {
            completableProjectId = response.value;
            projectApi.getProject(completableProject.urn).then((response) => {
                const taskId = response.body.tasksDataId.value;
                TaskHelperTransfers.updateExternalStakeholderKickOff(completableProjectId, "completed", "2025-10-01");
                TaskHelperTransfers.updateConfirmTransferHasAuthorityToProceed(taskId, "completed");
                TaskHelperTransfers.updateReceiveDeclarationOfExpenditureCertificate(
                    taskId,
                    ProjectType.Transfer,
                    "completed",
                );
                TaskHelperTransfers.updateConfirmDateAcademyTransferred(taskId, "2025-09-01");
            });
        });
        projectApi.createAndUpdateMatTransferProject(allTasksCompleteNoUKPRNProject).then((response) => {
            allTasksCompleteNoUKPRNProjectId = response.value;
            projectApi.getProject(allTasksCompleteNoUKPRNProject.urn).then((response) => {
                const taskId = response.body.tasksDataId.value;
                TaskHelperTransfers.updateExternalStakeholderKickOff(
                    allTasksCompleteNoUKPRNProjectId,
                    "completed",
                    "2025-10-01",
                );
                TaskHelperTransfers.updateConfirmTransferHasAuthorityToProceed(taskId, "completed");
                TaskHelperTransfers.updateReceiveDeclarationOfExpenditureCertificate(
                    taskId,
                    ProjectType.Transfer,
                    "completed",
                );
                TaskHelperTransfers.updateConfirmDateAcademyTransferred(taskId, "2025-09-01");
            });
        });
        projectApi
            .createAndUpdateMatTransferProject(noTasksCompletedProject)
            .then((response) => (noTasksCompletedProjectId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("should complete a project with all required tasks completed", () => {
        cy.visit(`projects/${completableProjectId}/tasks`);
        taskListPage
            .clickButton("Complete project")
            .contains(`You have completed the project for ${completableSchoolName} ${completableProject.urn}`);
        cy.visit("/projects/yours/completed");
        yourProjects.goToNextPageUntilFieldIsVisible(completableSchoolName);
    });

    it("should not be able to complete a MAT project with all tasks completed but no incoming trust UKPRN", () => {
        cy.visit(`projects/${allTasksCompleteNoUKPRNProjectId}/tasks`);
        taskListPage
            .clickButton("Complete project")
            .hasImportantCompletedBannerWith("This project cannot be completed until:", [
                "The incoming trust UKPRN is entered",
            ]);
    });

    it("should not be able to complete a project with no tasks completed", () => {
        cy.visit(`projects/${noTasksCompletedProjectId}/tasks`);
        taskListPage
            .clickButton("Complete project")
            .hasImportantCompletedBannerWith("This project cannot be completed until:", [
                "The transfer date has been confirmed and is in the past",
                "The confirm this transfer has authority to proceed task is completed",
                "The receive declaration of expenditure certificate task is completed",
                "The confirm the date the academy transferred task is completed",
                "The incoming trust UKPRN is entered",
            ]);
    });
});
