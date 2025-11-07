import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import TaskHelper from "cypress/api/taskHelper";
import yourProjects from "cypress/pages/projects/yourProjects";
import { getSignificantDateString } from "cypress/support/formatDate";

const completableProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.stChads
    isSignificantDateProvisional: true,
});
let completableProjectId: string;
const completableSchoolName = "St Chad's Catholic Primary School";

const someTasksCompletedProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversion.cradley,
});
let someTasksCompletedProjectId: string;

const noTasksCompletedProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversion.jessons,
});
let noTasksCompletedProjectId: string;

describe("Complete conversion projects tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(completableProject.urn.value);
        projectRemover.removeProjectIfItExists(noTasksCompletedProject.urn.value);
        projectRemover.removeProjectIfItExists(someTasksCompletedProject.urn.value);
        projectApi.createConversionProject(completableProject).then((response) => {
            completableProjectId = response.value;
            projectApi.getProject(completableProject.urn.value).then((response) => {
                const taskId = response.body.tasksDataId.value;
                TaskHelper.updateExternalStakeholderKickOff(completableProjectId, "completed", "2025-10-01");
                TaskHelper.updateConfirmAllConditionsMet(completableProjectId, "completed");
                TaskHelper.updateConfirmAcademyOpenedDate(taskId, "2025-10-10");
            });
        });
        projectApi.createMatConversionProject(someTasksCompletedProject).then((response) => {
            someTasksCompletedProjectId = response.value;
            projectApi.getProject(someTasksCompletedProject.urn.value).then((response) => {
                const taskId = response.body.tasksDataId.value;
                // set significate date in the future, so project cannot be completed
                TaskHelper.updateExternalStakeholderKickOff(
                    someTasksCompletedProjectId,
                    "completed",
                    getSignificantDateString(1),
                );
                TaskHelper.updateConfirmAllConditionsMet(someTasksCompletedProjectId, "completed");
                TaskHelper.updateConfirmAcademyOpenedDate(taskId, "2025-10-10");
            });
        });
        projectApi
            .createMatConversionProject(noTasksCompletedProject)
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
                "The conversion date has been confirmed and is in the past",
            ]);
    });

    it("should not be able to complete a project with no tasks completed", () => {
        cy.visit(`projects/${noTasksCompletedProjectId}/tasks`);
        taskListPage
            .clickButton("Complete project")
            .hasImportantCompletedBannerWith("This project cannot be completed until:", [
                "The conversion date has been confirmed and is in the past",
                "The confirm all conditions have been met task is completed",
                "The confirm the date the academy opened task is completed",
            ]);
    });
});
