import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { Logger } from "cypress/common/logger";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transferTasks.coquet },
});
let projectId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transferTasks.marden },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfer tasks - Confirm if the bank details for the general annual grant payment need to change", () => {
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
        cy.visit(`projects/${projectId}/tasks/bank_details_changing`);
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select the 'No' option and save");
        taskPage.hasCheckboxLabel("No").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm if the bank details for the general annual grant payment need to change")
            .selectTask("Confirm if the bank details for the general annual grant payment need to change");

        Logger.log("Change selection to 'Yes' and save");
        taskPage.hasCheckboxLabel("No").isTicked().hasCheckboxLabel("Yes").isUnticked().tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm if the bank details for the general annual grant payment need to change")
            .selectTask("Confirm if the bank details for the general annual grant payment need to change");
        taskPage.hasCheckboxLabel("Yes").isTicked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/bank_details_changing`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
