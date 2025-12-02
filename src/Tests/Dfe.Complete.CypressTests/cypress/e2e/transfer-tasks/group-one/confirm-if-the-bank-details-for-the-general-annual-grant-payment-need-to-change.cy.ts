import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";

const taskPath = "bank_details_changing";

describe("Transfer tasks - Confirm if the bank details for the general annual grant payment need to change", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupProjectsWithoutTaskId();
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupOneSetup.setupBeforeEach(taskPath);
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
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/bank_details_changing`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
