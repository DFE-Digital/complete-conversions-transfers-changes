import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "subleases";

describe("Conversion tasks - Subleases", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the school to ask if all relevant parties have agreed and signed any subleases")
            .tick()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed any subleases",
            )
            .tick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Subleases").selectTask("Subleases");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the school to ask if all relevant parties have agreed and signed any subleases")
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed any subleases",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Subleases").selectTask("Subleases");
        taskPage
            .hasCheckboxLabel("Email the school to ask if all relevant parties have agreed and signed any subleases")
            .isUnticked()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed any subleases",
            )
            .isUnticked()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateSubleases(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Subleases");

        TaskHelperConversions.updateSubleases(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Subleases");

        TaskHelperConversions.updateSubleases(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Subleases");

        TaskHelperConversions.updateSubleases(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Subleases");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
