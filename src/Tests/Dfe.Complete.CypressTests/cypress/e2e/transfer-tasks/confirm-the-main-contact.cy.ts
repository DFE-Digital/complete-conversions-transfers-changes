import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transferTasks.coquet },
});
let projectId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transferTasks.marden },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfer Tasks - Confirm the main contact", () => {
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
    });

    // awaiting 238981 to add a contact via API
    it.skip("Should be able to choose contact and save the task", () => {
        cy.visit(`projects/${projectId}/tasks/main_contact`);
        taskPage.hasCheckboxLabel("__").tick().saveAndReturn();
        taskListPage.hasTaskStatusCompleted("Confirm the main contact");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/main_contact`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
