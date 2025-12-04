import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import yourProjects from "cypress/pages/projects/yourProjects";
import { getSignificantDateString } from "cypress/support/formatDate";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskHelperConversions from "cypress/api/taskHelperConversions";

const completableProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.stChads,
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
        projectRemover.removeProjectIfItExists(completableProject.urn);
        projectRemover.removeProjectIfItExists(noTasksCompletedProject.urn);
        projectRemover.removeProjectIfItExists(someTasksCompletedProject.urn);
        projectApi.createAndUpdateConversionProject(completableProject).then((response) => {
            completableProjectId = response.value;
            projectApi.getProject(completableProject.urn).then((response) => {
                const taskId = response.body.tasksDataId.value;
                taskHelperConversions.updateExternalStakeholderKickOff(completableProjectId, "completed", "2025-10-01");
                taskHelperConversions.updateConfirmAllConditionsMet(completableProjectId, "completed");
                taskHelperConversions.updateConfirmAcademyOpenedDate(taskId, "2025-10-10");
            });
        });
        projectApi.createAndUpdateMatConversionProject(someTasksCompletedProject).then((response) => {
            someTasksCompletedProjectId = response.value;
            projectApi.getProject(someTasksCompletedProject.urn).then((response) => {
                const taskId = response.body.tasksDataId.value;
                // set significate date in the future, so project cannot be completed
                taskHelperConversions.updateExternalStakeholderKickOff(
                    someTasksCompletedProjectId,
                    "completed",
                    getSignificantDateString(1),
                );
                taskHelperConversions.updateConfirmAllConditionsMet(someTasksCompletedProjectId, "completed");
                taskHelperConversions.updateConfirmAcademyOpenedDate(taskId, "2025-10-10");
            });
        });
        projectApi
            .createAndUpdateMatConversionProject(noTasksCompletedProject)
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

    it("should not be able to complete a project with some tasks completed", () => {
        cy.visit(`projects/${someTasksCompletedProjectId}/tasks`);
        taskListPage
            .clickButton("Complete project")
            .hasImportantCompletedBannerWith("This project cannot be completed until:", [
                "The conversion date has been confirmed and is in the past",
                "The incoming trust UKPRN is entered",
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
                "The incoming trust UKPRN is entered",
            ]);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
