import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "redact_and_send";

describe("Conversion tasks - Redact and send documents", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupProjects();
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Redact all relevant documents")
            .expandGuidance("Help redacting the documents")
            .hasGuidance("You need to create a redacted version of each applicable document");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select first checkbox and save");
        taskPage.hasCheckboxLabel("Redact all relevant documents").tick().saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Redact and send documents").selectTask("Redact and send documents");

        Logger.log("Unselect same checkbox and save");
        taskPage.hasCheckboxLabel("Redact all relevant documents").isTicked().untick().saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents").selectTask("Redact and send documents");
        taskPage.hasCheckboxLabel("Redact all relevant documents").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateRedactAndSendDocuments(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents");

        TaskHelperConversions.updateRedactAndSendDocuments(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Redact and send documents");

        TaskHelperConversions.updateRedactAndSendDocuments(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Redact and send documents");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
