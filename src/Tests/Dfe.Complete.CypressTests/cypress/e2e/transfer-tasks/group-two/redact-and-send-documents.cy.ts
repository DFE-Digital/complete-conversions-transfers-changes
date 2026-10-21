import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupTwoSetup } from "cypress/support/transferTasksSetup";

const taskPath = "redact_and_send_documents";

describe("Transfer tasks - Redact and send documents", () => {
    let setup: ReturnType<typeof TransferTasksGroupTwoSetup.getSetup>;

    before(() => {
        TransferTasksGroupTwoSetup.setupProjects();
        setup = TransferTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select first checkbox and save");
        taskPage
            .hasCheckboxLabel("Send relevant documents to SFSO (Schools Financial Support and Oversight)")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Send and Save Documents").selectTask("Send and Save Documents");

        Logger.log("Unselect same checkbox and save");
        taskPage
            .hasCheckboxLabel("Send relevant documents to SFSO (Schools Financial Support and Oversight)")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Send and Save Documents").selectTask("Send and Save Documents");
        taskPage
            .hasCheckboxLabel("Send relevant documents to SFSO (Schools Financial Support and Oversight)")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateRedactAndSendDocuments(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Send and Save Documents");

        TaskHelperTransfers.updateRedactAndSendDocuments(setup.taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Send and Save Documents");

        TaskHelperTransfers.updateRedactAndSendDocuments(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Send and Save Documents");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/redact_and_send_documents`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
