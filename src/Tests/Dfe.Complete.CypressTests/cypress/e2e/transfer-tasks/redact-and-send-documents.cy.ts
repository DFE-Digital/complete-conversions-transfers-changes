import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "redact_and_send_documents";

describe("Transfer tasks - Redact and send documents", () => {
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
            .hasCheckboxLabel("Redact all relevant documents")
            .expandGuidance("Help redacting the documents")
            .hasGuidance("You need to create a redacted version of each document")
            .hasGuidance("deed of novation and variation")
            .hasGuidance("deed of termination");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select first checkbox and save");
        taskPage
            .hasCheckboxLabel("Send relevant redacted documents to ESFA (Education and Skills Funding Agency)")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Redact and send documents").selectTask("Redact and send documents");

        Logger.log("Unselect same checkbox and save");
        taskPage
            .hasCheckboxLabel("Send relevant redacted documents to ESFA (Education and Skills Funding Agency)")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents").selectTask("Redact and send documents");
        taskPage
            .hasCheckboxLabel("Send relevant redacted documents to ESFA (Education and Skills Funding Agency)")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateRedactAndSendDocuments(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents");

        TaskHelperTransfers.updateRedactAndSendDocuments(setup.taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Redact and send documents");

        TaskHelperTransfers.updateRedactAndSendDocuments(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Redact and send documents");
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
