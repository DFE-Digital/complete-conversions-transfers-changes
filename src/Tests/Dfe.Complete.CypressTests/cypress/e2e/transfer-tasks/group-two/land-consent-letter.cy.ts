import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupTwoSetup } from "cypress/support/transferTasksSetup";

const taskPath = "land_consent_letter";

describe("Transfer tasks - Land consent letter", () => {
    let setup: ReturnType<typeof TransferTasksGroupTwoSetup.getSetup>;

    before(() => {
        TransferTasksGroupTwoSetup.setupProjects();
        setup = TransferTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Drafted")
            .tick()
            .hasCheckboxLabel("Sent to both trusts, their solicitors and the local authority")
            .tick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Land consent letter").selectTask("Land consent letter");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Drafted")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Sent to both trusts, their solicitors and the local authority")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Land consent letter").selectTask("Land consent letter");
        taskPage
            .hasCheckboxLabel("Drafted")
            .isUnticked()
            .hasCheckboxLabel("Sent to both trusts, their solicitors and the local authority")
            .isUnticked()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateLandConsentLetter(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Land consent letter");

        TaskHelperTransfers.updateLandConsentLetter(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Land consent letter");

        TaskHelperTransfers.updateLandConsentLetter(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Land consent letter");

        TaskHelperTransfers.updateLandConsentLetter(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Land consent letter");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/land_consent_letter`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
