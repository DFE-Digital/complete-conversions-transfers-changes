import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "commercial_transfer_agreement";

describe("Transfer tasks - Commercial transfer agreement", () => {
    let setup: ReturnType<typeof TransferTasksTestSetup.getSetup>;

    before(() => {
        TransferTasksTestSetup.setupProjects();
        setup = TransferTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check and assure the commercial transfer agreement")
            .hasDropdownContent(
                "You can read guidance about how use check and assure the agreement (opens in new tab) on SharePoint.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'Confirm commercial transfer agreement is agreed' and save");
        taskPage.hasCheckboxLabel("Confirm commercial transfer agreement is agreed").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");

        Logger.log("Unselect 'Confirm commercial transfer agreement is agreed' and save");
        taskPage
            .hasCheckboxLabel("Confirm commercial transfer agreement is agreed")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");
        taskPage.hasCheckboxLabel("Confirm commercial transfer agreement is agreed").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateCommercialTransferAgreement(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Commercial transfer agreement");

        TaskHelperTransfers.updateCommercialTransferAgreement(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Commercial transfer agreement");

        TaskHelperTransfers.updateCommercialTransferAgreement(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Commercial transfer agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/commercial_transfer_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
