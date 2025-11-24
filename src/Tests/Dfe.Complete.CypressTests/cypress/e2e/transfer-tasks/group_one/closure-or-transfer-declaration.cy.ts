import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "closure_or_transfer_declaration";

describe("Transfers tasks - Closure or transfer declaration", () => {
    let setup: ReturnType<typeof TransferTasksTestSetup.getSetup>;

    before(() => {
        TransferTasksTestSetup.setupProjects();
        setup = TransferTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage.hasCheckboxLabel("Received").tick().hasCheckboxLabel("Cleared").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Closure or transfer declaration")
            .selectTask("Closure or transfer declaration");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Closure or transfer declaration")
            .selectTask("Closure or transfer declaration");
        taskPage.hasCheckboxLabel("Received").isUnticked().hasCheckboxLabel("Cleared").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateClosureOrTransferDeclaration(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Closure or transfer declaration");

        TaskHelperTransfers.updateClosureOrTransferDeclaration(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Closure or transfer declaration");

        TaskHelperTransfers.updateClosureOrTransferDeclaration(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Closure or transfer declaration");

        TaskHelperTransfers.updateClosureOrTransferDeclaration(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Closure or transfer declaration");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
