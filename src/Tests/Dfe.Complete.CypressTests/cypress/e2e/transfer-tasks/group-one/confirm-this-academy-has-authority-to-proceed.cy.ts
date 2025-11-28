import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";

const taskPath = "conditions_met";

describe("Transfer tasks - Confirm this transfer has authority to proceed", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupProjects();
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check the transfer has authority to proceed")
            .hasDropdownContent("the academy have confirmed all of their actions are complete")
            .clickDropdown("What to do if the transfer does not have authority to proceed")
            .hasDropdownContent(
                "You must agree a new transfer date with all stakeholders, then change the transfer date for this project.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Baseline spreadsheet approved' checkbox and save");
        taskPage.hasCheckboxLabel("Baseline spreadsheet approved").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Confirm this transfer has authority to proceed")
            .selectTask("Confirm this transfer has authority to proceed");

        Logger.log("Unselect the 'Baseline spreadsheet approved' checkbox and save");
        taskPage.hasCheckboxLabel("Baseline spreadsheet approved").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm this transfer has authority to proceed")
            .selectTask("Confirm this transfer has authority to proceed");
        taskPage.hasCheckboxLabel("Baseline spreadsheet approved").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateConfirmTransferHasAuthorityToProceed(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm this transfer has authority to proceed");

        TaskHelperTransfers.updateConfirmTransferHasAuthorityToProceed(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm this transfer has authority to proceed");

        TaskHelperTransfers.updateConfirmTransferHasAuthorityToProceed(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm this transfer has authority to proceed");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/conditions_met`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
