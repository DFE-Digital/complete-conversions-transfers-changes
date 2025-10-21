import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import TaskHelper from "cypress/api/taskHelper";
import { beforeEach } from "mocha";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import yourProjects from "cypress/pages/projects/yourProjects";
import { ProjectType } from "cypress/api/taskApi";

const completableProject = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transfer.abbey },
    isSignificantDateProvisional: true,
});
let completableProjectId: string;
const completableSchoolName = "Abbey College Manchester";

const someTasksCompletedProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transfer.priory },
});
let someTasksCompletedProjectId: string;

const noTasksCompletedProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transfer.prees },
});
let noTasksCompletedProjectId: string;

describe("Complete transfer projects tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(completableProject.urn.value);
        projectRemover.removeProjectIfItExists(someTasksCompletedProject.urn.value);
        projectRemover.removeProjectIfItExists(noTasksCompletedProject.urn.value);
        projectApi.createTransferProject(completableProject).then((response) => {
            completableProjectId = response.value;
            projectApi.getProject(completableProject.urn.value).then((response) => {
                const taskId = response.body.tasksDataId.value;
                TaskHelper.updateExternalStakeholderKickOff(completableProjectId, "completed", "2025-10-01");
                TaskHelper.updateConfirmTransferHasAuthorityToProceed(taskId, "completed");
                TaskHelper.updateReceiveDeclarationOfExpenditureCertificate(taskId, ProjectType.Transfer, "completed");
                TaskHelper.updateConfirmDateAcademyTransferred(taskId, "2025-09-01");
            });
        });
        projectApi.createMatTransferProject(someTasksCompletedProject).then((response) => {
            someTasksCompletedProjectId = response.value;
            TaskHelper.updateExternalStakeholderKickOff(someTasksCompletedProjectId, "completed", "2025-10-01");
        });
        projectApi
            .createMatTransferProject(noTasksCompletedProject)
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
            .contains(`You have completed the project for ${completableSchoolName} ${completableProject.urn.value}`);
        cy.visit("/projects/yours/completed");
        yourProjects.goToNextPageUntilFieldIsVisible(completableSchoolName);
    });

    it("should not be able to complete a project with some tasks completed", () => {
        cy.visit(`projects/${someTasksCompletedProjectId}/tasks`);
        taskListPage
            .clickButton("Complete project")
            .hasImportantCompletedBannerWith("This project cannot be completed until:", [
                "The confirm this transfer has authority to proceed task is completed",
                "The receive declaration of expenditure certificate task is completed",
                "The confirm the date the academy transferred task is completed",
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
            ]);
    });
});
