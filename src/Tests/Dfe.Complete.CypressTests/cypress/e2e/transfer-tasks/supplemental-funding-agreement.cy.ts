import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "supplemental_funding_agreement";

describe("Transfer tasks - Supplemental funding agreement", () => {
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
            .clickDropdown("Help checking the supplemental funding agreement")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .tick()
            .hasCheckboxLabel("Cleared")
            .tick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Supplemental funding agreement")
            .selectTask("Supplemental funding agreement");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Supplemental funding agreement")
            .selectTask("Supplemental funding agreement");
        taskPage
            .hasCheckboxLabel("Received")
            .isUnticked()
            .hasCheckboxLabel("Cleared")
            .isUnticked()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateSupplementalFundingAgreement(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Supplemental funding agreement");

        TaskHelperTransfers.updateSupplementalFundingAgreement(setup.taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Supplemental funding agreement");

        TaskHelperTransfers.updateSupplementalFundingAgreement(setup.taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Supplemental funding agreement");

        TaskHelperTransfers.updateSupplementalFundingAgreement(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Supplemental funding agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/supplemental_funding_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
