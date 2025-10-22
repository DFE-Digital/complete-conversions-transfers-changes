import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transferTasks.coquet },
});
let projectId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transferTasks.marden },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfer tasks - Confirm the date the academy transferred", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi.createTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createMatTransferProject(otherUserProject).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/confirm_date_academy_transferred`);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Input the date and save");
        taskPage.enterDate("11", "1", "2025", "date-academy-transferred").saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the date the academy transferred")
            .selectTask("Confirm the date the academy transferred");

        Logger.log("Confirm date persists and clear date");
        taskPage
            .hasDate("11", "1", "2025", "date-academy-transferred")
            .enterDate("", "", "", "date-academy-transferred")
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the date the academy transferred")
            .selectTask("Confirm the date the academy transferred");
        taskPage.hasDate("", "", "", "date-academy-transferred");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/confirm_date_academy_transferred`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
