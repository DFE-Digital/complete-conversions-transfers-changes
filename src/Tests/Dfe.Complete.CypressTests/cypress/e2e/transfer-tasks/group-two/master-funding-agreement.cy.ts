import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupTwoSetup } from "cypress/support/transferTasksSetup";

const taskPath = "master_funding_agreement";

describe("Transfer tasks - Master funding agreement", () => {
    let setup: ReturnType<typeof TransferTasksGroupTwoSetup.getSetup>;

    before(() => {
        TransferTasksGroupTwoSetup.setupProjects();
        setup = TransferTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("When to sign a deed of termination")
            .hasDropdownContent("A deed of termination can only be signed when both the")
            .clickDropdown("Help checking and updating the master funding agreement")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust")
            .clickDropdown("How to sign the master funding agreement")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by incoming trust")
            .tick()
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .tick()
            .hasCheckboxLabel("Saved in the academy and incoming trust SharePoint folders")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Master funding agreement").selectTask("Master funding agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by incoming trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the academy and incoming trust SharePoint folders")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement").selectTask("Master funding agreement");
        taskPage
            .hasCheckboxLabel("Signed by incoming trust")
            .isUnticked()
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .isUnticked()
            .hasCheckboxLabel("Saved in the academy and incoming trust SharePoint folders")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateMasterFundingAgreement(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement");

        TaskHelperTransfers.updateMasterFundingAgreement(setup.taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Master funding agreement");

        TaskHelperTransfers.updateMasterFundingAgreement(setup.taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Master funding agreement");

        TaskHelperTransfers.updateMasterFundingAgreement(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Master funding agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/master_funding_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
